using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.IO;
using System.Web.Services;
using AppSemanticIndex;
using AppSemanticIndex.Pobj;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Xml.Serialization;
using WSSemanticSearch.srvExpansionConsulta;
using System.Globalization;

namespace WSSemanticSearch
{
    /// <summary>
    /// Summary description for WSSemanticSearch
    /// </summary>
    [WebService(Namespace = "http://www.unicacuca.edu.co/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WSSemanticSearch : System.Web.Services.WebService
    {

        #region "Constructores de Clase"

        public WSSemanticSearch()
        {
            SemanticIndexManager _semanticIndexManager = new SemanticIndexManager();
        }
        #endregion

        #region "Variables globales del servicio Web"

        private CultureInfo culture = CultureInfo.InvariantCulture;

        //Objeto con la logica del negocio para la indexción
        private SemanticIndexManager _semanticIndexManager = null;

        public SemanticIndexManager semanticIndexManager
        {
            get { return _semanticIndexManager; }
            set { _semanticIndexManager = value; }
        }

        #endregion

        #region "Servicios Básicos del Indice semántico"

        //Realiza la Búsqueda de la consulta dada, con la ontología y las opciones de configuración
        //realizadas previamente y almacenada en el Web.config
        [WebMethod]
        public DataSet Buscar(string stringBuscar, string idioma = "Español")
        {
            try
            {
                if (!string.IsNullOrEmpty(stringBuscar) && ObtenerConfiguracion("BusquedaBloqueada") == "No")
                    return SearchSemanticIndex(stringBuscar, idioma);
                else
                {
                    DataSet dts = new DataSet();
                    DataTable dtt = new DataTable();

                    dtt.Columns.Add("Mensaje", System.Type.GetType("System.String"));
                    dtt.Rows.Add("La Busqueda esta deshabilitada temporalemente por un proceso de indexación. Intente más tarde");
                    dts.Tables.Add(dtt);

                    return dts;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        //Igual que la amterior pero no expande consulta y retorna una lista de FeedXively Serializados
        [WebMethod]
        public List<FeedXively> BuscarFeeds(string consulta, string idioma = "Español")
        {
            if (!string.IsNullOrEmpty(consulta) && ObtenerConfiguracion("BusquedaBloqueada") == "No")
            {
                //Crea el Objeto que guarda todo el conocimiento del negocio
                SemanticIndexManager semanticIndexManager = new SemanticIndexManager();

                //Crea la lista de documentos que serán el resultado de la búsqueda
                List<UrlDocument> allReponse = new List<UrlDocument>();

                //Se busca en español o el ingles. Si es nulo busca por ambos.
                if (idioma == "Español")
                {
                    allReponse = semanticIndexManager.BuscarSemanticIndex(consulta, true);
                }
                else if (idioma == "Ingles")
                {
                    allReponse = semanticIndexManager.BuscarSemanticIndex(consulta, false);
                }
                else
                    allReponse = semanticIndexManager.BuscarSemanticIndex(consulta, true); //Por defecto busca español

                //Armar la estructura de datos a retornar como resltado
                List<FeedXively> SensoresResult = new List<FeedXively>();

                //Traemos todos los metadatos del sensor que estan almacenados en la BDD local
                foreach (UrlDocument urldoc in allReponse)
                {
                    FeedXively xtemp = semanticIndexManager.ObtenerJSONFeedBDD(urldoc.Id);
                    SensoresResult.Add(xtemp);
                }

                return SensoresResult;
            }
            else
                return null;
        }

        //Igual que la anterior pero retorna una lista de FeedXively Serializados
        //Además de los datapoints correspondientes consultados en Xively.
        [WebMethod]
        public List<FeedXively> BuscarFeedsDataPoints(string consulta, DateTime fechaInicio, DateTime fechaFin, string idioma = "Español")
        {
            if (!string.IsNullOrEmpty(consulta) && ObtenerConfiguracion("BusquedaBloqueada") == "No")
            {
                //Crea el Objeto que guarda todo el conocimiento del negocio
                SemanticIndexManager semanticIndexManager = new SemanticIndexManager();

                //Crea la lista de documentos que serán el resultado de la búsqueda
                List<UrlDocument> allReponse = new List<UrlDocument>();

                //Se busca en español o el ingles. Si es nulo busca por ambos.
                if (idioma == "Español")
                {
                    allReponse = semanticIndexManager.BuscarSemanticIndex(consulta, true);
                }
                else if (idioma == "Ingles")
                {
                    allReponse = semanticIndexManager.BuscarSemanticIndex(consulta, false);
                }
                else
                    allReponse = semanticIndexManager.BuscarSemanticIndex(consulta, true); //Por defecto busca español

                //Armar la estructura de datos a retornar como resltado
                List<FeedXively> SensoresResult = new List<FeedXively>();

                //Traemos todos los metadatos del sensor qdesde le servidor Xively, junto con sus mediciones
                foreach (UrlDocument urldoc in allReponse)
                {
                    FeedXively xtemp = RetornarDatosSensor(urldoc.Id, fechaInicio, fechaFin);
                    SensoresResult.Add(xtemp);
                }

                return SensoresResult;
            }
            else
                return null;
        }

        //Retorna un listado de conceptos directamente relacionados al concepto. Estos de obtienen
        //de la ontología almacenada. Descarta términos no encontrados y no busca en wordnet.
        //Si el concepto no esta en la ontología retorna una cadena vacia.
        [WebMethod]
        public string RetornarConceptos(string Concepto, string idioma = "Español")
        {
            return BuscarConceptosOntologia(Concepto, idioma);
        }

        //Retorna un listado de sensores y su localización con respecto a la localización dada y una Consulta
        [WebMethod]
        public List<FeedXively> RetornarMapaLugar(double Latitud, double Longitud, string Consulta, string idioma = "Español", double radio = 100)
        {
            string expansion = ObtenerConfig("utilizarExpansion");

            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Crea una copia de la lista para eliminar los sensores que no se les puede calcular distancia
            List<FeedXively> FeedsConPosicion = new List<FeedXively>();

            //Lista de los resultados de la busqueda
            List<FeedXively> dtsResult = new List<FeedXively>();

            //Buscar la consulta en el índice
            if (!string.IsNullOrEmpty(Consulta) && ObtenerConfiguracion("BusquedaBloqueada") == "No")
                dtsResult = BuscarFeeds(Consulta, idioma);
            else
                return null;

            //Si encontro sensores entonces validar los que entran en el radio de acción
            if(dtsResult != null)
            {
                List<string> clavesBorrar = new List<string>();

                foreach (FeedXively dtr in dtsResult)
                {
                    if (dtr.feed.location != null)
                    {
                        //La funcion establece que el sensor este en el rango de N kms del lugar especificado. Si esta devuelve la distancia, sino devuelve cero
                        double distancia = semanticIndexManager.VerificarSensorenLugar(Latitud,
                                                     Longitud, Convert.ToDouble(dtr.feed.location.lat, culture),
                                                     Convert.ToDouble(dtr.feed.location.lon, culture), radio);
                        if (distancia > 0)
                        {
                            //ListaResultados.Add(indice, distancia);
                            GeonameNode nodolugardistancia = new GeonameNode();
                            nodolugardistancia.Latitud = Latitud.ToString();
                            nodolugardistancia.Longitud = Longitud.ToString();
                            lugar ld = new lugar();
                            ld.nodoGeoname = nodolugardistancia;
                            ld.distancia = distancia;
                            //Creamos el espacio en la variable de lista de lugares relacionada al sensor
                            dtr.lugares = new List<lugar>();
                            dtr.lugares.Add(ld);
                            FeedsConPosicion.Add(dtr);
                        }
                    }
                }
            }

            //Finalmente retorna la lista con los sensores correspondientes
            return FeedsConPosicion;
        }

        //Retorna un listado de sensores y su localización con respecto a la localización dada y una Consulta
        //Adicional retorna los datapoints de la fechas dadas
        [WebMethod]
        public List<FeedXively> RetornarMapaLugarDatapoints(double Latitud, double Longitud, string Consulta, DateTime fechaInicio, DateTime fechaFin, string idioma = "Español", double radio = 100)
        {
            string expansion = ObtenerConfig("utilizarExpansion");

            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Crea una copia de la lista para eliminar los sensores que no se les puede calcular distancia
            List<FeedXively> FeedsConPosicion = new List<FeedXively>();

            //Lista de los resultados de la busqueda
            List<FeedXively> dtsResult = new List<FeedXively>();

            //Buscar la consulta en el índice
            if (!string.IsNullOrEmpty(Consulta) && ObtenerConfiguracion("BusquedaBloqueada") == "No")
                dtsResult = BuscarFeedsDataPoints(Consulta,fechaInicio, fechaFin, idioma);
            else
                return null;

            //Si encontro sensores entonces validar los que entran en el radio de acción
            if (dtsResult != null)
            {
                List<string> clavesBorrar = new List<string>();

                foreach (FeedXively dtr in dtsResult)
                {
                    if (dtr.feed.location != null && dtr.feed.location.lat != null && dtr.feed.location.lon != null)
                    {
                        //La funcion establece que el sensor este en el rango de N kms del lugar especificado. Si esta devuelve la distancia, sino devuelve cero
                        double distancia = semanticIndexManager.VerificarSensorenLugar(Latitud,
                                                     Longitud, Convert.ToDouble(dtr.feed.location.lat, culture),
                                                     Convert.ToDouble(dtr.feed.location.lon, culture), radio);
                        if (distancia > 0)
                        {
                            //ListaResultados.Add(indice, distancia);
                            GeonameNode nodolugardistancia = new GeonameNode();
                            nodolugardistancia.Latitud = Latitud.ToString();
                            nodolugardistancia.Longitud = Longitud.ToString();
                            lugar ld = new lugar();
                            ld.nodoGeoname = nodolugardistancia;
                            ld.distancia = distancia;
                            //Creamos el espacio en la variable de lista de lugares relacionada al sensor
                            dtr.lugares = new List<lugar>();
                            dtr.lugares.Add(ld);
                            FeedsConPosicion.Add(dtr);
                        }
                    }
                }
            }

            //Finalmente retorna la lista con los sensores correspondientes
            return FeedsConPosicion;
        }

        //Retorna un listado de Sensores y su localización con respecto a una localización dada y un conjunto se sensores
        [WebMethod]
        public List<FeedXively> RetornarMapaLugarListaSensores(double Latitud, double Longitud, List<FeedXively> dtsResult, string idioma = "Español", double radio = 100)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Crea una copia de la lista para eliminar los sensores que no se les puede calcular distancia
            List<FeedXively> FeedsConPosicion = new List<FeedXively>();

            //Si encontro sensores entonces validar los que entran en el radio de acción
            if (dtsResult != null)
            {
                List<string> clavesBorrar = new List<string>();

                foreach (FeedXively dtr in dtsResult)
                {
                    if (dtr.feed.location != null)
                    {
                        //La funcion establece que el sensor este en el rango de N kms del lugar especificado. Si esta devuelve la distancia, sino devuelve cero
                        double distancia = semanticIndexManager.VerificarSensorenLugar(Latitud,
                                                     Longitud, Convert.ToDouble(dtr.feed.location.lat, culture),
                                                     Convert.ToDouble(dtr.feed.location.lon, culture), radio);
                        if (distancia > 0)
                        {
                            //ListaResultados.Add(indice, distancia);
                            GeonameNode nodolugardistancia = new GeonameNode();
                            nodolugardistancia.Latitud = Latitud.ToString();
                            nodolugardistancia.Longitud = Longitud.ToString();
                            lugar ld = new lugar();
                            ld.nodoGeoname = nodolugardistancia;
                            ld.distancia = distancia;
                            //Creamos el espacio en la variable de lista de lugares relacionada al sensor
                            dtr.lugares = new List<lugar>();
                            dtr.lugares.Add(ld);
                            FeedsConPosicion.Add(dtr);
                        }

                    }
                }
            }

            //Finalmente retorna la lista con los sensores correspondientes
            return FeedsConPosicion;
        }
        
        //Realiza un proceso de expanción de la consulta del usuario utilizando la ontología para hacerla más exacta
        //Para esto primero Retorna conceptos, posteriormente busca en wordnet conceptos no encontrados y finalmente 
        //enlaza los términos no identificados al final de la consulta por considerarse importantes para el usuario.
        [WebMethod]
        public string ExpandirConsultaConceptosOntologia(string Consulta, string idioma)
        {
            return ExpandirConsulta(Consulta, idioma);
        }
        #endregion

        #region "Servicios de Configuración del Indice Semántico"

        //Método Adicional: Permite recuperar los valores de las claves almacenadas en el Web.Config
        //con el fin de establecer la configuración actual del índice
        [WebMethod]
        public string ObtenerConfiguracion(string key)
        {
            if (!string.IsNullOrEmpty(key))
                return ObtenerConfig(key);
            else
                return "La clave no puede ser vacia o nula";
        }

        //Método Adicional: Permite asignar un valor a una clave específica en el Web.config con el fin
        //de establecer una nueva configuración de indexción
        [WebMethod]
        public string AsignarConfiguracion(string key, string valor)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(key))
                return AsignarConfig(key, valor);
            else
                return "La clave o el valor no pueden ser nulos o vacios";
        }

        //Método Adicional: Una vez se han cambiado las configuraciones o se desea iniciar una nueva ontología de dominio
        //Es necesario crear el índice semántico. Este método crea el índice teniendo en cuenta si lo hace desde el servidor
        //de objetos Xively o desde los archivos locales almacenados en una indexación previa.
        //Por defecto vuelve a poblar la BD de SQL Server con los nuevos conceptos para el proceso de Expanción de consulta
        [WebMethod]
        public string CrearIndiceSemantico(string NombreOntología)
        {
            string fuentedatos;
            string conceptoscargados = string.Empty;
            string filename = Path.GetFileName(NombreOntología);
            string ruta = SemanticIndexManager.RutaOntologiaSinArchivo;

            try
            {
                //Bloquea las búsquedas
                AsignarConfig("BusquedaBloqueada", "Si");              

                //Crea el Objeto que guarda todo el conocimiento del negocio
                semanticIndexManager = new SemanticIndexManager();

                //Aplicar segpun los parámetros
                if (ObtenerConfig("BDDfuente") == "Xively")
                {
                    semanticIndexManager.CrearIndiceSemantico(true);
                    fuentedatos = "el servidor Xively";
                }
                else
                {
                    semanticIndexManager.CrearIndiceSemantico(false);
                    fuentedatos = "la Base de Datos Local de Archivos JSON";
                }

                //Se debe cargar los conceptos de la ontologia para el proceso de expansión de consulta
                conceptoscargados = CargarConceptosOntologia(ruta + NombreOntología);

                //Desbloquear la búsqueda
                AsignarConfig("BusquedaBloqueada", "No");

                return "El índice se ha creado correctamente desde " + fuentedatos;
            }
            catch (Exception ex)
            {
                //Se restablece la busqueda
                AsignarConfig("BusquedaBloqueada", "No");  

                //Enviar error
                return "Error: Ocurrio un error al crear el Índice: " + ex.Message;
            }
        }


        //Método Adicional: Permite subir un archivo de ontología y lo coloca local en el índice.
        [WebMethod]
        public string CargarOntologia(string nuevaontologia, byte[] info)
        {
            try
            {
                string ruta = SemanticIndexManager.RutaOntologiaSinArchivo;
                string filename = Path.GetFileName(nuevaontologia);

                //Guardar la ontología en el directorio correspondiente
                BinaryWriter writer = new BinaryWriter(File.Open((@"" + ruta + filename),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite));
                writer.Write(info);
                writer.Close();

                return "Ontología Cargada de forma Exitosa. Recuerde iniciar e Proceso de Indexación";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        //Método Adicional: Permite cargar un feed de Xively espécifico a la BDD
        //Posteriormente lo indexa en el indice semántico
        [WebMethod]
        public string CargarFeedXivelyBDD(string feedId)
        {
            try
            {
                //Bloquea las búsquedas
                AsignarConfig("BusquedaBloqueada", "Si");

                //Recupera el objeto de logica de negoicio de la aplicación para que el cambio de ontologia tenga efecto
                semanticIndexManager = new SemanticIndexManager();

                //semanticIndexManager.Timeout = -1;
                semanticIndexManager.CargarFeedXivelyBDD(feedId);

                //Desbloquea las búsquedas
                AsignarConfig("BusquedaBloqueada", "No");

                return "El índice se ha Actualizado correctamente desde el servidor Xively.";
            }
            catch(Exception ex)
            {
                AsignarConfig("BusquedaBloqueada", "No");
                return "Ocurrio un error para cargar e indexar el Feed: " + ex.Message;
            }
        }

        #endregion

        #region "Servicios de Geolocalización de Sensores del Indice Semántico"

        //Método Adicional: Permite Buscar un lugar geográfico por su nobre en el servicio Web de GeoNames
        [WebMethod]
        public List<GeonameNode> ObtenerLocalizacion(string lugar)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            return semanticIndexManager.ObtenerLocalizaciones(lugar);
        }

        //Método Adicional: Permite determinar y retornar los conceptos que son lugares geográficos y con sus posiciones
        //geolocalizadasa a trvés del servicio de Geonames.
        [WebMethod]
        public List<GeonameNode> ObtenerCiudadesConsulta(string Consulta)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            return semanticIndexManager.ObtenerCiudadesConsulta(Consulta);
        }

        //Obtiene un solo lugar dadas sus coordenadas
        [WebMethod]
        public GeonameNode ObtenerLugardeCoordenadas(string latitud, string longitud)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            return semanticIndexManager.ObtenerLugardeCoordenadas(latitud, longitud);
        }

        //Obtiene Los nombres de los lugares de una lista de GeonameNodes que recibe
        [WebMethod]
        public List<GeonameNode> ObtenerListaLugarCoordenadas(string geonodesSerializados)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Desserializamos el objeto enviado
            List<GeonameNode> geonodes = geonodesSerializados.DeserializarTo<List<GeonameNode>>();

            //Objeto copia para las operaciones
            List<GeonameNode> geonodestmp = new List<GeonameNode>();
            
            foreach (GeonameNode geotmp in geonodes)
            {
                geonodestmp.Add(semanticIndexManager.ObtenerLugardeCoordenadas(geotmp.Latitud, geotmp.Longitud));
            }

            return geonodestmp;
        }

        //Obtiene las coordenadas de una lista. Adicionalmente Obtiene la jerarquia de lugares de cada nodo
        [WebMethod]
        public List<GeonameNode> ObtenerListaLugarCoordenadasJerarquica(string geonodesSerializados)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Desserializamos el objeto enviado
            List<GeonameNode> geonodes = geonodesSerializados.DeserializarTo<List<GeonameNode>>();

            //Objeto copia para las operaciones
            List<GeonameNode> geonodestmp = new List<GeonameNode>();

            foreach (GeonameNode geotmp in geonodes)
            {
                List<GeonameNode> geonodestmpj = new List<GeonameNode>();
                geonodestmpj = ObtenerLugardeCoordenadasJerarquia(geotmp.Latitud, geotmp.Longitud);
                //Añadimos el último nodo que contiene el lugar buscado
                if(geonodestmpj != null)
                    geonodestmp.Add(geonodestmpj[geonodestmpj.Count-1]);
            }

            return geonodestmp;
        }

