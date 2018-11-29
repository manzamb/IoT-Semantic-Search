using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ModeloSemantico_PU.ObjetosNegocio;
using ModeloSemantico_PU.AccesoDatos;

namespace ModeloSemantico_PU.LogicaNegocio
{
    public static class ConsultaManager
    {
        #region Metodos
        /// <summary>
        /// Metodo que delega a ConsultaDB almacenar una Consulta
        /// </summary>       
        /// <param name="myConsulta">Objeto de tipo Consulta que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizó con exito, de lo contrario retorna 0</returns>
        public static int Save(Consulta myConsulta)
        {
            return ConsultaDB.Save(myConsulta);
        }
        /// <summary>
        /// Metodo que delega a ConsultaDB obtener una Consulta
        /// </summary>       
        /// <param name="id">identificador de la Consulta que se va obtener</param>          
        /// <returns>retorna Objeto Consulta si el mismo se encuentra, de lo contrario retorna null</returns>
        public static Consulta GetItem(int id)
        {
            return ConsultaDB.GetItem(id);
        }
        /// <summary>
        /// Metodo que delega a ConsultaDB obtener una lista de todas las Consultas de un determinado texto de consulta
        /// </summary>       
        /// <param name="consulText">Texto de una consulta con el cual se obtiene una lista de sus Consultas</param>
        /// <returns>Lista de las Consultas de un determinado texto de consulta</returns>
        public static ConsultaList GetListByText(string consulText)
        {
            return ConsultaDB.GetListByText(consulText);
        }
        /// <summary>
        /// Metodo que delega a ConsultaDB obtener una lista de todas las Consultas de un determinado usuario
        /// </summary>       
        /// <param name="login">Login de un usuario con el cual se obtiene una lista de sus Consultas</param>
        /// <returns>Lista de las Consultas de un determinado usuario</returns>
        public static ConsultaList GetListByUserName(string login)
        {
            return ConsultaDB.GetListByUserName(login);
        }
        #endregion
    }
}
