using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using org.semanticweb.owlapi.model;
using System.Runtime.Serialization;

namespace AppSemanticIndex.Pobj
{
    //[AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    [Serializable]
    public class OntologyConcept
    {
        #region Attributes
        private string conceptKey = string.Empty;
        private string conceptValue = string.Empty;
        private int weigth = 0;
        private List<Anotacion> anotaciones;
        [NonSerialized] private OWLClass concept;
        [NonSerialized] private java.util.Set conceptParent;
        [NonSerialized] private java.util.Set conceptChild;

        #endregion

        #region Construtors
        public OntologyConcept()
        {
        }

        public OntologyConcept(string _ConceptKey, string _ConceptName, int _Weigth, OWLClass _Concept, java.util.Set _ConceptParent, java.util.Set _ConceptChild)
            : this()
        {
            this.conceptKey = _ConceptKey;
            this.conceptValue = _ConceptName;
            this.weigth = _Weigth;
            this.concept = _Concept;
            this.conceptParent = _ConceptParent;
            this.conceptChild = _ConceptChild;
        }
        #endregion

        #region Properties
        public string ConceptKey
        {
            set { this.conceptKey = value; }
            get { return this.conceptKey; }
        }
        public string ConceptValue
        {
            set { this.conceptValue = value; }
            get { return this.conceptValue; }
        }
        public int Weigth
        {
            set { this.weigth = value; }
            get { return this.weigth; }
        }

        public OWLClass GetConcept(OWLOntology ontology)
        {
            if(this.concept == null)
                this.concept = getClass(ontology.getClassesInSignature(), this.conceptKey);

            return this.concept;
        }
        
        public java.util.Set GetConceptParents(OWLOntology ontology)
        {
            if (this.conceptParent == null)
            {
                if (GetConcept(ontology) != null) //Si no es una instancia, obtener la clase
                {
                    this.conceptParent = GetConcept(ontology).getSuperClasses(ontology);
                }
                else
                {
                    //Es una instancia entonces no tiene padres
                    this.conceptParent = null;
                }
            }
            return this.conceptParent;
        }

        public java.util.Set GetConceptChild(OWLOntology ontology)
        {
            if (this.conceptChild == null)
                this.conceptChild = GetConcept(ontology).getSubClasses(ontology);

            return this.conceptChild;
        }

        private OWLClass getClass(java.util.Set _Class, string keyOwl)
        {
            java.util.Iterator iterator = _Class.iterator();
            OWLClass _class;

            while (iterator.hasNext())
            {
                _class = (OWLClass)iterator.next();

                if (_class.getIRI().toString().Equals(keyOwl))
                    return _class;
            }

            return null;
        }

        internal List<Anotacion> Anotaciones
        {
            get { return anotaciones; }
            set { anotaciones = value; }
        }
        #endregion
    }
}