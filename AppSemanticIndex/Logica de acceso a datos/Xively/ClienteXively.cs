using System;
using System.Net;
using System.Net.Sockets;

using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using System.Collections.Generic;
using AppSemanticIndex.Pobj;
using AppSemanticIndex;
using System.Configuration;
using System.Web;
namespace AppSemanticIndex.Xively
{
    public class ClienteXively
    {
        #region "Variables de Conexión a Xively"
 
        private string _BaseUrl = "";
        private ApiKey _ApiKey =  new ApiKey();
        private string _MsgDatos = "";
        private FeedXively feedSemantico;

        #endregion

        #region "Propiedades de clase"

        public string BaseUrl
        { 
            get{return _BaseUrl;}
            set { _BaseUrl = value;}
        }

        public string ApiKey 
        { 
            get{return _ApiKey.api_key;}
            set { _ApiKey.api_key = value;}
        }

        public string MsgDatos
        {
            get { return _MsgDatos; }
            set { _MsgDatos = value; }
        }

        #endregion

        #region "Constructor de clase con los datos necesarios"

        public ClienteXively(string XivelyApiKey, string XivelyFeedId)
        {
            feedSemantico = new FeedXively(XivelyFeedId, null, "", "");
            _ApiKey.api_key = XivelyApiKey;
            _BaseUrl = ConfigurationManager.AppSettings["urlBaseXively"] + ConfigurationManager.AppSettings["urlAPIXively"];
        }
        #endregion

        #region "Metodos necesarios para la gestion de mediciones"

