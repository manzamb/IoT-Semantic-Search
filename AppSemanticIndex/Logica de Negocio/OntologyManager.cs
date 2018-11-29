using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.Hosting;
using java.io;
using java.util;
using org.semanticweb.owlapi.io;
using org.semanticweb.owlapi.model;
using org.semanticweb.owlapi.apibinding;
using org.semanticweb.owlapi.reasoner;
using org.semanticweb.owlapi.vocab;
using System.Web.Configuration;
using AppSemanticIndex.Pobj;
using System.IO;
using File = java.io.File;
using System.Diagnostics;

namespace AppSemanticIndex
{
    public class OntologyManager
    {
        #region Fields
        // se declara un obj tipo diccionario para todos los conceptos o clases
        private Dictionary<string, OntologyConcept> concepts = new Dictionary<string, OntologyConcept>();

        // se declara un obj tipo diccionario para todos los valores a consultar en Xively (Anotaciones de Conceptos e instancias)
        private List<string> valores = new List<string>();

        // Objeto para el manejo de la Ontologia
        private OWLOntologyManager ontologyManager = null;

        //Ontologia
        OWLOntology ontology = null;

        //Clase Padre
        OWLClass ClassParent = null;

        // URL de la ontologia
        private string url = string.Empty ;

        #endregion

