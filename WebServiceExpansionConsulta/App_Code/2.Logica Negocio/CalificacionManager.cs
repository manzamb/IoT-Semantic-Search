using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ModeloSemantico_PU.ObjetosNegocio;
using ModeloSemantico_PU.AccesoDatos;

namespace ModeloSemantico_PU.LogicaNegocio
{
    public static class CalificacionManager
    {
        #region Metodos
        /// <summary>
        /// Metodo que delega a CalificacionDB almacenar una Calificacion
        /// </summary>       
        /// <param name="myCalificacion">Objeto de tipo Calificacion que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizó con exito, de lo contrario retorna 0</returns>
        public static int Save(Calificacion myCalificacion)
        {
            return CalificacionDB.Save(myCalificacion);
        }
        /// <summary>
        /// Metodo que delega a CalificacionDB obtener una Calificacion
        /// </summary>       
        /// <param name="id">identificador de la Calificacion que se va obtener</param>          
        /// <returns>retorna Objeto Calificacion si el mismo se encuentra, de lo contrario retorna null</returns>
        public static Calificacion GetItem(int id)
        {
            return CalificacionDB.GetItem(id);
        }
        /// <summary>
        /// Metodo que delega a CalificacionDB obtener una lista de todas las Calificacion de una determinada consulta
        /// </summary>       
        /// <param name="idConsulta">Identificador de una consulta con el cual se obtiene una lista de sus Documentos Calificados</param>
        /// <returns>Lista de las Consultas de un determinado texto de consulta</returns>
        public static CalificacionList GetList(int idConsulta)
        {
            return CalificacionDB.GetList(idConsulta);
        }
        /// <summary>
        /// Metodo que delega a CalificacionDB obtener una lista de todas las Calificaciones de un determinado Documento
        /// </summary>       
        /// <param name="doc">URL del documento con la cual se obtiene una lista de sus Calificaciones</param>
        /// <returns>Lista de las Calificaciones de un determinado Documento</returns>
        public static CalificacionList GetListByDocument(string doc)
        {
            return CalificacionDB.GetListByDocument(doc);
        }
        #endregion
    }
}
