// File:    Suscrito.cs
// Author:  manzamb
// Created: jueves, 23 de enero de 2014 08:11:38 p.m.
// Purpose: Definition of Class Suscrito

using System;
namespace ObjetoSemantico
{
    public class Suscrito : Metodos
    {
        public System.Collections.Generic.List<Objeto_Semantico> enviaDatosA;

        /// <summary>
        /// apiKey for collection of Objeto_Semantico
        /// </summary>
        /// <pdGenerated>Default opposite class collection property</pdGenerated>
        public System.Collections.Generic.List<Objeto_Semantico> EnviaDatosA
        {
            get
            {
                if (enviaDatosA == null)
                    enviaDatosA = new System.Collections.Generic.List<Objeto_Semantico>();
                return enviaDatosA;
            }
            set
            {
                RemoveAllEnviaDatosA();
                if (value != null)
                {
                    foreach (Objeto_Semantico oObjeto_Semantico in value)
                        AddEnviaDatosA(oObjeto_Semantico);
                }
            }
        }

        /// <summary>
        /// Add a new Objeto_Semantico in the collection
        /// </summary>
        /// <pdGenerated>Default Add</pdGenerated>
        public void AddEnviaDatosA(Objeto_Semantico newObjeto_Semantico)
        {
            if (newObjeto_Semantico == null)
                return;
            if (this.enviaDatosA == null)
                this.enviaDatosA = new System.Collections.Generic.List<Objeto_Semantico>();
            if (!this.enviaDatosA.Contains(newObjeto_Semantico))
                this.enviaDatosA.Add(newObjeto_Semantico);
        }

        /// <summary>
        /// Remove an existing Objeto_Semantico from the collection
        /// </summary>
        /// <pdGenerated>Default Remove</pdGenerated>
        public void RemoveEnviaDatosA(Objeto_Semantico oldObjeto_Semantico)
        {
            if (oldObjeto_Semantico == null)
                return;
            if (this.enviaDatosA != null)
                if (this.enviaDatosA.Contains(oldObjeto_Semantico))
                    this.enviaDatosA.Remove(oldObjeto_Semantico);
        }

        /// <summary>
        /// Remove all instances of Objeto_Semantico from the collection
        /// </summary>
        /// <pdGenerated>Default removeAll</pdGenerated>
        public void RemoveAllEnviaDatosA()
        {
            if (enviaDatosA != null)
                enviaDatosA.Clear();
        }

    }
}