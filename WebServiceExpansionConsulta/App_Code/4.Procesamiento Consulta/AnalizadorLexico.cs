using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using System.Text.RegularExpressions;


namespace ModeloSemantico_PU.ProcesamientoConsulta
{
    public class AnalizadorLexico
    {         
        #region Constructor
        /// <summary>
        /// Inicializa una instancia de la  clase <see cref=" AnalizadorLexico"/>.
        /// </summary>
        public AnalizadorLexico()
        {           
        }
        #endregion        

        #region Metodos        

        public ArrayList getKeywords(string result, string tipoAnalizador)
        {
            ArrayList ListStemsList = new ArrayList();
            TokenStream tokenStream = new StandardTokenizer(new System.IO.StringReader(result));

            tokenStream = new StandardFilter(tokenStream);  //elimina los signos de puntuación
            tokenStream = new LowerCaseFilter(tokenStream); //convierte el contenido a minúsculas
            if (tipoAnalizador == "Español")
            {
                //filtrará el contenido con el listado de stopWords
                tokenStream = new StopFilter(tokenStream, StopFilter.MakeStopSet(SpanishAnalyzer.SPANISH_STOP_WORDS), true);
                //Operacion de lematización de la palabras
                //SpanishAnalyzer ansp = new SpanishAnalyzer();
                //tokenStream = ansp.SpanishSteammer(tokenStream);
            }
            else
            {
                //filtrará el contenido con el listado de stopWords
                tokenStream = new StopFilter(tokenStream, StopAnalyzer.ENGLISH_STOP_WORDS, true);
                //Operacion de lematización de la palabras
                //tokenStream = new PorterStemFilter(tokenStream);
            }

            string cadena = "";
            string[] token;
            Lucene.Net.Analysis.Token current;
            while ((current = tokenStream.Next()) != null)
            {
                cadena = current.ToString();
                token = cadena.Split(',');
                cadena = cadena.Substring(1, token[0].Length - 1);
                ListStemsList.Add(cadena);
            }
            return ListStemsList;
        }

        #endregion
    }
}
