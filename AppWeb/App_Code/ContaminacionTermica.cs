using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;

public class ContaminacionTermica
{
    public List<Variable> Variables { get; set; }

    public ContaminacionTermica()
    {
        Variables = new List<Variable>();
    }
}
