using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU.ObjetosNegocio
{
    public class OntologyConceptCopy
    {

        #region Atributos
        /// <summary>
        /// Nombre del concepto modificado desde la ontologia
        /// </summary>
        private string _ontCopyNameConcept;

        /// <summary>
        /// Metodo que asigna y obtiene el nombre del concepto de la ontologia modificado
        /// </summary>
        public string OntCopyNameConcept
        {
            get { return _ontCopyNameConcept; }
            set { _ontCopyNameConcept = value; }
        }
        /// <summary>
        /// identificador del concepto escrito de forma original en la ontologia
        /// </summary>
        private int _ontId;

        /// <summary>
        /// Almacena el url que tiene o firma en la Ontologia. Sirve para ubicarlo rapidamente
        /// </summary>
        private string urlconcepto;

        public string Urlconcepto
        {
            get { return urlconcepto; }
            set { urlconcepto = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref="OntolgyConceptCopy"/>.
        /// </summary>
        public void OntolgyConceptCopy()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Propiedades
        /// <summary>
        /// Identificador de un determinada concepto modificado desde la ontologia
        /// </summary>
        private int _ontCopyId;

        /// <summary>
        /// Metodo que asigna y obtiene el identificador del concepto modificado desde la ontologia
        /// </summary>
        public int OntCopyId
        {
            get { return _ontCopyId; }
            set { _ontCopyId = value; }
        }
        /// <summary>
        /// Metodo que asigna y obtiene el identificador del concepto escrito de forma original en la ontologia
        /// </summary>
        public int OntId
        {
            get { return _ontId; }
            set { _ontId = value; }
        }

        #endregion

    }
}
