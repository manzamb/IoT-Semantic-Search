using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Configuration;
using System.Diagnostics;
//Importaciones de la Librería DE lUCENE 
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard; 
using Lucene.Net.Documents; 
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Similar;
using Lucene.Net.Search.Function;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Tokenattributes;
using Version = Lucene.Net.Util.Version;
using Iveonik.Stemmers;
//Importaciones de las clases del proyecto
using AppSemanticIndex.Pobj;
using System.Collections;

namespace AppSemanticIndex
{
    public class LuceneManager
    {
        public static string _luceneDir = SemanticIndexManager.DocumentFileLucene;
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                //Asigna la ruta en donde se almacena el índice
                if (_directoryTemp == null)
                {
                    _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                }
                //Si el índice esta bloqueado se procede a desbloquearlo
                if (IndexWriter.IsLocked(_directoryTemp))
                {
                    IndexWriter.Unlock(_directoryTemp);
                }
                //Elimina el archivo de bloqueo para su actualización
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        //Directorio en el que se encuentran los archivos JSON que se van a indexar
        private static String PATH = SemanticIndexManager.RutaBDDJSON;

        //Nombre del campo en Lunece que contienen el contenido del documento
        //private static String FIELD_CONTENT_NAME = "content";

        //Nombre de los campos a buscar en Lucene
        private static String[] DEFAULT_FIELD_NAMES = new String[] { "Title", "Tags", "Descripcion", "Conceptos" };

        //Ignore las palabras menores del código del documento
        private static int DEFALT_MIN_DOC_FREQ = 1;

        //Ignore los terminos menores del código del documento
        private static int DEFAULT_MIN_TERM_FREQ = 1;

        //El número máximo de términos que serán incluidos en la consulta
        private static int MAX_QUERY_TERMS = 1000;

        //La longitud mínima de una palabra para ser tenido en cuenta
        private static int DEFAULT_MIN_WORD_LENGTH = 2;

        //Longitud máxima de documentos a presentar en la búsqueda
        private static int DEFAULT_DOCUMENT_TO_SEARCH = 50;

        /// Número total de documentos Indexados
        private static int totalDocs = 0;

        #region "Métodos para crear el Índice"

        public static void AddUpdateLuceneIndex(Dictionary<string, UrlDocument> URLResult, Boolean usarEspañol )
        {
            // Establecer el analizado lucene
            if (usarEspañol)
            {
                CrearIndice(URLResult, new SpanishAnalyzer());
            }
            else
            {
                CrearIndice(URLResult, new StandardAnalyzer(Version.LUCENE_30));
            }
        }

        //Procedimiento que crea el índice Lucene
        private static void CrearIndice(Dictionary<string, UrlDocument> URLResult, Lucene.Net.Analysis.Analyzer analyzer)
        {
            Trace.WriteLine("Creando el índice de Lucene");
            IndexWriter writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            writer.UseCompoundFile = false;
            writer.Dispose();

            //Indexar los documentos
            Trace.WriteLine("Indexando los documentos...");
            indexFilesXively(URLResult,analyzer);
            Trace.WriteLine("'" + totalDocs + "' documentos indexados.");
        }

        public static void ActualizarLuceneIndex(Dictionary<string, UrlDocument> URLResult, Boolean usarEspañol)
        {
            // Establecer el analizado lucene
            if (usarEspañol)
            {
                AgregarAlIndice(URLResult, new SpanishAnalyzer());
            }
            else
            {
                AgregarAlIndice(URLResult, new StandardAnalyzer(Version.LUCENE_30));
            }
        }
        //Procedimiento para Modificar el indice con un nuevo documento
        private static void AgregarAlIndice(Dictionary<string, UrlDocument> URLResult, Lucene.Net.Analysis.Analyzer analyzer)
        {
            //Eliminar el documento si ya existe en el indice
            KeyValuePair<string, UrlDocument> primero = URLResult.First();
            deleteDocument(primero.Value.Id, analyzer);

            //Indexar los documentos
            Trace.WriteLine("Indexando el Documentos...");
            indexFilesXively(URLResult, analyzer);
            Trace.WriteLine("'" + totalDocs + "' documentos indexados.");
        }
        