        #region Constructor
        public OntologyManager(string urlOntologia)
        {
            //obtenemos la ruta para la ontologia
            url = urlOntologia;

            //carga el manejador de ontologias
            if (ontologyManager == null)
            {
                ontologyManager = OWLManager.createOWLOntologyManager();
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Obtiene los conceptos por demanda
        /// </summary>
        public Dictionary<string, OntologyConcept> Concepts
        {
            get
            {
                if (this.concepts.Count == 0)
                {
                    this.LoadOWLAPI();
                }
                return this.concepts;
            }
        }

        public List<string> Valores
        {
            get
            {
                if (this.valores.Count == 0)
                {
                    this.LoadOWLAPI();
                }
                return this.valores;
            }
        }

        //fin prueba 
        public string Url
        {
            get 
            {
                //String strAppDir = AppDomain.CurrentDomain.BaseDirectory + "Ontology";
                //String strFullPathToMyFile = Path.Combine(strAppDir, "ontology.owl");
                return this.url; 
            }
            set { this.url = value; }
        }

        public OWLOntology Ontology
        {
            get { return ontology; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// proceso para cargar las clases, subclases y axiomas de la ontologia 
        /// </summary>
        private void LoadOWLAPI()
        {
            //Inicializamos variables
            OntologyConcept ontologyConcept = null;
            
            int weigth = 1;
            string claseKey = string.Empty;
            string claseValor = string.Empty;
            
            //OWLClass _class;
            //Se carga la ontologia
            string path =  url;
            java.io.File file = new java.io.File(path);
            //Aseguramos una ontologia en el managet
            ontology = ontologyManager.loadOntologyFromOntologyDocument(file);

            //Creamos el datafactory para obtener los labels de las clases
            OWLDataFactory df = ontologyManager.getOWLDataFactory();
            OWLAnnotationProperty label = df.getOWLAnnotationProperty(OWLRDFVocabulary.RDFS_LABEL.getIRI());

            //Obtenemos las clases de la ontologia y las recorremos para obtener las anotaciones
            java.util.Set clases = ontology.getClassesInSignature();
            for (java.util.Iterator iteratorClases = clases.iterator(); iteratorClases.hasNext(); )
            {
                OWLClass clase = (OWLClass)iteratorClases.next();
                List<Anotacion> Anotaciones = new List<Anotacion>();
                java.util.Set lit = clase.getAnnotations(ontology, label);
                //Se recorren las anotaciones tipo Label
                for (java.util.Iterator iteratorAnno = lit.iterator(); iteratorAnno.hasNext(); )
                {
                    OWLAnnotation anno = (OWLAnnotation)iteratorAnno.next();
                    OWLLiteral val = (OWLLiteral)anno.getValue();
                    string valorAnotacion = string.Empty;
                    if (val.hasLang("es"))
                    {
                        //Se debe hacer un tratamiento a la cadena por los acentos
                        valorAnotacion = ToAscii(val.getLiteral()).Trim();
                    }
                    else
                    {
                        valorAnotacion = val.getLiteral().Trim();
                    }
                    //Agregar la anotacion a la lista
                    Anotacion annotemp = new Anotacion(anno.getSignature().ToString(), valorAnotacion, val.getLang());
                    Anotaciones.Add(annotemp);
                    //Añadimos la anotacion a la lista de valores a consultar verificando que no este repetido
                    if (!valores.Contains(valorAnotacion) && !string.IsNullOrEmpty(valorAnotacion))
                    {
                        valores.Add(valorAnotacion);
                    }
                    
                    //Reportar la anotacion encontrada
                    Trace.WriteLine(clase.getSignature().ToString() + " -> " + valorAnotacion);
                    //Almacenamos todas anotaciones en un string para el valor de la clase
                    claseValor = claseValor + " " + valorAnotacion;
                    
                    //En caso de tener individuos se almacena en la lista de valores para su búsqueda
                    Trace.WriteLine("Obteniendo conceptos de los individuos");
                    string individuoKey = string.Empty;
                    java.util.Set individuos = clase.getIndividuals(ontology);
                    for (java.util.Iterator iteratorIndividuos = individuos.iterator(); iteratorIndividuos.hasNext(); )
                    {
                        OWLIndividual individuo = (OWLIndividual)iteratorIndividuos.next();
                        java.util.Set litind = individuo.asOWLNamedIndividual().getAnnotations(ontology, df.getRDFSLabel()); 

                        //Se recorre las anotaciones tipo Label individuo
                        for (java.util.Iterator iteratorAnnoind = litind.iterator(); iteratorAnnoind.hasNext(); )
                        {
                            OWLAnnotation annoind = (OWLAnnotation)iteratorAnnoind.next();
                            OWLLiteral valind = (OWLLiteral)annoind.getValue();
                            string valorAnotacionind = string.Empty;
                            if (valind.hasLang("es"))
                            {
                                //Se debe hacer un tratamiento a la cadena por los acentos
                                valorAnotacionind = ToAscii(valind.getLiteral()).Trim();
                            }
                            else
                            {
                                valorAnotacionind = valind.getLiteral().Trim();
                            }
                            //Agregar la anotacion a la lista
                            Anotacion annotempind = new Anotacion(annoind.getSignature().ToString(), valorAnotacionind, valind.getLang());
                            Anotaciones.Add(annotempind);
                            //Añadimos la anotacion a la lista de valores a consultar verificando que no este repetido
                            if (!valores.Contains(valorAnotacionind) && !string.IsNullOrEmpty(valorAnotacionind))
                            {
                                valores.Add(valorAnotacionind);
                            }
                            //Reportar la anotacion encontrada
                            Trace.WriteLine(clase.getSignature().ToString() + ":" + individuo.getSignature().ToString() +" -> " + valorAnotacionind);
                            //Almacenamos todas anotaciones en un string para el valor de la clase
                            claseValor = claseValor + " " + valorAnotacionind;
                        }
                }

                //Agregar el concepto a la lista
                claseKey = clase.getSignature().ToString();
                ontologyConcept = new OntologyConcept(claseKey, claseValor, weigth, clase, clase.getSuperClasses(ontology), clase.getSubClasses(ontology));
                ontologyConcept.Anotaciones = Anotaciones;
                //Reinicializamos el valor para la nueva clase
                claseValor = "";
                //Verificar que se obtengan conceptos diferentes. Las claseKey se repiten siempre que aporten anotaciones diferentes
                if (!this.concepts.ContainsKey(claseKey) && !this.concepts.ContainsValue(ontologyConcept) && !valores.Contains(ontologyConcept.ConceptValue) && !string.IsNullOrEmpty(ontologyConcept.ConceptValue))
                {
                    this.concepts.Add(claseKey, ontologyConcept);
                }
             }
           }
        }

        /// <summary>
        /// Obtiene la llave de la axioma
        /// </summary>
        /// <param name="axiom">Axioma</param>
        /// <returns>Llave</returns>
        private string getKeyAxiom(OWLAnnotationAssertionAxiom axiom)
        {
            return axiom.getSubject().ToString();
        }

        /// <summary>
        /// Obtiene la Valor de la axioma ya que el axioma puede venir con el lenguaje y el tipo de datos en su valor
        /// </summary>
        /// <param name="axiom">Axioma</param>
        /// <returns>Valor de la axioma</returns>
        private string getValueAxiom(OWLAnnotationAssertionAxiom axiom)
        {
            string straxioma = axiom.getValue().ToString();

            //Revisamos si el axiona viene con el idioma
            int existeidioma = straxioma.IndexOf('@');
            //Si los axionas se retornan con el idioma Ej. @es se debe eliminar
            if (existeidioma >0)
            {
                straxioma= straxioma.Remove(existeidioma);
            }

            //Revisamos si el axiona viene con el tipo de datos
            int existetipo = straxioma.IndexOf('^');
            //Si los axionas se retornan con el idioma Ej. @es se debe eliminar
            if (existetipo > 0)
            {
                straxioma = straxioma.Remove(existetipo);
            }

            //Finalmente retornamos el sólo valor del axioma
            return straxioma.Replace('"', ' ').Trim();
         }

        internal int getWeigthByConcept(OntologyConcept ontologyConcept)
        {
            int Weigth = 0;
            if (ClassParent == null)
            {
                //Obtiene solo las clases.
                OWLClass _class = getClass(ontology.getClassesInSignature(), ontologyConcept.ConceptKey);
                if (_class != null)
                {
                    Weigth = getDepthClass(_class);
                }
                //else
                //{
                //    //Al ser individuo no se puede buscar en subclase, por tanto el peso es 1
                //    Weigth = 1;
                //}
            }
            return Weigth;
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

        //Hacer Metodo recursivo que me devuelva la mayor profundidad por OWLClass
       /* private int getDepthClass(OWLClass _class, int level)
        {
            java.util.Set subClass = _class.getSubClasses(ontology);

            java.util.Iterator iterator = subClass.iterator();

            level = level + 1;

            int result = level;

            while (iterator.hasNext())
            {
                int val = getDepthClass((OWLClass)iterator.next(), level);

                if (val > result)
                    result = val;
            }

            return result;
        }*/

        //Hacer Metodo recursivo que me devuelva la mayor profundidad por OWLClass
        private int getDepthClass(OWLClass _class)
        {
            int level = 1;

            java.util.Set supClass = _class.getSuperClasses(ontology);
            //java.util.Iterator iterator = supClass.iterator();

            //int result = level;
            while (supClass != null)
            {
                java.util.Iterator iterator = supClass.iterator();

                supClass = null;

                if (iterator.hasNext())
                {
                    
                    object iteratorNext = iterator.next();
                    if (iteratorNext is OWLClass)
                    {
                        level = level + 1;
                        supClass = (iteratorNext as OWLClass).getSuperClasses(ontology);
                    }
                }
            }

            return level;
        }

        private string ToAscii(string sOriginal)
        {
            //Este procedimiento Recupera la cadena con acentos (Español)
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] byteArray = Encoding.Default.GetBytes(sOriginal);
            byte[] asciiArray = Encoding.Convert(Encoding.UTF8, Encoding.Default, byteArray);
            string finalString = Encoding.Default.GetString(asciiArray);
            return finalString;
        }

        #endregion
    }
}