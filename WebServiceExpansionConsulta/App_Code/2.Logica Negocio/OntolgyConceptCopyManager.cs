using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ModeloSemantico_PU.ObjetosNegocio;
using ModeloSemantico_PU.AccesoDatos;

namespace ModeloSemantico_PU.LogicaNegocio
{
    public static class OntologyConceptCopyManager
    {
        #region Metodos
        /// <summary>
        /// Metodo que delega a OntologyConceptCopyDB almacenar un OntologyConceptCopy
        /// </summary>       
        /// <param name="myOntolgyConceptCopy">Objeto de tipo OntologyConceptCopy que se va a almacenar</param>
        /// <returns>retorna 1 si la transaccion se realizo con exito de lo contrario retorna 0</returns>
        public static int Save(OntologyConceptCopy myOntologyConceptCopy)
        {
            return OntologyConceptCopyDB.Save(myOntologyConceptCopy);
        }
        /// <summary>
        /// Metodo que delega a OntologyConceptCopyDB obtener un OntologyConceptCopy
        /// </summary>       
        /// <param name="id">Identificador del OntologyConceptCopy que se va obtener</param>
        /// <returns>retorna Objeto OntologyConceptCopy si el mismo se encuentra,de lo contrario retorna null</returns>
        public static OntologyConceptCopy GetItem(string concepto, string invertConcept)
        {
            return OntologyConceptCopyDB.GetItem(concepto, invertConcept);
        }

        /// <summary>
        /// Metodo que delega a OntologyConceptDB obtener un OntologyConcept
        /// </summary>       
        /// <param name="id">Identificador del OntologyConcept que se va obtener</param>
        /// <returns>retorna Objeto OntologyConcept si el mismo se encuentra,de lo contrario retorna null</returns>
        public static OntologyConceptCopy GetItem(int id)
        {
            return OntologyConceptCopyDB.GetItem(id);
        }
        #endregion

        public static void BorrarConceptos()
        {
            OntologyConceptCopyDB.BorrarConceptos();
        }
    }
}
