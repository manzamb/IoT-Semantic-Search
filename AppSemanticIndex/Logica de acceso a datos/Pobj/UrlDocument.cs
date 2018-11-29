using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AppSemanticIndex.Pobj
{
    [Serializable]
    public class UrlDocument
    {
        #region Atributos que son Indexables

        //Atributo de identificación del documento
        private string id = string.Empty;
        //************* ATRIBUTOS TIPO TEXT PARA LUCENE *******************//
        //Atributo del índice semántico que almacena el Título de la página Web del documento actual
        private string tittle = string.Empty;
        //Atributo del índice semántico que almacena las anotaciones de la página Web del documento actual
        private string tags = string.Empty;
        //Atributo del índice semántico que almacena la descripción (Resumen) de la página Web del documento actual
        private string description = string.Empty;
        //Atributo del índice semántico que almacena una url relacionada al sensor para obtener más informción (utilizar 2 version)
        private string website = string.Empty;

        //************* ATRIBUTOS TIPO KEYWORD Y UNINDEXED PARA LUCENE *******************//
        //Atributo KEYWORD del índice semántico que almacena la dirección Web del sensor (feed) antes url
        private string feed = string.Empty;
        //Atributos del índice semántico que almacena datos de ubicación a indexar KEYWORD como: name y domain. 
        //La información UNINDEXED es: elevacion (ele), latitud (lat), longitud (lon)
        private string localizacion_name = string.Empty;
        private string domain = string.Empty;
        private string elevacion = string.Empty;
        private string latitud = string.Empty;
        private string longitud = string.Empty;

        //Atributo del índice semántico que almacena datos del sensor a indexar como: nombre (Id), símbolo (symbol) y nombre unidad (label)
        //Como un dispositivo puede tener muchos, se colocaran toso en el string seguido uno de otro para su correspondiente indexación
        private string datastreams = string.Empty;

        //Alacena toda la información del documento json que ha sido Analizado
        private string documentParsed = string.Empty;
        //Almacena toda la información del documento original
        private string documentUnParsed = string.Empty;
        //Almacena el concepto relacionado a la búsqueda
        private List<string> conceptos = new List<string>();

        #endregion

        #region Construtors
        public UrlDocument()
        {
        }

        public UrlDocument(string _URL, string _DocumentParsed, string _DocumentUnParsed, string _Tittle, string _Resume)
            : this()
        {
            feed = _URL;
            documentParsed = _DocumentParsed;
            documentUnParsed = _DocumentUnParsed;
            tittle = _Tittle;
            description = _Resume;
        }
        #endregion

        #region Properties
        [System.ComponentModel.Category("DocumentoURL")]
        [System.ComponentModel.DisplayName("Id")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DataObjectField(true)]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [System.ComponentModel.Category("DocumentoURL")]
        [System.ComponentModel.DisplayName("Referencia URL")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DataObjectField(true)]
        public string URL
        {
            get { return this.feed; }
            set { this.feed = value; }
        }

        [System.ComponentModel.Category("DocumentoURL")]
        [System.ComponentModel.DisplayName("Documento Parseado")]
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DataObjectField(false, false)]
        public string DocumentParsed
        {
            get { return this.documentParsed; }
            set { this.documentParsed = value; }
        }

        [System.ComponentModel.Category("DocumentoURL")]
        [System.ComponentModel.DisplayName("Documento Original")]
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DataObjectField(false, false)]
        public string DocumentUnParsed
        {
            get { return this.documentUnParsed; }
            set { this.documentUnParsed = value; }
        }

        [System.ComponentModel.Category("DocumentoURL")]
        [System.ComponentModel.DisplayName("Conceptos")]
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DataObjectField(false, false)]
        public List<string> Conceptos
        {
            get { return conceptos; }
            set { conceptos = value; }
        }

        [System.ComponentModel.Category("DocumentoURL")]
        [System.ComponentModel.DisplayName("Titulo del Documento")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DataObjectField(false, false)]
        public string Tittle
        {
            get { return this.tittle; }
            set { this.tittle = value; }
        }

        [System.ComponentModel.Category("DocumentoURL")]
        [System.ComponentModel.DisplayName("Resumen del Documento")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DataObjectField(false, false)]
        public string Resume
        {
            get { return this.description; }
            set { this.description = value; }
        }

        [System.ComponentModel.Category("DocumentoURL")]
        [System.ComponentModel.DisplayName("Anotaciones del Documento")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DataObjectField(false, false)]
        public string Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }

        [System.ComponentModel.Category("DocumentoURL")]
        [System.ComponentModel.DisplayName("Sitio Web del Sensor")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DataObjectField(false, false)]
        public string Website
        {
            get { return this.website; }
            set { this.website = value; }
        }

        public string Localizacion_name
        {
            get { return localizacion_name; }
            set { localizacion_name = value; }
        }
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }
        public string Elevacion
        {
            get { return elevacion; }
            set { elevacion = value; }
        }
        public string Latitud
        {
            get { return latitud; }
            set { latitud = value; }
        }
        public string Longitud
        {
            get { return longitud; }
            set { longitud = value; }
        }

        public string Datastreams_feed
        {
            get { return datastreams; }
            set { datastreams = value; }
        }

        //Este método devuelve la url de xively que puede ser vista en el explorador Web sobre el sensor
        //a partir de la dirección json
        public string URLMostrar()
        {
            string temp = this.feed;
            if (!string.IsNullOrEmpty(temp))
            {
                temp = feed.Remove(feed.IndexOf("api."), 4);
                temp = temp.Remove(temp.IndexOf("v2/"), 3);
                temp = temp.Remove(temp.IndexOf(".json"), 5);
            }
            return temp;
        }

        //Este método devuelve el titulo en formato Link HTML, con el vínculo al URL
        public string TituloHTML()
        {
            return string.Format("<a style=\"color: #336600; font-size:110%;\"  href=\"{1}\" >{0}</a>", Tittle, URLMostrar());
        }
        #endregion

        //Este método devuelve la lista de conceptos separadas por comas
        public string ConceptosLista()
        {
            string str = string.Empty;

            foreach (string strtmp in Conceptos)
                if (string.IsNullOrEmpty(str))
                    str = strtmp;
                else
                    str += " , " + strtmp;

            return str;
        }
    }
}
