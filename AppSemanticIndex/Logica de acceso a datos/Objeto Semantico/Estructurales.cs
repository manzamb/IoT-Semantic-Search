// File:    Estructurales.cs
// Author:  manzamb
// Created: jueves, 23 de enero de 2014 08:40:45 p.m.
// Purpose: Definition of Class Estructurales

using System;
namespace ObjetoSemantico
{
    public class Estructurales : Interacciones
    {
        private DKind kind;

        public DKind Kind
        {
            get
            {
                return kind;
            }
            set
            {
                this.kind = value;
            }
        }

    }
}