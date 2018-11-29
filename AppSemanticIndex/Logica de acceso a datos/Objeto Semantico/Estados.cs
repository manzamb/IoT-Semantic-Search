// File:    Estados.cs
// Author:  Miguel Angel Niño Za
// Created: lunes, 23 de junio de 2003 10:21:08 a.m.
// Purpose: Definition of Class Estados

using System;
namespace ObjetoSemantico
{
    public class Estados
    {        
        public String _version;
        public DStatus _status;
        public DRole _role;
        public String _entity;
        public DateTime _date;
        public DEstPlan _estadoPlanificacion;

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

        public DStatus status
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

        public DRole Role
        {
            get
            {
                return _role;
            }
            set
            {
                this._role = value;
            }
        }

        public String entity
        {
            get
            {
                return _entity;
            }
            set
            {
                this._entity = value;
            }
        }

        public DateTime date
        {
            get
            {
                return _date;
            }
            set
            {
                this._date = value;
            }
        }

        public DEstPlan estadoPlanificacion
        {
            get
            {
                return _estadoPlanificacion;
            }
            set
            {
                this._estadoPlanificacion = value;
            }
        }
    }
}