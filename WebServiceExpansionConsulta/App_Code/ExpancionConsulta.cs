using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using ModeloSemantico_PU.ProcesamientoConsulta;
using ModeloSemantico_PU.ObjetosNegocio;

/// <summary>
/// Descripción breve de ExpancionConsulta
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class ExpancionConsulta : System.Web.Services.WebService {

    public ExpancionConsulta () {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string ExpandirConsulta(string CaminoOntologia, string consulta, string tipoAnalizador) 
    {
        //Expansion de consulta con MSEC
        OntologiaDominio ontologia = new OntologiaDominio();
        ontologia.readOntology(CaminoOntologia);

        //Creamos el objeto que permite expandir la consulta
        ExpansionHandler eh = new ExpansionHandler();

        //Finalmente se expande la consulta
        string expandedQuery = eh.expandirConsulta(consulta, ontologia.Model, tipoAnalizador, CaminoOntologia);

        return expandedQuery;
    }

    [WebMethod]
    public string CargarConceptos(string CaminoOntologia)
    {
        try
        {
            OntologiaDominio ontologia = new OntologiaDominio();
            ontologia.readOntology(CaminoOntologia);

            //Creamos el objeto que permite expandir la consulta
            ExpansionHandler eh = new ExpansionHandler();

            //Se copian los conceptos para la BD
            eh.Conceptos = OntologiaDominio.ObtenerConceptosOntologia(CaminoOntologia);
            ontologia.AlamcenarConceptos(eh.Conceptos);

            //Retornamos los conceptos almacenados
            return "Conceptos Cargados Correctamente";
        }
        catch (Exception ex)
        {
            return ("No se pudo cargar los conceptos el error es: " + ex.Message);
        }
    }

    [WebMethod]
    public string RetornarConceptosOntologia(string CaminoOntologia, string consulta, string tipoAnalizador)
    {
        //Expansion de consulta con MSEC
        OntologiaDominio ontologia = new OntologiaDominio();
        ontologia.readOntology(CaminoOntologia);

        //Creamos el objeto que permite expandir la consulta
        ExpansionHandler eh = new ExpansionHandler();

        //Finalmente se expande la consulta
        List<string> lstConceptos = new List<string>();
        lstConceptos = eh.RetornarConceptosOntologia(consulta, ontologia.Model, tipoAnalizador, CaminoOntologia);

        //Retornar la lista serializada
        return lstConceptos.SerializarToXml();
    }
    
}
