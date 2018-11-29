using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using Google.API.Search;
using Yahoo.API;
using System.Data;
using System.Collections.Generic;

namespace ModeloSemantico_PU.ProcesamientoConsulta
{
    public class WebSearchHandler
    {
        #region Métodos
        #region Google Web Search
        /// <summary>
        /// Obtiene una colección de resultados Web de acuerdo a una consulta dada, (API utilizada: Google API Web Search 2.0)
        /// </summary>
        /// <param name="keywords">Palabras clave de la consulta de usuario</param>
        /// <param name="numResults">Numero de resultados que se van a obtener</param>
        /// <returns></returns>
        public System.Collections.ArrayList webSearchGoogle(string keywords, int numResults)
        {
            System.Collections.ArrayList results = new System.Collections.ArrayList();
            //Crear un cliente Google
            GwebSearchClient client = new GwebSearchClient("http://www.google.com.co");
            //Obtener lista de resultados
            System.Collections.Generic.IList<IWebResult> resultsGoogle = client.Search(keywords, numResults);
            foreach (IWebResult result in resultsGoogle)
            {
                WebDocument wd = new WebDocument();
                wd.Titulo = result.Title;
                wd.Resumen = result.Content;
                wd.Url = result.Url;
                results.Add(wd);                
            }
            return results;
        }
        #endregion
        #region Yahoo Web Search
        /// <summary>
        /// Obtiene una colección de resultados Web de acuerdo a una consulta dada, (API utilizada: Yahoo Web Search API)
        /// </summary>
        /// <param name="keywords">Palabras clave de la consulta de usuario</param>
        /// <param name="numResults">Numero de resultados que se van a obtener</param>
        /// <returns></returns>
        public System.Collections.ArrayList webSearchYahoo(string keywords, int numResults)
        {
            System.Collections.ArrayList results = new System.Collections.ArrayList();
            YahooSearchService yahoo = new YahooSearchService();
            Yahoo.API.WebSearchResponse.ResultSet resultsYahoo = yahoo.WebSearch("YahooExample", keywords, "any", 10, 1, "all", false, true, "en");
            foreach (Yahoo.API.WebSearchResponse.ResultType result in resultsYahoo.Result)
            {
                WebDocument wd = new WebDocument();
                wd.Titulo = result.Title;
                wd.Resumen = result.Summary;
                wd.Url = result.Url;
                results.Add(wd);                
            }
            return results;
        }
        #endregion
        #region Busqueda Web General y Ordenamiento de resultados
        /// <summary>
        /// Obtiene una colección de resultados Web de acuerdo a una consulta dada haciendo uso de los servicios de busqueda de Google y Yahoo
        /// </summary>
        /// <param name="keywords">Palabras clave de la consulta de usuario</param>
        /// <param name="numResults">Numero de resultados que se van a obtener</param>
        /// <returns></returns>
        public System.Collections.ArrayList webSearch(string keywords, int numResults)
        {            
            //System.Collections.ArrayList resultsGoogle = webSearchGoogle(keywords, numResults);
            //System.Collections.ArrayList resultsYahoo = webSearchYahoo(keywords, numResults);
            //System.Collections.ArrayList results = ranking(resultsGoogle, resultsYahoo);
            System.Collections.ArrayList results = new System.Collections.ArrayList();
            WebDocument wd = new WebDocument();
            wd.Titulo = "pagina1"; wd.Resumen = "resumen1"; wd.Url = "url1";
            results.Add(wd);
            return results;            
        }
        #endregion
        #region Ranking y Eliminacion de Documentos Duplicados
        /// <summary>
        /// Realiza un ranking de documentos de Google y Yahoo
        /// </summary>
        /// <param name="resultsGoogle">Listado de documentos de Google</param>
        /// <param name="resultsYahoo">Listado de documentos de Yahoo</param>
        /// <returns></returns>
        public System.Collections.ArrayList ranking(System.Collections.ArrayList resultsGoogle, System.Collections.ArrayList resultsYahoo)
        {
            System.Collections.ArrayList rankedDocs = new System.Collections.ArrayList();
            for (int i = 0; i < resultsGoogle.Count; i++)
            {
                WebDocument wdg = (WebDocument)resultsGoogle[i];
                WebDocument wdy = (WebDocument)resultsYahoo[i];
                rankedDocs.Add(wdg);                
                if(i < resultsYahoo.Count)
                {
                    if (esDocumentoDuplicado(wdy, resultsGoogle) == false)
                    {
                        rankedDocs.Add(wdy);
                    }
                }
            } 
            return rankedDocs;
        }
        #endregion
        #region Verificar si el documento esta duplicado
        /// <summary>
        /// Verifica si un documento se encuentra duplicado en las listas de documentos retornadas por los motores de busqueda
        /// </summary>
        /// <param name="doc">Documento Web que se va a verificar</param>
        /// <param name="results">Lista de resultados de un motor de busqueda</param>
        /// <returns></returns>
        public bool esDocumentoDuplicado(WebDocument doc, System.Collections.ArrayList results)
        {
            bool band = false;
            for (int j = 0; j < results.Count; j++)
            {
                WebDocument wdg = (WebDocument)results[j];
                if (doc.Url.Equals(wdg.Url))
                {
                    band = true;
                }
            }
            return band;
        }
        #endregion
        #region Conversion de Resultados
        /// <summary>
        /// Convierte un DataSet de resultados de búsqueda en un ArrayList
        /// </summary>
        /// <param name="ds">DataSet de resultados de búsqueda</param>
        /// <returns>Resultados en formato de ArrayList</returns>
        public System.Collections.ArrayList convertDataSetToArrayList(DataSet ds)
        {
            System.Collections.ArrayList results = new System.Collections.ArrayList();
            DataTable dt = ds.Tables[0];
            List<string> grupos = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                WebDocument wd = new WebDocument();
                wd.Titulo = row[0].ToString();
                wd.Resumen = row[1].ToString();
                wd.Url = row[2].ToString();
                results.Add(wd);
            }
            return results;
        }
        #endregion
        #endregion
    }
}
