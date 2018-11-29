using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.hp.hpl.jena.ontology;
using com.hp.hpl.jena.rdf.model;
using com.hp.hpl.jena.util;
using com.hp.hpl.jena.vocabulary;
using ModeloSemantico_PU.LogicaNegocio;
using ModeloSemantico_PU.ObjetosNegocio;
using ModeloSemantico_PU.ProcesamientoConsulta;


namespace ModeloSemantico_PU.ProcesamientoConsulta
{
    public class OntologiaDominio
    {
        #region Atributos
        /// <summary>
        /// Modelo para acceder a la ontologia
        /// </summary>
        private OntModel model;
        /// <summary>
        /// Nombre del Usuario que inicia sesion
        /// </summary>
        private string userLogin;
        /// <summary>
        /// Lista de terminos no identificados en la ontologia de dominio
        /// </summary>
        private System.Collections.ArrayList _listTerminos;
        /// <summary>
        /// Lista de conceptos compuestos identificados en la ontologia de dominio
        /// </summary>
        private System.Collections.ArrayList _listConceptoCompuesto;
        /// <summary>
        /// Lista de conceptos simples identificados en la ontologia de dominio
        /// </summary>
        private System.Collections.ArrayList _listConceptoSimple;
        /// <summary>
        /// Lista conformada por cada concepto individual perteneciente a un concepto compuesto
        /// </summary>
        private System.Collections.ArrayList _listConcepIndividual;
        /// <summary>
        /// Lista conformada por SuperClass pertenecientes solo a conceptos con mas de una SuperClass 
        /// </summary>
        private System.Collections.ArrayList _listGeneral;
        /// <summary>
        /// Lista conformada por restrinciones pertenecientes a una determinada subClass 
        /// </summary>
        private System.Collections.ArrayList _listRestrictionConceps;
        /// <summary>
        /// Lista conformada por conceptos que forman palabras con in ejemplo "in vivo" por lo tanto no se elimina el in
        /// </summary>        
        private string cadenaRestrictionConceps;
        /// <summary>
        /// Cadena que forman la consulta expandida a ser enviada al buscador Web
        /// </summary>
        private string _cadenaExpandConsult;
        /// <summary>
        /// Cadena que forma los conceptos no encontrados en la ontologia con sus pertenecientes sinonimos
        /// </summary>
        private string _conceptSynonym;


        private int menor;

        private string _archivo;

        private string _ontologyBasePath;

        #endregion

