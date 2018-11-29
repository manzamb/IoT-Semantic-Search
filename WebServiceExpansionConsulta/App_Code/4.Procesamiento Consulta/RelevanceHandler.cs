using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections;
using System.IO;

namespace ModeloSemantico_PU
{
    public class RelevanceHandler
    {
        #region Métodos
        //Cálculo de estadísticas Kappa
        public void getKappaStatistics(System.Collections.Generic.Dictionary<string, int> evaluacion1, string login, string consulta)
        {
            //Guardar documentos y calificaciones
            StreamWriter docs = new StreamWriter("G:\\" + login + " - " + consulta + ".txt");
            foreach (System.Collections.Generic.KeyValuePair<string, int> kvp in evaluacion1)
            {
                if(kvp.Value >= 3)
                    docs.WriteLine(kvp.Key + "\tSI");
                else
                    docs.WriteLine(kvp.Key + "\tNO");
            }
            docs.Close();            
        }
        #endregion
    }
}
