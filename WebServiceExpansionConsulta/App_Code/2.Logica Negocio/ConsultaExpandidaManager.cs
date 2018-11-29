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
    /// Descripción breve de la clase ConsultaExpandidaManager.
    /// Delega las operaciones de Consulta Expandida a la respectiva clase de la capa de acceso a Datos 
    /// </summary> 
    public static class ConsultaExpandidaManager
    {
         #region Metodos
         /// <summary>
         /// Metodo que delega a ConsultaExpandidaDB almacenar una Consulta
         /// </summary>       
         /// <param name="myConsulta">Objeto de tipo ConsultaExpandida que se va almacenar</param>
         /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
         public static int Save(ConsultaExpandida myConsultaExpandida)
         {
             return ConsultaExpandidaDB.Save(myConsultaExpandida);
         }
         /// <summary>
         /// Metodo que delega a ConsultaExpandidaDB obtener una ConsultaExpandida
         /// </summary>       
         /// <param name="id">identificador de la consulta Expandida que se va obtener</param>
         /// <param name="login">login del Usuario al cual pertenece la consulta Expandida</param>        
         /// <param name="sesionId">id de la Sesion al cual pertenece el Usuario </param>        
         /// <returns>retorna Objeto ConsultaExpandida si el mismo se encuentra,de lo contrario retorna null</returns>
         public static ConsultaExpandida GetItem(int id, string login, int sesionId)
         {
             return ConsultaExpandidaDB.GetItem(id, login, sesionId);
         }
         /// <summary>
         /// Metodo que delega a ConsultaExpandidaDB obtener una lista de todas las consultas expandidas de un determinado Usuario        
         /// </summary>       
         /// <param name="login">login del usuario con el cual se obtiene una lista de sus Consultas expandidas</param>
         /// <param name="sesionId">id de la Sesion al cual pertenece el Usuario para obtener una lista de consultas expandidas de dicha sesion</param>        
         /// <returns>retorna una lista de las consultas expandidas de un determinado Usuario</returns>
         public static ConsultaExpandidaList GetList(string login, int sesionId)
         {
             return ConsultaExpandidaDB.GetList(login, sesionId);
         }
         #endregion
    }
}
