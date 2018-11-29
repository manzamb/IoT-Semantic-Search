//Librerias framewwork .NET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Web;
using System.Configuration;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
//Libreria del Indexador Lucene
using Lucene.Net.Index;
//Libreria para el manejo de Json Sharp
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
//Librerias desarrolladas por la Aplicación
using AppSemanticIndex.Xively;
using AppSemanticIndex.Properties;
using AppSemanticIndex.Pobj;

namespace AppSemanticIndex
{
    public class SemanticIndexManager
    {
        #region "Prpiedades de Clase"

        //Corresponde al documento que contiene todos los documentos recuperados (BDD)
        private string DocumentFileURL = string.Empty;
        //Corresponde al documento que tiene los pesos de los documentos que se usan al momento de la Búsqueda
        private string DocumentFileWF = string.Empty;

        //Corresponde al directorio en el cual se encuentra el índice creado por Luce.NET
        public static string DocumentFileLucene = HttpContext.Current.Server.MapPath("./SemanticIndex");
        //Corresponde a la Ruta y nombre de archivo en donde se encuentra la ontología en la aplicacion web
        public static string RutaOntologia = HttpContext.Current.Server.MapPath("./Ontology/") + ConfigurationManager.AppSettings["FileOntology"];

        //Corresponde a la Ruta en el cual se guardaran los JSON a indexar
        public static string RutaBDDJSON = HttpContext.Current.Server.MapPath("./App_Data/Json_Data/");
        //Corresponde al directorio de las ontologias
        public static string RutaOntologiaSinArchivo = HttpContext.Current.Server.MapPath("./Ontology/");
        public string ruta = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

        //Coresponde a la lista de urlFeed que se recuperan de Xively
        private List<FeedXively> UrlFeeds;

        //Esta sección define los vinculos a las librerias de terceros que se usan en el proyecto
        //Objeto de clase que obtiene los JSON dada una Ontología OWL que reemplaza _ScootWater
        private XivelyManager _XivelyManager = null;

        //Es la Libreria que permite crear el índice semántico
        private LuceneManager _LuceneManager = null;
        //Es la libreria que permite manipular la Ontología de dominio a utilizar
        private OntologyManager _OntologyManager = null;
        //Es una estructura de datos que permite manejar en forma de lista todos los documentos que se analizan
        private Dictionary<string, UrlDocument> URLResult = new Dictionary<string, UrlDocument>();

        #endregion

        public SemanticIndexManager()
        {
            //Obtienen las Rutas para almacenar
            DocumentFileURL = HttpContext.Current.Server.MapPath("./")+"DocumentsUrl.txt";
            DocumentFileWF = HttpContext.Current.Server.MapPath("./")+ "DocumentsWF.txt";
           
            //Inicializa las clases principales para la indexación
            _XivelyManager = new XivelyManager(RutaOntologia, RutaBDDJSON);
            _LuceneManager = new LuceneManager();
            _OntologyManager = new OntologyManager(RutaOntologia);
        }

        #region "Crear o Recuperar el Índice Semántico"

        private string GetFolderSemanticIndexWithPath(string path)
        {
            return Path.Combine("SemanticIndex", path);
        }

        public void CrearIndiceSemantico(Boolean usarXively)
        {
            //Iniciamos el proceso de creación de la BDD a partir de los conceptos de la Ontología
            //Consultando el servidor IoT Xively y almacenando los JSON en un directorio local
            //Este permite traer todo el contenido de los json en una sola petición por concepto consultado.
            if (usarXively)
            {
                Trace.WriteLine("Iniciando el proceso de traer documentos de Xively");
                //true es para que no traiga recumenes sino todo el json, es más rápido
                UrlFeeds = _XivelyManager.ProcesarDocumentos(true);
            }
            else
            {
                //Cargar la información del Disco
                Trace.WriteLine("Procesando los documentos de BDD Local");
                UrlFeeds = LoadJSONfromDiskDocumentsURL();
            }
         
            //Creando los documentos virtuales de Lucene
            Trace.WriteLine("Creando documentos Lucene de Xively");
            URLResult = AnalizarDocumentosBDD();

            //Se Agregan todas los documentos que se encuentren el el Diccionario URLResult.Values
            //Se indexa a Lucene cada documento
            Trace.WriteLine("Indexando los documentos");
            LuceneManager.AddUpdateLuceneIndex(URLResult,true);
        }