        //Indexar los documentos procesados en la fase anterior
        private static void indexFilesXively(Dictionary<string, UrlDocument> URLResult, Lucene.Net.Analysis.Analyzer analyzer)
        {
            // Añadir los datos al indice de Lucene
            foreach (UrlDocument URLDocument in URLResult.Values)
            {
                //_addToLuceneIndex(URLDocument, writer);
                totalDocs++;
                Trace.WriteLine("[" + totalDocs + "] doc :" + URLDocument.Tittle);
                indexDocument(createLuceneDocumentXyvely(URLDocument),analyzer);
            }
        }

        //Crea un documento Lucene a partir de los Datos del documento Xively
        private static Document createLuceneDocumentXyvely(UrlDocument URLDocument)
        {
            Document document = new Document();
            //Identificadores. Se almacenan pero no se indexan ni analizan
            document.Add(new Field("Id", URLDocument.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("feed", URLDocument.URL, Field.Store.YES, Field.Index.NOT_ANALYZED));
            //Campos de texto a ser analizados e indexados
            document.Add(new Field("Descripcion", URLDocument.Resume, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            document.Add(new Field("Title", URLDocument.Tittle, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Tags", URLDocument.Tags, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Conceptos", URLDocument.ConceptosLista(), Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("DataStreams", URLDocument.Datastreams_feed, Field.Store.YES, Field.Index.ANALYZED));
            //Campos que se almacenan pero no se analizan pero si se indexan
            document.Add(new Field("Location", URLDocument.Localizacion_name, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("Domain", URLDocument.Domain, Field.Store.YES, Field.Index.NOT_ANALYZED));
            //Campos almacenados por propósitos de información temparana del feed, por ello no se analizan y no se indexan
            document.Add(new Field("Website", URLDocument.Website, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Elevacion", URLDocument.Elevacion, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Latitud", URLDocument.Latitud, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Longitud", URLDocument.Longitud, Field.Store.YES, Field.Index.NO));

            //document.Add(new Field(FIELD_CONTENT_NAME, URLDocument.Resume, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            return document;
        }        
        
        //Indexa un documento
        private static void indexDocument(Document document, Lucene.Net.Analysis.Analyzer analyzer)
        {
            try
            {
                Trace.WriteLine("Indexando el documento='" + document
                        + "' con el analizador='" + analyzer + "'.");
                //Abre el indice para adicionar documentos
                IndexWriter writer = new IndexWriter(_directory, analyzer,false, IndexWriter.MaxFieldLength.UNLIMITED);
                writer.AddDocument(document);
                writer.Optimize();
                writer.Dispose();
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception indexando:" + e.Message);
            }
        }

        //Eliminar el índice
        private static void destroyIndex()
        {
            string[] ficherosCarpeta = System.IO.Directory.GetFiles(_luceneDir);
            foreach (string ficheroActual in ficherosCarpeta)
                File.Delete(ficheroActual);
            Trace.WriteLine("Indice destruido.");
        }

        //Para el procesamiento de textos
        internal string DeleteInvalidData(string result, string tipoAnalizador)
        {
            TokenStream tokenStream = new StandardTokenizer(Version.LUCENE_30, new System.IO.StringReader(result));

            tokenStream = new StandardFilter(tokenStream);  //elimina los signos de puntuación
            tokenStream = new LowerCaseFilter(tokenStream); //convierte el contenido a minúsculas
            if (tipoAnalizador == "Español")
            {
                //filtrará el contenido con el listado de stopWords
                tokenStream = new StopFilter(true, tokenStream, StopFilter.MakeStopSet(SpanishAnalyzer.SPANISH_STOP_WORDS));
                //Convierte caracteres que estan por encima del 127 en la tabla ASCII
                tokenStream = new ASCIIFoldingFilter(tokenStream);
                //Operacion de lematización de la palabras
                tokenStream = SpanishSteammer(tokenStream);
            }
            else
            {
                //filtrará el contenido con el listado de stopWords
                tokenStream = new StopFilter(true, tokenStream, StopAnalyzer.ENGLISH_STOP_WORDS_SET);
                //Operacion de lematización de la palabras
                tokenStream = new PorterStemFilter(tokenStream);
            }

            return GetDataTokens(tokenStream);
        }

        private string GetDataTokens(TokenStream tokenStream)
        {
            string term = string.Empty;
            var termAttr = tokenStream.GetAttribute<ITermAttribute>();

            while (tokenStream.IncrementToken())
            {
                term = term + " " + termAttr.Term;
            }

            return (string.IsNullOrEmpty(term)) ? string.Empty : term.Trim();
        }

        private TokenStream SpanishSteammer(TokenStream tokenStream)
        {
            //Obtener en una cadena cada token y aplicar el lematizador a cada término
            string term = string.Empty;
            IStemmer stemmer = new SpanishStemmer();
            TokenStream tokenStreamtemp;
            var termAttr = tokenStream.GetAttribute<ITermAttribute>();

            while (tokenStream.IncrementToken())
            {
                term = term + " " + stemmer.Stem(termAttr.Term);
            }
            tokenStreamtemp = new StandardTokenizer(Version.LUCENE_30, new System.IO.StringReader(term));
            return tokenStreamtemp;
            //
        }

        //Para el procesamiento de textos
        public string AnalizarConsulta(string consulta, string tipoAnalizador)
        {
            ArrayList ListStemsList = new ArrayList();
            TokenStream tokenStream = new StandardTokenizer(Version.LUCENE_30, new System.IO.StringReader(consulta));

            tokenStream = new StandardFilter(tokenStream);  //elimina los signos de puntuación
            tokenStream = new LowerCaseFilter(tokenStream); //convierte el contenido a minúsculas
            if (tipoAnalizador == "Español")
            {
                //filtrará el contenido con el listado de stopWords
                tokenStream = new StopFilter(true, tokenStream, StopFilter.MakeStopSet(SpanishAnalyzer.SPANISH_STOP_WORDS));
                //Convierte caracteres que estan por encima del 127 en la tabla ASCII
                tokenStream = new ASCIIFoldingFilter(tokenStream);
                //Operacion de lematización de la palabras
                tokenStream = SpanishSteammer(tokenStream);
            }
            else if (tipoAnalizador == "Ingles")
            {
                //filtrará el contenido con el listado de stopWords
                tokenStream = new StopFilter(true, tokenStream, StopAnalyzer.ENGLISH_STOP_WORDS_SET);
                //Operacion de lematización de la palabras
                tokenStream = new PorterStemFilter(tokenStream);
            }
            else //Sino establece idioma solo elimina palabras vacias en ambos idiomas
            {
                //filtrará el contenido con el listado de stopWords
                tokenStream = new StopFilter(true, tokenStream, StopFilter.MakeStopSet(SpanishAnalyzer.SPANISH_STOP_WORDS));
                //filtrará el contenido con el listado de stopWords
                tokenStream = new StopFilter(true, tokenStream, StopAnalyzer.ENGLISH_STOP_WORDS_SET);

            }

            string term = string.Empty;
            var termAttr = tokenStream.GetAttribute<ITermAttribute>();

            int i = 0;
            while (tokenStream.IncrementToken())
            {
                if (i == 0)
                {
                    term = termAttr.Term;
                    i++;
                }
                else
                    term = term + "," + termAttr.Term;
            }

            return (string.IsNullOrEmpty(term)) ? string.Empty : term.Trim();
        }

        internal static bool IndexExists()
        {
            //Valida si existe el indice
            return IndexReader.IndexExists(_directory);
        }

        #endregion

        #region "Métodos para generar Busquedas con Similaridad TFIDF"

        //Realiza la búsqueda de un texto "original" contra los documentos indexados utilizando TFIDF
        public static List<Document> moreLikeThisAnalyzer(String original, ISet<string> stopWords, Lucene.Net.Analysis.Analyzer analyzer)
        {
            Trace.WriteLine("Realizando la Búsqueda");
            List<Document> DocumenResult = new List<Document>();

            IndexReader indexReader = IndexReader.Open(_directory, true);
            IndexSearcher indexSearcher = new IndexSearcher(indexReader);

            MoreLikeThis mlt = new MoreLikeThis(indexReader);
            
            mlt.SetFieldNames(DEFAULT_FIELD_NAMES);
            mlt.MinDocFreq = DEFALT_MIN_DOC_FREQ;
            mlt.MinTermFreq = DEFAULT_MIN_TERM_FREQ;
            mlt.MaxQueryTerms = MAX_QUERY_TERMS;
            mlt.MinWordLen = DEFAULT_MIN_WORD_LENGTH;
            mlt.Analyzer = analyzer;
            mlt.SetStopWords(stopWords);

            Query query = mlt.Like(new System.IO.StringReader(original));

            int topCount = DEFAULT_DOCUMENT_TO_SEARCH;

            TopScoreDocCollector collector = TopScoreDocCollector.Create(topCount, true);
            indexSearcher.Search(query, collector);
            ScoreDoc[] hits = collector.TopDocs().ScoreDocs;
            var result = new List<string>();
            //Hits hits = indexSearcher.Search(query);  

            int len = hits.Length;

            Trace.WriteLine("Entering");
            Trace.WriteLine("-------------------------------------------");
            Trace.WriteLine("original :" + original);
            Trace.WriteLine("query: " + query);
            Trace.WriteLine("found: " + len + " documents");
            for (int i = 0; i < Math.Min(25, len); i++)
            {
                int d = hits[i].Doc;
                Trace.WriteLine("score   : " + hits[i].Score);
                Trace.WriteLine("name    : " + d.ToString());
                //Colocar los datos en el arreglo de resultados
                Document doc = indexSearcher.Doc(hits[i].Doc);
                DocumenResult.Add(doc);
            }
            Trace.WriteLine("-------------------------------------------");
            Trace.WriteLine("Exiting");
            return DocumenResult;
        }

        public static List<UrlDocument> BuscarEnIndiceSemantico(String original, Boolean usarEspañol)
        {
            //Lista de documentos resultado de la búsqueda
            List<Document> DocumenResult = new List<Document>();
            List<UrlDocument> UrlResult = new List<UrlDocument>();

            //Llama al procedimiento que realiza la busqueda
            if (usarEspañol)
            {
                DocumenResult = moreLikeThisAnalyzer(original, StopFilter.MakeStopSet(SpanishAnalyzer.SPANISH_STOP_WORDS), new SpanishAnalyzer());
            }
            else
            {
                DocumenResult = moreLikeThisAnalyzer(original, Lucene.Net.Analysis.StopAnalyzer.ENGLISH_STOP_WORDS_SET, new StandardAnalyzer(Version.LUCENE_30));
            }
            
            //Convertir en UrlDocument para su procesamiento Web
            foreach (Document doc in DocumenResult)
            {
                UrlDocument UrlDoc = new UrlDocument();
                UrlDoc.Id = doc.GetField("Id").StringValue;
                UrlDoc.Tittle = doc.GetField("Title").StringValue;
                UrlDoc.URL = doc.GetField("feed").StringValue;
                UrlDoc.Resume = doc.GetField("Descripcion").StringValue;
                UrlDoc.Tags = doc.GetField("Tags").StringValue;
                UrlDoc.Localizacion_name = doc.GetField("Location").StringValue;
                UrlDoc.Domain = doc.GetField("Domain").StringValue;
                UrlDoc.Datastreams_feed = doc.GetField("DataStreams").StringValue;
                UrlDoc.Website = doc.GetField("Website").StringValue;
                UrlDoc.Elevacion = doc.GetField("Elevacion").StringValue;
                UrlDoc.Latitud = doc.GetField("Latitud").StringValue;
                UrlDoc.Longitud = doc.GetField("Longitud").StringValue;

                //Campos propios de indexacion
                string listaconconceptos = doc.GetField("Conceptos").StringValue;
                UrlDoc.Conceptos =  ConvertirenLista(listaconconceptos);

                UrlResult.Add(UrlDoc);
            }
            return UrlResult;
        }

    public static void deleteDocument(string feedId, Lucene.Net.Analysis.Analyzer analyzer) 
    {
        try
        {
            IndexWriter writer = new IndexWriter(_directory, analyzer,false, IndexWriter.MaxFieldLength.UNLIMITED);
            int numDocs = writer.NumDocs();

            Term term = new Term("Id", feedId); 
            writer.DeleteDocuments(term);
            writer.Optimize();
            writer.Dispose();

            numDocs = writer.NumDocs();
        }
        catch(Exception ex)
        {
            Trace.WriteLine("No se pudo remover " + feedId + " del índice : Error: " + ex.Message);
        }
    }
        
        #endregion

        private static List<string> ConvertirenLista(string listastr)
        {
            List<string> devolver = new List<string>();
            string[] terminos = ObtenerTerminos(listastr);
            foreach (string str in terminos)
                devolver.Add(str);
            return devolver;
        }

        private static string[] ObtenerTerminos(string cadena)
        {
            char[] delimitadores = { ',' };

            string[] terminos = cadena.Split(delimitadores);

            return terminos;
        }
    }
}
