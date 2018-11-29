using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data.SqlClient;
using ModeloSemantico_PU.ObjetosNegocio;
using System.Data;
using System.Data.Common;

namespace ModeloSemantico_PU.AccesoDatos
{
    /// <summary>
    /// Descripción breve de la clase PerfilUsuarioDB.
    /// Interaccion del PerfilUsuario a traves de la logica del negocio con la Base de Datos
    /// </summary> 
    public static class PerfilUsuarioDB
    {
        #region Metodos
        /// <summary>
        /// Metodo que almacena a un PerfilUsuario 
        /// </summary>       
        /// <param name="myPerfilUsuario">Objeto de tipo PerfilUsuario que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
        public static int Save(PerfilUsuario myPerfilUsuario)
        {
            int result = 0;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spPerfilUsuarioUpsert", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;

                myCommand.Parameters.AddWithValue("@Perf_Id", -1);
                myCommand.Parameters.AddWithValue("@Usu_Login", myPerfilUsuario.Usu_login);
                myCommand.Parameters.AddWithValue("@Concept_Id", myPerfilUsuario.ConceptId);
                myCommand.Parameters.AddWithValue("@Wrud", myPerfilUsuario.Wrud);

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
        /// Metodo que obtiene un PerfilUsuario 
        /// </summary>       
        /// <param name="ID">Identificador del PerfilUsuario que se va obtener</param>
        /// <returns>retorna Objeto PerfilUsuario si el mismo se encuentra,de lo contrario retorna null</returns>
        public static PerfilUsuario GetItem(string login, int id)
        {
            PerfilUsuario myPerfilUsuario = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetPerfilUsuario", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Usu_Login", login);
                myCommand.Parameters.AddWithValue("@Concept_Id", id);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myPerfilUsuario = FillDataRecord(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myPerfilUsuario;
        }

        /// <summary>
        /// Metodo que inicializa un Objeto PerfilUsuario a partir de un registro de la Base de datos
        /// </summary>       
        /// <param name="myDataRecord">Registro de la tabla TBL_PerfilUsuario</param>
        /// <returns>retorna Objeto PerfilUsuario inicializado</returns>
        public static PerfilUsuario FillDataRecord(IDataRecord myDataRecord)
        {
            PerfilUsuario myPerfilUsuario = new PerfilUsuario();
            myPerfilUsuario.PerfId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("PERF_ID"));
            myPerfilUsuario.ConceptId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("CONCEPT_ID"));
            myPerfilUsuario.Usu_login = myDataRecord.GetString(myDataRecord.GetOrdinal("USU_LOGIN"));
            myPerfilUsuario.Wrud =(float)myDataRecord.GetDouble(myDataRecord.GetOrdinal("WRUD"));
            return myPerfilUsuario;
        }
        #endregion
    }
}
