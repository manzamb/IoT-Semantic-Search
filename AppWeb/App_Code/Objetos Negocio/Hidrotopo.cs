using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Hidrotopo
/// </summary>
public class Hidrotopo
{
    public List<Variable> Variables { get; set; }

    public Hidrotopo()
	{
        Variables = new List<Variable>();
	}
}