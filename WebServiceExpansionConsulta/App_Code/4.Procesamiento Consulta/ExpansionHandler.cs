using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections;
using com.hp.hpl.jena.ontology;
using System.Collections.Generic;
using ModeloSemantico_PU.ObjetosNegocio;

namespace ModeloSemantico_PU.ProcesamientoConsulta
{
    public class ExpansionHandler
    {
        public List<OntologyConceptCopy> Conceptos;

        #region Metodos
        /// <summary>
        /// Expande una consulta de usuario mediante MSEC (Modelo Semántico de Expansión de Consulta)
        /// </summary>
        /// <param name="consulta">Cadena de consulta digitada por el usuario</param>
        /// <param name="ontModel">Ontologia de dominio</param>
        /// <returns>Cadena de consulta expandida</returns>
        public string expandirConsulta(string consulta, OntModel ontModel, string tipoAnalizador, string strontologia)
        {
            //conversion a minusculas de la cadena de consulta
            consulta = consulta.ToLower();

            //Se crea un objeto de tipo AnalizadorLexico para eliminar los stopwords de la consulta de usuario, Libreria utilizada:Lucenet.Net
            AnalizadorLexico analizador = new AnalizadorLexico();

            //Obtencion de palabras clave de la consulta
            System.Collections.ArrayList ListKeyWords = new System.Collections.ArrayList();
            ListKeyWords = analizador.getKeywords(consulta, tipoAnalizador);

            //Cargar la Ontologia
            OntologiaDominio ontologia = new OntologiaDominio();
            ontologia.Model = ontModel;

            //Se almacenan los conceptos e individuos de la ontologia en la BD para su posterior uso
            //ontologia.AlamcenarConceptos(Conceptos);

            //identificacion de Conceptos
            ontologia.termsIdentification(ListKeyWords);
            System.Collections.ArrayList lcc = ontologia.ListConceptoCompuesto;
            System.Collections.ArrayList lcs = ontologia.ListConceptoSimple;
            lcc.AddRange(lcs);  //Union de conceptos simples y compuestos
            System.Collections.ArrayList lcr = ontologia.ListRestrictionConceps; //conceptos de restricciones
            //Se extraen conceptos de similitud entre la combinatoria de pares de concepto de lcc
            //System.Collections.ArrayList lce = ontologia.similarityBetweenConcepts(lcc);

            //Añadimos los conceptos hijos e instancias para hacer la consulta más precisa
            
            string consultaExpandida = ontologia.CadenaExpandConsult;
            foreach (string concepto in lcc)
            {
                List<OntologyConceptCopy> hijos = new List<OntologyConceptCopy>();
                hijos = ontologia.ObtenerConceptosHijos(ontModel, concepto, tipoAnalizador, strontologia);
                foreach(OntologyConceptCopy oncp in hijos)
                    if (!consultaExpandida.Contains(oncp.OntCopyNameConcept)) 
                        consultaExpandida += " + " + oncp.OntCopyNameConcept;
            }

            //string consultaExpandida = ontologia.CadenaExpandConsult;
            int pos = consultaExpandida.IndexOf("+");
            consultaExpandida = consultaExpandida.Substring(pos + 1);

            //foreach (string similitudConcept in lce)
            //{
            //    if (similitudConcept != "" && !lcc.Contains(similitudConcept) && !lcr.Contains(similitudConcept))
            //        consultaExpandida += " + " + similitudConcept;
            //}

            string auxConsultaExpandida = consultaExpandida;
            auxConsultaExpandida = auxConsultaExpandida.Replace("|", " ");
            auxConsultaExpandida = auxConsultaExpandida.Replace(")", " ");
            auxConsultaExpandida = auxConsultaExpandida.Replace("(", " ");
            auxConsultaExpandida = auxConsultaExpandida.Replace("+", " ");
            auxConsultaExpandida = auxConsultaExpandida.Replace("     ", ",");
            auxConsultaExpandida = auxConsultaExpandida.Replace("    ", ",");
            auxConsultaExpandida = auxConsultaExpandida.Replace("   ", ",");
            auxConsultaExpandida = auxConsultaExpandida.Replace("  ", ",");
            auxConsultaExpandida = auxConsultaExpandida.Replace(" ", ",");

            string[] token = auxConsultaExpandida.Split(',');
            ArrayList listKeywordsConsExp = new ArrayList();
            listKeywordsConsExp.AddRange(token);
            listKeywordsConsExp.RemoveAt(0);

            consultaExpandida = consultaExpandida.Replace("_", " ");
            consultaExpandida += ontologia.ConceptSynonym;

            //Finalmente le añadimos los términos que no se encontraron en la ontología
            //Primero se hace una copia en minusculas para comparar con los términos originales en minusculas y así no repetir concepto
            System.Collections.ArrayList consultaExpandidaTemp = new System.Collections.ArrayList();
            consultaExpandidaTemp.AddRange(listKeywordsConsExp);
            int i = 0;
            foreach (string terminoexp in listKeywordsConsExp)
            {
                consultaExpandidaTemp[i] = terminoexp.ToLower();
                i++;
            }
            foreach (string termino in ListKeyWords)
            {
                if (!consultaExpandidaTemp.Contains(termino.ToLower()))
                    consultaExpandida += " &" + termino;
            }

            //Retornar la cadena de consulta expandida
            return consultaExpandida;
        }

        public List<string> RetornarConceptosOntologia(string concepto, OntModel ontModel, string tipoAnalizador, string strontologia)
        {
            //Lista de conceptos a Retornar
            List<string> ListaConceptos = new List<string>();

            //conversion a minusculas de la cadena de consulta
            concepto = concepto.ToLower();

            ////Se crea un objeto de tipo AnalizadorLexico para eliminar los stopwords de la consulta de usuario, Libreria utilizada:Lucenet.Net
            //AnalizadorLexico analizador = new AnalizadorLexico();

            ////Obtencion de palabras clave de la consulta
            //System.Collections.ArrayList ListKeyWords = new System.Collections.ArrayList();
            //ListKeyWords = analizador.getKeywords(consulta, tipoAnalizador);

            //Cargar la Ontologia
            OntologiaDominio ontologia = new OntologiaDominio();
            ontologia.Model = ontModel;

            ////identificacion de Conceptos
            //ontologia.termsIdentification(ListKeyWords);
            //System.Collections.ArrayList lcc = ontologia.ListConceptoCompuesto;
            //System.Collections.ArrayList lcs = ontologia.ListConceptoSimple;
            //lcc.AddRange(lcs);  //Union de conceptos simples y compuestos
            //System.Collections.ArrayList lcr = ontologia.ListRestrictionConceps; //conceptos de restricciones

            ////System.Collections.ArrayList lcc = ontologia.ListConceptoCompuesto;
            ////lcc.AddRange(ListKeyWords);

            ////Añadimos los conceptos hijos e instancias para hacer la consulta más precisa
            //string consultaExpandida = ontologia.CadenaExpandConsult;
            //foreach (string concepto in lcc)
            //{
                List<OntologyConceptCopy> hijos = new List<OntologyConceptCopy>();
                hijos = ontologia.ObtenerConceptosHijos(ontModel, concepto, tipoAnalizador, strontologia);
                foreach (OntologyConceptCopy oncp in hijos)
                    if (!ListaConceptos.Contains(oncp.OntCopyNameConcept))
                        ListaConceptos.Add(oncp.OntCopyNameConcept);
            //}

            //Retornar la cadena de consulta expandida
            return ListaConceptos;
        }

        #endregion
    }
}
