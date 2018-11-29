using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU.ObjetosNegocio
{
    public class Usuario
    {
        #region Atributos
        /// <summary>
        /// Identificador del Usuario    
        /// </summary>
        private string _usuLogin;
        
        /// <summary>
        /// password del usuario   
        /// </summary>
        private string _usuPassword;        
        #endregion      

        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref="Usuario"/>.
        /// </summary>
        public Usuario()
        {
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Metodo que asigna y obtiene el login del usuario
        /// </summary> 
        /// <value>_login</value>
        public string UsuLogin
        {
            get { return _usuLogin; }
            set { _usuLogin = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el password del usuario
        /// </summary> 
        /// <value>_´password</value>
        public string UsuPassword
        {
            get { return _usuPassword; }
            set { _usuPassword = value; }
        }
        #endregion
    }
}
