using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Configuration;

namespace ModeloSemantico_PU.AccesoDatos
{
    public static class AppConfiguration
    {
        #region Public Properties

        /// <summary>
        /// Returns the connectionstring  for the application.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ConexionMsec"].ConnectionString;
            }
        }
        #endregion
    }
}
