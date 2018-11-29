using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU.ObjetosNegocio
{
    public class OntologyConcept
    {

        #region Atributos

        /// <summary>
        /// Identificador de un determinado concepto original de la ontologia
        /// </summary>
        private int _ontId;
        /// <summary>
        /// Nombre del concepto tal como esta en la ontologia
        /// </summary>
        private string _ontNameConcept;

        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref="OntolgyConceptCopy"/>.
        /// </summary>
        public void OntolgyConcept()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Metodo que asigna y obtiene el identificador del  concepto original de la ontologia
        /// </summary>
        public int OntId
        {
            get { return _ontId; }
            set { _ontId = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el nombre del concepto original de la ontologia
        /// </summary>
        public string OntNameConcept
        {
            get { return _ontNameConcept; }
            set { _ontNameConcept = value; }
        }

        #endregion

    }
}
