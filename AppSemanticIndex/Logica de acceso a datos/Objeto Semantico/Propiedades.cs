// File:    Propiedades.cs
// Author:  Miguel Angel Niño Za
// Created: lunes, 23 de junio de 2003 08:49:56 a.m.
// Purpose: Definition of Class Propiedades

using System;
namespace ObjetoSemantico
{
    public abstract class Propiedades
    {
        private String _Identifier;
        private String _Title;
        private String _Catalog_Entry;
        private String _Description;
        private String _Keyword;
        private String _Structure;
        private String _Aggregation_Level;

        public String Identifier
        {
            get
            {
                return _Identifier;
            }
            set
            {
                this._Identifier = value;
            }
        }

        public String Title
        {
            get
            {
                return _Title;
            }
            set
            {
                this._Title = value;
            }
        }

        public String Catalog_Entry
        {
            get
            {
                return _Catalog_Entry;
            }
            set
            {
                this._Catalog_Entry = value;
            }
        }

        public String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                this._Description = value;
            }
        }

        public String Keyword
        {
            get
            {
                return _Keyword;
            }
            set
            {
                this._Keyword = value;
            }
        }

        public String Structure
        {
            get
            {
                return _Structure;
            }
            set
            {
                this._Structure = value;
            }
        }

        public String Aggregation_Level
        {
            get
            {
                return _Aggregation_Level;
            }
            set
            {
                this._Aggregation_Level = value;
            }
        }

    }
}