using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace ClienteXivelySockets
{
    public class ClienteXively
    {
        //Variables de Conexión a Xively
        private string _ApiKey = "";
        private string _FeedId = "";
        private string _MsgDatos = "";

        //Propiedades de clase
        public string ApiKey
        {
            get { return _ApiKey; }
            set { _ApiKey = value; }
        }

        public string FeedId
        {
            get { return _FeedId; }
            set { _FeedId = value; }
        }

        public string MsgDatos
        {
            get { return _MsgDatos; }
            set { _MsgDatos = value; }
        }

        //Constructor de clase con los datos necesatrios
        public ClienteXively(String XivelyApiKey, String XivelyFeedId)
        {
            _ApiKey = XivelyApiKey;
            _FeedId = XivelyFeedId;
        }

        //Metodos necesarios para la comunicación
        public void CrearMediciones(string MensajeDatos, ref string MsjServidor)
        {
            try
            {
                _MsgDatos = MensajeDatos;
                // Convierte los datos en un array de bytes
                byte[] buffer = Encoding.UTF8.GetBytes(MensajeDatos);


                // Enviar los Datos a Xively
                using (Socket connection = Connect("api.xively.com", 5000))
                {
                    //SendRequest(connection, _ApiKey, _FeedId, MensajeDatos);
                    byte[] contentBuffer = Encoding.UTF8.GetBytes(MensajeDatos);
                    const string CRLF = "\r\n";
                    var requestLine =
                        "PUT /v2/feeds/" + _FeedId + ".csv HTTP/1.1" + CRLF;
                    byte[] requestLineBuffer = Encoding.UTF8.GetBytes(requestLine);
                    var headers =
                        "Host: api.xively.com" + CRLF +
                        "X-ApiKey: " + _ApiKey + CRLF +
                        "Content-Type: text/csv" + CRLF +
                        "Content-Length: " + contentBuffer.Length + CRLF +
                        CRLF;
                    byte[] headersBuffer = Encoding.UTF8.GetBytes(headers);
                    connection.Send(requestLineBuffer);
                    connection.Send(headersBuffer);
                    connection.Send(contentBuffer);
                
                    //Recibir los datos de Xively
                    // status code is at positions 9 to 11, e.g.,
                    // "HTTP/1.1 200..."
                    var bufferR = new byte[12];
                    var i = 0;
                    while (i != 12)
                    {
                        int read = connection.Receive(bufferR, i, 1, SocketFlags.None);
                        i = i + 1;
                    }
                    const int zero = (int)'0';
                    int statusCode =
                    100 * (bufferR[9] - zero) +
                    10 * (bufferR[10] - zero) +
                    (bufferR[11] - zero);
                    Console.WriteLine("Response status code = " + statusCode);
                }
             }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public int LeerMedicion(ref string MsjServidor)
        {
            try
            {
                // produce request
                var requestUri =
                    "http://api.xively.com/v2/feeds/" + _FeedId + ".csv";
                var request = (HttpWebRequest)WebRequest.Create(requestUri);
                
                request.Method = "GET";

                // headers
                request.Headers.Add("X-ApiKey", _ApiKey);

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
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public int EliminarMedicion(string MedidaBorrar, ref string MsjServidor)
        {
            try
            {
                // produce request
                var requestUri =
                    "http://api.xively.com/v2/feeds/" + _FeedId + "/datastreams/" + MedidaBorrar;
                var request = (HttpWebRequest)WebRequest.Create(requestUri);
                
                request.Method = "DELETE";

                // headers
                request.Headers.Add("X-ApiKey", _ApiKey);

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
                Console.WriteLine(ex.Message);
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
            Stream dataStream = response.GetResponseStream();

            // Abrimos un stream y usamos StreamReader para facilidad de acceso.
            StreamReader reader = new StreamReader(dataStream);

            // Leemos el contenido
            MsjServidor = MsjServidor + reader.ReadToEnd();
            dataStream.Close();
            reader.Close();
            return MsjServidor;
        }

        static Socket Connect(string host, int timeout)
        {
            // look up host's domain name to find IP address(es)
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            // extract a returned address
            IPAddress hostAddress = hostEntry.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, 80);

            // connect!
            Console.WriteLine("connect...");
            var connection = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            connection.Connect(remoteEndPoint);
            connection.SetSocketOption(SocketOptionLevel.Tcp,
                SocketOptionName.NoDelay, true);
            connection.SendTimeout = timeout;
            return connection;
        }

    }
}
