// File:    Dinamicas.cs
// Author:  manzamb
// Created: jueves, 23 de enero de 2014 08:40:46 p.m.
// Purpose: Definition of Class Dinamicas

using System;
namespace ObjetoSemantico
{
    public class Dinamicas : Interacciones
    {
        private String evento;
        private String condicion;
        private String accion;

        public String Evento
        {
            get
            {
                return evento;
            }
            set
            {
                this.evento = value;
            }
        }

        public String Condicion
        {
            get
            {
                return condicion;
            }
            set
            {
                this.condicion = value;
            }
        }

        public String Accion
        {
            get
            {
                return accion;
            }
            set
            {
                this.accion = value;
            }
        }

    }
}