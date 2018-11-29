// File:    Location.cs
// Author:  manzamb
// Created: jueves, 23 de enero de 2014 06:06:29 p.m.
// Purpose: Definition of Class Location

using System;
namespace ObjetoSemantico
{
    public class Location
    {
        private String name;
        private String domain;
        private String ele;
        private String lat;
        private String lon;

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }

        public String Domain
        {
            get
            {
                return domain;
            }
            set
            {
                this.domain = value;
            }
        }

        public String Ele
        {
            get
            {
                return ele;
            }
            set
            {
                this.ele = value;
            }
        }

        public String Lat
        {
            get
            {
                return lat;
            }
            set
            {
                this.lat = value;
            }
        }

        public String Lon
        {
            get
            {
                return lon;
            }
            set
            {
                this.lon = value;
            }
        }

    }
}