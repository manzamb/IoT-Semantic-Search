using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppSemanticIndex.Pobj;
using RestSharp;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Configuration;
using System.Diagnostics;

namespace AppSemanticIndex.Xively
{
    class RecolectorDocumentosXively
    {
        //Almacena la ruta de los documentos json a recuperar de Xively
        private String _rutaDocumentos = string.Empty;

        //Almacena la ruta en la que se encuentra la ontologia
        private String _UrlOntologia = string.Empty;

        //Almacena la lista de URLsFeed retornados de las consultas
        private List<FeedXively> _ListaUrlsFeed;

        //Almacena la URL Base en la cual se consultara los JSON
        private string _BaseUrl = "";

        //Prooiedades de Clase 
        public String rutaDocumentos
        {
            get { return _rutaDocumentos; }
            set { _rutaDocumentos = value; }
        }

        public String UrlOntologia
        {
            get { return _UrlOntologia; }
            set { _UrlOntologia = value; }
        }

        public List<FeedXively> ListaUrlsFeed
        {
            get { return _ListaUrlsFeed; }
            set { _ListaUrlsFeed = value; }
        }

        public string BaseUrl
        {
            get { return _BaseUrl; }
            set { _BaseUrl = value; }
        }

        //Es la libreria que permite manipular la Ontología de dominio a utilizar
        private OntologyManager _OntologyManager = null;

        //Constructor de la Clase
        public RecolectorDocumentosXively(String UbicacionOntologia, String URLBaseAPIXively, String UbicacionDocumentos)
        {
            UrlOntologia = UbicacionOntologia;
            BaseUrl = URLBaseAPIXively;
            rutaDocumentos = UbicacionDocumentos;

            //La ruta de la ubicación de la ontología se cambio en el Web.config
            _OntologyManager = new OntologyManager(UrlOntologia);
        }

        //Constructor de clase sin ontologia
        public RecolectorDocumentosXively(String URLBaseAPIXively, String UbicacionDocumentos)
        {
            BaseUrl = URLBaseAPIXively;
            rutaDocumentos = UbicacionDocumentos;

            //La ruta de la ubicación de la ontología se cambio en el Web.config
            _OntologyManager = new OntologyManager(UrlOntologia);
        }

        //Procedimientos para obtener los conceptos de la ontologia y crear la lista de URlsFeed
        //Este procedimiento obtiene una lista de conceptos de la ontología especificada en UrlOntologia
        public List<String> ObtenerListaConceptos()
        {
            List<String> LConceptos = new List<string>();
            foreach (OntologyConcept ontologyConcept in _OntologyManager.Concepts.Values)
            {
                LConceptos.Add(ToAscii(ontologyConcept.ConceptValue).ToLower());
            }
            return LConceptos;
        }

        public List<String> ObtenerListaConceptosmasIndividuos()
        {
            List<String> LConceptos = new List<string>();
            foreach (string concepto in _OntologyManager.Valores)
            {
                LConceptos.Add(concepto.ToLower());
            }
            return LConceptos;
        }

        private string ToAscii(string sOriginal)
        {
            //Este procedimiento Recupera la cadena con acentos (Español)
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] byteArray = Encoding.Default.GetBytes(sOriginal);
            byte[] asciiArray = Encoding.Convert(Encoding.UTF8, Encoding.Default, byteArray);
            string finalString = Encoding.Default.GetString(asciiArray);
            return finalString;
        }

