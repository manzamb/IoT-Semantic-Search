using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Climatopo
/// </summary>
public class Climatopo
{
    public List<Variable> Variables { get; set; }

    public Climatopo()
	{
        Variables = new List<Variable>();
	}
}