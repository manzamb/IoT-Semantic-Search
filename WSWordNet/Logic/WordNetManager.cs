using System.Collections.Generic;
using edu.smu.tspell.wordnet;
using WSWordNet.DataAccess;

namespace WSWordNet.Logic
{
    public class WordNetManager
    {
        #region Fields

        private const string _wordnet = "wordnet.database.dir";
        private string _pathDataBase;       
        WordNetDatabase _database;

        #endregion
        #region Properties

        public string PathDataBase
        {
            get { return _pathDataBase; }
            set { _pathDataBase = value; }
        }                        

        #endregion
        #region Constructor

        public WordNetManager(string pathDataBase)
        {           
            _pathDataBase = pathDataBase;
            

            java.lang.System.setProperty(_wordnet, _pathDataBase);
            _database = WordNetDatabase.getFileInstance();
        }

        #endregion
        #region Public Methods

        //obtiene un concepto a partir de una consulta
        public Concept GetConceptOfQuery(string query)
        {            
            Concept concept = new Concept(query, _database);
            return concept;         
        }

        //lista de conceptos a partir de la combinacion de terminos
        public List<Concept> GetConceptsOfQuerys(string[] querys)
        {
            List<Concept> concepts = new List<Concept>();
            foreach (string query in querys)
            {
                Concept concept = new Concept(query, _database);
                concepts.Add(concept);
            }
            return concepts;
        }

        #endregion
    }
}