        //Obtiene las coordenadas de una lista. Adicionalmente Obtiene de la jerarquia de lugares de cada nodo
        //La primera división política que corresponde a dicho lugar
        [WebMethod]
        public string ObtenerListaLugarCoordenadas_AM1(string geonodesSerializados)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Desserializamos el objeto enviado
            List<GeonameNode> geonodes = geonodesSerializados.DeserializarTo<List<GeonameNode>>();

            //Objeto copia para las operaciones
            List<GeonameNode> geonodestmp = new List<GeonameNode>();

            foreach (GeonameNode geotmp in geonodes)
            {
                List<GeonameNode> geonodestmpj = new List<GeonameNode>();
                geonodestmpj = semanticIndexManager.ObtenerLugardeCoordenadasJerarquia(geotmp.Latitud, geotmp.Longitud);

                if (geonodestmpj != null)
                    foreach (GeonameNode getmpj in geonodestmpj)
                    {
                        if (getmpj.fcode == "ADM1")
                            geonodestmp.Add(getmpj);
                    }
            }
            //Serializar la lista de salida
            return geonodestmp.SerializarToXml();
        }

        //Obtiene la jerarquía de lugares de un punto en el mapa
        [WebMethod]
        public List<GeonameNode> ObtenerLugardeCoordenadasJerarquia(string latitud, string longitud)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            return semanticIndexManager.ObtenerLugardeCoordenadasJerarquia(latitud, longitud);
        }

        //Obtiene la jerarquía dado un geonameId
        [WebMethod]
        public List<GeonameNode> GeoNames_Hierarchy(int GeonameId)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            return semanticIndexManager.GeoNames_Hierarchy(GeonameId);
        }

        #endregion

        #region "Métodos de los Sensores"

        //Este método devuelve en formato FeedXively los Metadatos del Sensor
        [WebMethod]
        public FeedXively RetornarMetadatosSensor(string IdSensor)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Obtener el objeto de la BDD
            FeedXively xtemp = semanticIndexManager.ObtenerJSONFeedBDD(IdSensor);
            
            return xtemp;
        }

        //Este método devuelve en formato json los Metadatos del Sensor desde Xively
        [WebMethod]
        public string  RetornarJsonSensor(string IdSensor)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Obtener el JSON de la BDD
            string xtemp = semanticIndexManager.ObtenerJsonFeed(ConfigurationManager.AppSettings["APIkey"], IdSensor);
            
            //Retornar el json
            return xtemp;
        }

        //Este método obtiene información de los datos que mide el sensor.
        //Se retorna todo el feed con los datapoints
        [WebMethod]
        public FeedXively RetornarDatosSensor(string feedID, DateTime fechaInicio, DateTime fechaFin)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Obtener el JSON de la BDD
            return semanticIndexManager.RetornarDatosSensor(feedID, fechaInicio, fechaFin);
        }

        //Este método retorna los datapoints de un datastream específico, de un feed específico
        [WebMethod]
        public string RetornarDatapointsFeed(string feedID, string DatastreamId, string fechaInicio, string fechaFin)
        {
            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Obtener el JSON de la BDD
            string datapoints = semanticIndexManager.RetornarDatapointsFeed(feedID, DatastreamId, 
                                                                            Convert.ToDateTime(fechaInicio),
                                                                            Convert.ToDateTime(fechaFin));
            return datapoints;
        }

        #endregion

        #region "Métodos Internos del Servicio de Indice Semántico"

        private DataSet SearchSemanticIndex(string Consulta, string idioma)
        {
            //string idioma = ObtenerConfig("idiomaBusqueda");
            string expansion = ObtenerConfig("utilizarExpansion");

            //Crea el Objeto que guarda todo el conocimiento del negocio
            semanticIndexManager = new SemanticIndexManager();

            //Crea la lista de documentos que serán el resultado de la búsqueda
            List<UrlDocument> allReponse = new List<UrlDocument>();

            if (expansion == "Si")
            {
                //Realiza la expansión de la consulta a través del servicio.
                Consulta = ExpandirConsulta(Consulta, idioma);
            }

            //Se busca en español o el ingles. Si es nulo busca por ambos.
            if (idioma == "Español")
            {
                allReponse = semanticIndexManager.BuscarSemanticIndex(Consulta, true);
            }
            else if(idioma == "Ingles")
            {
                allReponse = semanticIndexManager.BuscarSemanticIndex(Consulta, false);
            }
            else
                allReponse = semanticIndexManager.BuscarSemanticIndex(Consulta, true); //Por defecto busca español

            //para DATASET que se devolvera como resultado
            UrlDocument objUrld = new UrlDocument();
            DataSet nuevoDS = new DataSet();
            DataTable table1 = new DataTable("Objetos Encontrados");

            table1.Columns.Add("Id");
            table1.Columns.Add("Title");
            table1.Columns.Add("Titulo_HTML");
            table1.Columns.Add("Url");
            table1.Columns.Add("URL_Sensor");
            table1.Columns.Add("Resumen");
            table1.Columns.Add("Tags");
            table1.Columns.Add("Localizacion");
            table1.Columns.Add("Dominio");
            table1.Columns.Add("Datastreams_feed");
            table1.Columns.Add("Website");
            table1.Columns.Add("Elevacion");
            table1.Columns.Add("Latitud");
            table1.Columns.Add("Longitud");
            //Campos para el PageRangkin
            table1.Columns.Add("Conceptos");   //Conceptos por los cuales fue encontrado en Xively
            table1.Columns.Add("Consulta");    //Consulta por la cual fue seleccionado el sensor 
            table1.Columns.Add("Distancia");   //Distancia para efectos de un lugar específico
            table1.PrimaryKey = new DataColumn[] { table1.Columns["Id"] };

            foreach (UrlDocument urldoc in allReponse)
            {
                table1.Rows.Add(urldoc.Id, urldoc.Tittle, urldoc.TituloHTML(), urldoc.URL, urldoc.URLMostrar(), urldoc.Resume,
                                urldoc.Tags, urldoc.Localizacion_name, urldoc.Domain, urldoc.Datastreams_feed, urldoc.Website,
                                urldoc.Elevacion, urldoc.Latitud, urldoc.Longitud, urldoc.ConceptosLista(), Consulta);
            }

            nuevoDS.Tables.Add(table1);
            return nuevoDS;
        }

        private string ExpandirConsulta(string Consulta, string idioma)
        {
            //Realiza la expansión de la consulta a través del servicio.
            ExpancionConsulta ex = new ExpancionConsulta();
            return  ex.ExpandirConsulta(SemanticIndexManager.RutaOntologia, Consulta, idioma);
        }

        private string BuscarConceptosOntologia(string Concepto, string idioma)
        {
            //Realiza la expansión de la consulta a través del servicio.
            ExpancionConsulta ex = new ExpancionConsulta();
            return ex.RetornarConceptosOntologia(SemanticIndexManager.RutaOntologia, Concepto, idioma);
        }

        private string CargarConceptosOntologia(string nuevaontologia)
        {
            srvExpansionConsulta.ExpancionConsulta ex = new srvExpansionConsulta.ExpancionConsulta();
            string conceptos = ex.CargarConceptos(nuevaontologia);
            return conceptos;
        }

        private string ObtenerConfig(string key)
            {
                return ConfigurationManager.AppSettings[key];
            }

        private string AsignarConfig(string key, string valor)
            {
                try
                {
                    // Get the application configuration file.
                    System.Configuration.Configuration config =
                      System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");

                    // Update the configuration file appSettings section.
                    config.AppSettings.Settings.Remove(key);
                    config.AppSettings.Settings.Add(key, valor);

                    // Save the configuration file.
                    config.Save(System.Configuration.ConfigurationSaveMode.Modified);

                    //Configuración guardada correctamente
                    return "ok";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

        #endregion

        #region "Métododos de Calificaciones"
        /// <summary>
        /// Método que almacena una Consulta
        /// </summary>       
        /// <param name="myConsulta">Objeto de tipo Consulta que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizó con exito, de lo contrario retorna 0</returns>
        [WebMethod]
        public int SaveConsulta(string ConsultaOriginal, string ConsultaExpandida, DateTime FechaConsulta, string Usuario)
        {
            try
            {
                int result = 0;

                SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionMsec"].ConnectionString);

                SqlCommand myCommand = new SqlCommand("spConsultaUpsert", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Consulta_Id", -1);
                myCommand.Parameters.AddWithValue("@Consulta_original", ConsultaOriginal);
                myCommand.Parameters.AddWithValue("@Consulta_expandida", ConsultaExpandida);
                myCommand.Parameters.AddWithValue("@FechaConsulta", FechaConsulta);
                myCommand.Parameters.AddWithValue("@Usu_Login", Usuario);

                DbParameter returnValue;
                returnValue = myCommand.CreateParameter();
                returnValue.Direction = ParameterDirection.ReturnValue;
                myCommand.Parameters.Add(returnValue);

                myConnection.Open();
                myCommand.ExecuteNonQuery();
                result = Convert.ToInt32(returnValue.Value);
                myConnection.Close();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
 
        /// <summary>
        /// Método que almacena una Calificacion
        /// </summary>       
        /// <param name="myCalificacion">Objeto de tipo Calificacion que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizó con exito, de lo contrario retorna 0</returns>
        [WebMethod]
        public int SaveCalificacion(int ConsultaId, string feedId, int calificacion)
        {
            try
            {
            int result = 0;
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionMsec"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("spCalificacionUpsert", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@Cal_Id", -1);
            myCommand.Parameters.AddWithValue("@Consulta_Id", ConsultaId);
            myCommand.Parameters.AddWithValue("@feedId", feedId);
            myCommand.Parameters.AddWithValue("@Calificacion", calificacion);


            DbParameter returnValue;
            returnValue = myCommand.CreateParameter();
            returnValue.Direction = ParameterDirection.ReturnValue;
            myCommand.Parameters.Add(returnValue);

            myConnection.Open();
            myCommand.ExecuteNonQuery();
            result = Convert.ToInt32(returnValue.Value);
            myConnection.Close();

            return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
         #endregion
    }
}
