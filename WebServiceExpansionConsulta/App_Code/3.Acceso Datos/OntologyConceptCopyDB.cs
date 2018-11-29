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
    public static class OntologyConceptCopyDB
    {
        #region Metodos
        /// <summary>
        /// Metodo que almacena un objeto de tipo OntologyConceptCopy
        /// </summary>       
        /// <param name="myOntologyConcept">Objeto de tipo OntologyConceptCopy que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
        public static int Save(OntologyConceptCopy myOntologyConceptCopy)
        {
            int result = 0;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spOntologyConceptCopyUpsert", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;

                myCommand.Parameters.AddWithValue("@OntCopy_Id", myOntologyConceptCopy.OntCopyId);
                myCommand.Parameters.AddWithValue("@OntCopy_NameConcept", myOntologyConceptCopy.OntCopyNameConcept);
                myCommand.Parameters.AddWithValue("@Ont_Id", myOntologyConceptCopy.OntId);
                myCommand.Parameters.AddWithValue("@Ont_uri", myOntologyConceptCopy.Urlconcepto);


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
        /// Metodo que obtiene un OntologyConceptCopy
        /// </summary>       
        /// <param name="concepto">concepto de OntologyConceptCopy por el cual se va a buscar</param>
        /// <returns>retorna Objeto OntologyConceptCopy si el mismo se encuentra,de lo contrario retorna null</returns>
        public static OntologyConceptCopy GetItem(string concepto, string invertConcept)
        {
            OntologyConceptCopy myOntologyConceptCopy = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetOntologyConceptCopy", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@OntCopy_NameConcept", concepto);
                myCommand.Parameters.AddWithValue("@NameConceptInvert", invertConcept);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myOntologyConceptCopy = FillDataRecord(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myOntologyConceptCopy;
        }

        /// <summary>
        /// Metodo que inicializa un Objeto OntologyConceptCopy a partir de un registro de la Base de datos
        /// </summary>       
        /// <param name="myDataRecord">Registro de la tabla TBL_ONTOLOGY_CONCEPTS_COPY</param>
        /// <returns>retorna Objeto OntologyConceptCopy inicializado</returns>
        public static OntologyConceptCopy FillDataRecord(IDataRecord myDataRecord)
        {
            OntologyConceptCopy myOntologyConceptCopy = new OntologyConceptCopy();
            myOntologyConceptCopy.OntId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("ONT_ID"));
            if (!myDataRecord.IsDBNull(myDataRecord.GetOrdinal("ONTCOPY_NAMECONCEPT")))
            {
                myOntologyConceptCopy.OntCopyNameConcept = myDataRecord.GetString(myDataRecord.GetOrdinal("ONTCOPY_NAMECONCEPT"));
            }
            return myOntologyConceptCopy;
        }

        public static void BorrarConceptos()
        {
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spOntologyBorrarConceptos", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myConnection.Open();
                myCommand.ExecuteNonQuery();             
                myConnection.Close();
            }
        }

        public static OntologyConceptCopy GetItemByConceptName(string conceptName)
        {
            OntologyConceptCopy myOntologyConcept = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetOntologyConceptByConceptName", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Ont_NameConcept", conceptName);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myOntologyConcept = FillDataRecordFull(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myOntologyConcept;
        }

        //Devuelve el url de un concepto, en caso de no encontrarlo devuelve un Ontologyconcept null
        public static OntologyConceptCopy GetItemByUniqueConceptName(string conceptName)
        {
            OntologyConceptCopy myOntologyConcept = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetUniqueOntologyConceptByConceptName", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Ont_NameConcept", conceptName);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myOntologyConcept = FillDataRecordFull(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myOntologyConcept;
        }

        public static OntologyConceptCopy GetItemConceptInUrl(string conceptName)
        {
            OntologyConceptCopy myOntologyConcept = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetOntologyConceptInUrl", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Ont_NameConcept", conceptName);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myOntologyConcept = FillDataRecordFull(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myOntologyConcept;
        }

        public static OntologyConceptCopy GetItemUniqueConceptInUrl(string conceptName)
        {
            OntologyConceptCopy myOntologyConcept = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetUniqueOntologyConceptInUrl", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Ont_NameConcept", conceptName);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myOntologyConcept = FillDataRecordFull(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myOntologyConcept;
        }

        public static OntologyConceptCopy FillDataRecordFull(IDataRecord myDataRecord)
        {
            OntologyConceptCopy myOntologyConceptCopy = new OntologyConceptCopy();
            myOntologyConceptCopy.OntId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("ONT_ID"));
            if (!myDataRecord.IsDBNull(myDataRecord.GetOrdinal("ONTCOPY_ID")))
            {
                myOntologyConceptCopy.OntCopyId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("ONTCOPY_ID"));
            }
            if (!myDataRecord.IsDBNull(myDataRecord.GetOrdinal("ONTCOPY_NAMECONCEPT")))
            {
                myOntologyConceptCopy.OntCopyNameConcept = myDataRecord.GetString(myDataRecord.GetOrdinal("ONTCOPY_NAMECONCEPT"));
            }
            if (!myDataRecord.IsDBNull(myDataRecord.GetOrdinal("ONT_URI")))
            {
                myOntologyConceptCopy.Urlconcepto = myDataRecord.GetString(myDataRecord.GetOrdinal("ONT_URI"));
            }

            return myOntologyConceptCopy;
        }

        /// <summary>
        /// Metodo que obtiene un OntologyConcept
        /// </summary>       
        /// <param name="id">Identificador del OntologyConcept que se va obtener</param>
        /// <returns>retorna Objeto OntologyConcept si el mismo se encuentra,de lo contrario retorna null</returns>
        public static OntologyConceptCopy GetItem(int id)
        {
            OntologyConceptCopy myOntologyConcept = null;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spGetOntologyConcept", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Ont_Id", id);

                myConnection.Open();
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                {
                    if (myReader.Read())
                    {
                        myOntologyConcept = FillDataRecordOriginal(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myOntologyConcept;
        }

        /// <summary>
        /// Metodo que inicializa un Objeto myOntologyConcept a partir de un registro de la Base de datos
        /// </summary>       
        /// <param name="myDataRecord">Registro de la tabla TBL_ONTOLOGY_CONCEPTS</param>
        /// <returns>retorna Objeto OntologyConcept inicializado</returns>
        public static OntologyConceptCopy FillDataRecordOriginal(IDataRecord myDataRecord)
        {
            OntologyConceptCopy myOntologyConcept = new OntologyConceptCopy();
            myOntologyConcept.OntId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("ONT_ID"));
            myOntologyConcept.OntCopyNameConcept = myDataRecord.GetString(myDataRecord.GetOrdinal("ONTCOPY_NAMECONCEPT"));
            return myOntologyConcept;
        }
        #endregion

    }
}
