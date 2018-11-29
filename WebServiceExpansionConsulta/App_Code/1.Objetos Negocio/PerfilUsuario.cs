using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU.ObjetosNegocio
{
    public class PerfilUsuario
    {
        #region Atributos
        /// <summary>
        /// Identificador del Perfil de Usuario    
        /// </summary>
        private int _perfId;
        /// <summary>
        /// Identificador del Concepto
        /// </summary>
        private int _conceptId;
        /// <summary>
        /// Login del usuario
        /// </summary>
        private string usu_login;
        /// <summary>
        /// Peso del concepto en los documentos relevantes para el usuario
        /// </summary>
        private float _wrud;
        
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref="PerfilUsuario"/>.
        /// </summary>
        public PerfilUsuario()
        {
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Metodo que asigna y obtiene el identificador del perfil
        /// </summary> 
        /// <value>_perfId</value>
        public int PerfId
        {
            get { return _perfId; }
            set { _perfId = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el identificador del concepto
        /// </summary>
        /// <value>_conceptId</value>
        public int ConceptId
        {
            get { return _conceptId; }
            set { _conceptId = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el login del usuario
        /// </summary>
        /// /// <value>_wru</value>
        public string Usu_login
        {
            get { return usu_login; }
            set { usu_login = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el Peso del concepto en los documentos relevantes para el usuario
        /// </summary>
        /// <value>_wru</value>
        public float Wrud
        {
            get { return _wrud; }
            set { _wrud = value; }
        }
        #endregion                
    }
}
