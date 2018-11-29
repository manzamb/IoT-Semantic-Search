// File:    Datastreams.cs
// Author:  manzamb
// Created: jueves, 23 de enero de 2014 06:06:30 p.m.
// Purpose: Definition of Class Datastreams

using System;

namespace ObjetoSemantico
{
    public class Datastreams
    {
        private String datastreamId;
        private String currentValue;
        private String at;
        private String maxValue;
        private String minValue;
        private String tags;

        public String DatastreamId
        {
            get
            {
                return datastreamId;
            }
            set
            {
                this.datastreamId = value;
            }
        }

        public String CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                this.currentValue = value;
            }
        }

        public String At
        {
            get
            {
                return at;
            }
            set
            {
                this.at = value;
            }
        }

        public String MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                this.maxValue = value;
            }
        }

        public String MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                this.minValue = value;
            }
        }

        public String Tags
        {
            get
            {
                return tags;
            }
            set
            {
                this.tags = value;
            }
        }

        public System.Collections.Generic.List<KosUnit> define;

        /// <summary>
        /// apiKey for collection of KosUnit
        /// </summary>
        /// <pdGenerated>Default opposite class collection property</pdGenerated>
        public System.Collections.Generic.List<KosUnit> Define
        {
            get
            {
                if (define == null)
                    define = new System.Collections.Generic.List<KosUnit>();
                return define;
            }
            set
            {
                RemoveAllDefine();
                if (value != null)
                {
                    foreach (KosUnit oKosUnit in value)
                        AddDefine(oKosUnit);
                }
            }
        }

        /// <summary>
        /// Add a new KosUnit in the collection
        /// </summary>
        /// <pdGenerated>Default Add</pdGenerated>
        public void AddDefine(KosUnit newKosUnit)
        {
            if (newKosUnit == null)
                return;
            if (this.define == null)
                this.define = new System.Collections.Generic.List<KosUnit>();
            if (!this.define.Contains(newKosUnit))
                this.define.Add(newKosUnit);
        }

        /// <summary>
        /// Remove an existing KosUnit from the collection
        /// </summary>
        /// <pdGenerated>Default Remove</pdGenerated>
        public void RemoveDefine(KosUnit oldKosUnit)
        {
            if (oldKosUnit == null)
                return;
            if (this.define != null)
                if (this.define.Contains(oldKosUnit))
                    this.define.Remove(oldKosUnit);
        }

        /// <summary>
        /// Remove all instances of KosUnit from the collection
        /// </summary>
        /// <pdGenerated>Default removeAll</pdGenerated>
        public void RemoveAllDefine()
        {
            if (define != null)
                define.Clear();
        }

    }
}