using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Lucene.Net.Analysis; 
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Version = Lucene.Net.Util.Version;
using Iveonik.Stemmers;

namespace AppSemanticIndex
{
    public class SpanishAnalyzer : Lucene.Net.Analysis.Analyzer
    {
        public static readonly string[] SPANISH_STOP_WORDS = {
            "a", "acá", "ahí", "ajena", "ajenas", "ajeno", "ajenos", "al", "algo", "algún", 
            "alguna", "algunas", "alguno", "algunos", "allá", "alli", "allí", "ambos", 
            "ampleamos", "ante", "antes", "aquel", "aquella", "aquellas", "aquello", "aquellos", 
            "aqui", "aquí", "arriba", "asi", "atras", "aun", "aunque", "bajo", "bastante", 
            "bien", "cabe", "cada", "casi", "cierta", "ciertas", "cierto", "ciertos", "como", 
            "cómo", "con", "conmigo", "conseguimos", "conseguir", "consigo", "consigue", 
            "consiguen", "consigues", "contigo", "contra", "cual", "cuales", "cualquier", 
            "cualquiera", "cualquieras", "cuancuán", "cuando", "cuanta", "cuánta", "cuantas", 
            "cuántas", "cuanto", "cuánto", "cuantos", "cuántos", "de", "dejar", "del", "demas", 
            "demás", "demasiada", "demasiadas", "demasiado", "demasiados", "dentro", "desde", 
            "donde", "dos", "el", "él", "ella", "ellas", "ello", "ellos", "empleais", "emplean", 
            "emplear", "empleas", "empleo", "en", "encima", "entonces", "entre", "era", "eramos", 
            "eran", "eras", "eres", "es", "esa", "esas", "ese", "eso", "esos", "esta", "estaba",
            "estado", "estais", "estamos", "estan", "estar", "estas", "este", "esto", "estos", 
            "estoy", "etc", "fin", "fue", "fueron", "fui", "fuimos", "gueno", "ha", "hace", 
            "haceis", "hacemos", "hacen", "hacer", "haces", "hacia", "hago", "han", "hasta", 
            "incluso", "intenta", "intentais", "intentamos", "intentan", "intentar", "intentas", 
            "intento", "ir", "jamás", "junto", "juntos", "la", "largo", "las", "le", "les", 
            "lo", "los", "mas", "más", "me", "menos", "mi", "mia", "mía", "mias", "mientras", 
            "mio", "mío", "mios", "mis", "misma", "mismas", "mismo", "mismos", "modo", "mucha", 
            "muchas", "muchísima", "muchísimas", "muchísimo", "muchísimos", "mucho", "muchos", 
            "muy", "nada", "ni", "ningun", "ninguna", "ningunas", "ninguno", "ningunos", "no", 
            "nos", "nosotras", "nosotros", "nuestra", "nuestras", "nuestro", "nuestros", "nunca", 
            "o", "os", "otra", "otras", "otro", "otros", "para", "parecer", "pero", "poca", 
            "pocas", "poco", "pocos", "podeis", "podemos", "poder", "podria", "podriais", 
            "podriamos", "podrian", "podrias", "por", "por qué", "porque", "primero", "primero desde", 
            "puede", "pueden", "puedo", "pues", "que", "qué", "querer", "quien", "quién", "quienes", 
            "quienquiera", "quiera", "quiza", "quizas", "sabe", "sabeis", "sabemos", "saben", "saber", 
            "sabes", "se", "segun", "ser", "si", "sí", "siempre", "siendo", "sin", "sín", "sino", 
            "so", "sobre", "sois", "solamente", "solo", "somos", "son", "soy", "sr", "sra", "sres", "su", 
            "sus", "suya", "suyas", "suyo", "suyos", "tal", "tales", "tambien", "también", "tampoco", 
            "tan", "tanta", "tantas", "tanto", "tantos", "te", "teneis", "tenemos", "tener", "tengo",
            "ti", "tiempo", "tiene", "tienen", "toda", "todas", "todo", "todos", "tomar", "trabaja", 
            "trabajais", "trabajamos", "trabajan", "trabajar", "trabajas", "trabajo", "tras", "tu", 
            "tú", "tus", "tuya", "tuyo", "tuyos", "u", "ultimo", "un", "una", "unas", "uno", "unos", 
            "usa", "usais", "usamos", "usan", "usar", "usas", "uso", "usted", "ustedes", "va", "vais", 
            "valor", "vamos", "van", "varias", "varios", "vaya", "verdad", "verdadera", "verdadero", 
            "vosotras", "vosotros", "voy", "vuestra", "vuestras", "vuestro", "vuestros", "y", "ya", "yo"};

        private Object stopTable = new HashSet<Object>();

        public SpanishAnalyzer()
        {
            stopTable = StopFilter.MakeStopSet(SPANISH_STOP_WORDS);
        }

        public SpanishAnalyzer(String[] stopWords)
        {
            stopTable = StopFilter.MakeStopSet(stopWords);
        }

        public override TokenStream TokenStream(String FieldName, TextReader reader)
        {
            TokenStream result = new StandardTokenizer(Version.LUCENE_30, reader);
            result = new StandardFilter(result);
            result = new LowerCaseFilter(result);
            result = new StopFilter(true, result, StopFilter.MakeStopSet(SPANISH_STOP_WORDS));
            //result = new PorterStemFilter(result);
            result = SpanishSteammer(result);
            return result;
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
    }
}