        //Procedimiento para Realizar la busqueda en el índice
        public List<UrlDocument> BuscarSemanticIndex(string Consulta, Boolean usarEspañol)
        {
            //Se realiza la búsqueda solicitada
            return LuceneManager.BuscarEnIndiceSemantico(Consulta, usarEspañol);
        }

        private Dictionary<string, UrlDocument> AnalizarDocumentosBDD()
        {
            Dictionary<string, UrlDocument> UrlResultTemp = new Dictionary<string, UrlDocument>();

            foreach (FeedXively feed in UrlFeeds)
            {
                //RecolectorDocumentosXively recx;
                UrlDocument docLucene = new UrlDocument();
                
                //Obtiene el Id del feed como clave para el indice
                docLucene.Id = feed.feed.id.ToString();
                docLucene.URL = feed.feed.feed.ToString();
                Trace.WriteLine("Creando Documento Lucene: " + docLucene.Id);

                //Obtiene el documento JSON original con etiquetas
                if (!string.IsNullOrEmpty(feed.DocumentJSON))
                {
                    docLucene.DocumentUnParsed = feed.DocumentJSON;
                }
                //Relaciona los conceptos por los cuales que se encontró el documento en el servidor
                docLucene.Conceptos = feed.Conceptos;

                //Obtiene un resumen del Documento
                //documentResume = _HTMLParseManager.GetResume(documentParsed);
                if (!string.IsNullOrEmpty(feed.feed.description))
                {
                    docLucene.Resume = feed.feed.description;
                }

                //Obtiene el Titulo del Documento
                if (!string.IsNullOrEmpty(feed.feed.title))
                {
                    docLucene.Tittle = feed.feed.title;
                }

                //Obtiene las anotaciones
                if (feed.feed.tags != null)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (string tag in feed.feed.tags)
                    {
                        builder.Append(tag).Append(" ");
                    }
                    docLucene.Tags = builder.ToString();
                }

                //Obtiene el sitio web de informacion adicional
                if (feed.feed.website != null)
                {
                    docLucene.Website = feed.feed.website.ToString();
                }

                //Obtiene los datos de la localización
                if (feed.feed.location != null)
                {
                    if (feed.feed.location.name != null) docLucene.Localizacion_name = feed.feed.location.name;
                    docLucene.Domain = feed.feed.location.domain.ToString();
                    if (feed.feed.location.ele != null) docLucene.Elevacion = feed.feed.location.ele;
                    if (feed.feed.location.lat != null) docLucene.Latitud = feed.feed.location.lat;
                    if (feed.feed.location.lon != null) docLucene.Longitud = feed.feed.location.lon;
                }

                //Obtiene una Lista de datastreams del feed en una cadena con el fin de darsela a Lucene para su indexación
                if (feed.feed.datastreams!= null)
                {
                    foreach(Datastream Dts in feed.feed.datastreams)
                    {
                        docLucene.Datastreams_feed = Dts.id + " " + Dts.unit.symbol + " " + Dts.unit.label + " ";
                        string listatags=string.Empty;
                        if(Dts.tags != null)
                            foreach (string tg in Dts.tags)
                                listatags += "," + tg.ToString();
                        docLucene.Datastreams_feed = listatags;
                    }
                }

                //Finalmente esta información se coloca en un solo string, pór posibles reusos
                string temp = string.Empty;
                if (!string.IsNullOrEmpty(docLucene.Resume)) temp = temp + " " + docLucene.Resume + " ";
                if (!string.IsNullOrEmpty(docLucene.Tittle)) temp = temp + " " + docLucene.Tittle + " ";
                if (!string.IsNullOrEmpty(docLucene.Tags)) temp = temp + " " + docLucene.Tags + " ";
                if (!string.IsNullOrEmpty(docLucene.Website)) temp = temp + " " + docLucene.Website + " ";
                if (!string.IsNullOrEmpty(docLucene.ConceptosLista())) temp = temp + " " + docLucene.ConceptosLista() + " ";
                if (!string.IsNullOrEmpty(docLucene.Localizacion_name)) temp = temp + " " + docLucene.Localizacion_name + " ";
                if (!string.IsNullOrEmpty(docLucene.Domain)) temp = temp + " " + docLucene.Domain + " ";
                if (!string.IsNullOrEmpty(docLucene.Datastreams_feed)) temp = temp + " " + docLucene.Datastreams_feed + " ";
                docLucene.DocumentParsed = docLucene.DocumentParsed + " " + temp;

                //Estos datos no aportan a la búsqueda de texto por ello no se incluyen
                //docLucene.Elevacion + " " +
                //docLucene.Latitud + " " +
                //docLucene.Longitud + " " +

                //Pasamos aminusculas todo el texto ya que algunos campos no se analizaron
                docLucene.DocumentParsed = docLucene.DocumentParsed.ToLower();

                //Crear el diccionario de UrlResult analizados
                UrlResultTemp.Add(feed.feed.feed.ToString(), docLucene);
            }

