using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using srvIdexSemanticIoT;

/// <summary>
/// Descripción breve de wsDataPoints
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
[System.Web.Script.Services.ScriptService]
public class wsDataPoints : System.Web.Services.WebService {

    public wsDataPoints () {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public string RetornarDatapointsFeed(string feedID, string DatastreamId, string fechaInicio, string fechaFin)
    {
        //Crea el Objeto que guarda todo el conocimiento del negocio
        WSSemanticSearch srvIndex = new WSSemanticSearch();

        //Obtener el JSON de la BDD
        string datapoints = srvIndex.RetornarDatapointsFeed(feedID, DatastreamId, fechaInicio, fechaFin);
        return datapoints;
    }
    
}
