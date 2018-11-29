using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ModeloSemantico_PU.ObjetosNegocio;
using System.Data;
using System.Data.SqlClient;
using ModeloSemantico_PU.AccesoDatos;
using System.Data.Common;
using System.Collections.Generic;
using System.IO;

using com.hp.hpl.jena.ontology;
using com.hp.hpl.jena.query;
using com.hp.hpl.jena.rdf.model;
using com.hp.hpl.jena.util;
using com.hp.hpl.jena.util.iterator;
using com.hp.hpl.jena.rdf.model.impl;
using com.hp.hpl.jena.rdf.model.test;
using java.util;

namespace ModeloSemantico_PU
{
    public static class OntologyConceptDB
    {
        #region Metodos
        /// <summary>
        /// Metodo que almacena un objeto de tipo OntologyConcept
        /// </summary>       
        /// <param name="myOntologyConcept">Objeto de tipo OntologyConcept que se va almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
        public static int Save(OntologyConcept myOntologyConcept)
        {
            int result = 0;
            using (SqlConnection myConnection = new SqlConnection(AppConfiguration.ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand("spOntologyConceptUpsert", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;

                myCommand.Parameters.AddWithValue("@Ont_Id", myOntologyConcept.OntId);
                myCommand.Parameters.AddWithValue("@Ont_NameConcept", myOntologyConcept.OntNameConcept);


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
        /// Metodo que obtiene un OntologyConcept
        /// </summary>       
        /// <param name="id">Identificador del OntologyConcept que se va obtener</param>
        /// <returns>retorna Objeto OntologyConcept si el mismo se encuentra,de lo contrario retorna null</returns>
        public static OntologyConcept GetItem(int id)
        {
            OntologyConcept myOntologyConcept = null;
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
                        myOntologyConcept = FillDataRecord(myReader);
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
        public static OntologyConcept FillDataRecord(IDataRecord myDataRecord)
        {
            OntologyConcept myOntologyConcept = new OntologyConcept();
            myOntologyConcept.OntId = myDataRecord.GetInt32(myDataRecord.GetOrdinal("ONT_ID"));
            myOntologyConcept.OntNameConcept = myDataRecord.GetString(myDataRecord.GetOrdinal("ONTCOPY_NAMECONCEPT"));
            return myOntologyConcept;
        }

        /// <summary>
        /// Metodo que obtiene un OntologyConcept
        /// </summary>       
        /// <param name="conceptName">Nombre del OntologyConcept que se va obtener</param>
        /// <returns>retorna Objeto OntologyConcept si el mismo se encuentra,de lo contrario retorna null</returns>
        public static OntologyConcept GetItemByConceptName(string conceptName)
        {
            OntologyConcept myOntologyConcept = null;
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
                        myOntologyConcept = FillDataRecord(myReader);
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            return myOntologyConcept;
        }

        /// <summary>
        /// Metodo que retorna una lista de nombres de conceptos (clases) e individuos de la ontología
        /// </summary>       
        public static List<OntologyConceptCopy> ObtenerConceptosOntologia(string ontologia)
        {
            List<OntologyConceptCopy> listaClases = new List<OntologyConceptCopy>();
            List<string> noduplicados = new List<string>();

            string rutaDirActual = System.AppDomain.CurrentDomain.BaseDirectory;
            string rutaRelOwl = @ontologia;
            string rutaOwl = Path.Combine(rutaDirActual, rutaRelOwl);
            //string clases = "";

            rutaOwl = "file:///" + rutaOwl;
            OntModel m;
            m = ModelFactory.createOntologyModel(OntModelSpec.OWL_MEM);
            m.read(rutaOwl, "RDF/XML");

            for (Iterator i = m.listNamedClasses(); i.hasNext(); ) //se recorren todas clases nombradas
            {
                //se la convierte a OntClass para su analisis
                OntClass cls = (OntClass)i.next(); 

                //Añadimos el nombre de las anotaciones Label que tenga la clase
                ExtendedIterator label = cls.listLabels(null);
                while (label.hasNext()) 
                {
                    //Variable temporal en el que se almacena el concepto actual
                    OntologyConceptCopy Oconcept = new OntologyConceptCopy();

                    Oconcept.Urlconcepto = cls.getURI().ToString();
                    RDFNode thisLabel = (RDFNode) label.next();
                    if (thisLabel.isLiteral())
                    {
                        Oconcept.OntCopyNameConcept = ((Literal)thisLabel).getValue().ToString();
                        //Verificar no colocar conceptos duplicados
                        if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                        {
                            listaClases.Add(Oconcept);
                            noduplicados.Add(Oconcept.OntCopyNameConcept);
                        }
                    }
                    else
                    {
                        Oconcept.OntCopyNameConcept = thisLabel.ToString();
                        //Verificar no colocar conceptos duplicados
                        if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                        {
                            listaClases.Add(Oconcept);
                            noduplicados.Add(Oconcept.OntCopyNameConcept);
                        }
                    }
                    
                }
                //Se añaden las instancias. Para que no se aliminada una instancia con el mismo nombre de una clase se inicializa el vector de duplicados
                noduplicados.Clear();
                for (Iterator j = cls.listInstances(true); j.hasNext(); )
                {
                    Individual ind = (Individual) j.next();
                    if (ind.isIndividual())
                    {
                        //Añadimos el nombre de las anotaciones Label que tenga el individuo
                        ExtendedIterator labelind = ind.listLabels(null);
                        while (labelind.hasNext())
                        {
                            //Variable temporal en el que se almacena el concepto actual
                            OntologyConceptCopy Oconcept = new OntologyConceptCopy();
                            Oconcept.Urlconcepto = cls.getURI().ToString();
                            RDFNode thisLabel = (RDFNode)labelind.next();
                            if (thisLabel.isLiteral())
                            {
                                Oconcept.OntCopyNameConcept = ((Literal)thisLabel).getValue().ToString();
                                //Verificar no colocar conceptos duplicados
                                if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                                {
                                    listaClases.Add(Oconcept);
                                    noduplicados.Add(Oconcept.OntCopyNameConcept);
                                }
                            }
                            else
                            {
                                Oconcept.OntCopyNameConcept = thisLabel.toString();
                                //Verificar no colocar conceptos duplicados
                                if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                                {
                                    listaClases.Add(Oconcept);
                                    noduplicados.Add(Oconcept.OntCopyNameConcept);
                                }
                            }

                        }
                    }
                  
                }
            }

            return listaClases;    
        }

        #endregion
    }
}
