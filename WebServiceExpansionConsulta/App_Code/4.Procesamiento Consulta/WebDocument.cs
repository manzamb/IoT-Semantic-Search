using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU.ProcesamientoConsulta
{
    #region Clase WebDocument
    public class WebDocument
    {
        #region Atributos
        private string _titulo;
        private string _resumen;
        private string _url;        
        #endregion
        #region Propiedades
        public string Titulo
        {
            get { return _titulo; }
            set { _titulo = value; }
        }
        public string Resumen
        {
            get { return _resumen; }
            set { _resumen = value; }
        }
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
        #endregion
    }
    #endregion
}
