using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU
{
    public class Documento
    {

        #region Atributos
        /// <summary>
        /// Identificador del documento
        /// </summary>
        private int _docId;
        /// <summary>
        /// Url del documento
        /// </summary>        
        private string _docUrl;
        /// <summary>
        /// Calificacion del documento proporcionada por el Usuario
        /// </summary>
        private double _docCalificacion;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref="Documento"/>.
        /// </summary>
        public Documento()
        {
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Metodo que asigna y obtiene el identificador del Documento
        /// </summary>        
        public int DocId
        {
            get { return _docId; }
            set { _docId = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene la Url del Documento
        /// </summary>
        public string DocUrl
        {
            get { return _docUrl; }
            set { _docUrl = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene la calificacion del Documento
        /// </summary>
        public double DocCalificacion
        {
            get { return _docCalificacion; }
            set { _docCalificacion = value; }
        }
        #endregion        
    }
}
