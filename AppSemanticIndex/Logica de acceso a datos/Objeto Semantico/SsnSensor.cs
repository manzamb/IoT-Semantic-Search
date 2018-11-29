// File:    SsnSensor.cs
// Author:  manzamb
// Created: jueves, 23 de enero de 2014 05:03:19 p.m.
// Purpose: Definition of Class SsnSensor

using System;
namespace ObjetoSemantico
{
    public abstract class SsnSensor
    {
        private String _sensorId;

        public String sensorId
        {
            get
            {
                return _sensorId;
            }
            set
            {
                this._sensorId = value;
            }
        }

        public System.Collections.Generic.List<Location> direccionaUn;

        /// <summary>
        /// apiKey for collection of Location
        /// </summary>
        /// <pdGenerated>Default opposite class collection property</pdGenerated>
        public System.Collections.Generic.List<Location> DireccionaUn
        {
            get
            {
                if (direccionaUn == null)
                    direccionaUn = new System.Collections.Generic.List<Location>();
                return direccionaUn;
            }
            set
            {
                RemoveAllDireccionaUn();
                if (value != null)
                {
                    foreach (Location oLocation in value)
                        AddDireccionaUn(oLocation);
                }
            }
        }

        /// <summary>
        /// Add a new Location in the collection
        /// </summary>
        /// <pdGenerated>Default Add</pdGenerated>
        public void AddDireccionaUn(Location newLocation)
        {
            if (newLocation == null)
                return;
            if (this.direccionaUn == null)
                this.direccionaUn = new System.Collections.Generic.List<Location>();
            if (!this.direccionaUn.Contains(newLocation))
                this.direccionaUn.Add(newLocation);
        }

        /// <summary>
        /// Remove an existing Location from the collection
        /// </summary>
        /// <pdGenerated>Default Remove</pdGenerated>
        public void RemoveDireccionaUn(Location oldLocation)
        {
            if (oldLocation == null)
                return;
            if (this.direccionaUn != null)
                if (this.direccionaUn.Contains(oldLocation))
                    this.direccionaUn.Remove(oldLocation);
        }

        /// <summary>
        /// Remove all instances of Location from the collection
        /// </summary>
        /// <pdGenerated>Default removeAll</pdGenerated>
        public void RemoveAllDireccionaUn()
        {
            if (direccionaUn != null)
                direccionaUn.Clear();
        }
        public Datastreams datosDe;

    }
}