            //Guarda los documentos en Disco para que la proxima ves no haga todo el proceso.
            //Esto no es práctico con millones de documentos, pero hace más rápido el analisis inicial
            SaveDocumentsURL(URLResult);

            return UrlResultTemp;
        }

        public List<String> ObtenerListaConceptos()
        {
            RecolectorDocumentosXively recx = new RecolectorDocumentosXively(RutaOntologia, ConfigurationManager.AppSettings["urlBaseXively"], RutaBDDJSON);
            return recx.ObtenerListaConceptos();
        }

        public List<String> ObtenerListaConceptosmasIndividuos()
        {
            RecolectorDocumentosXively recx = new RecolectorDocumentosXively(RutaOntologia, ConfigurationManager.AppSettings["urlBaseXively"], RutaBDDJSON);
            return recx.ObtenerListaConceptosmasIndividuos();
        }

        /// <summary>
        /// Guarda una lista de Objetos desde Fichero
        /// </summary>
        private void SaveDocumentsURL(Dictionary<string, UrlDocument> _URLResult)
        {
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            FileStream stream = new FileStream(DocumentFileURL, FileMode.Create);
            try
            {
                formatter.Serialize(stream, _URLResult);
            }
            finally
            {
                stream.Close();
            }
        }

        /// <summary>
        /// Cargar una lista de Objetos desde Fichero
        /// </summary>
        private Dictionary<string, UrlDocument> LoadDiskDocumentsURL()
        {
            Dictionary<string, UrlDocument> _URLResult = new Dictionary<string, UrlDocument>();

            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            FileStream stream = new FileStream(DocumentFileURL, FileMode.Open, FileAccess.Read, FileShare.None);

            try
            {
                _URLResult = (Dictionary<string, UrlDocument>)formatter.Deserialize(stream);
            }
            finally
            {
                stream.Close();
            }

            return _URLResult;
        }

        //Cargar la lista de documentos a analizar del disco.
        private List<FeedXively> LoadJSONfromDiskDocumentsURL()
        {
            return _XivelyManager.LoadJSONfromDiskDocumentsURL();
        }

        //Obtiene un JSON (FeedXively serializado) de la BDD
        public FeedXively ObtenerJSONFeedBDD(string idFeed)
        {
            return _XivelyManager.ObtenerJsonFeedBDD(idFeed);
        }

        //Obtiene un JSON serializado con json del Servidor Xively
        public string ObtenerJsonFeed(string APIkey, string feedID)
        {
            return _XivelyManager.ObtenerJsonFeed(APIkey, feedID);
        }

        //Hace una consulta a Xiveli por datpoint. Se necesita tener acceso al feed
        public FeedXively  RetornarDatosSensor(string feedID, DateTime fechaInicio, DateTime fechaFin)
        {
            return _XivelyManager.RetornarDatosSensor(feedID, fechaInicio, fechaFin);
        }
        //Hace una consulta a Xively por los datapoints
        public string RetornarDatapointsFeed(string feedID, string DatastreamId, DateTime fechaInicio, DateTime fechaFin)
        {
            return _XivelyManager.RetornarDatapointsFeed(feedID, DatastreamId, fechaInicio, fechaFin);
        }
        #endregion

