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
    /// Descripción breve de la clase ConsultaDB.
    /// Interaccion de la Consulta a traves de la lógica del negocio con la Base de Datos
    /// </summary> 
    public static class ConsultaDB
    {
        #region Metodos
        /// <summary>
        /// Método que almacena una Consulta
        /// </summary>       
        /// <param name="myConsulta">Objeto de tipo Consulta que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizó con exito, de lo contrario retorna 0</returns>
        public static int Save(Consulta myConsulta)
        {
            int result = 0;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spConsultaUpsert", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Consul_Id", -1);
                myCommand.Parameters.AddWithValue("@Consul_Texto", myConsulta.ConsulTexto);
                myCommand.Parameters.AddWithValue("@Usu_Login", myConsulta.UsuLogin);

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
        /// Método que obtiene una Consulta
        /// </summary>       
        /// <param name="id">identificador de la Consulta que se va obtener</param>       
        /// <returns>retorna Objeto Consulta si el mismo se encuentra, de lo contrario retorna null</returns>
        public static Consulta GetItem(int id)
        {
            Consulta myConsulta = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetConsulta", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Consul_Id", id);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myConsulta = FillDataRecord(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myConsulta;
        }
        /// <summary>
        /// Método que inicializa un Objeto Consulta a partir de un registro de la Base de datos
        /// </summary>       
        /// <param name="myDataRecord">Registro de la tabla TBL_CONSULTA</param>
        /// <returns>retorna Objeto Consulta inicializado</returns>
        public static Consulta FillDataRecord(IDataRecord myDataRecord)
        {
            Consulta myConsulta = new Consulta();
            myConsulta.ConsulId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("CONSUL_ID"));
            myConsulta.ConsulTexto = myDataRecord.GetString(myDataRecord.GetOrdinal("CONSUL_TEXTO"));
            myConsulta.UsuLogin = myDataRecord.GetString(myDataRecord.GetOrdinal("USU_LOGIN"));
            return myConsulta;
        }
        /// <summary>
        /// Metodo que obtiene una lista de todas las Consultas de un determinado texto de consulta
        /// </summary>       
        /// <param name="consulText">Texto de una consulta con el cual se obtiene una lista de sus Consultas</param>
        /// <returns>Lista de las Consultas de un determinado texto de consulta</returns>
        public static ConsultaList GetListByText(string consulTexto)
        {
            ConsultaList tempList = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetListConsulta", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Consul_Texto", consulTexto);
                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.HasRows)
                    {
                        tempList = new ConsultaList();
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
        /// Método que obtiene una lista de todas las Consultas de un determinado usuario
        /// </summary>       
        /// <param name="login">Login de un usuario con el cual se obtiene una lista de sus Consultas</param>
        /// <returns>Lista de las Consultas de un determinado usuario</returns>
        public static ConsultaList GetListByUserName(string login)
        {
            ConsultaList tempList = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetListConsultaByUserName", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Usu_Login", login);
                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.HasRows)
                    {
                        tempList = new ConsultaList();
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
