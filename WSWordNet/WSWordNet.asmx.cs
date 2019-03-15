using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WSWordNet.Logic;
using WSWordNet.DataAccess;


namespace WSWordNet
{

    //Syn.WordNet.WordNetEngine wordNetEngine = new Syn.WordNet.WordNetEngine();
    //Syn.WordNet.PartOfSpeech pos = new Syn.WordNet.PartOfSpeech();
    //Syn.WordNet.SynSet ss = new Syn.WordNet.SynSet(pos, 0, wordNetEngine);
    //Console.WriteLine("words form wordnet" + ss.Words);

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
        public string[] GetConceptsOfQuery(string query)
        {
            string path = HttpContext.Current.Server.MapPath("./");
            string newPath = path + @"\bin\dict\";
            WordNetManager manager = new WordNetManager(newPath);
            Concept concepto = manager.GetConceptOfQuery(query);
            string[] words = concepto.GetAllNounSynSets();
            return words;
        }

        [WebMethod]
        public string GetSynonym(string concept)
        {
            string conceptSynonym = "";
            string Synonym = "";
            int mayor = int.MinValue;
            edu.smu.tspell.wordnet.NounSynset nounSynset;
            string path = HttpContext.Current.Server.MapPath("./");
            string newPath = path + @"\bin\dict\";
            java.lang.System.setProperty("wordnet.database.dir", newPath);
            edu.smu.tspell.wordnet.WordNetDatabase database = edu.smu.tspell.wordnet.WordNetDatabase.getFileInstance();
            //se obtienen los sinonimos del concepto concept
            edu.smu.tspell.wordnet.Synset[] synsets = database.getSynsets(concept, edu.smu.tspell.wordnet.SynsetType.NOUN);

            if (synsets.Length != 0)
            {   //se guarda los elementos del concepto
                nounSynset = (edu.smu.tspell.wordnet.NounSynset)(synsets[0]);
                foreach (string wf in nounSynset.getWordForms())
                {
                    //valor que indica el uso del sinonimo
                    int pos = nounSynset.getTagCount(wf);
                    //se obtiene el sinonimo de mayor uso
                    if (pos > mayor && wf != concept)
                    {
                        mayor = pos;
                        Synonym = wf;
                    }
                }
                if (Synonym != "")
                    conceptSynonym = "(" + concept + " + " + Synonym + ")";
            }
            return conceptSynonym;
        }   
    }
}
