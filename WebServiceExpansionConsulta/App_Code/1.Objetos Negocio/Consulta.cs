using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU.ObjetosNegocio
{
    public class Consulta
    {
        #region Atributos
        /// <summary>
        /// Identificador de la consulta
        /// </summary>
        private int _consulId;
        /// <summary>
        /// cadena de consulta
        /// </summary>
        private string _consulTexto;
        /// <summary>
        /// Login de Usuario
        /// </summary>
        private string _usuLogin;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref="Consulta"/>.
        /// </summary>
        public Consulta()
        {
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Metodo que asigna y obtiene el identifiacador de la consulta
        /// </summary>
        /// <value>_consulId</value>
        public int ConsulId
        {
            get { return _consulId; }
            set { _consulId = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el texto de la consulta 
        /// </summary> 
        /// <value>_consulTexto</value>
        public string ConsulTexto
        {
            get { return _consulTexto; }
            set { _consulTexto = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el login del usuario que realiza la consulta
        /// </summary> 
        /// <value>_usuLogin</value>
        public string UsuLogin
        {
            get { return _usuLogin; }
            set { _usuLogin = value; }
        }
        #endregion
    }
}
