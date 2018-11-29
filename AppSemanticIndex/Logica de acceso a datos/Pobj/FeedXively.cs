using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppSemanticIndex;

//Esta clase permite crear información relevante de un feed al momento de consultar la BDD
namespace AppSemanticIndex.Pobj
{
    [Serializable]
    public class FeedXively
    {
        //Almacena todos los campos que vienen en un json de Xively
        public Feed feed { get; set; }

        //Variables adicionales para manejo interno del procesamiento
        public string pathfeed { get; set; }
        public string DocumentJSON { get; set; }

        //Almacena el concepto relacionado a la búsqueda
        public List<string> Conceptos = new List<string>();
        //public string concepto = string.Empty;

        //Alamacena los lugares con los que se puede relacionar el feed
        public List<lugar> lugares;

        public FeedXively()
        {
            feed = new Feed();
        }

        public FeedXively(string Idfeed, string URLfeed, string JSONDoc, string Feedpath)
        {
            feed = new Feed();
            feed.id = Idfeed;
            //Xively almacena el uri del feed en el campo feed
            feed.feed = URLfeed;
            DocumentJSON = JSONDoc;
            pathfeed = Feedpath;
        }

    }
}
