// File:    Objeto.cs
// Author:  manzamb
// Created: miércoles, 22 de enero de 2014 03:40:00 p.m.
// Purpose: Definition of Class Objeto

using System;


namespace ObjetoSemantico
{

    public class Objeto : Propiedades
    {
        private String _Id;
        private DObjectType _ObjectType;

        public String Id
        {
            get
            {
                return _Id;
            }
            set
            {
                this._Id = value;
            }
        }

        public DObjectType ObjectType
        {
            get
            {
                return _ObjectType;
            }
            set
            {
                this._ObjectType = value;
            }
        }

        public System.Collections.Generic.List<SsnSensor> pertenecenA;

        /// <summary>
        /// apiKey for collection of SsnSensor
        /// </summary>
        /// <pdGenerated>Default opposite class collection property</pdGenerated>
        public System.Collections.Generic.List<SsnSensor> PertenecenA
        {
            get
            {
                if (pertenecenA == null)
                    pertenecenA = new System.Collections.Generic.List<SsnSensor>();
                return pertenecenA;
            }
            set
            {
                RemoveAllPertenecenA();
                if (value != null)
                {
                    foreach (SsnSensor oSsnSensor in value)
                        AddPertenecenA(oSsnSensor);
                }
            }
        }

        /// <summary>
        /// Add a new SsnSensor in the collection
        /// </summary>
        /// <pdGenerated>Default Add</pdGenerated>
        public void AddPertenecenA(SsnSensor newSsnSensor)
        {
            if (newSsnSensor == null)
                return;
            if (this.pertenecenA == null)
                this.pertenecenA = new System.Collections.Generic.List<SsnSensor>();
            if (!this.pertenecenA.Contains(newSsnSensor))
                this.pertenecenA.Add(newSsnSensor);
        }

        /// <summary>
        /// Remove an existing SsnSensor from the collection
        /// </summary>
        /// <pdGenerated>Default Remove</pdGenerated>
        public void RemovePertenecenA(SsnSensor oldSsnSensor)
        {
            if (oldSsnSensor == null)
                return;
            if (this.pertenecenA != null)
                if (this.pertenecenA.Contains(oldSsnSensor))
                    this.pertenecenA.Remove(oldSsnSensor);
        }

        /// <summary>
        /// Remove all instances of SsnSensor from the collection
        /// </summary>
        /// <pdGenerated>Default removeAll</pdGenerated>
        public void RemoveAllPertenecenA()
        {
            if (pertenecenA != null)
                pertenecenA.Clear();
        }
        public System.Collections.Generic.List<SsnDevice> perteneceA;

        /// <summary>
        /// apiKey for collection of SsnDevice
        /// </summary>
        /// <pdGenerated>Default opposite class collection property</pdGenerated>
        public System.Collections.Generic.List<SsnDevice> PerteneceA
        {
            get
            {
                if (perteneceA == null)
                    perteneceA = new System.Collections.Generic.List<SsnDevice>();
                return perteneceA;
            }
            set
            {
                RemoveAllPerteneceA();
                if (value != null)
                {
                    foreach (SsnDevice oSsnDevice in value)
                        AddPerteneceA(oSsnDevice);
                }
            }
        }

        /// <summary>
        /// Add a new SsnDevice in the collection
        /// </summary>
        /// <pdGenerated>Default Add</pdGenerated>
        public void AddPerteneceA(SsnDevice newSsnDevice)
        {
            if (newSsnDevice == null)
                return;
            if (this.perteneceA == null)
                this.perteneceA = new System.Collections.Generic.List<SsnDevice>();
            if (!this.perteneceA.Contains(newSsnDevice))
                this.perteneceA.Add(newSsnDevice);
        }

        /// <summary>
        /// Remove an existing SsnDevice from the collection
        /// </summary>
        /// <pdGenerated>Default Remove</pdGenerated>
        public void RemovePerteneceA(SsnDevice oldSsnDevice)
        {
            if (oldSsnDevice == null)
                return;
            if (this.perteneceA != null)
                if (this.perteneceA.Contains(oldSsnDevice))
                    this.perteneceA.Remove(oldSsnDevice);
        }

        /// <summary>
        /// Remove all instances of SsnDevice from the collection
        /// </summary>
        /// <pdGenerated>Default removeAll</pdGenerated>
        public void RemoveAllPerteneceA()
        {
            if (perteneceA != null)
                perteneceA.Clear();
        }

    }
}
