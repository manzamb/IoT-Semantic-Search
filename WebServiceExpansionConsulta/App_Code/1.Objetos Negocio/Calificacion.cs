using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU.ObjetosNegocio
{
    public class Calificacion
    {
        #region Atributos
        /// <summary>
        /// Identificador de la calificación
        /// </summary>
        private int _calId;
        /// <summary>
        /// URL del documento calificado
        /// </summary>
        private string _calDocumento;
        /// <summary>
        /// Valor de la calificación al documento
        /// </summary>
        private int _calValor;
        /// <summary>
        /// Identificador de la consulta
        /// </summary>
        private int _consulId;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref="Calificacion"/>.
        /// </summary>
        public Calificacion()
        {
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Metodo que asigna y obtiene el identifiacador de la calificación
        /// </summary>
        /// <value>_calId</value>
        public int CalId
        {
            get { return _calId; }
            set { _calId = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene la URL del documento calificado
        /// </summary>
        /// <value>_calDocumento</value>
        public string CalDocumento
        {
            get { return _calDocumento; }
            set { _calDocumento = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el valor de la calificación del documento
        /// </summary>
        /// <value>_calValor</value>
        public int CalValor
        {
            get { return _calValor; }
            set { _calValor = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el identificador de la consulta
        /// </summary>
        /// <value>_consulId</value>
        public int ConsulId
        {
            get { return _consulId; }
            set { _consulId = value; }
        }
        #endregion
    }
}
