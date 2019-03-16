using System.Web;
using System.Web.Services;
using Syn.WordNet;

namespace WSWordNet
{



    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceWordNet : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetSynonym(string concept)
        {
            string sinonimos = "";
            string conceptSynonym = "";

            //Cargar la base de datos de WordNet
            WordNetEngine wordNetEngine = new WordNetEngine();
            PartOfSpeech pos = new PartOfSpeech();

            //var directory = Directory.GetCurrentDirectory();
            var directory = HttpContext.Current.Server.MapPath("./") + "bin\\WordNet";
            wordNetEngine.LoadFromDirectory(directory);


            //Utilizar la librería para traer los sinonimos
            var synSetList = wordNetEngine.GetSynSets(concept);
            if (synSetList.Count == 0) return "(" + concept + " + )";

            foreach (var synSet in synSetList)
            {
                var words = string.Join(" + ", synSet.Words);

                sinonimos = sinonimos + words;
            }

            conceptSynonym = "(" + sinonimos + ")";


            return conceptSynonym;
        }   
    }
}
