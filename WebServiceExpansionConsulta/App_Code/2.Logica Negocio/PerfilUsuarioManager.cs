using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ModeloSemantico_PU.ObjetosNegocio;
using ModeloSemantico_PU.AccesoDatos;

namespace ModeloSemantico_PU.LogicaNegocio
{
    /// <summary>
    /// Descripción breve de la clase PerfilUsuarioManager.
    /// Delega las operaciones del PerfilUsuario a la respectiva clase de la capa de acceso a Datos 
    /// </summary> 
    public static class PerfilUsuarioManager
    {
        #region Metodos
        /// <summary>
        /// Metodo que delega a PerfilUsuarioDB almacenar a un PerfilUsuario 
        /// </summary>       
        /// <param name="myPerfilUsuario">Objeto de tipo PerfilUsuario que se va a almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
        public static int Save(PerfilUsuario myPerfilUsuario)
        {
            return PerfilUsuarioDB.Save(myPerfilUsuario);
        }
        /// <summary>
        /// Metodo que delega a PerfilUsuarioDB obtener un PerfilUsuario 
        /// </summary>       
        /// <param name="id">Identificador del PerfilUsuario que se va obtener</param>
        /// <returns>retorna Objeto PerfilUsuario si el mismo se encuentra,de lo contrario retorna null</returns>
        public static PerfilUsuario GetItem(string login, int id)
        {
            return PerfilUsuarioDB.GetItem(login, id);
        }
        #endregion
    }
}