        //Este procedimiento calcula la lista de feed retornados por la consulta de conceptos y lo guarda en ListaUrlsFeed.
        //Adicionalmente almacena en el directorio cada json que corresponda a la consulta del concepto
        public void ObtenerUrlsFeedConceptos(Boolean Obtenerfulljson)
        {
            //Recuperamos los conceptos de la ontología
            //List<String> LConceptos = ObtenerListaConceptos();
            List<String> LConceptos = ObtenerListaConceptosmasIndividuos();

            //Inicializamos el vector en el que se guarda cada urlfeed
            ListaUrlsFeed = new List<FeedXively>();

            //Variable temporal de los datos de cada feed
            FeedXively tempFeed;

            //Eliminamos los JSON existentes en el directorio con el fin de que el proceso sea más rápido y limpio
            EliminarBDDJSON();

            //Realizamos consultas a Xively por cada concepto
            foreach (string Concepto in LConceptos)
            {
                try
                {
                    var client = new RestClient();

                    // Asigna el directorio base del servidor IoT
                    client.BaseUrl = BaseUrl;

                    //Crea la autenticación al servidor
                    client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["usuarioXively"], ConfigurationManager.AppSettings["passwordXively"]);

                    //Crea el obj webRequest para interactuar con el servidor
                    var request = new RestRequest();

                    //request.Resource. Ejemplo "v2/feeds?q=arduino". Adicionalmente se solicita solo los resumenes con: content=summary
                    //Por defecto retorna json el servidor Xively
                    if (Obtenerfulljson)
                    {
                        request.Resource = ConfigurationManager.AppSettings["urlAPIXively"] + "?q=" + Concepto;
                    }
                    else
                    {
                        request.Resource = ConfigurationManager.AppSettings["urlAPIXively"] + "?q=" + Concepto + "&content=summary";
                    }

                    //Ejecuta la peticion y devuelve los resultados
                    Trace.WriteLine("Realizando consulta a Xively por el concepto: " + Concepto);
                    var response = client.Execute(request) as RestResponse;

                    // Se verifica la ejecución correcta. It's probably not necessary to test both
                    if (response != null && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                    {
                        //Recuperamos los arreglos de json
                        string temp;
                        temp = "[" + response.Content + "]";
                        var arr = JsonConvert.DeserializeObject<JArray>(temp);
                        foreach (JObject obj in arr)
                        {
                            foreach (JObject obj2 in obj["results"])
                            {
                                //Añadimos el feed a la lista siempre y cuando ya no haya sido procesado
                                if (!ListaUrlsFeed.Exists(element => element.feed.feed.ToString() == (string)obj2["feed"]))
                                {
                                    //Obtenemos los metadatos del feed recuperado
                                    tempFeed = ParserJSON(obj2);
                                    //Guardamos el concepto relacionado a su búsqueda
                                    tempFeed.Conceptos.Add(Concepto);
                                    //Guardamos el FeedXively como Json para su posterior recuperación
                                    Trace.WriteLine("Almacenando JSON: " + tempFeed.pathfeed);
                                    File.WriteAllText(@tempFeed.pathfeed, JsonConvert.SerializeObject(tempFeed, Formatting.Indented));
                                    //Guardamos el JSON original en el objeto FeedXively
                                    tempFeed.DocumentJSON = JsonConvert.SerializeObject(obj2);                                   
                                    //Se almacena en la lista de memoria para su posterior uso
                                    ListaUrlsFeed.Add(tempFeed);
                                }
                                else
                                {
                                    //Aunque el feed ya ha sido agregado es necesario añadir el nuevo concepto a la lista de conceptos del feed existente
                                    //Esto permite dar mayor peso al feed en su búsqueda
                                    int indice = ListaUrlsFeed.FindIndex(element => element.feed.feed.ToString() == (string)obj2["feed"]);
                                    ListaUrlsFeed[indice].Conceptos.Add(Concepto);
                                }
                            }
                        }
                    }
                    else if (response != null)
                    {
                        Console.WriteLine("No se obtuvieron resultados");
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        
        //Esta funcion realiza una parser del objeto JSON al objeto ClienteXively
        //Esto permite manipular los metadatos de los sensores via código.
        //Posteriormente como el objeto se almacena en disco como json, se puede recuperar 
        //directamente con el API json
        public FeedXively ParserJSON(JObject jsonresults)
        {
            try
            {
                //Variable en la cual obtendremos los metadatos del feed recuperado
                FeedXively tempFeed = new FeedXively();
                //Obtenemos las propiedades de ubicación del feed en el sistema
                tempFeed.pathfeed = rutaDocumentos + (string)jsonresults["id"] + ".json";
                //Obtener el url del feed
                //Uri url = new Uri((string)jsonresults["feed"]);
                tempFeed.feed.feed = jsonresults["feed"].ToString();
                //Obtener el id del feed y demás metadatos obligatorios
                tempFeed.feed.id = jsonresults["id"].ToString();
                tempFeed.feed.created = (string)jsonresults["created"];
                //Uri urlcreator = new Uri((string)jsonresults["creator"]);
                tempFeed.feed.creator = jsonresults["creator"].ToString();
                tempFeed.feed.Private = Convert.ToBoolean((string)jsonresults["private"]);
                //Obtner los metadatos no obligatorios
                if (!(jsonresults["auto_feed_url"] == null))
                {
                    tempFeed.feed.feed = jsonresults["auto_feed_url"].ToString();
                }
                if (!(jsonresults["description"] == null))
                {
                    tempFeed.feed.description = (string)jsonresults["description"];
                }
                if (!(jsonresults["title"] == null))
                {
                    tempFeed.feed.title = (string)jsonresults["title"];
                }
                if (!(jsonresults["updated"] == null))
                {
                    tempFeed.feed.updated = (string)jsonresults["updated"];
                }
                if (!(jsonresults["website"] == null))
                {
                    //Uri urlwebsite = new Uri((string)jsonresults["website"]);
                    tempFeed.feed.website = jsonresults["website"].ToString();
                }
                if (!(jsonresults["tags"] == null))
                {
                    //Almacena los tags de todo el feed
                    List<string> tempst = JsonConvert.DeserializeObject<List<string>>(jsonresults["tags"].ToString());
                    tempFeed.feed.tags = new string[tempst.Count];
                    int j = 0;
                    foreach (string temps in tempst)
                    {
                        tempFeed.feed.tags[j] = temps;
                        j++;
                    }
                }
                if (!(jsonresults["location"] == null))
                {
                    tempFeed.feed.location = new Location();
                    if (!(jsonresults["location"]["name"] == null))
                    {
                        tempFeed.feed.location.name = (string)jsonresults["location"]["name"];
                    }
                    if (!(jsonresults["location"]["lat"] == null))
                    {
                        tempFeed.feed.location.lat = (string)jsonresults["location"]["lat"];
                    }
                    if (!(jsonresults["location"]["lon"] == null))
                    {
                        tempFeed.feed.location.lon = (string)jsonresults["location"]["lon"];
                    }
                    if (!(jsonresults["location"]["ele"] == null))
                    {
                        tempFeed.feed.location.ele = (string)jsonresults["location"]["ele"];
                    }
                    if (!(jsonresults["location"]["exposure"] == null))
                    {
                        tempFeed.feed.location.exposure = (Exposure)Enum.Parse(typeof(Exposure), (string)jsonresults["location"]["exposure"]);
                    }
                    if (!(jsonresults["location"]["disposition"] == null))
                    {
                        tempFeed.feed.location.disposition = (Disposition)Enum.Parse(typeof(Disposition), (string)jsonresults["location"]["disposition"]);
                    }
                    if (!(jsonresults["location"]["domain"] == null))
                    {
                        tempFeed.feed.location.domain = (Domain)Enum.Parse(typeof(Domain), (string)jsonresults["location"]["domain"]);
                    }
                }

                tempFeed.feed.status = (Status)Enum.Parse(typeof(Status), (string)jsonresults["status"]);

                //En caso de traer el summary estos objetos son nulos
                if (!(jsonresults["datastreams"] == null))
                {
                    //Datastream dtfeed = new Datastream();
                    tempFeed.feed.datastreams = new Datastream[jsonresults["datastreams"].Count()];
                    int dtscount = 0;
                    foreach (JObject obj3 in jsonresults["datastreams"])
                    {
                        Datastream dtfeed = new Datastream();

                        dtfeed.id = (string)obj3["id"];
                        if (!(obj3["at"] == null))
                        {
                            dtfeed.at = (string)obj3["at"];
                        }
                        //Si hay un valor actual, tambien hay min_value y max_value
                        if (!(obj3["current_value"] == null))
                        {
                            dtfeed.current_value = (string)obj3["current_value"];
                            dtfeed.max_value = (string)obj3["max_value"];
                            dtfeed.min_value = (string)obj3["min_value"];
                        }
                        
                        if (!(obj3["tags"] == null))
                        {
                            List<string> tempstr = JsonConvert.DeserializeObject<List<string>>(obj3["tags"].ToString());
                            dtfeed.tags = new string[tempstr.Count];
                            int i = 0;
                            foreach (string temps in tempstr)
                            {
                                dtfeed.tags[i] = temps;
                                i++;
                            }
                        }

                        Unit unidad = new Unit();
                        if (!(obj3["unit"] == null))
                        {
                            if (!(obj3["unit"]["symbol"] == null))
                            {
                                unidad.symbol = (string)obj3["unit"]["symbol"];
                            }
                            if (!(obj3["unit"]["label"] == null))
                            {
                                unidad.label = (string)obj3["unit"]["label"];
                            }
                            if (!(obj3["unit"]["unitType"] == null))
                            {
                                unidad.unitType = (IFCClassification)Enum.Parse(typeof(IFCClassification), (string)obj3["unit"]["unitType"]);
                            }
                        }
                        dtfeed.unit = unidad;

                        //Almacenando Datapoints
                        if (!(obj3["datapoints"] == null))
                        {
                            dtfeed.datapoints = new Datapoint[obj3["datapoints"].Count()];
                            int dtpcount = 0;
                            foreach (JObject obj4 in obj3["datapoints"])
                            {
                                Datapoint dtp = new Datapoint();

                                if (!(obj4["at"] == null))
                                {
                                    dtp.at = (string)obj4["at"];
                                    dtp.value = (string)obj4["value"];
                                }
                                dtfeed.datapoints[dtpcount] = dtp;
                                dtpcount++;
                            }
                        }

                        //Agregamos el datastream a la lista de datastream del feed
                        tempFeed.feed.datastreams[dtscount] = dtfeed;
                        dtscount++;
                    }
                }
                return tempFeed;
            }
            catch (Exception ex)
            {
                throw new System.ArgumentException("Parameter cannot be null: " + ex.ToString(), "original");
            }
           
        }

        //Esta funcion Elimina Todos los archivos de la BDD
        public void EliminarBDDJSON()
        {
            foreach (string fichero in Directory.GetFiles(_rutaDocumentos, "*.*"))
            {
                File.Delete(fichero);
            }
        }

        //Carga los FeedXively (json) de la BDD con el fin de reindexar
        public List<FeedXively> LoadJSONfromDiskDocumentsURL()
        {
            List<FeedXively> UrlFeedstmp = new List<FeedXively>();
            foreach (var nombreArchivo in Directory.GetFiles(rutaDocumentos))
            {
                Trace.WriteLine("Cargando documentos de BDD Local llamado:" + nombreArchivo);
                FeedXively feedtmp = JsonConvert.DeserializeObject<FeedXively>(File.ReadAllText(nombreArchivo));
                UrlFeedstmp.Add(feedtmp);
            }
            return UrlFeedstmp;
        }

        //Esta funcion retorna el JSON del feed de la BDD
        public FeedXively ObtenerJsonFeedBDD(string IdFeed)
        {
            string nombrejson = rutaDocumentos + IdFeed + ".json";

            if (File.Exists(nombrejson))
            {
                using (StreamReader reader = File.OpenText(nombrejson))
                {
                    //Parsing al json directamente desde la carpeta de lectura
                    FeedXively feedtmp = JsonConvert.DeserializeObject<FeedXively>(File.ReadAllText(nombrejson));

                    //Retornamos el strin serializado del json
                    return feedtmp;
                }
            }
            else
            {
                Console.Write("No se encontró el arcvhio del JSON en la ruta especificada");
                return null;
            }
        }

        //Función para Extraer de Xively un JSON específico dado su identificador
        public void AdjuntarJSONFeedBDD(string IdFeed)
        {
            //Recuperamos los conceptos de la ontología
            //List<String> LConceptos = ObtenerListaConceptos();
            List<String> LConceptos = ObtenerListaConceptosmasIndividuos();

            //Inicializamos el vector en el que se guarda cada urlfeed
            ListaUrlsFeed = new List<FeedXively>();

            //Variable temporal de los datos de cada feed
            FeedXively tempFeed;

            //Realizamos consultas a Xively por el feed y buscamos sus conceptos relacionados
            var client = new RestClient();

            // Asigna el directorio base del servidor IoT
            client.BaseUrl = BaseUrl;

            //Crea la autenticación al servidor
            client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["usuarioXively"], ConfigurationManager.AppSettings["passwordXively"]);

            //Crea el obj webRequest para interactuar con el servidor
            var request = new RestRequest();

            //request.Resource. Ejemplo "v2/feeds?q=arduino". Adicionalmente se solicita solo los resumenes con: content=summary
            //Por defecto retorna json el servidor Xively
            request.Resource = ConfigurationManager.AppSettings["urlAPIXively"] + IdFeed;


            //Ejecuta la peticion y devuelve los resultados
            Trace.WriteLine("Realizando consulta a Xively por el feed: " + IdFeed);
            var response = client.Execute(request) as RestResponse;

            // Se verifica la ejecución correcta. It's probably not necessary to test both
            if (response != null && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
            {
                //Recuperamos los arreglos de json
                string temp;
                temp = "[" + response.Content + "]";
                var arr = JsonConvert.DeserializeObject<JArray>(temp);
                foreach (JObject obj in arr)
                {
                    //Ejecuta la peticion y devuelve los resultados
                    Trace.WriteLine("Anotando Feed con respecto la Ontología: " + IdFeed);

                    //Obtenemos los metadatos del feed recuperado
                    tempFeed = ParserJSON(obj);
                    //Extraer conceptos relacionados al json. De forma sencilla se busca el concepto en el json
                    
                    string cuerpoBuscar = string.Empty;
                    cuerpoBuscar += tempFeed.feed.description + 
                                    tempFeed.feed.title +
                                    ConvertStringArrayToString(tempFeed.feed.tags);
                    Datastream[] dts = (Datastream[])tempFeed.feed.datastreams;
                    for (int i = 0; i < dts.Length; i++)
                    {
                        cuerpoBuscar += dts[i].id;
                        string[] tagdts = (string[])(dts[i].tags);
                        cuerpoBuscar += ConvertStringArrayToString(tagdts);
                    }
                    cuerpoBuscar = cuerpoBuscar.ToLower();

                    foreach (string Concepto in LConceptos)
                    {
                        if (cuerpoBuscar.Contains(Concepto))
                            tempFeed.Conceptos.Add(Concepto);
                    }
                    //Guardamos el FeedXively como Json para su posterior recuperación
                    Trace.WriteLine("Almacenando JSON: " + tempFeed.pathfeed);
                    File.WriteAllText(@tempFeed.pathfeed, JsonConvert.SerializeObject(tempFeed, Formatting.Indented));
                    //Guardamos el JSON original en el objeto FeedXively
                    tempFeed.DocumentJSON = JsonConvert.SerializeObject(obj);                                   
                    //Se almacena en la lista de memoria para su posterior uso
                    ListaUrlsFeed.Add(tempFeed);
                }
            }
        }

        string ConvertStringArrayToString(string[] array)
        {
            //
            // Concatenate all the elements into a StringBuilder.
            //
            StringBuilder strinbuilder = new StringBuilder();
            foreach (string value in array)
            {
                strinbuilder.Append(value);
                strinbuilder.Append(' ');
            }
            return strinbuilder.ToString();
        }

    }
}
