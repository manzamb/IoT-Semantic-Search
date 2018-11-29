using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Collections;

namespace ModeloSemantico_PU.ProcesamientoConsulta
{
    /// <summary>
    /// La clase DocumentProcessor permite procesar el contenido de un documento dependiendo de las keywords de la consulta de usuario
    /// </summary>
    public class DocumentProcessor
    {
        #region Métodos
        #region Obtener Contenido de un documento Web
        /// <summary>
        /// Obtiene el contenido de un documento Web
        /// </summary>
        /// <param name="url">Dirección URL del documento al cual se pretende extraer su contenido</param>
        /// <returns>content = Cadena de texto que representa el contenido de un documento Web</returns>
        public string getDocumentContent(string url)
        {
            HTMLHandler htmlObject = new HTMLHandler();
            string content = "";
            //Si la URL es válida en su formato
            if (htmlObject.isValidUrl(ref url))
            {
                //Se obtiene todo el contenido de un documento Web incluyendo etiquetas HTML
                string textHTML = getDocumentHTML(url);

                //Se eliminan las etiquetas HTML
                MatchCollection m;
                Regex r = new Regex("<[^>]+>");
                m = r.Matches(textHTML);
                for (int i = 0; i < m.Count - 1; i++)
                {
                    int posi = m[i].Index + m[i].Length; // pos inicial
                    int posf = m[i + 1].Index; // pos final
                    content += textHTML.Substring(posi, posf - posi);
                }
            }
            //Se retorna en minúsculas el contenido de un documento Web además sin incluir etiquetas HTML
            return content.ToLower();
        }

        /// <summary>
        /// Obtiene el contenido de un documento Web incluyendo etiquetas HTML
        /// </summary>
        /// <param name="url">Dirección URL del documento al cual se pretende extraer su contenido incluyendo etiquetas HTML</param>
        /// <returns>reader = Cadena de texto que representa el contenido del documento Web incluyendo etiquetas HTML</returns>
        public string getDocumentHTML(string url)
        {
            //Intercambio de datos con un recurso Web identificado por un URI
            WebClient client = new WebClient();
            //Se obtiene una secuencia de bytes desde el recurso Web                
            Stream stream = client.OpenRead(url);
            //Se leen los caracteres de la secuencia de bytes
            StreamReader reader = new StreamReader(stream);
            //Mediante el método ReadToEnd() se retorna el contenido del StreamReader en formato string
            return reader.ReadToEnd();
        }
        #endregion
        #region Frecuencia de cada concepto en un solo documento
        /// <summary>
        /// Obtiene la frecuencia de aparición de cada término perteneciente a una colección en el contenido de un documento
        /// </summary>
        /// <param name="texto">Contenido del documento</param>
        /// <param name="queryTerms">Colección de términos a los que se pretende calcular su frecuencia de aparición</param>
        /// <returns>Colección con los pares término-frecuencia</returns>
        public System.Collections.Generic.Dictionary<string, int> getTermsFrequency(string texto, System.Collections.ArrayList queryTerms)
        {
            System.Collections.ArrayList auxQueryTerms = new System.Collections.ArrayList();
            foreach (string term in queryTerms)
            {
                auxQueryTerms.Add(term.Replace("_", " ").ToLower());
            }

            int pos = -1;
            int pos1 = 0;
            int pos2 = 0;
            int pos3 = 0;
            int freq = 0;
            int i = 0;

            System.Collections.Generic.Dictionary<string, int> freqVector = new System.Collections.Generic.Dictionary<string, int>();

            if (auxQueryTerms.Count > 0)
            {
                //al texto original se le concatena un espacio al principio y al final del mismo
                texto = " " + texto + " ";

                //Se busca cada término del ArrayList en el contenido del documento
                foreach (string term in auxQueryTerms)
                {
                    do
                    {
                        //Posibles coincidencias válidas
                        pos1 = texto.IndexOf(" " + term + " ", pos + 1);
                        pos2 = texto.IndexOf(" " + term + ",", pos + 1);
                        pos3 = texto.IndexOf(" " + term + ".", pos + 1);

                        //Se obtiene la coincidencia más cercana a partir de la posición actual de búsqueda
                        pos = minimo(pos1, pos2, pos3);

                        //Por cada coincidencia se incrementa la frecuencia
                        if (pos != -1)
                            freq++;
                    }
                    while (pos != -1);
                    //Se adiciona el par término-frecuencia
                    freqVector.Add(queryTerms[i].ToString(), freq);
                    freq = 0;
                    pos = -1;
                    i++;
                }
            }
            return freqVector;
        }

