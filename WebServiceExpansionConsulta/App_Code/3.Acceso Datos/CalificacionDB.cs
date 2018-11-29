using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ModeloSemantico_PU.ObjetosNegocio;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace ModeloSemantico_PU.AccesoDatos
{
    /// <summary>
    /// Descripción breve de la clase CalificacionDB.
    /// Interaccion de la Calificacion a traves de la lógica del negocio con la Base de Datos
    /// </summary>
    public static class CalificacionDB
    {
        #region Metodos
        /// <summary>
        /// Método que almacena una Calificacion
        /// </summary>       
        /// <param name="myCalificacion">Objeto de tipo Calificacion que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizó con exito, de lo contrario retorna 0</returns>
        public static int Save(Calificacion myCalificacion)
        {
            int result = 0;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spCalificacionUpsert", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Cal_Id", -1);
                myCommand.Parameters.AddWithValue("@Cal_Documento", myCalificacion.CalDocumento);
                myCommand.Parameters.AddWithValue("@Cal_Valor", myCalificacion.CalValor);
                myCommand.Parameters.AddWithValue("@Consul_Id", myCalificacion.ConsulId);

                DbParameter returnValue;
                returnValue = myCommand.CreateParameter();
                returnValue.Direction = ParameterDirection.ReturnValue;
                myCommand.Parameters.Add(returnValue);

                myConnection.Open();
                myCommand.ExecuteNonQuery();
                result = Convert.ToInt32(returnValue.Value);
                myConnection.Close();
            }
            return result;
        }
        /// <summary>
        /// Método que obtiene una Calificacion
        /// </summary>       
        /// <param name="id">identificador de la Calificacion que se va obtener</param>       
        /// <returns>retorna Objeto Calificacion si el mismo se encuentra, de lo contrario retorna null</returns>
        public static Calificacion GetItem(int id)
        {
            Calificacion myCalificacion = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetCalificacion", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Cal_Id", id);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myCalificacion = FillDataRecord(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myCalificacion;
        }
        /// <summary>
        /// Método que inicializa un Objeto Calificacion a partir de un registro de la Base de datos
        /// </summary>       
        /// <param name="myDataRecord">Registro de la tabla TBL_CALIFICACION</param>
        /// <returns>retorna Objeto Calificacion inicializado</returns>
        public static Calificacion FillDataRecord(IDataRecord myDataRecord)
        {
            Calificacion myCalificacion = new Calificacion();
            myCalificacion.CalId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("CAL_ID"));
            myCalificacion.CalDocumento = myDataRecord.GetString(myDataRecord.GetOrdinal("CAL_DOCUMENTO"));
            myCalificacion.CalValor = myDataRecord.GetInt32(myDataRecord.GetOrdinal("CAL_VALOR"));
            myCalificacion.ConsulId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("CONSUL_ID"));
            return myCalificacion;
        }
        /// <summary>
        /// Método que obtiene una lista de todas las Calificaciones de una determinada consulta
        /// </summary>       
        /// <param name="idConsulta">Identificador de una consulta con el cual se obtiene una lista de sus Documentos Calificados</param>
        /// <returns>Lista de las Consultas de un determinado texto de consulta</returns>
        public static CalificacionList GetList(int idConsulta)
        {
            CalificacionList tempList = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetListCalificacionByConsulta", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Consul_Id", idConsulta);
                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.HasRows)
                    {
                        tempList = new CalificacionList();
                        while (myReader.Read())
                        {
                            tempList.Add(FillDataRecord(myReader));
                        }
                    }
                    myReader.Close();
                }
            }
            return tempList;
        }
        /// <summary>
        /// Metodo que obtiene una lista de todas las Calificaciones de un determinado Documento
        /// </summary>       
        /// <param name="doc">URL del documento con la cual se obtiene una lista de sus Calificaciones</param>
        /// <returns>Lista de las Calificaciones de un determinado Documento</returns>
        public static CalificacionList GetListByDocument(string doc)
        {
            CalificacionList tempList = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetListCalificacionByDocument", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Cal_Documento", doc);
                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.HasRows)
                    {
                        tempList = new CalificacionList();
                        while (myReader.Read())
                        {
                            tempList.Add(FillDataRecord(myReader));
                        }
                    }
                    myReader.Close();
                }
            }
            return tempList;
        }
        #endregion
    }
}
