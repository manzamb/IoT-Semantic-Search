using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU.ObjetosNegocio
{
    public class ConsultaExpandida : Consulta
    {
        #region Atributos
        /// <summary>
        /// Identificador de la consulta expandida
        /// </summary>
        private int _consulExpId;
        /// <summary>
        /// longitud de la consulta expandida
        /// </summary>
        private int _longitud;
        /// <summary>
        /// Consulta de usuario
        /// </summary>
        private string _cadenaConsulta;
        #endregion     

        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref="ConsultaExpandida"/>.
        /// </summary>
        public ConsultaExpandida()
        {            
        }
        #endregion

        #region Asociados
        /// <summary>
        /// Identificador del Concepto 
        /// </summary>
        private int _concepId;        
        /// <summary>
        /// Identificador de la Sesion
        /// </summary>
        private int _sesionId;        
        /// <summary>
        /// Identificador del Usuario
        /// </summary>
        private string _usuLogin;        
        #endregion       

        #region Propiedades
        /// <summary>
        /// Metodo que asigna y obtiene la longitud de una consulta  expandida
        /// </summary>
        public int Longitud
        {
            get { return _longitud; }
            set { _longitud = value; }
        }        
        /// <summary>
        /// Metodo que asigna y obtiene el identificador de la consulta expandida
        /// </summary>
        public int ConsulExpId
        {
            get { return _consulExpId; }
            set { _consulExpId = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene una cadena de consulta de usuario
        /// </summary>
        public string CadenaConsulta
        {
            get { return _cadenaConsulta; }
            set { _cadenaConsulta = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el identificador del concepto
        /// </summary>
        public int ConcepId
        {
            get { return _concepId; }
            set { _concepId = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el identificador de la sesion
        /// </summary>
        public int SesionId
        {
            get { return _sesionId; }
            set { _sesionId = value; }
        }        
        #endregion
    }
}
