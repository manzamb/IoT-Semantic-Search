// File:    Interacciones.cs
// Author:  Miguel Angel Niño Za
// Created: lunes, 23 de junio de 2003 10:21:08 a.m.
// Purpose: Definition of Class Interacciones

using System;
namespace ObjetoSemantico
{
    public abstract class Interacciones
    {
        private String _Description;

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

        public System.Collections.Generic.List<Objeto_Semantico> participaCon;

        /// <summary>
        /// apiKey for collection of Objeto_Semantico
        /// </summary>
        /// <pdGenerated>Default opposite class collection property</pdGenerated>
        public System.Collections.Generic.List<Objeto_Semantico> ParticipaCon
        {
            get
            {
                if (participaCon == null)
                    participaCon = new System.Collections.Generic.List<Objeto_Semantico>();
                return participaCon;
            }
            set
            {
                RemoveAllParticipaCon();
                if (value != null)
                {
                    foreach (Objeto_Semantico oObjeto_Semantico in value)
                        AddParticipaCon(oObjeto_Semantico);
                }
            }
        }

        /// <summary>
        /// Add a new Objeto_Semantico in the collection
        /// </summary>
        /// <pdGenerated>Default Add</pdGenerated>
        public void AddParticipaCon(Objeto_Semantico newObjeto_Semantico)
        {
            if (newObjeto_Semantico == null)
                return;
            if (this.participaCon == null)
                this.participaCon = new System.Collections.Generic.List<Objeto_Semantico>();
            if (!this.participaCon.Contains(newObjeto_Semantico))
                this.participaCon.Add(newObjeto_Semantico);
        }

        /// <summary>
        /// Remove an existing Objeto_Semantico from the collection
        /// </summary>
        /// <pdGenerated>Default Remove</pdGenerated>
        public void RemoveParticipaCon(Objeto_Semantico oldObjeto_Semantico)
        {
            if (oldObjeto_Semantico == null)
                return;
            if (this.participaCon != null)
                if (this.participaCon.Contains(oldObjeto_Semantico))
                    this.participaCon.Remove(oldObjeto_Semantico);
        }

        /// <summary>
        /// Remove all instances of Objeto_Semantico from the collection
        /// </summary>
        /// <pdGenerated>Default removeAll</pdGenerated>
        public void RemoveAllParticipaCon()
        {
            if (participaCon != null)
                participaCon.Clear();
        }

    }
}