        /// <summary>
        /// Obtiene el mínimo entre tres numeros, siempre y cuando sea mayor que -1
        /// </summary>
        /// <param name="n1">Número 1</param>
        /// <param name="n2">Número 2</param>
        /// <param name="n3">Número 3</param>
        /// <returns>Menor número que sea mayor que -1</returns>
        public int minimo(int n1, int n2, int n3)
        {
            System.Collections.ArrayList numeros = new System.Collections.ArrayList();
            if (n1 > -1) numeros.Add(n1);
            if (n2 > -1) numeros.Add(n2);
            if (n3 > -1) numeros.Add(n3);

            if (numeros.Count > 0)
            {
                numeros.Sort();
                return (int)numeros[0];
            }
            else
            {
                return -1;
            }
        }
        #endregion
        #region Frecuencia de cada concepto en todos los documentos
        /// <summary>
        /// Obtiene la frecuencia de cada concepto en todos los documentos
        /// </summary>        
        /// <param name="freqTotales">Diccionario con la URL de los documentos y sus respectivas frecuencias de conceptos</param>        
        /// <param name="tam">Número de términos</param>
        /// <returns>Diccionario con cada concepto y su respectiva frecuencia total</returns>
        public System.Collections.Generic.Dictionary<string, int> getTotalFrequency(System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, int>> freqTotales, int tam)
        {
            System.Collections.Generic.Dictionary<string, int> freqTotal = new System.Collections.Generic.Dictionary<string, int>();
            System.Collections.Generic.Dictionary<string, int> freqPorDocumento = new System.Collections.Generic.Dictionary<string, int>();
            int i = 0;

            //inicializar frecuencias totales
            int[] auxFrecTotales = new int[tam];
            for (int j = 0; j < tam; j++)
            {
                auxFrecTotales[j] = 0;
            }

            //calcular frecuencias totales
            //Se recorre el diccionario por documentos
            foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.Dictionary<string, int>> kvp in freqTotales)
            {
                //Por cada documento se obtiene un diccionario con las frecuencias de los conceptos
                freqPorDocumento = kvp.Value;
                i = 0;
                //Se recorre el diccionario por conceptos
                foreach (System.Collections.Generic.KeyValuePair<string, int> kvp2 in freqPorDocumento)
                {
                    //Cada frecuencia de un concepto en cada docuemnto se suma
                    auxFrecTotales[i] = auxFrecTotales[i] + kvp2.Value;
                    i++;
                }
            }

            i = 0;
            //Se adicionan las frecuencias totales al diccionario que se va a retornar
            foreach (System.Collections.Generic.KeyValuePair<string, int> kvp in freqPorDocumento)
            {
                freqTotal.Add(kvp.Key, auxFrecTotales[i]);
                i++;
            }

            return freqTotal;
        }
        #endregion
        #region Frecuencias de cada concepto por cada documento (Matriz de Frecuencias)
        /// <summary>
        /// Obtiene la Matriz de Frecuencias de cada término en cada documento
        /// </summary>
        /// <param name="docsCalificados">Colección de documentos y la respectiva calificación de cada uno de ellos</param>
        /// <param name="queryTerms">Colección con los términos de la consulta de usuario</param>
        /// <returns>Matriz de Frecuencias</returns>
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, int>> getFreqMatrix(System.Collections.Generic.Dictionary<string, int> docsCalificados, System.Collections.ArrayList queryTerms)
        {
            string textContent = "";
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, int>> freqMatrix = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, int>>();
            //Se recorre cada documento calificado
            foreach (System.Collections.Generic.KeyValuePair<string, int> kvp in docsCalificados)
            {
                System.Collections.Generic.Dictionary<string, int> freqPorDocumento = new System.Collections.Generic.Dictionary<string, int>();
                //Obtener el contenido del documento
                textContent = getDocumentContent(kvp.Key);
                //Obtener frecuencias de los términos en el documento
                freqPorDocumento = getTermsFrequency(textContent, queryTerms);
                //Adicionar el documento y sus respectivas frecuencias de los términos
                freqMatrix.Add(kvp.Key, freqPorDocumento);
            }
            return freqMatrix;
        }
        #endregion
        #endregion
    }
}
