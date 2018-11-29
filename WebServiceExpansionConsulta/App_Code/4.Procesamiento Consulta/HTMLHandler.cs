using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Text;
using Google.API.Search;
using System.Collections;
using System.Text.RegularExpressions;

namespace ModeloSemantico_PU.ProcesamientoConsulta
{
    public class HTMLHandler
    {
        #region Métodos
        #region Mostrar resultados de búsqueda
        /// <summary>
        /// Devuelve los resultados de la búsqueda del usuario listos para presentarlos en el explorador Web
        /// </summary>
        /// <param name="results">Resultados de la búsqueda obtenidos mediante el API del buscador Web</param>
        /// <param name="keywords">Términos clave de la consulta de usuario en formato de parámetro URL. <example>breast+cancer+treatment</example></param>
        /// <returns>Cadena de texto con los resultados de búsqueda en formato HTML</returns>
        public string loadSearchResults(System.Collections.ArrayList results)
        {
            StringBuilder sb = new StringBuilder();
            //Se recorre la lista de resultados devueltos por el API del buscador
            foreach (WebDocument result in results)
            {
                //Se aplica el formato adecuado para mostrarlo en el navegador Web
                sb.Append("<a href=\"PageView.aspx?url1=" + result.Url);
                sb.Append("\">" + result.Titulo + "</a><br />");
                sb.Append(result.Resumen + "<br /><br />");
            }
            return sb.ToString();
        }        
        #endregion
        #region Obtener formato de Keywords
        /// <summary>
        /// Obtiene la palabras clave de la consulta de usuario en el formato como parámetro de URL <example>breast+cancer+treatment</example>
        /// o simplemente con un espacio entre cada palabra
        /// </summary>
        /// <param name="listKeywords">Colección con los términos clave de de la búsqueda del usuario</param>
        /// <param name="isForURL">Booleano que indica si el formato de retorno es para el parámetro de la URL o simplemente se necesita con espacios entre términos</param>
        /// <returns>Cadena de texto con los términos clave en el formato especificado</returns>
        public string getQueryFormat(ArrayList listKeywords, bool isForURL)
        {
            string consulta = "";
            foreach (string keyword in listKeywords)
            {
                if (isForURL)
                    consulta += keyword + "+";
                else
                    consulta += keyword + " ";
            }
            consulta = consulta.Substring(0, consulta.Length - 1);
            return consulta;
        }
        #endregion
        #region Palabras clave de la búsqueda del usuario, pasadas como parámetro en la URL
        /// <summary>
        /// Obtiene las palabras clave de la búsqueda del usuario, pasadas como parámetro en la URL
        /// </summary>
        /// <param name="urlKeywords">Cadena de texto con las palabras clave de la búsqueda del usuario</param>
        /// <returns>Arreglo de palabras clave de la búsqueda del usuario</returns>
        public System.Collections.ArrayList getKeywordsFromURL(string urlKeywords)
        {
            System.Collections.ArrayList listKeywords = new System.Collections.ArrayList();
            string[] keywords = urlKeywords.Split(' ');
            foreach (string keyword in keywords)
            {
                listKeywords.Add(keyword);
            }
            return listKeywords;
        }
        #endregion
        #region Valida una URL con Expresiones Regulares
        /// <summary>
        /// Valida una URL con Expresiones Regulares
        /// </summary>
        /// <param name="url">Dirección URL a ser validada</param>
        /// <returns>Verdadero si es una URL válida, Falso en caso contrario</returns>
        public bool isValidUrl(ref string url)
        {
            string pattern = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(url);
        }
        #endregion
        #endregion
    }
}