        private Dictionary<string, UrlDocument> AnalizarDocumento(FeedXively feed)
        {
            Dictionary<string, UrlDocument> UrlResultTemp = new Dictionary<string, UrlDocument>();

            //RecolectorDocumentosXively recx;
            UrlDocument docLucene = new UrlDocument();

            //Obtiene el Id del feed como clave para el indice
            docLucene.Id = feed.feed.id.ToString();
            docLucene.URL = feed.feed.feed.ToString();
            Trace.WriteLine("Creando Documento Lucene: " + docLucene.Id);

            //Obtiene el documento JSON original con etiquetas
            if (!string.IsNullOrEmpty(feed.DocumentJSON))
            {
                docLucene.DocumentUnParsed = feed.DocumentJSON;
            }
            //Relaciona los conceptos por los cuales que se encontró el documento en el servidor
            docLucene.Conceptos = feed.Conceptos;

            //Obtiene un resumen del Documento
            //documentResume = _HTMLParseManager.GetResume(documentParsed);
            if (!string.IsNullOrEmpty(feed.feed.description))
            {
                docLucene.Resume = feed.feed.description;
            }

            //Obtiene el Titulo del Documento
            if (!string.IsNullOrEmpty(feed.feed.title))
            {
                docLucene.Tittle = feed.feed.title;
            }

            //Obtiene las anotaciones
            if (feed.feed.tags != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (string tag in feed.feed.tags)
                {
                    builder.Append(tag).Append(" ");
                }
                docLucene.Tags = builder.ToString();
            }

            //Obtiene el sitio web de informacion adicional
            if (feed.feed.website != null)
            {
                docLucene.Website = feed.feed.website.ToString();
            }

            //Obtiene los datos de la localización
            if (feed.feed.location != null)
            {
                if (feed.feed.location.name != null) docLucene.Localizacion_name = feed.feed.location.name;
                docLucene.Domain = feed.feed.location.domain.ToString();
                if (feed.feed.location.ele != null) docLucene.Elevacion = feed.feed.location.ele;
                if (feed.feed.location.lat != null) docLucene.Latitud = feed.feed.location.lat;
                if (feed.feed.location.lon != null) docLucene.Longitud = feed.feed.location.lon;
            }

            //Obtiene una Lista de datastreams del feed en una cadena con el fin de darsela a Lucene para su indexación
            if (feed.feed.datastreams != null)
            {
                foreach (Datastream Dts in feed.feed.datastreams)
                {
                    docLucene.Datastreams_feed = Dts.id + " " + Dts.unit.symbol + " " + Dts.unit.label + " ";
                    string listatags = string.Empty;
                    if (Dts.tags != null)
                        foreach (string tg in Dts.tags)
                            listatags += "," + tg.ToString();
                    docLucene.Datastreams_feed = listatags;
                }
            }

            //Finalmente esta información se coloca en un solo string, pór posibles reusos
            string temp = string.Empty;
            if (!string.IsNullOrEmpty(docLucene.Resume)) temp = temp + " " + docLucene.Resume + " ";
            if (!string.IsNullOrEmpty(docLucene.Tittle)) temp = temp + " " + docLucene.Tittle + " ";
            if (!string.IsNullOrEmpty(docLucene.Tags)) temp = temp + " " + docLucene.Tags + " ";
            if (!string.IsNullOrEmpty(docLucene.Website)) temp = temp + " " + docLucene.Website + " ";
            if (!string.IsNullOrEmpty(docLucene.ConceptosLista())) temp = temp + " " + docLucene.ConceptosLista() + " ";
            if (!string.IsNullOrEmpty(docLucene.Localizacion_name)) temp = temp + " " + docLucene.Localizacion_name + " ";
            if (!string.IsNullOrEmpty(docLucene.Domain)) temp = temp + " " + docLucene.Domain + " ";
            if (!string.IsNullOrEmpty(docLucene.Datastreams_feed)) temp = temp + " " + docLucene.Datastreams_feed + " ";
            docLucene.DocumentParsed = docLucene.DocumentParsed + " " + temp;

            //Estos datos no aportan a la búsqueda de texto por ello no se incluyen
            //docLucene.Elevacion + " " +
            //docLucene.Latitud + " " +
            //docLucene.Longitud + " " +

            //Pasamos aminusculas todo el texto ya que algunos campos no se analizaron
            docLucene.DocumentParsed = docLucene.DocumentParsed.ToLower();

            //Crear el diccionario de UrlResult analizados
            UrlResultTemp.Add(feed.feed.feed.ToString(), docLucene);


            return UrlResultTemp;
        }


