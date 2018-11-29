// File:    Objeto_Semantico.cs
// Author:  Miguel Angel Niño Za
// Created: lunes, 23 de junio de 2003 08:49:56 a.m.
// Purpose: Definition of Class Objeto_Semantico

using System;
namespace ObjetoSemantico
{
    public class Objeto_Semantico
    {
        private double _So_Id;

        public double So_Id
        {
            get
            {
                return _So_Id;
            }
            set
            {
                this._So_Id = value;
            }
        }

        public Propiedades propiedades;
        public Estados estados;
        public Metodos metodos;
        public Interacciones[] interacciones;
        public Calidad[] calidad;
        public Conocimientos[] conociminetos;

        public Objeto_Semantico(Propiedades p, Estados e, Metodos m, Interacciones[] i, Calidad[] c, Conocimientos[] k)
        {
            propiedades = p;
            estados=e;
            metodos=m;
            interacciones= i;
            calidad= c;
            conociminetos=k;
        }
    }
}