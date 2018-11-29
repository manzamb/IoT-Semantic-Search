using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ModeloSemantico_PU.ProcesamientoConsulta
{
    public class WruMeasure
    {
        #region Métodos
        #region Cálculo del Wru
        /// <summary>
        /// Obtiene los valores Wrud de cada término de la consulta de usuario
        /// </summary>
        /// <param name="docsCalificados">Colección de documentos que han sido calificados por el usuario a manera de realimentación</param>
        /// <param name="queryTerms">Colección con los términos de la consulta de ususario</param>
        /// <returns>Colección con los valores Wrud de cada concepto</returns>
        public System.Collections.Generic.Dictionary<string, double> calculateWru(System.Collections.Generic.Dictionary<string, int> docsCalificados, System.Collections.ArrayList queryTerms)
        {
            //Se crea un objeto DocumentProcessor para obtener frecuencias de los términos
            DocumentProcessor dp = new DocumentProcessor();
            //Obtener la Matriz de Frecuencias (Frecuencia de cada término en cada documento)
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, int>> freqMatrix = dp.getFreqMatrix(docsCalificados, queryTerms);
            //Obtener la Frecuencia Total (frecuencia de cada término en todos los documentos)
            System.Collections.Generic.Dictionary<string, int> freqTotal = dp.getTotalFrequency(freqMatrix, queryTerms.Count);
            //Obtener los valores TF-IDF (Peso de cada término en cada documento)
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, double>> conceptsTFIDF = getTF_IDF(freqTotal, freqMatrix);
            //Obtener los valores Wrud (Peso de cada concepto en los documentos relevantes para el usuario)
            System.Collections.Generic.Dictionary<string, double> conceptsWru = getWru(docsCalificados, conceptsTFIDF, queryTerms.Count);
            return conceptsWru;
        }

        /// <summary>
        /// Obtiene los valores Wrud de cada concepto perteneciente a la consulta de usuario
        /// </summary>
        /// <param name="docsCalificados">Colección de documentos que han sido calificados por el usuario a manera de realimentación</param>
        /// <param name="conceptsTFIDF">Colección que almacena los valores TF-IDF de cada concepto en cada documento</param>
        /// <param name="tam">Número de términos de la consulta de usuario</param>
        /// <returns></returns>
        public System.Collections.Generic.Dictionary<string, double> getWru(System.Collections.Generic.Dictionary<string, int> docsCalificados, System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, double>> conceptsTFIDF, int tam)
        {
            int i = 0;
            string urlKey = "";
            System.Collections.Generic.Dictionary<string, double> conceptsWru = new System.Collections.Generic.Dictionary<string, double>();
            //inicializar Wru
            double[] auxWru = new double[tam];
            for (int j = 0; j < tam; j++)
            {
                auxWru[j] = 0.0;
            }
            //Se recorre conceptsTFIDF por cada documento
            foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.Dictionary<string, double>> kvp in conceptsTFIDF)
            {
                i = 0;
                urlKey = kvp.Key;
                System.Collections.Generic.Dictionary<string, double> concTFIDF = kvp.Value;
                //Se recorre concTFIDF por cada término
                foreach (System.Collections.Generic.KeyValuePair<string, double> kvp2 in concTFIDF)
                {
                    //Sumatoria de cada TF-IDF multiplicado por la calificacion del usuario al respectivo documento
                    auxWru[i] = auxWru[i] + (kvp2.Value * Convert.ToDouble(docsCalificados[kvp.Key]));
                    i++;
                }
            }

            i = 0;
            System.Collections.Generic.Dictionary<string, double> aux = conceptsTFIDF[urlKey];
            foreach (System.Collections.Generic.KeyValuePair<string, double> kvp in aux)
            {
                conceptsWru.Add(kvp.Key, auxWru[i]);
                i++;
            }

            return conceptsWru;
        }
        #endregion
        #region Cálculo del TF-IDF
        /// <summary>
        /// Cálculo de los valores TF-IDF del modelo de espacio vectorial clásico en la RI
        /// </summary>
        /// <param name="freqTotal">Frecuencia de cada término en todos los documentos</param>
        /// <param name="freqMatrix">Frecuencia de cada término en cada documento</param>
        /// <returns></returns>
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, double>> getTF_IDF(System.Collections.Generic.Dictionary<string, int> freqTotal, System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, int>> freqMatrix)
        {
            double tf = 0;
            double idf = 0;
            double tf_idf = 0;

            System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, double>> conceptsTFIDF = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, double>>();
            //Se recorre freqMatrix por cada documento
            foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.Dictionary<string, int>> kvp in freqMatrix)
            {
                System.Collections.Generic.Dictionary<string, double> concTFIDF = new System.Collections.Generic.Dictionary<string, double>();
                //Se obtiene la frecuencia de cada término en el documento
                System.Collections.Generic.Dictionary<string, int> freqPorDocumento = kvp.Value;
                //se recorre freqTotal por cada término
                foreach (System.Collections.Generic.KeyValuePair<string, int> kvp2 in freqTotal)
                {
                    //frecuencia del término en un documento
                    tf = Convert.ToDouble(freqPorDocumento[kvp2.Key]);
                    //frecuencia inversa (numero de docs sobre la frecuencia del termino en el total de documentos)
                    idf = Convert.ToDouble(freqMatrix.Count) / Convert.ToDouble(kvp2.Value);       //tener en cuenta si kvp2.Value = 0
                    //Calcular valor TF-IDF
                    tf_idf = tf * idf;
                    //Se adiciona a concTFIDF el concepto y su respectivo valor TF-IDF para el documento
                    concTFIDF.Add(kvp2.Key, tf_idf);
                }
                //Se adiciona a conceptsTFIDF el documento y sus respectivos conceptos con los TF-IDF calculados
                conceptsTFIDF.Add(kvp.Key, concTFIDF);
            }
            return conceptsTFIDF;
        }
        #endregion
        #endregion
    }
}
