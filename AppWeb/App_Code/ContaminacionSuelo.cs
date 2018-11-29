using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;

public class ContaminacionSuelo
{
    public List<Variable> Variables { get; set; }

    public ContaminacionSuelo()
    {
        Variables = new List<Variable>();
    }
}
