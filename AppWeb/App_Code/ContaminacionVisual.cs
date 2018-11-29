using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;

public class ContaminacionVisual
{
    public List<Variable> Variables { get; set; }

    public ContaminacionVisual()
    {
        Variables = new List<Variable>();
    }
}
