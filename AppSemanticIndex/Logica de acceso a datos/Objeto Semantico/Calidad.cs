// File:    Calidad.cs
// Author:  Miguel Angel Niño Za
// Created: lunes, 23 de junio de 2003 10:21:08 a.m.
// Purpose: Definition of Class Calidad

using System;

namespace ObjetoSemantico
{
    /// Esta clase esta resumida ya que no se usara en el primera verión del índice
    public class Calidad
    {
        private String objetivos;
        private String indicadores;
        private String metas;

        public String Objetivos
        {
            get
            {
                return objetivos;
            }
            set
            {
                this.objetivos = value;
            }
        }

        public String Indicadores
        {
            get
            {
                return indicadores;
            }
            set
            {
                this.indicadores = value;
            }
        }

        public String Metas
        {
            get
            {
                return metas;
            }
            set
            {
                this.metas = value;
            }
        }

    }
}