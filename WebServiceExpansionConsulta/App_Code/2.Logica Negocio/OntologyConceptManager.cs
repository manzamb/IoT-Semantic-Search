using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ModeloSemantico_PU.ObjetosNegocio;
using ModeloSemantico_PU.AccesoDatos;
using System.Collections.Generic;

namespace ModeloSemantico_PU.LogicaNegocio
{
    public static class OntologyConceptManager
    {
        #region Metodos
        /// <summary>
        /// Metodo que delega a OntologyConceptDB almacenar un OntolgyConcept
        /// </summary>       
        /// <param name="myOntologyConcept">Objeto de tipo OntologyConcept que se va a almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
        public static int Save(OntologyConcept myOntologyConcept)
        {
            return OntologyConceptDB.Save(myOntologyConcept);
        }
        /// <summary>
        /// Metodo que delega a OntologyConceptDB obtener un OntologyConcept
        /// </summary>       
        /// <param name="id">Identificador del OntologyConcept que se va obtener</param>
        /// <returns>retorna Objeto OntologyConcept si el mismo se encuentra,de lo contrario retorna null</returns>
        public static OntologyConcept GetItem(int id)
        {
            return OntologyConceptDB.GetItem(id);
        }

        /// <summary>
        /// Metodo que delega a OntologyConceptDB obtener un OntologyConcept mediante su nombre
        /// </summary>       
        /// <param name="conceptName">Identificador del OntologyConcept que se va obtener</param>
        /// <returns>retorna Objeto OntologyConcept si el mismo se encuentra,de lo contrario retorna null</returns>
        public static OntologyConceptCopy GetItemByConceptName(string conceptName)
        {
            return OntologyConceptCopyDB.GetItemByConceptName(conceptName);
        }

        public static OntologyConceptCopy GetItemByUniqueConceptName(string conceptName)
        {
            return OntologyConceptCopyDB.GetItemByUniqueConceptName(conceptName);
        }

        public static OntologyConceptCopy GetItemConceptInUrl(string conceptName)
        {
            return OntologyConceptCopyDB.GetItemConceptInUrl(conceptName);
        }

        public static OntologyConceptCopy GetItemUniqueConceptInUrl(string conceptName)
        {
            return OntologyConceptCopyDB.GetItemUniqueConceptInUrl(conceptName);
        }
        #endregion

        /// <summary>
        /// Metodo que retorna una lista de nombres de conceptos (clases) e individuos de la ontología
        /// </summary>       
        public static List<OntologyConceptCopy> ObtenerConceptosOntologia(string ontologia)
        {
            return OntologyConceptDB.ObtenerConceptosOntologia(ontologia);
        }

    }
}
