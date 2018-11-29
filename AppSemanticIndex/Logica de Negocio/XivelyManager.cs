using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AppSemanticIndex.Xively;
using AppSemanticIndex.Pobj;
using System.Configuration;

namespace AppSemanticIndex
{
    class XivelyManager
    {
        //Objeto de clase que obtiene los JSON dada una Ontología OWL
        private RecolectorDocumentosXively _RecolectorXively = null;

        //Objeto de la clase ClienteXively para operar sobre los feed
        private ClienteXively _ClienteXively = null;

        //Variable que almacena el directorio en el cual se almacenará finalmente cada uno de los JSON
        //relacionados a la búsqueda en Xively con los conceptos de la ontologia
        private String _DirectorioJSON =string.Empty;

        //Variable que almacena la ruta de la Ontología que se utilizará para traer los JSON
        private String _RutaOntologia = string.Empty;

        public XivelyManager(String RutaOntologia,String DirectorioJSON)
        {
            _RutaOntologia = RutaOntologia;
            _DirectorioJSON = DirectorioJSON;
            _RecolectorXively = new RecolectorDocumentosXively(_RutaOntologia, ConfigurationManager.AppSettings["urlBaseXively"], DirectorioJSON);
            
            //this.ProcesarDocumentos();
        }

        //Funcion que realiza la obtención de los documentos de xively buscando por cada concepto de la ontologia
        //Almacena los JSON localmente en DirectorioJSON y  retorna la lista de urlfeed correspondientes
        public List<FeedXively> ProcesarDocumentos(Boolean fullJson)
        {
            _RecolectorXively.ObtenerUrlsFeedConceptos(fullJson);
            return _RecolectorXively.ListaUrlsFeed;
        }

        //Carga los FeedXively (json) de la BDD con el fin de reindexar
        public List<FeedXively> LoadJSONfromDiskDocumentsURL()
        {
            return _RecolectorXively.LoadJSONfromDiskDocumentsURL();
        }

        //Este procedimiento retorna el JSON del feed estipulado de la BDD
        public FeedXively ObtenerJsonFeedBDD(string Idfeed)
        {
            return _RecolectorXively.ObtenerJsonFeedBDD(Idfeed);
        }

        //Esta funcion retorna el Json desde el servidor Xively
        public string ObtenerJsonFeed(string APIkey, string feedID)
        {
            _ClienteXively = new ClienteXively(APIkey, feedID);
            return _ClienteXively.ObtenerJsonFeed();
        }

        //Define un objeto ClienteXively con el fin de traer datos directamente del servidor
        public FeedXively RetornarDatosSensor(string feedID,DateTime fechaInicio, DateTime fechaFin)
        {
            _ClienteXively = new ClienteXively(ConfigurationManager.AppSettings["APIkey"], feedID);

            return _ClienteXively.LeerMedicion(fechaInicio, fechaFin);
        }

        //Retorna los datapoints
        public string RetornarDatapointsFeed(string feedID, string DatastreamId, DateTime fechaInicio, DateTime fechaFin)
        {
            _ClienteXively = new ClienteXively(ConfigurationManager.AppSettings["APIkey"], feedID);

            return _ClienteXively.RetornarDatapointsFeed(DatastreamId, fechaInicio, fechaFin);
        }

        //Almacena el JSON en la BDD y lo Anota semánticamente
        public List<FeedXively> ProcesarDocumento(string feedId)
        {
            _RecolectorXively.AdjuntarJSONFeedBDD(feedId);
            return _RecolectorXively.ListaUrlsFeed;
        }
        
     }
}
