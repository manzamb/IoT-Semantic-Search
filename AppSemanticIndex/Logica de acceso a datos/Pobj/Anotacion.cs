using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSemanticIndex.Pobj
{
    class Anotacion
    {
        public enum Idiomas
        {
            es,
            en,
            sinidioma,
        }

        private string anotacionKey;

        public string AnotacionKey
        {
            get { return anotacionKey; }
            set { anotacionKey = value; }
        }

        private string valor;

        public string Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        private Idiomas idioma;

        internal Idiomas Idioma
        {
            get { return idioma; }
            set { idioma = value; }
        }

        public Anotacion(string claveAnotacion, string valorAnotación, string valorlenguaje)
        {
            AnotacionKey = claveAnotacion;
            Valor = valorAnotación;
            if (valorlenguaje == "es")
            {
                idioma = Idiomas.es;
            }
            else if (valorlenguaje == "en")
            {
                idioma = Idiomas.en;
            }
            else
            {
                idioma = Idiomas.sinidioma;
            }
        }
    }
}
