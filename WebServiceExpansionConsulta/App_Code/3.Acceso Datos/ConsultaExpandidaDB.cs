using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ModeloSemantico_PU.ObjetosNegocio;
using System.Data;

namespace ModeloSemantico_PU.AccesoDatos
{
    /// <summary>
    /// Descripción breve de la clase ConsultaExpandidaDB.
    /// Interaccion de la ConsultaExpandida atraves de la logica del negocio con la Base de Datos
    /// </summary> 
    public static class ConsultaExpandidaDB
    {
        #region Metodos
        /// <summary>
        /// Metodo que almacena una Consulta Expandida
        /// </summary>       
        /// <param name="myConsultaExpandida">Objeto de tipo ConsultaExpandida que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
        public static int Save(ConsultaExpandida myConsultaExpandida)
        {
            int result = 0;
            //using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            //{
            //SqlCommand myCommand = new SqlCommand("spConsultaExpandidaUpsert", myConnection);
            //myCommand.CommandType = CommandType.StoredProcedure;           

            //myCommand.Parameters.AddWithValue("@Con_Id", myConsulta.ConId);
            //myCommand.Parameters.AddWithValue("@Usu_Login", myConsulta.UsuLogin);
            //myCommand.Parameters.AddWithValue("@Con_Consulta", myConsulta.ConConsulta);
            //myCommand.Parameters.AddWithValue("@Con_ConsultaExpandida", myConsulta.ConConsultaExpandida);          

            //DbParameter returnValue;
            //returnValue = myCommand.CreateParameter();
            //returnValue.Direction = ParameterDirection.ReturnValue;
            //myCommand.Parameters.Add(returnValue);

            //myConnection.Open();
            //myCommand.ExecuteNonQuery();
            //result = Convert.ToInt32(returnValue.Value);
            //myConnection.Close();
            //}
            return result;
        }
        /// <summary>
        /// Metodo que obtiene una Consulta
        /// </summary>       
        /// <param name="id">identificador de la ConsultaExpandida que se va obtener</param>
        /// <param name="login">login del Usuario al cual pertenece la ConsultaExpandida</param>
        /// <param name="sesionId">id de la Sesion al cual pertenece el Usuario </param> 
        /// <returns>retorna Objeto ConsultaExpandida si el mismo se encuentra,de lo contrario retorna null</returns>
        public static ConsultaExpandida GetItem(int id, string login, int sesionId)
        {
            ConsultaExpandida myConsultaExpandida = null;
            //using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            //{
            //    SqlCommand myCommand = new SqlCommand("spObtenerUsuarioUsername", myConnection);
            //    myCommand.CommandType = CommandType.StoredProcedure;
            //    myCommand.Parameters.AddWithValue("@Con_Id", id);
            //    myCommand.Parameters.AddWithValue("@Usu_Login",login);            

            //    myConnection.Open();
            //    using (SqlDataReader myReader = myCommand.ExecuteReader())
            //    {
            //        if (myReader.Read())
            //        {
            //            myUsuario = FillDataRecord(myReader);
            //        }
            //        myReader.Close();
            //    }
            //    myConnection.Close();
            //}
            return myConsultaExpandida;
        }
        /// <summary>
        /// Metodo que inicializa un Objeto ConsultaExpandida a partir de un registro de la Base de datos
        /// </summary>       
        /// <param name="myDataRecord">Registro de la tabla TBL_ConsultaExpandida</param>
        /// <returns>retorna Objeto ConsultaExpandida inicializado</returns>
        public static ConsultaExpandida FillDataRecord(IDataRecord myDataRecord)
        {
            ConsultaExpandida myConsultaExpandida = new ConsultaExpandida();
            //myConsulta.ConId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("Con_Id"));
            //myConsulta.UsuLogin = myDataRecord.GetString(myDataRecord.GetOrdinal("Usu_Login"));
            //myConsulta.ConConsulta = myDataRecord.GetString(myDataRecord.GetOrdinal("Con_Consulta"));
            //myConsulta.ConConsultaExpandida = myDataRecord.GetString(myDataRecord.GetOrdinal("Con_ConsultaExpandida"));
            return myConsultaExpandida;
        }
        /// <summary>
        /// Metodo que obtiene una lista de todas las consultas expandidas de un determinado Usuario        
        /// </summary>       
        /// <param name="login">login del usuario con el cual se obtiene una lista de sus Consultas expandidas</param>
        /// <param name="sesionId">id de la Sesion al cual pertenece el Usuario para obtener una lista de consultas expandidas de dicha sesion</param>        
        /// <returns>retorna una lista de las consultas expandidas de un determinado Usuario</returns>
        static public ConsultaExpandidaList GetList(string login, int sesionId)
        {
            ConsultaExpandidaList tempList = null;
            //using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            //{
            //    SqlCommand myCommand = new SqlCommand("spObtenerListaDocenteActividad", myConnection);
            //    myCommand.CommandType = CommandType.StoredProcedure;
            //    myCommand.Parameters.AddWithValue("@Usu_Login",login);
            //    myConnection.Open();
            //    using (SqlDataReader myReader = myCommand.ExecuteReader())
            //    {
            //        if (myReader.HasRows)
            //        {
            //            tempList = new ConsultaList();
            //            while (myReader.Read())
            //            {
            //                tempList.Add(FillDataRecord(myReader));
            //            }
            //        }
            //        myReader.Close();
            //    }
            //}
            return tempList;
        }

        #endregion
    }
}
