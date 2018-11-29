using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using srvIdexSemanticIoT;

/// <summary>
/// Descripción breve de Variable
/// </summary>
public class Variable
{
    public string Id { get; set; }
    public string Normal { get; set; }
    public string Minimo { get; set; }
    public string Maximo { get; set; }
    public Unit unit { get; set; }
    public Periodo periodo { get; set; }
    public List<FeedXively> sensores;

    public Variable()
	{
        unit = new Unit();
        periodo = new Periodo();
        sensores = new List<FeedXively>();
	}
}