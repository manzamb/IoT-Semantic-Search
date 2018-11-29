// File:    Externo.cs
// Author:  manzamb
// Created: jueves, 23 de enero de 2014 08:11:37 p.m.
// Purpose: Definition of Class Externo

using System;
namespace ObjetoSemantico
{
    public class Externo : Metodos
    {
        /// Envía la información de las propiedades de estado del obj semántico
        public int EnviarEstado()
        {
            throw new NotImplementedException();
        }

        /// Envía los Datos de las mediciones (dtastreams). Pueden crearse varios con fecha específicas o rango de fechas
        public int EnviarDatos()
        {
            throw new NotImplementedException();
        }

        /// Devuelve el identificador del obj semántico.
        public int EnviarIdentificador()
        {
            throw new NotImplementedException();
        }

        /// Recive un comando REST para modificar el estado del obj
        public int RecibeComando()
        {
            throw new NotImplementedException();
        }

        /// Retorna información de las url de conocimiento asociadas al obj semántico
        public int InviarInformacion()
        {
            throw new NotImplementedException();
        }

    }
}