        #region Propiedades
        /// <summary>
        /// Propiedad que asigna y obtiene el Modelo para acceder a las funciones de la ontologia
        /// </summary>
        public OntModel Model
        {
            get { return model; }
            set { model = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene el Login del usuario
        /// </summary>
        public string UserLogin
        {
            get { return userLogin; }
            set { userLogin = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene una lista de terminos no identificados en la ontologia de dominio 
        /// </summary>
        public System.Collections.ArrayList ListTerminos
        {
            get { return _listTerminos; }
            set { _listTerminos = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene una lista de conceptos compuestos identificados en la ontologia de dominio 
        /// </summary>
        public System.Collections.ArrayList ListConceptoCompuesto
        {
            get { return _listConceptoCompuesto; }
            set { _listConceptoCompuesto = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene una lista de conceptos simples identificados en la ontologia de dominio 
        /// </summary>
        public System.Collections.ArrayList ListConceptoSimple
        {
            get { return _listConceptoSimple; }
            set { _listConceptoSimple = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene una lista de conceptos individuales pertenecientes a un concepto compuesto
        /// </summary>
        public System.Collections.ArrayList ListConcepIndividual
        {
            get { return _listConcepIndividual; }
            set { _listConcepIndividual = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene una lista de SuperClass pertenecientes a un concepto con mas de 1 SuperClass
        /// </summary>
        public System.Collections.ArrayList ListGeneral
        {
            get { return _listGeneral; }
            set { _listGeneral = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene una lista de Restricciones pertenecientes a una subClase o concepto
        /// </summary>
        public System.Collections.ArrayList ListRestrictionConceps
        {
            get { return _listRestrictionConceps; }
            set { _listRestrictionConceps = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene una cadena conformada por restrinciones pertenecientes a una determinada subClass 
        /// </summary>
        public string CadenaRestrictionConceps
        {
            get { return cadenaRestrictionConceps; }
            set { cadenaRestrictionConceps = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene una cadena conformada por conceptos a ser enviados al buscador Web
        /// </summary>
        public string CadenaExpandConsult
        {
            get { return _cadenaExpandConsult; }
            set { _cadenaExpandConsult = value; }
        }
        /// <summary>
        /// Propiedad que asigna y obtiene una cadena conformada por conceptos que no pertenecen a la ontologia con sus pertenecientes sinonimos 
        /// </summary>
        public string ConceptSynonym
        {
            get { return _conceptSynonym; }
            set { _conceptSynonym = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref=" OntologiaDominio"/>.
        /// </summary>
        public OntologiaDominio()
        {
            this._listTerminos = new System.Collections.ArrayList();
            this._listConceptoCompuesto = new System.Collections.ArrayList();
            this._listConceptoSimple = new System.Collections.ArrayList();
            this._listConcepIndividual = new System.Collections.ArrayList();
            this._listGeneral = new System.Collections.ArrayList();
            this._listRestrictionConceps = new System.Collections.ArrayList();
            this._archivo = "ontologiaambiental.owl";
            this._ontologyBasePath = "http://localhost/default";
            this.cadenaRestrictionConceps = "";
            this._cadenaExpandConsult = "";
            this._conceptSynonym = "";
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Metodo que lee una ontologia 
        /// </summary>
        public void readOntology(string archivo)
        {
            String inputFileName = archivo; //"LMG.owl";            
            // create an empty model
            //OntModelSpec oms = new OntModelSpec(OntModelSpec.OWL_MEM);
            this.Model = ModelFactory.createOntologyModel(OntModelSpec.OWL_MEM);

            // use the FileManager to find the input file
            java.io.InputStream inputStream = FileManager.get().open(inputFileName);
            if (inputStream == null)
            {
                throw new ArgumentException("File: " + inputFileName + " not found");
            }
            // read the RDF/XML file
            model.read(new java.io.InputStreamReader(inputStream), "");
        }

        /// <summary>
        /// Metodo que inicializa la ontologia de dominio
        /// </summary> 
        public void init_ontology(OntModel modelo)
        {
            this.Model = modelo;
        }
        /// <summary>
        /// Metodo que obtiene todos las ramas pertenecientes a una ontologia desde un concepto hasta el concepto cabeza de la ontologia
        /// </summary> 
        public System.Collections.ArrayList conceptHirarchicalLines(string parentConcept, System.Collections.ArrayList conceptPath, System.Collections.ArrayList auxConceptPath, System.Collections.ArrayList markedSuperClass, string rootParent, System.Collections.Generic.Dictionary<string, System.Collections.ArrayList> dicSubClassManySuperClass, string aux)
        {
            //Inserta cada concepto(parentConcept) el cual hace parte de una linea jerarquica
            conceptPath.Add(parentConcept);
            auxConceptPath.Add(parentConcept);

            com.hp.hpl.jena.util.iterator.ExtendedIterator hijos = null;
            com.hp.hpl.jena.util.iterator.ExtendedIterator listSuperClass = null;

            java.util.List listasuperClass = null;

            int relaSalenConcepto = 0;
            int numSuperClass = 0;
            int pos = 0;
            int hasta = 0;
            int i = 0;

            bool enc = false;

            string SuperClass = "";
            string subClass = "";

            OntologyConceptCopy con = new OntologyConceptCopy();
            con = OntologyConceptManager.GetItemByConceptName(parentConcept);
            if (con != null)
            {
                subClass = con.Urlconcepto;
            }
            else
            {
                subClass = this._ontologyBasePath + this._archivo + "#" + parentConcept;
            }
            //Obtiene los antecesores o SuperClass de subClass
            hijos = getChildEdges(this.model, subClass);
            java.util.List listParentsSubClass = hijos.toList();

            //Filtra solo las SuperClass de tipo Class de un concepto 
            System.Collections.ArrayList listSuperClassSubclass = new System.Collections.ArrayList();
            listSuperClassSubclass = getTypeClass(listParentsSubClass);

            subClass = parentConcept;
            //Codicional solo para subClass con mas de 2 SuperClass,obteniendo un listado de dichas SuperClass
            if (dicSubClassManySuperClass.ContainsKey(subClass))
            {
                ListGeneral = dicSubClassManySuperClass[subClass];
            }

            while (i < listSuperClassSubclass.Count)
            {
                relaSalenConcepto++; //Cuenta superClases de subClass
                OntClass nodo;
                OntologyConceptCopy consub = new OntologyConceptCopy();
                consub = OntologyConceptManager.GetItemByConceptName(listSuperClassSubclass[i].ToString());
                //consub = OntologyConceptManager.GetItemByConceptName(listSuperClassSubclass[i].ToString());
                if (consub != null)
                {
                    nodo = model.getOntClass(consub.Urlconcepto);
                }
                else
                {
                    nodo = model.getOntClass(this._ontologyBasePath + this._archivo + "#" + listSuperClassSubclass[i].ToString());
                }
                SuperClass = nodo.getLocalName().ToString();
                listSuperClass = nodo.listSuperClasses();
                listasuperClass = listSuperClass.toList();

                System.Collections.ArrayList listSuperClassNodo = new System.Collections.ArrayList();
                listSuperClassNodo = getTypeClass(listasuperClass);
                /*                              [0   0]=listSuperClassNodo.Count=2              */
                /*                                \ /                                           */
                /* listSuperClassSubclass.Count=1 [0] SuperClass se elimina de markedSuperClass */
                /*                                 |                                            */
                /*                       subClass=[0]                                           */
                if (markedSuperClass.Contains(SuperClass) && listSuperClassNodo.Count > 1 && listSuperClassSubclass.Count <= 1)
                {
                    pos = markedSuperClass.IndexOf(SuperClass);
                    if (pos != -1)
                    {
                        markedSuperClass.RemoveAt(pos);
                        //enc=false ejecuta logica del condicion(enc==false)
                        enc = false;
                        break;
                    }
                }
                else
                {
                    if (!markedSuperClass.Contains(SuperClass))
                    {   //enc=true ejecuta logica del condicion(enc==true) 
                        enc = true;
                        break;
                    }
                    else
                    {
                        numSuperClass++;//Cuenta superClases de subClass que se encuentran en markedSuperClass o marcadas 
                    }
                }
                i++;
            }
            /*condicional que contiene la logica de insertar subClass en markedSubClass*/
            if (enc == false)
            {   //condicional que elimina conceptos que no hacen parte o no forman una linea jerarquica 
                /*   C1                    */
                /*   |                     */
                /*   C2 Termina linea jerarquica */
                /*   |                     */
                /*   C1 Elimina concepto   */
                if (relaSalenConcepto != 0)
                {
                    pos = conceptPath.LastIndexOf(rootParent);
                    hasta = (conceptPath.Count) - pos;
                    conceptPath.RemoveRange(pos, hasta);
                    int pos1 = auxConceptPath.LastIndexOf(rootParent);
                    int hasta1 = (auxConceptPath.Count) - pos1;
                    auxConceptPath.RemoveRange(pos1, hasta1);
                }

                if (auxConceptPath.Count < menor && auxConceptPath.Count != 0)
                    this.menor = auxConceptPath.Count;
                auxConceptPath.Clear();

                //inserta la subClass y pasa a ser marcada  
                markedSuperClass.Insert(0, subClass);

                if (markedSuperClass.Count == 1)
                    aux = subClass;
                /*  0 aux=subClass   */
                /*  |                */
                /*  0                */
                //subClass no tiene antecesores por lo tanto listSuperClassSubclass.Count=0
                if (listSuperClassSubclass.Count == 0)
                    aux = subClass;
                /* [0   0]=listParentsSubClass.size()=2 */
                /*   \ /                                            */
                /*    0   subClass                                 */
                //subClass al tener todos sus SuperClass en markedSuperClass numSuperClass es igual a listSuperClassSubclass.Count  
                if (numSuperClass == listSuperClassSubclass.Count && listSuperClassSubclass.Count > 1)
                {
                    pos = markedSuperClass.IndexOf(subClass);
                    markedSuperClass.RemoveRange(pos + 1, listSuperClassSubclass.Count);
                }
                //Si subClass esta contenida en listGeneral,hace parte de una lista con mas de 2 SuperClass
                /*   [0]= aux eliminado de markedSuperClass    */
                /*    |                                        */
                /*    0 pos+1  eliminado de markedSuperClass   */
                /*   / \                                       */
                /* {0  [0]=subClass }=listGeneral              */
                /*   \ /                                       */
                /*    0                                        */
                if (ListGeneral.Contains(subClass) == true)
                {
                    pos = markedSuperClass.IndexOf(subClass);
                    hasta = markedSuperClass.IndexOf(aux);
                    if (hasta != -1)
                        markedSuperClass.RemoveRange(pos + 1, hasta);
                }
                //Si se cumple condicional,significa que ya no hay mas ramas o lineas jerarquicas por extraer
                if (subClass == rootParent)
                    return conceptPath;

                //Nuevamente se pasa el concepto de origen(rootParent) con el fin de explorar antecesores sin marcar               
                return conceptHirarchicalLines(rootParent, conceptPath, auxConceptPath, markedSuperClass, rootParent, dicSubClassManySuperClass, aux);
            }
            /*condicional que pasa como parametro SuperClass antecesora de subClass a la funcion conceptHirarchicalLines para verificar si tiene
              SuperClass sin marcar*/
            if (enc == true)
            {
                //Inserta subClass que contengan mas de 2 SuperClass
                if (listSuperClassSubclass.Count > 1 && dicSubClassManySuperClass.ContainsKey(subClass) == false)
                {
                    dicSubClassManySuperClass.Add(subClass, listSuperClassSubclass);
                }
                //Pasa SuperClass como parametro a la funcion conceptHirarchicalLines para verificar sus antecesores sin marcar                
                if (markedSuperClass.Contains(SuperClass) == false)
                {
                    return conceptHirarchicalLines(SuperClass, conceptPath, auxConceptPath, markedSuperClass, rootParent, dicSubClassManySuperClass, aux);
                }
            }
            return conceptPath;
        }
        /// <summary>
        /// Metodo que identifica conceptos compuestos y simples a partir de una ontologia
        /// </summary>
        public void termsIdentification(System.Collections.ArrayList keywords)
        {
            OntClass oclass;

            int tam = keywords.Count; ;

            string conceptoCompuesto;
            string concepto;
            string cc = "";

            bool enc = false;
            bool igualLineaJeraquica = false;

            System.Collections.ArrayList conceptsPath;
            System.Collections.ArrayList markedSuperClass;
            System.Collections.ArrayList auxConceptsPath;
            System.Collections.ArrayList ListConceptAntecesor = new System.Collections.ArrayList();

            System.Collections.Generic.Dictionary<string, System.Collections.ArrayList> dicSubClassManySuperClass;

            for (int i = 0; i < tam; i++)
            {
                enc = false;
                for (int j = i; j < tam; j++)
                {
                    //la funcion termsConcatenation verifica la existencia de terminos compuestos o simples almacenados en la ontologia
                    concepto = termsConcatenation(keywords, i, tam - (j - i), this._archivo);

                    OntologyConceptCopy con = new OntologyConceptCopy();
                    con = OntologyConceptManager.GetItemByConceptName(concepto);
                    if (con != null)
                    {
                        oclass = this.model.getOntClass(con.Urlconcepto);
                    }
                    else
                    {
                        oclass = this.model.getOntClass(this._ontologyBasePath + this._archivo + "#" + concepto);
                    }
               
                    //el concepto se encuentra en la ontologia si oclass es diferente de null
                    if (oclass != null)
                    {   //Verifica que el concepto sea compuesto
                        if (concepto.Contains("_"))
                        {
                            cc = "";
                            //concatena todos los conceptos compuestos pertenecientes a this._listConceptoCompuesto
                            foreach (string item in this._listConceptoCompuesto)
                            {
                                cc += item + " ";
                            }
                            //verifica que el concepto compuesto(concepto) no haga parte de ningun concepto compuesto 
                            //es decier que no este contenido en la concatenacion de conceptos compuestos cc
                            if (!cc.Contains(concepto))
                            {
                                conceptoCompuesto = concepto;
                                this._listConceptoCompuesto.Add(conceptoCompuesto);
                                enc = true;

                                conceptsPath = new System.Collections.ArrayList();
                                markedSuperClass = new System.Collections.ArrayList();
                                auxConceptsPath = new System.Collections.ArrayList();
                                dicSubClassManySuperClass = new Dictionary<string, System.Collections.ArrayList>();
                                this._listGeneral = new System.Collections.ArrayList();

                                this.CadenaExpandConsult += "+ (" + conceptoCompuesto;

                                conceptsPath = conceptHirarchicalLines(conceptoCompuesto, conceptsPath, auxConceptsPath, markedSuperClass, conceptoCompuesto, dicSubClassManySuperClass, "");
                                //cada concepto por separado los cuales hacen parte de conceptoCompuesto
                                //se verifica que ninguno sea padre de conceptoCompuesto
                                //el proceso de this._listConcepIndividual se encuentra en la Metodo:termsConcatenation
                                foreach (string conceptoSimple in this._listConcepIndividual)
                                {
                                    OntologyConceptCopy cons = new OntologyConceptCopy();
                                    //lAS SIGUIENTES 2 LINEAS verifica si conceptoSimple existe en forma individual en la ontologia
                                    cons = OntologyConceptManager.GetItemByConceptName(concepto);
                                    if (cons != null)
                                    {
                                        oclass = this.model.getOntClass(cons.Urlconcepto);
                                    }
                                    else
                                    {
                                        oclass = this.model.getOntClass(this._ontologyBasePath + this._archivo + "#" + concepto);
                                    }
                                    if (oclass != null)
                                    {
                                        igualLineaJeraquica = false;
                                        //Si esta contenido dentro de los conceptos que forman lineas jerarquicas extraidas a partir
                                        //del conceptoCompuesto entonces conceptoSimple es antecesor de conceptoCompuesto
                                        if (conceptsPath.Contains(conceptoSimple))
                                        {
                                            igualLineaJeraquica = true;
                                            //lista que almacena conceptos simples que se encuetran en la misma linea jerarquica que
                                            //el concepto compuesto lo que implica que concepto simple es antecesor de concepto compuesto
                                            ListConceptAntecesor.Add(conceptoSimple);
                                        }
                                        //No esta contenido dentro de los conceptos que forman lineas jerarquicas extraidas a partir
                                        //del conceptoCompuesto entonces conceptoSimple no es antecesor de conceptoCompuesto
                                        //es decir conceptoSimple se encuentra en otra linea jerarquica de la ontologia
                                        if (igualLineaJeraquica == false)
                                        {
                                            if (!(this._listConceptoSimple.Contains(conceptoSimple)))
                                            {
                                                this._listConceptoSimple.Add(conceptoSimple);
                                                this.CadenaExpandConsult += " | " + conceptoSimple;
                                            }
                                        }
                                    }
                                }
                                this.CadenaExpandConsult += ")";//resultado (conceptoCompuesto | conceptoSimple_i | conceptoSimple_i+1... | conceptoSimple_n)

                                //cadenaRestrictionConceps es una concatenacion de conceptos que representan restricciones de un concepto 
                                //en este caso del comceptoCompuesto y este proceso se observa en el metodo auxiliar:getTypeClass
                                if (this.cadenaRestrictionConceps != "")
                                {
                                    this.CadenaExpandConsult += this.cadenaRestrictionConceps; //resultado (conceptoCompuesto | conceptoSimple_i | conceptoSimple_i+1... | conceptoSimple_n) + conceptos de restriccion del conceptoCompuesto
                                    this.cadenaRestrictionConceps = "";
                                }
                            }
                            // enc=true:significa que se identifico un concepto compuesto el cual fue almacenado en this._listConceptoCompuesto
                            //saliendose del segundo for, debido a que los demas conceptos a identificar ya se encuentran dentro el conceptoCompuesto
                            //identificado
                            if (enc == true)
                                break;
                            else//de lo contrario queda el concepto simple el cual no se identifica como concepto compuesto 
                                if (!ListConceptAntecesor.Contains(concepto) && !(this._listConceptoSimple.Contains(concepto)) && !concepto.Contains("_"))
                                    this._listConceptoSimple.Add(concepto);
                        }
                        else
                        {
                            cc = "";
                            //concatena todos los conceptos compuestos pertenecientes a this._listConceptoCompuesto
                            foreach (string item in this._listConceptoCompuesto)
                            {
                                cc += item + " ";
                            }
                            //verifica que el concepto el cual no es concepto compuesto no haga parte de ningun 
                            //concepto compuesto dentro de la concatenacion de conceptos compuestos cc
                            if (!(this._listConceptoSimple.Contains(concepto)) && !cc.Contains(concepto))
                                this._listConceptoSimple.Add(concepto);
                        }
                    }
                    else//condicional que indica que el concepto no se encuentra en la ontologia 
                    {
                        //concepto.Contains(" ")=false:indica que el concepto no es compuesto                                                 
                        if (!concepto.Contains(" ") && concepto != "")
                        {
                            //obtiene el sinonino mas usado del concepto
                            //la clase srvWordNet se encuentra en la solucion denominada WSWordNet el cual es un servicio web 
                            //libreria IKVM.OpenJDK.ClassLibrary, jaws-bin

                            srvWordNet.WebServiceWordNet sinonimos = new srvWordNet.WebServiceWordNet();
                            string sinonimo = sinonimos.getSynonym(concepto);
                            if (sinonimo != " ")
                                this._conceptSynonym += " + " + sinonimo;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Metodo que calcula la similitud entre la combinatoria de cada concepto de la consulta
        /// </summary>
        public System.Collections.ArrayList similarityBetweenConcepts(System.Collections.ArrayList keywords)
        {
            
            System.Collections.ArrayList pathsMaxDepthConcept = new System.Collections.ArrayList();
            System.Collections.ArrayList pathsMinDepthConcept = new System.Collections.ArrayList();
            System.Collections.ArrayList conceptsPath;
            System.Collections.ArrayList auxConceptsPath;
            System.Collections.ArrayList markedSuperClass;
            System.Collections.ArrayList auxPathsMaxDepthConcept = new System.Collections.ArrayList(); ;
            System.Collections.ArrayList conceptsExpanded = new System.Collections.ArrayList();
            System.Collections.ArrayList ListHirarchicalLinesC1 = new System.Collections.ArrayList();
            System.Collections.ArrayList ListHirarchicalLinesC2 = new System.Collections.ArrayList();

            System.Collections.Generic.Dictionary<string, System.Collections.ArrayList> dicSubClassManySuperClass;

            OntClass oclassC1;
            OntClass oclassC2;

            System.Collections.ArrayList arraySimilarity = new System.Collections.ArrayList();

            int j = 0;

            double N = 0;
            double similarityValue = 0;
            double mayor = double.MinValue;

            bool band = false;
            bool band2 = false;

            string padrecomun = "";
            string minDepthConcept = "";
            string maxDepthConcept = "";
            string auxMinDepthConcept = "";
            string auxMaxDepthConcept = "";

            try
            {
                foreach (string concepto1 in keywords)
                {
                    j++;
                    markedSuperClass = new System.Collections.ArrayList();
                    conceptsPath = new System.Collections.ArrayList();
                    auxConceptsPath = new System.Collections.ArrayList();
                    this._listGeneral = new System.Collections.ArrayList();

                    dicSubClassManySuperClass = new Dictionary<string, System.Collections.ArrayList>();
                    //Obtiene la minima profundidad entre las diferentes lineas jerarquicas de el concepto1 hasta el concepto penultimo antes de la raiz(thing) de la ontologia
                    this.menor = int.MaxValue;
                    //obtiene el conjunto de conceptos que conforman las lineas jerarquicas del concepto1
                    ListHirarchicalLinesC1 = conceptHirarchicalLines(concepto1, conceptsPath, auxConceptsPath, markedSuperClass, concepto1, dicSubClassManySuperClass, "");
                    //N1 representa la profundidad desde el concepto1 hasta el concepto penultimo antes de la raiz(thing) de la ontologia                
                    double N1 = (double)this.menor - 1;

                    //Concatena a this._cadenaExpandConsult conceptos que representan restricciones de solo conceptos simples
                    //debido a que a los compuestos se realizo en el Metodo:termsIdentification y ademas el concepto
                    //Simple no debe estar contenida en this._cadenaExpandConsult para que no haya ambiguedad de conceptos 
                    //el proceso de this.cadenaRestrictionConceps se puede ver en el Metodo auxiliar:getTypeClass
                    if (this._listConceptoSimple.Contains(concepto1) && !this._cadenaExpandConsult.Contains(concepto1))
                    {
                        this._cadenaExpandConsult += " + " + concepto1 + this.cadenaRestrictionConceps;
                        this.cadenaRestrictionConceps = "";

                    }

                    for (int i = j; i < keywords.Count; i++)
                    {
                        OntologyConceptCopy con1 = new OntologyConceptCopy();
                        con1 = OntologyConceptManager.GetItemByConceptName(concepto1);
                        if (con1 != null)
                        {
                            oclassC1 = model.getOntClass(con1.Urlconcepto);
                        }
                        else
                        {
                            oclassC1 = model.getOntClass(this._ontologyBasePath + this._archivo + "#" + concepto1);
                        }
                        OntologyConceptCopy con2 = new OntologyConceptCopy();
                        con2 = OntologyConceptManager.GetItemByConceptName(keywords[i].ToString());
                        if (con2 != null)
                        {
                            oclassC2 = model.getOntClass(con2.Urlconcepto);
                        }
                        else
                        {
                            oclassC2 = model.getOntClass(this._ontologyBasePath + this._archivo + "#" + keywords[i].ToString());
                        }

                        band = false;

                        dicSubClassManySuperClass = new Dictionary<string, System.Collections.ArrayList>();

                        markedSuperClass = new System.Collections.ArrayList();
                        conceptsPath = new System.Collections.ArrayList();
                        auxConceptsPath = new System.Collections.ArrayList();
                        this._listGeneral = new System.Collections.ArrayList();

                        dicSubClassManySuperClass = new Dictionary<string, System.Collections.ArrayList>();

                        //Obtiene la minima profundidad entre las diferentes lineas jerarquicas de el concepto keywords[i] hasta el concepto raiz de la ontologia
                        this.menor = int.MaxValue;
                        //obtiene el conjunto de conceptos que conforman las lineas jerarquicas del concepto keywords[i]
                        ListHirarchicalLinesC2 = conceptHirarchicalLines(keywords[i].ToString(), conceptsPath, auxConceptsPath, markedSuperClass, keywords[i].ToString(), dicSubClassManySuperClass, "");
                        //N2 representa la profundidad desde el concepto keywords[i] hasta el concepto penultimo antes de la raiz(thing) de la ontologia                   
                        double N2 = (double)this.menor - 1;

                        if (N1 > N2)
                        {   //Conjunto de lineas jerarquicas obtenidas a partir del concepto1 el cual tiene maxima profundidad respecto al concepto keywords[i]
                            pathsMaxDepthConcept = ListHirarchicalLinesC1;
                            //Conjunto de lineas jerarquicas obtenidas a partir del keywords[i] el cual tiene minima profundida respecto al concepto1
                            pathsMinDepthConcept = ListHirarchicalLinesC2;
                            //maxDepthConcept: concepto que tiene la maxima profundiad
                            maxDepthConcept = concepto1;
                            //minDepthConcept:concepto que tiene la minima profundiad
                            minDepthConcept = keywords[i].ToString();
                            //N profundidad desde el concepto1 hasta el concepto penultimo antes de la raiz(thing) de la ontologia                    
                            N = N1;

                        }
                        else
                        {   //Conjunto de lineas jerarquicas obtenidas a partir del concepto keywords[i] el cual tiene maxima profundida respecto al concepto1 
                            pathsMaxDepthConcept = ListHirarchicalLinesC2;
                            //Conjunto de lineas jerarquicas obtenidas a partir del concepto1 el cual tiene minima profundida respecto al concepto keywords[i]
                            pathsMinDepthConcept = ListHirarchicalLinesC1;
                            //maxDepthConcept: concepto que tiene la maxima profundiad
                            maxDepthConcept = keywords[i].ToString();
                            //minDepthConcept:concepto que tiene la minima profundiad
                            minDepthConcept = concepto1;
                            //N profundidad desde el concepto keywords[i] hasta el concepto penultimo antes de la raiz(thing) de la ontologia                   
                            N = N2;

                        }

                        //condicion que verifica si un concepto de < profundidad se encuentra en la misma linea jerarquica que uno con > profundidad 
                        if (pathsMaxDepthConcept.Contains(minDepthConcept))
                        {
                            similarityValue = (2 * N) / (N1 + N2);
                            band = true;
                        }
                        else
                        {
                            padrecomun = "";
                            //obtiene la raiz el cual es el concepto penultimo antes de la raiz(thing) de la ontologia 
                            string raiz = pathsMinDepthConcept[pathsMinDepthConcept.Count - 1].ToString();
                            //recorre cada concepto de la lista de lineas jerarquicas(pathsMinDepthConcept) extraidas del concepto de minima profundidad                        
                            foreach (string s in pathsMinDepthConcept)
                            {   //y lo busca en la lista de lineas jerarquicas(pathsMaxDepthConcept)extraidas del concepto de maxima profundidad 
                                if (pathsMaxDepthConcept.Contains(s))
                                {
                                    padrecomun = s;
                                    break;
                                }
                            }
                            //el padre comun no debe ser nulo ni debe ser igual a la raiz de cada linea jerarquica perteneciente a pathsMinDepthConcept
                            //debido a que la profundidad de la raiz o concepto penultimo hasta la raiz thing de la ontologia es cero
                            if (padrecomun != "" && padrecomun.Equals(raiz) == false)
                            {
                                markedSuperClass = new System.Collections.ArrayList();
                                conceptsPath = new System.Collections.ArrayList();
                                auxConceptsPath = new System.Collections.ArrayList();
                                this._listGeneral = new System.Collections.ArrayList();

                                dicSubClassManySuperClass = new Dictionary<string, System.Collections.ArrayList>();

                                this.menor = int.MaxValue;
                                ListHirarchicalLinesC2 = conceptHirarchicalLines(padrecomun, conceptsPath, auxConceptsPath, markedSuperClass, padrecomun, dicSubClassManySuperClass, "");
                                N = (double)this.menor - 1;

                                similarityValue = (2 * N) / (N1 + N2);
                            }
                            //similitud cero debido a que la profundidad desde el PADRE COMUN el cual seria la raiz o concepto penultimo hasta la raiz thing de la ontologia es cero
                            //es decir N=0; o si no existe padre comun entonces la similitud es cero
                            if (padrecomun.Equals(raiz) || padrecomun == "")
                                similarityValue = 0;
                        }
                        //Obtiene la maxima similitud
                        if (similarityValue > mayor && similarityValue != 0)
                        {
                            if (band == true)
                            {
                                //los conceptos(auxMaxDepthConcept y auxMinDepthConcept) estan en la misma linea jerarquica
                                //y su similitud es mayor que la mas alta similitud(mayor) es decir similarityValue > mayor
                                auxPathsMaxDepthConcept = new System.Collections.ArrayList();
                                //almacena LA LISTA DE LINEAS JERARQUICAS extraidas del concepto maxDepthConcept  
                                auxPathsMaxDepthConcept = pathsMaxDepthConcept;
                                //almacena el concepto con minima profundidad
                                auxMinDepthConcept = minDepthConcept;
                                //almacena el concepto con maxima profundidad
                                auxMaxDepthConcept = maxDepthConcept;
                                band2 = true;
                            }
                            else
                            {   //los conceptos(maxDepthConcept y minDepthConcept) no estan en la misma linea jerarquica
                                band2 = false;
                            }
                            mayor = similarityValue;
                        }
                    }
                }
                //EXTRACCION DE CONCEPTOS COMO RESULTADO DE LA MAYOR SIMILITUD
                if (band2 == false)
                {   //concepto de expansion por similitud es padre comun
                    if (!padrecomun.Equals(""))
                        conceptsExpanded.Add(padrecomun);
                }
                else
                {
                    //obtiene todos los conceptos que se encuentre desde el concepto auxMaxDepthConcept hasta el concepto auxMinDepthConcept 
                    /*                              _ 
                          c6 auxMaxDepthConcept      |   
                          |                          |  
                          c5                         | 
                         / \                         | 
                        c3  c4                       | => lineas jerarquicas de lista auxPathsMaxDepthConcept  
                         \ /                         | 
                          c2 auxMinDepthConcept      | 
                          |                          | 
                          c1                        _| 
                       lista conceptsExpanded={c5,c3,c4} */

                    System.Collections.ArrayList auxConceptsExpanded = new System.Collections.ArrayList();
                    mayor = int.MinValue;
                    string conceptExpand = "";
                    for (int i = 0; i < auxPathsMaxDepthConcept.Count - 1; i++)
                    {
                        int pos = auxPathsMaxDepthConcept.IndexOf(auxMinDepthConcept);

                        //if (conceptsExpanded.Contains(auxPathsMaxDepthConcept[i].ToString()) == false && auxPathsMaxDepthConcept[i].ToString().Equals(auxMaxDepthConcept) == false && auxPathsMaxDepthConcept[i].ToString().Equals(auxMinDepthConcept) == false)
                        //{
                        //    OntologyConcept myOntologyConcept = new OntologyConcept();
                        //    myOntologyConcept = OntologyConceptManager.GetItemByConceptName(auxPathsMaxDepthConcept[i].ToString());
                        //    PerfilUsuario myPerfilUsuario = new PerfilUsuario();
                        //    myPerfilUsuario = PerfilUsuarioManager.GetItem(this.UserLogin, myOntologyConcept.OntId);
                        //    auxConceptsExpanded.Add(auxPathsMaxDepthConcept[i].ToString());
                        //    if(myPerfilUsuario != null)
                        //    {
                        //        if (myPerfilUsuario.Wrud > mayor)
                        //        {
                        //            conceptExpand = auxPathsMaxDepthConcept[i].ToString();
                        //            mayor = myPerfilUsuario.Wrud;
                        //        }
                        //    }                        
                        //}
                        if (auxPathsMaxDepthConcept[i].ToString().Equals(auxMinDepthConcept) == true)
                        {
                            auxPathsMaxDepthConcept.RemoveRange(0, pos);
                            int pos1 = auxPathsMaxDepthConcept.IndexOf(auxMaxDepthConcept);
                            pos = auxPathsMaxDepthConcept.IndexOf(auxMinDepthConcept);
                            if (pos1 == -1)
                                break;
                            auxPathsMaxDepthConcept.RemoveRange(pos, pos1);
                            i = 0;
                        }
                    }
                    if (conceptExpand != "")
                    {
                        conceptsExpanded.Add(conceptExpand);
                    }
                    else
                    {
                        conceptsExpanded = auxConceptsExpanded;
                    }
                }
                return conceptsExpanded;
            }
            catch (Exception ex)
            {
                //En el caso de un error salir
                return conceptsExpanded;
            }
        }

        #region Metodos Auxiliares
        /// <summary>
        /// Metodo Auxiliar que retorna las SuperClass de un concepto
        /// </summary>        
        public com.hp.hpl.jena.util.iterator.ExtendedIterator getChildEdges(OntModel model, string concepto)
        {
            OntClass oclass = model.getOntClass(concepto);
            com.hp.hpl.jena.util.iterator.ExtendedIterator it = null;
            it = oclass.listSuperClasses();
            return it;
        }
        /// <summary>
        /// Metodo Auxiliar que retorna solo las SuperClass de tipo Class o Restriction de un concepto
        /// </summary>      
        public System.Collections.ArrayList getTypeClass(java.util.List listParentsSubClass)
        {
            System.Collections.ArrayList listSuperClassSubclass = new System.Collections.ArrayList();
            int j = 0;
            while (j < listParentsSubClass.size())
            {
                OntClass nodo1 = (OntClass)listParentsSubClass.get(j);
                if (nodo1.getRDFType().getLocalName().ToString().Equals("Class"))
                    listSuperClassSubclass.Add(nodo1.getLocalName().ToString());
                if (nodo1.isRestriction())
                {
                    Restriction r = nodo1.asRestriction();
                    if (r.isSomeValuesFromRestriction())
                    {
                        SomeValuesFromRestriction some = r.asSomeValuesFromRestriction();
                        string propiedad = some.getOnProperty().getURI();
                        OntClass auxNodo = model.getOntClass(some.getSomeValuesFrom().getURI());
                        if (!this._listRestrictionConceps.Contains(auxNodo.getLocalName()) && propiedad.Contains("_Has_"))
                        {
                            if (!this.CadenaExpandConsult.Contains(auxNodo.getLocalName() + "_") || !this.CadenaExpandConsult.Contains("_" + auxNodo.getLocalName() + "_") || !this.CadenaExpandConsult.Contains("_" + auxNodo.getLocalName()))
                            {
                                this._listRestrictionConceps.Add(auxNodo.getLocalName());
                                this.cadenaRestrictionConceps += " + " + auxNodo.getLocalName();
                            }
                        }
                    }
                }
                j++;
            }

            return listSuperClassSubclass;
        }
        /// <summary>
        /// Metodo Auxiliar que concatena terminos de la consulta que posiblemente pueden ser conceptos compuestos o simples
        /// </summary>
        public string termsConcatenation(System.Collections.ArrayList keywords, int inicio, int tam, string archivo)
        {
            string cadena = "";
            string cadenainversa = "";
            string[] token;
            string auxcadena = "";

            int auxtam = tam;
            this._listConcepIndividual = new System.Collections.ArrayList();
            System.Collections.ArrayList palabras;

            for (int i = inicio; i < tam; i++)
            {
                cadena += keywords[i].ToString() + " ";
                cadenainversa += keywords[--auxtam].ToString() + " ";
            }

            if (cadena.Length > 0)
            {
                cadena = cadena.Substring(0, cadena.Length - 1);
                cadenainversa = cadenainversa.Substring(0, cadenainversa.Length - 1);
                auxcadena = cadena;
                //obtiene el concepto tal cual como esta en la ontologia                
                cadena = originalConcept(cadena, cadenainversa);
                if (!cadena.Equals(""))
                {
                    //palabras:almacena cada concepto dividido de la cadena
                    palabras = new System.Collections.ArrayList();
                    token = cadena.Split('_');
                    palabras.AddRange(token);
                    //funcion validatePart:elimina articulos o stopWord(and,the..etc) de la cadena 
                    //this._listConcepIndividual:conforma cada concepto individual de un concepto compuesto
                    palabras = this.validatePart(palabras);
                    foreach (string concepto in palabras)
                    {
                        if (!this.ListConceptoSimple.Contains(concepto) && !this._listConcepIndividual.Contains(concepto))
                            this._listConcepIndividual.Add(concepto);
                    }
                }
                else
                {
                    cadena = auxcadena;
                }

            }
            return cadena;
        }
        /// <summary>
        /// Metodo Auxiliar que elimina stopWord de un concepto determinado de la ontologia
        /// </summary>
        public System.Collections.ArrayList validatePart(System.Collections.ArrayList arrayPart)
        {
            System.Collections.ArrayList partInvalid = new System.Collections.ArrayList();
            partInvalid.Add("by");
            partInvalid.Add("the");
            partInvalid.Add("of");
            partInvalid.Add("for");
            partInvalid.Add("with");
            partInvalid.Add("a");
            partInvalid.Add("is");
            partInvalid.Add("0is");
            partInvalid.Add("than");
            partInvalid.Add("other");
            partInvalid.Add("and");
            partInvalid.Add("or");
            partInvalid.Add("--");

            string[] token;
            int pos;
            for (int i = 0; i < arrayPart.Count; i++)
            {
                string parte = arrayPart[i].ToString();
                if (parte.Contains("-") && !parte.Contains("--"))
                {
                    parte = parte.Replace("-", " ");
                    token = parte.Split(' ');
                    arrayPart.RemoveAt(i);
                    arrayPart.InsertRange(i, token);
                    i--;
                }

                if (partInvalid.Contains(parte))
                {
                    pos = arrayPart.IndexOf(parte);
                    arrayPart.RemoveAt(pos);
                    i--;
                }
            }
            return arrayPart;
        }
        /// <summary>
        /// Metodo Auxiliar que busca el concept o invertConcept en la BD, de encontrarse
        /// retorna el nombre original del concepto tal cual como esta almacenado en la ontologia de dominio
        /// </summary>
        public string originalConcept(string concept, string invertConcept)
        {
            string nombreOriginalconcepto = "";

            OntologyConceptCopy myOntologyConceptCopy = new OntologyConceptCopy();
            myOntologyConceptCopy = OntologyConceptCopyManager.GetItem(concept, invertConcept);
            if (myOntologyConceptCopy != null)
            {
                //int Ont_id = myOntologyConceptCopy.OntId;
                //OntologyConceptCopy myOntologyConcept = new OntologyConceptCopy();
                //myOntologyConcept = OntologyConceptCopyManager.GetItem(Ont_id);
                //nombreOriginalconcepto = myOntologyConcept.OntCopyNameConcept;
                return myOntologyConceptCopy.OntCopyNameConcept;
            }
            return nombreOriginalconcepto;
        }

        //Método para almacenar el la BD los conceptos de la ontología
        public int AlamcenarConceptos(List<OntologyConceptCopy> conceptos)
        {
            try
            {
                int indice = 0;
                //Eliminar los conceptos Existentes
                OntologyConceptCopyManager.BorrarConceptos();

                foreach (OntologyConceptCopy concepto in conceptos)
                {
                    indice++;
                    //OntologyConcept Oc = new OntologyConcept();
                    OntologyConceptCopy Occopy = new OntologyConceptCopy();
                    //Oc.OntId = indice;
                    //Oc.OntNameConcept = concepto.OntCopyNameConcept;
                    Occopy.OntCopyId = 1;//Por ahora solo utilizamos una sola ontologia
                    Occopy.OntCopyNameConcept = concepto.OntCopyNameConcept;
                    Occopy.OntId = indice;
                    Occopy.Urlconcepto = concepto.Urlconcepto;
                    //OntologyConceptManager.Save(Oc);
                    OntologyConceptCopyManager.Save(Occopy);
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Metodo que retorna una lista de nombres de conceptos (clases) e individuos de la ontología
        /// </summary>       
        public static List<OntologyConceptCopy> ObtenerConceptosOntologia(string ontologia)
        {
            return OntologyConceptManager.ObtenerConceptosOntologia(ontologia);
        }

        //Método que retorna los conceptos hijos de un concepto
        public List<OntologyConceptCopy> ObtenerConceptosHijos(OntModel model, string concepto, string tipoAnalizador,string ontologia)
        {
            List<OntologyConceptCopy> listaClases = new List<OntologyConceptCopy>();
            List<string> noduplicados = new List<string>();

            string strlenguaje = string.Empty;
            //Dependiendo del analizador usado se buscan etiquetas en el idioma correspondiente
            if (tipoAnalizador == "Español")
            {
                strlenguaje = "es";
            }
            else if(tipoAnalizador == "Ingles")
            {
                strlenguaje = "en";
            }
            else
                strlenguaje = null;

            if (!string.IsNullOrEmpty(concepto))
            {
                OntClass oclass = ObtenerClaseCompletadeNombre(concepto, ontologia);

                //Verificamos que el concepto si se haya encontrado en la ontologia
                if (oclass != null)
                {
                    //OBTENEMOS LOS METADATOS DE LA MISMA CLASE
                    //Obtenemos las etiquetas de la clase actual
                    ObtenerListaLabel(ref listaClases, ref noduplicados, oclass, strlenguaje);
                    //Obtenemos las etiquetas de las instancias si las tiene
                    ObtenerListaInstancias(ref listaClases, ref noduplicados, oclass, strlenguaje);

                    //OBTENEMOS LOS METADATOS DE LAS CLASES EQUIVALENTES 
                    //Si tiene clase equivalentes, busca en las mismas
   
                    //if (oclass.hasEquivalentClass(null))
                    //{
                        for (java.util.Iterator i = oclass.listEquivalentClasses(); i.hasNext(); )
                        {
                            OntClass oclasstmp = (OntClass)i.next();
                            ObtenerListaLabel(ref listaClases, ref noduplicados, oclasstmp, strlenguaje);
                            ObtenerListaInstancias(ref listaClases, ref noduplicados, oclasstmp, strlenguaje);
                        }
                    //}

                    //OBTENEMOS LOS METADATOS DE LAS CLASES HIJAS
                    //Si tienen Sub clases busca los conceptos de las mismas
                    if (oclass.hasSubClass())
                        for (java.util.Iterator i = oclass.listSubClasses(); i.hasNext(); )
                        {
                            //se la convierte a OntClass para su analisis
                            OntClass cls = (OntClass)i.next();
                            ObtenerListaLabel(ref listaClases, ref noduplicados, cls, strlenguaje);

                            //Se añaden las instancias de la clase hija
                            ObtenerListaInstancias(ref listaClases, ref noduplicados, cls, strlenguaje);
                        }
                    //OBTENEMOS LOS METADATOS DE LAS RESTRICCIONES DIRECTAS DE CLASE
                    //Las restricciones en Jena se buscan en SuperCalsses sin embargo en OWL es un RDF:Subclass retriccion
                    if (oclass.hasSuperClass())
                    {
                        for (java.util.Iterator i = oclass.listSuperClasses(); i.hasNext(); )
                        {
                            //se la convierte a OntClass para su analisis
                            OntClass sta = (OntClass)i.next();
                            if (sta.isRestriction())
                            {
                                //Se obtiene la clase dominio dependiendo del tipo de restricción
                                OntClass clsres = null;
                                Restriction sup = sta.asRestriction();
                                if (sup.isAllValuesFromRestriction())
                                {
                                    clsres = ObtenerClaseCompletadeURI(sup.asAllValuesFromRestriction().getAllValuesFrom().ToString(), ontologia);

                                }
                                else if (sup.isSomeValuesFromRestriction())
                                {
                                    clsres = ObtenerClaseCompletadeURI(sup.asSomeValuesFromRestriction().getSomeValuesFrom().ToString(), ontologia);
                                }
                                else if (sup.isHasValueRestriction())
                                {
                                    clsres = ObtenerClaseCompletadeURI(sup.asHasValueRestriction().getHasValue().ToString(), ontologia);
                                }
                                //Las restricciones de cardinalidad no se miran ya que se verificaron las clases por la propiedad. Asi si clsres es null se pasan por alto
                                if (clsres != null)
                                {
                                    //Buscamos Etiquetas e Hijos de las clases de restricción
                                    ObtenerListaLabel(ref listaClases, ref noduplicados, clsres, strlenguaje);
                                    ObtenerListaInstancias(ref listaClases, ref noduplicados, clsres, strlenguaje);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return listaClases;
            }
            return listaClases;
        }

        //Este método devuelve la lista de Anotaciones dada la clase y las almacena en los vectores dados
        private void ObtenerListaLabel(ref List<OntologyConceptCopy> listaclases, ref List<string> noduplicados, OntClass cls, string lenguaje)
        {
            //Añadimos el nombre de las anotaciones Label que tenga la clase
            com.hp.hpl.jena.util.iterator.ExtendedIterator label = cls.listLabels(null);
            //com.hp.hpl.jena.util.iterator.ExtendedIterator label = cls.listProperties(RDFS.label);
            while (label.hasNext())
            {
                //Statement thisLabels = (Statement)label.next();
                //Obtenemos el primer label
                RDFNode thisLabel = (RDFNode)label.next();

                //Variable temporal en el que se almacena el concepto actual
                OntologyConceptCopy Oconcept = new OntologyConceptCopy();

                Oconcept.Urlconcepto = cls.getURI().ToString();

                if (thisLabel.isLiteral())
                {
                    Literal lit = (Literal)thisLabel;
                    if (lit.getLanguage() == lenguaje)
                    {
                        Oconcept.OntCopyNameConcept = lit.getValue().ToString();
                        //Verificar no colocar conceptos duplicados
                        if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                        {
                            listaclases.Add(Oconcept);
                            noduplicados.Add(Oconcept.OntCopyNameConcept);
                        }
                    }
                    else if (string.IsNullOrEmpty(lenguaje))
                    {
                        Oconcept.OntCopyNameConcept = lit.getValue().ToString();
                        //Verificar no colocar conceptos duplicados
                        if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                        {
                            listaclases.Add(Oconcept);
                            noduplicados.Add(Oconcept.OntCopyNameConcept);
                        }
                    }
                }
                else
                {
                    Oconcept.OntCopyNameConcept = thisLabel.ToString();
                    //Verificar no colocar conceptos duplicados
                    if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                    {
                        listaclases.Add(Oconcept);
                        noduplicados.Add(Oconcept.OntCopyNameConcept);
                    }
                }

            }
        }


        //Este metodo obtiene las instancias de la clase dada y las coloca en los vectores de entrada
        private void ObtenerListaInstancias(ref List<OntologyConceptCopy> listaclases, ref List<string> noduplicados, OntClass cls, string lenguaje)
        {
            for (java.util.Iterator j = cls.listInstances(true); j.hasNext(); )
            {
                Individual ind = (Individual)j.next();
                if (ind.isIndividual())
                {
                    //Añadimos el nombre de las anotaciones Label que tenga el individuo
                    com.hp.hpl.jena.util.iterator.ExtendedIterator labelind = ind.listLabels(null);
                    while (labelind.hasNext())
                    {
                        //Variable temporal en el que se almacena el concepto actual
                        OntologyConceptCopy Oconcept = new OntologyConceptCopy();
                        Oconcept.Urlconcepto = cls.getURI().ToString();
                        RDFNode thisLabel = (RDFNode)labelind.next();
                        if (thisLabel.isLiteral())
                        {
                            Literal lit = (Literal)thisLabel;
                            if (lit.getLanguage() == lenguaje)
                            {
                                Oconcept.OntCopyNameConcept = lit.getValue().ToString();
                                //Verificar no colocar conceptos duplicados
                                if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                                {
                                    listaclases.Add(Oconcept);
                                    noduplicados.Add(Oconcept.OntCopyNameConcept);
                                }
                            }
                            else if (string.IsNullOrEmpty(lenguaje))
                            {
                                Oconcept.OntCopyNameConcept = lit.getValue().ToString();
                                //Verificar no colocar conceptos duplicados
                                if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                                {
                                    listaclases.Add(Oconcept);
                                    noduplicados.Add(Oconcept.OntCopyNameConcept);
                                }
                            }
                        }
                        else
                        {
                            Oconcept.OntCopyNameConcept = thisLabel.toString();
                            //Verificar no colocar conceptos duplicados
                            if (!noduplicados.Contains(Oconcept.OntCopyNameConcept))
                            {
                                listaclases.Add(Oconcept);
                                noduplicados.Add(Oconcept.OntCopyNameConcept);
                            }
                        }

                    }
                }

            }
        }

        //Obtiene La clase dado el concepto. Sino lo encuentra devuelve concepto nulo
        protected OntClass ObtenerClaseCompletadeNombre(string concepto, string ontologia)
        {
            string rutaDirActual = System.AppDomain.CurrentDomain.BaseDirectory;
            string rutaRelOwl = @ontologia;
            string rutaOwl = System.IO.Path.Combine(rutaDirActual, rutaRelOwl);
            //string clases = "";

            rutaOwl = "file:///" + rutaOwl;
            OntModel m;
            m = ModelFactory.createOntologyModel(OntModelSpec.OWL_MEM);
            m.read(rutaOwl, "RDF/XML");

            OntologyConceptCopy occtemp = null;
            occtemp = OntologyConceptManager.GetItemByConceptName(concepto);
            if (occtemp != null)
            {
                string urltemp = occtemp.Urlconcepto;
                //se recorren todas clases nombradas hasta encontrar la clase de la cual se obtendrán las etiquetas
                //Toco asi porque obteniendo la clase directamente no encontraba todas las etiquetas
                for (java.util.Iterator i = m.listNamedClasses(); i.hasNext(); )
                {
                    OntClass oclasstmp = (OntClass)i.next();
                    if (oclasstmp.getURI() == urltemp)
                    {
                        return oclasstmp;
                    }
                }
            }
            return null;
        }

        //Obtiene la clase que corresponde con el uri exacto, de lo contrario devuelve nulo
        protected OntClass ObtenerClaseCompletadeURI(string URIstr, string ontologia)
        {
            string rutaDirActual = System.AppDomain.CurrentDomain.BaseDirectory;
            string rutaRelOwl = @ontologia;
            string rutaOwl = System.IO.Path.Combine(rutaDirActual, rutaRelOwl);
            //string clases = "";

            rutaOwl = "file:///" + rutaOwl;
            OntModel m;
            m = ModelFactory.createOntologyModel(OntModelSpec.OWL_MEM);
            m.read(rutaOwl, "RDF/XML");

            OntologyConceptCopy occtemp = null;
            occtemp = OntologyConceptManager.GetItemUniqueConceptInUrl(URIstr);
            if (occtemp != null)
            {
                string urltemp = occtemp.Urlconcepto;
                //se recorren todas clases nombradas hasta encontrar la clase de la cual se obtendrán las etiquetas
                //Toco asi porque obteniendo la clase directamente no encontraba todas las etiquetas
                for (java.util.Iterator i = m.listNamedClasses(); i.hasNext(); )
                {
                    OntClass oclasstmp = (OntClass)i.next();
                    if (oclasstmp.getURI() == urltemp)
                    {
                        return oclasstmp;
                    }
                }
            }
            return null;
        }
        
        #endregion
    #endregion
    }
}