        public void CargarFeedXivelyBDD(string feedId)
        {
            UrlFeeds = _XivelyManager.ProcesarDocumento(feedId);
            //Creando los documentos virtuales de Lucene
            Trace.WriteLine("Creando documentos Lucene de Xively");
            URLResult = AnalizarDocumento(UrlFeeds[0]);

            //Se Agregan todas los documentos que se encuentren el el Diccionario URLResult.Values
            //Se indexa a Lucene el documento
            Trace.WriteLine("Indexando los documentos");
            LuceneManager.ActualizarLuceneIndex(URLResult, true);
        }

        #region "Métodos de Geolocalizacion"
        //Aqui colocamos todas las funciones de geolocalización.
         //Obtener la Lista de sensores que estan cerca de una localización
        public List<UrlDocument> ObtenerSensoresCercanos(string latitud, string longitud)
        {
            Dictionary<string, UrlDocument> resultado = new Dictionary<string, UrlDocument>();
            List<UrlDocument> UrlResult = new List<UrlDocument>();

            if (UrlFeeds == null)
            {
                //Cargamos la información de los JSON para su uso posterior
                UrlFeeds = LoadJSONfromDiskDocumentsURL();
                resultado = AnalizarDocumentosBDD();
            }
            //Analizamos cada documento con respecto las coordenadas de localización
            foreach (UrlDocument UrlDoc in resultado.Values)
            {
                UrlResult.Add(UrlDoc);
            }
            //Se retorna el resultado
            return UrlResult;
        }

        //Obtiene un lista con todas las ubicaciones de un lugar. 
        //Busca cualquier coincidencia del lugar asi este al principio, medio o fin dle nombre real del lugar
        public List<GeonameNode> ObtenerLocalizaciones(string lugar)
        {
            //JavaScriptSerializer serializador = new JavaScriptSerializer();
            List<GeonameNode> geonodes = new List<GeonameNode>();

            GeoNamesManager geoName = new GeoNamesManager();
            geonodes = geoName.GeoNames_SearchByName(lugar);

            return geonodes;
            //return serializador.Serialize(geonodes);
        }

        //Recibe una cadena y verifica cuales términos corresponden a un lugar
        public List<GeonameNode> ObtenerCiudadesConsulta(string Consulta)
        {
            //Nodos detectados
            List<GeonameNode> geonodes = new List<GeonameNode>();

            //Analizar la consulta para eliminar terminos vacios, puntuaciones, etc.
            Consulta =  _LuceneManager.AnalizarConsulta(Consulta, "Sin Analizador");

            //Separar la consulta en palabras sueltas
            string[] terminos = ObtenerTerminos(Consulta);

            //Probar en geonames si se encuentra el lugar
            for (int i = 0; i < terminos.Length; i++)
            {
                List<GeonameNode> geontemp = new List<GeonameNode>();

                //Sacamos todas las posibles localizaciones del termino buscando el nombre identico
                GeoNamesManager geoName = new GeoNamesManager();
                geontemp = geoName.GeoNames_SearchByNameEquals(terminos[i]);

                //Si encontro localizaciones entonces obtener la primera opcion, sino no lo adiciona
                foreach (GeonameNode gnitem in geontemp)
                {
                    geonodes.Add(gnitem);
                    break;
                }
            }
            return geonodes;
        }

        //Recibe coordenadas de Latitutd y longitud y retorna el lugar más cercano
        public GeonameNode ObtenerLugardeCoordenadas(string latitud, string longitud)
        {
            GeoNamesManager geoName = new GeoNamesManager();
            return geoName.GeoNames_FindNearbyPlaceName(latitud, longitud);
        }

        public List<GeonameNode> ObtenerLugardeCoordenadasJerarquia(string latitud, string longitud)
        {
            GeoNamesManager geoName = new GeoNamesManager();
            return geoName.GeoNames_ExtendedFindNearby(latitud, longitud);
        }
 
        public List<GeonameNode> GeoNames_Hierarchy(int GeonameId)
        {
            GeoNamesManager geoName = new GeoNamesManager();
            return geoName.GeoNames_Hierarchy(GeonameId);
        }

        public double VerificarSensorenLugar(double latitud, double longitud, double LatSensor, double LonSensor, double radio)
        {
            GeoNamesManager geoName = new GeoNamesManager();
            return geoName.VerificarSensorenLugar(latitud, longitud, LatSensor, LonSensor, radio,DistanceType.Kilometers);
        }

        private string[] ObtenerTerminos(string cadena)
        {
            char[] delimitadores = { ',', '.' };

            string[] terminos = cadena.Split(delimitadores);

            return terminos;
        }

        #endregion
    }
}