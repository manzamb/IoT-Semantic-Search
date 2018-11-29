using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Biotipo
/// </summary>
public class Biotipo
{
    public string GeonameId { get; set; }
    public string name { get; set; }
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public AreaImpacto AreaImpacto { get; set; }
    public Edafotopo Edafotopo { get; set; }
    public Hidrotopo Hidrotopo { get; set; }
    public Climatopo Climatopo { get; set; }
    public ContaminacionAire ContaminacionAire { get; set; }
    public ContaminacionSuelo ContaminacionSuelo  { get; set; }
    public ContaminacionAgua ContaminacionAgua  { get; set; }
    public ContaminacionSonora ContaminacionSonora  { get; set; }
    public ContaminacionTermica ContaminacionTermica  { get; set; }
    public ContaminacionVisual ContaminacionVisual  { get; set; }

    public Biotipo()
	{
        AreaImpacto = new AreaImpacto();
        Edafotopo = new Edafotopo();
        Hidrotopo = new Hidrotopo();
        Climatopo = new Climatopo();
        ContaminacionAire = new ContaminacionAire();
        ContaminacionSuelo = new ContaminacionSuelo();
        ContaminacionAgua = new ContaminacionAgua();
        ContaminacionSonora = new ContaminacionSonora();
        ContaminacionTermica = new ContaminacionTermica();
        ContaminacionVisual = new ContaminacionVisual();
	}
}