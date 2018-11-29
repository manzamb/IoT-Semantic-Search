using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ModeloSemantico_PU.AccesoDatos;
using ModeloSemantico_PU.ObjetosNegocio;

namespace ModeloSemantico_PU.LogicaNegocio
{
    /// <summary>
    /// Descripción breve de la clase UsuarioManager.
    /// Delega las operaciones del Usuario a la respectiva clase de la capa de acceso a Datos 
    /// </summary> 
    public static class UsuarioManager
    {
        #region Metodos
        /// <summary>
        /// Metodo que delega a UsuarioDB almacenar a un Usuario 
        /// </summary>       
        /// <param name="myUsuario">Objeto de tipo Usuario que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
        public static int Save(Usuario myUsuario, int band)
        {
            return UsuarioDB.Save(myUsuario, band);
        }
        /// <summary>
        /// Metodo que delega a UsuarioDB obtener un Usuario 
        /// </summary>       
        /// <param name="myUsuario">Objeto de tipo Usuario que se va obtener</param>
        /// <returns>retorna Objeto Usuario si el mismo se encuentra,de lo contrario retorna null</returns>
        public static Usuario GetItem(string login)
        {
            return UsuarioDB.GetItem(login);
        }
        #endregion
    }
}