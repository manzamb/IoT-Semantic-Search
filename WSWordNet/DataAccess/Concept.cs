using System.Collections.Generic;
using edu.smu.tspell.wordnet;

namespace WSWordNet.DataAccess
{
    public class Concept
    {
        #region Fields

        private List<NounSynset[]> _hyponyms;
        private List<NounSynset[]> _hyperonyms;
        private List<NounSynset> _nounSynSet;       
        private Synset[] _synsets;
        private WordNetDatabase _database;

        #endregion
        #region Properties

        public List<NounSynset> NounSynSet
        {
            get { return _nounSynSet; }
            set { _nounSynSet = value; }
        }
        public List<NounSynset[]> Hyperonyms
        {
            get { return _hyperonyms; }
            set { _hyperonyms = value; }
        }
        public List<NounSynset[]> Hyponyms
        {
            get { return _hyponyms; }
            set { _hyponyms = value; }
        }        
        public int CountHypernonyms
        {
            get {                
                if (_hyperonyms != null)
                {
                    int count = 0;
                    foreach(NounSynset[] nounSynset in _hyperonyms)
                    {
                      count+= nounSynset.Length;
                    }
                    return count; //retorna la cantidad de elementos de un array
                }                    
                else
                    return 0;
                

            }
        }

        #endregion
        #region Constructor

        public Concept(string query, WordNetDatabase database)
        {
            _database = database;            
            if (_hyponyms == null)
                _hyponyms = new List<NounSynset[]>();
            if (_hyperonyms == null)
                _hyperonyms = new List<NounSynset[]>();
            if (_nounSynSet == null)
                _nounSynSet = new List<NounSynset>();

            //se hace la consulta 
            _synsets = database.getSynsets(query, SynsetType.NOUN);
            for (int i = 0; i < _synsets.Length; i++)
            {
                //se guarda los elementos del concepto
                NounSynset nounSynset = (NounSynset)(_synsets[i]);
                NounSynset[] hyponyms = nounSynset.getHyponyms();
                NounSynset[] hyperonyms = nounSynset.getHypernyms();

                _hyperonyms.Add(hyperonyms);
                _hyponyms.Add(hyponyms);
                _nounSynSet.Add(nounSynset);
            }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// GetAllNounSynSets
        /// </summary>
        /// <returns>Return a list of all the related concepts of a concept</returns>
        public string[] GetAllNounSynSets() 
        { 
            //para sinonimos
          List<string> nounSynSetInString = new List<string>();
          foreach (NounSynset nounSynSet in _nounSynSet)
          {
              string[] wordForms = nounSynSet.getWordForms();
              foreach (string word in wordForms)
              {
                  if(!nounSynSetInString.Contains(word))
                    nounSynSetInString.Add(word);
              }
          }

            //para hiperonimos
            foreach(NounSynset [] hypernonymsVector in _hyperonyms )
            {
                foreach(NounSynset hypernomy in hypernonymsVector )
                {
                      string[] wordForms = hypernomy.getWordForms();
                      foreach (string word in wordForms)
                      {
                          if (!nounSynSetInString.Contains(word))
                              nounSynSetInString.Add(word);
                      }
                }
            }

            //para hiponimos
            foreach (NounSynset[] hyponymsVector in _hyponyms)
            {
                foreach (NounSynset hyponym in hyponymsVector)
                {
                    string[] wordForms = hyponym.getWordForms();
                    foreach (string word in wordForms)
                    {
                        if (!nounSynSetInString.Contains(word))
                            nounSynSetInString.Add(word);
                    }
                }
            }

          return nounSynSetInString.ToArray();
        }

        #endregion
    }
}
