// File:    Conocimientos.cs
// Author:  Miguel Angel Niño Za
// Created: lunes, 23 de junio de 2003 10:21:08 a.m.
// Purpose: Definition of Class Conocimientos

using System;

namespace ObjetoSemantico
{
    /// Las URL pueden ser recursos web en los cuales se puede obtener información adicional sobre el conocimiento del obj. Sin embargo en una solución posterior serán las ontologías que a traves de solicitudes de servidio responden preguntas del obj
    public class Conocimientos
    {
        private String urlContexto;
        private String urlServicio;
        private String urlUsuario;
        private DContexto tipoContexto;

        public String UrlContexto
        {
            get
            {
                return urlContexto;
            }
            set
            {
                this.urlContexto = value;
            }
        }

        public String UrlServicio
        {
            get
            {
                return urlServicio;
            }
            set
            {
                this.urlServicio = value;
            }
        }

        public String UrlUsuario
        {
            get
            {
                return urlUsuario;
            }
            set
            {
                this.urlUsuario = value;
            }
        }

        public DContexto TipoContexto
        {
            get
            {
                return tipoContexto;
            }
            set
            {
                this.tipoContexto = value;
            }
        }

    }
}