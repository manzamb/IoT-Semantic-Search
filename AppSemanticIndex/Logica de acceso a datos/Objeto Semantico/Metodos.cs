// File:    Metodos.cs
// Author:  Miguel Angel Niño Za
// Created: lunes, 23 de junio de 2003 10:21:08 a.m.
// Purpose: Definition of Class Metodos

using System;
namespace ObjetoSemantico
{
    public abstract class Metodos
    {
        private String Format;
        private String Size;
        private String descripcion;
        private String url;

        public String _Format
        {
            get
            {
                return Format;
            }
            set
            {
                this.Format = value;
            }
        }

        public String _Size
        {
            get
            {
                return Size;
            }
            set
            {
                this.Size = value;
            }
        }

        public String Descripcion
        {
            get
            {
                return descripcion;
            }
            set
            {
                this.descripcion = value;
            }
        }

        public String Url
        {
            get
            {
                return url;
            }
            set
            {
                this.url = value;
            }
        }

    }
}