// File:    Estado.cs
// Author:  manzamb
// Created: miércoles, 22 de enero de 2014 03:40:02 p.m.
// Purpose: Definition of Class Estado

using System;
namespace ObjetoSemantico
{
    public sealed class Estado : Propiedades
    {
        private String _title;
        private String _private;
        private String _tags;
        private String _description;
        private String _feed;
        private String _status;
        private DateTime  _updated;
        private DateTime _created;
        private String _website;
        private String _email;
        private String _creator;
        private String _version;

        public String title
        {
            get
            {
                return _title;
            }
            set
            {
                this._title = value;
            }
        }

        public String tags
        {
            get
            {
                return _tags;
            }
            set
            {
                this._tags = value;
            }
        }

        public String description
        {
            get
            {
                return _description;
            }
            set
            {
                this._description = value;
            }
        }

        public String feed
        {
            get
            {
                return _feed;
            }
            set
            {
                this._feed = value;
            }
        }

        public String status
        {
            get
            {
                return _status;
            }
            set
            {
                this._status = value;
            }
        }

        public DateTime updated
        {
            get
            {
                return _updated;
            }
            set
            {
                this._updated = value;
            }
        }

        public DateTime created
        {
            get
            {
                return _created;
            }
            set
            {
                this._created = value;
            }
        }

        public String website
        {
            get
            {
                return _website;
            }
            set
            {
                this._website = value;
            }
        }

        public String email
        {
            get
            {
                return _email;
            }
            set
            {
                this._email = value;
            }
        }

        public String creator
        {
            get
            {
                return _creator;
            }
            set
            {
                this._creator = value;
            }
        }

        public String version
        {
            get
            {
                return _version;
            }
            set
            {
                this._version = value;
            }
        }

        public String Privado
        {
            get
            {
                return _private;
            }
            set
            {
                this._private = value;
            }
        }

    }
}