        public int CrearMediciones(string MensajeDatos, ref string MsjServidor)
        {
            try
            {
                _MsgDatos = MensajeDatos;
                // Convierte los datos en un array de bytes
                byte[] buffer = Encoding.UTF8.GetBytes(MensajeDatos);

                // produce request
                var requestUri = _BaseUrl + feedSemantico.feed.id + ".csv";

                var request = (HttpWebRequest)WebRequest.Create(requestUri);
                try
                {
                     request.Method = "PUT";

                    // headers
                    request.ContentType = "text/csv";
                    request.ContentLength = buffer.Length;
                    request.Headers.Add("X-ApiKey", _ApiKey.api_key);

                    // content
                    Stream s = request.GetRequestStream();
                    s.Write(buffer, 0, buffer.Length);
                    s.Close();
                }
                finally
                {
                    ((IDisposable)request).Dispose();
                }

                // send request and receive response
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // Consumimos el response
                    MsjServidor = ObtenerRespuesta(response);

                    // Establecemos si la operación fue exitosa
                    if (response.StatusCode.ToString() == "200")
                    {
                        return 1; //El envío fue exitoso
                    }
                    else
                    {
                        return 0; //El envío a fallado
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                return 0;
            }
        }

        public FeedXively LeerMedicion(DateTime start, DateTime end)
        {
            try
            {
                //Variable cliente REST
                var client = new RestClient();
                client.BaseUrl = _BaseUrl;

                //Crea la autenticación al servidor
                client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["usuarioXively"], ConfigurationManager.AppSettings["passwordXively"]);

                //Crea el obj webRequest para interactuar con el servidor
                var request = new RestRequest();

                //Definimos el URL de peticion
                string strrequri = string.Empty;
                if(start != null)
                    strrequri = feedSemantico.feed.id + "?start=" + start.ToUniversalTime().ToString("yyyy'-'MM'-'ddTHH':'mm':'ss'Z'");
                if(end != null)
                    strrequri += "&end=" + end.ToUniversalTime().ToString("yyyy'-'MM'-'ddTHH':'mm':'ss'Z'");

                //Asignamos la peticion al objeto request
                request.Resource = strrequri;
                var response = client.Execute(request) as RestResponse;

                // Se verifica la ejecución correcta.
                if (response != null && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    //Recuperamos los arreglos de json
                    string temp;
                    temp = response.Content;
                    JObject feedjson = JsonConvert.DeserializeObject<JObject>(temp);
                    ////Creamos un recolector para parsear el objeto json recuperado
                    //RecolectorDocumentosXively rd = new RecolectorDocumentosXively(HttpContext.Current.Server.MapPath("./Ontology/") + ConfigurationManager.AppSettings["FileOntology"],
                    //                                                                ConfigurationManager.AppSettings["urlBaseXively"],
                    //                                                                HttpContext.Current.Server.MapPath("./App_Data/Json_Data/"));
                    RecolectorDocumentosXively rd = new RecolectorDocumentosXively(ConfigurationManager.AppSettings["urlBaseXively"],
                                                                                   HttpContext.Current.Server.MapPath("./App_Data/Json_Data/"));
                    FeedXively feedx = rd.ParserJSON(feedjson);
                    return feedx;
                }
                else
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public string RetornarDatapointsFeed(string DatastreamId, DateTime start, DateTime end)
        {
            try
            {
                //Variable cliente REST
                var client = new RestClient();
                client.BaseUrl = _BaseUrl;

                //Crea la autenticación al servidor
                client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["usuarioXively"], ConfigurationManager.AppSettings["passwordXively"]);

                //Crea el obj webRequest para interactuar con el servidor
                var request = new RestRequest();
                string strdatastream = feedSemantico.feed.id + "/datastreams/" + DatastreamId;
                //Definimos el URL de peticion Por defecto Xively SOLO PERMITE UNA VENTANA DE 14 DÍAS DE HISTORICO
                string strrequri = string.Empty;
                if (start != null)
                    strrequri = strdatastream + "?start=" + start.ToUniversalTime().ToString("yyyy'-'MM'-'ddTHH':'mm':'ss'Z'");
                if (end != null)
                    strrequri += "&end=" + end.ToUniversalTime().ToString("yyyy'-'MM'-'ddTHH':'mm':'ss'Z'");

                //Asignamos la peticion al objeto request
                request.Resource = strrequri;
                var response = client.Execute(request) as RestResponse;

                // Se verifica la ejecución correcta.
                if (response != null && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    //Recuperamos los arreglos de json
                    return response.Content;
                }
                else
                {
                    return string.Empty;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }

        public int EliminarMedicion(string MedidaBorrar, ref string MsjServidor)
        {
            try
            {
                // produce request
                var requestUri = _BaseUrl + feedSemantico.feed.id + "/datastreams/" + MedidaBorrar;
                var request = (HttpWebRequest)WebRequest.Create(requestUri);
                try
                {
                    request.Method = "DELETE";

                    // headers
                    request.Headers.Add("X-ApiKey", _ApiKey.api_key);
                }
                finally
                {
                    ((IDisposable)request).Dispose();
                }

                // send request and receive response
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // Consumimos el response
                    MsjServidor = ObtenerRespuesta(response);

                    // Establecemos si la operación fue exitosa
                    if (response.StatusCode.ToString() == "200")
                    {
                        return 1; //El envío fue exitoso
                    }
                    else
                    {
                        return 0; //El envío a fallado
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                return 0;
            }
        }

        public string ObtenerRespuesta(HttpWebResponse response)
        {
            String MsjServidor = "";
            // response status line
            MsjServidor = "HTTP/" + response.ProtocolVersion + " " +
                        response.StatusCode + " " +
                        response.StatusDescription;
            MsjServidor = MsjServidor + "\n";

            // response headers
            string[] headers = response.Headers.AllKeys;
            foreach (string name in headers)
            {
                MsjServidor = MsjServidor + " " + name + ": " + response.Headers[name] + "\n";
            }

            //Creamos un Stream para la lectura
            Stream dataStream = response.GetResponseStream ();

            // Abrimos un stream y usamos StreamReader para facilidad de acceso.
            StreamReader reader = new StreamReader (dataStream);

            // Leemos el contenido
            MsjServidor = MsjServidor + reader.ReadToEnd();
            dataStream.Close();
            reader.Close();
            return MsjServidor;
        }

        #endregion

        #region "Metodos necesarios para la gestion de Metadatos"
        //Este procedimiento retorna el JSON del feed del Servidor que actualmente trabaja la clase
        public string ObtenerJsonFeed()
        {
            try
            {
                //Variable cliente REST
                var client = new RestClient();
                client.BaseUrl = _BaseUrl;

                //Crea la autenticación al servidor
                client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["usuarioXively"], ConfigurationManager.AppSettings["passwordXively"]);

                //Crea el objeto webRequest para interactuar con el servidor
                var request = new RestRequest();

                //Asignamos la peticion al objeto request
                request.Resource = feedSemantico.feed.id;
                var response = client.Execute(request) as RestResponse;

                // Se verifica la ejecución correcta.
                if (response != null && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    string temp;
                    temp = response.Content;
                    //JObject feedjson = JsonConvert.DeserializeObject<JObject>(temp);
                    return temp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2);
                //return string.Empty;
                return ex2.Message.ToString();
            }
        }
        
        #endregion
    }
}
