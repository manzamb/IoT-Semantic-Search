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
    /// Descripción breve de la clase UsuarioDB.
    /// Interaccion del Usuario atraves de la logica del negocio con la Base de Datos
    /// </summary> 
    public static class UsuarioDB
    {
        #region Metodos
        /// <summary>
        /// Metodo que almacena a un Usuario 
        /// </summary>       
        /// <param name="myUsuario">Objeto de tipo Usuario que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
        public static int Save(Usuario myUsuario, int band)
        {
            int result = 0;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spUsuarioUpsert", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;

                myCommand.Parameters.AddWithValue("@band", band);

                myCommand.Parameters.AddWithValue("@Usu_Login", myUsuario.UsuLogin);
                myCommand.Parameters.AddWithValue("@Usu_Password", myUsuario.UsuPassword);

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
        /// Metodo que obtiene un Usuario 
        /// </summary>       
        /// <param name="myUsuario">Objeto de tipo Usuario que se va obtener</param>
        /// <returns>retorna Objeto Usuario si el mismo se encuentra,de lo contrario retorna null</returns>
        public static Usuario GetItem(string login)
        {
            Usuario myUsuario = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetUsuario", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Usu_Login", login);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myUsuario = FillDataRecord(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myUsuario;
        }
        /// <summary>
        /// Metodo que inicializa un Objeto Usuario a partir de un registro de la Base de datos
        /// </summary>       
        /// <param name="myDataRecord">Registro de la tabla TBL_Usuario</param>
        /// <returns>retorna Objeto Usuario inicializado</returns>
        public static Usuario FillDataRecord(IDataRecord myDataRecord)
        {
            Usuario myUsuario = new Usuario();
            myUsuario.UsuLogin = myDataRecord.GetString(myDataRecord.GetOrdinal("USU_LOGIN"));
            return myUsuario;
        }
        #endregion
    }
}