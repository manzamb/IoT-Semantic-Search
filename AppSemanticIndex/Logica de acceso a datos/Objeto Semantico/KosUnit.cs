// File:    KosUnit.cs
// Author:  manzamb
// Created: jueves, 23 de enero de 2014 07:59:30 p.m.
// Purpose: Definition of Class KosUnit

using System;
namespace ObjetoSemantico
{
    public class KosUnit
    {
        private String symbol;
        private String label;

        public String Symbol
        {
            get
            {
                return symbol;
            }
            set
            {
                this.symbol = value;
            }
        }

        public String Label
        {
            get
            {
                return label;
            }
            set
            {
                this.label = value;
            }
        }

    }
}