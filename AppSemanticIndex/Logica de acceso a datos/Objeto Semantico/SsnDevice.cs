// File:    SsnDevice.cs
// Author:  manzamb
// Created: jueves, 23 de enero de 2014 05:03:20 p.m.
// Purpose: Definition of Class SsnDevice

using System;
namespace ObjetoSemantico
{
    public class SsnDevice
    {
        private String DeviceSerial;

        public String _DeviceSerial
        {
            get
            {
                return DeviceSerial;
            }
            set
            {
                this.DeviceSerial = value;
            }
        }

    }
}