using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using srvIdexSemanticIoT;
using System.Text;

public partial class Controles_GridSensores : System.Web.UI.UserControl
{
    //Almacena la lista a ser presnetada en el datasource del grid
    private List<Feed> feedLista;

    public List<Feed> FeedLista
    {
        get { return feedLista; }
        set 
        { 
            feedLista = value;
            gridResultados.DataSource = value;
            gridResultados.DataBind();
        }
    }

    public DateTime FechaInicio
    {
        get { return Convert.ToDateTime(ViewState["fechaInicio"]); }
        set { ViewState["fechaInicio"] = value; hidfechaInicio.Value = value.ToString();}
    }

    public DateTime FechaFin
    {
        get { return Convert.ToDateTime(ViewState["fechaFin"]); }
        set { ViewState["fechaFin"] = value; hidFechaFin.Value = value.ToString();}
    }

    public string IDSensor
    {
        get { return Convert.ToString(ViewState["IDSensor"]); }
        set { ViewState["IDSensor"] = value;}
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        //RegistrarScript(gridResultados.ID, gridResultados.ID + "key");
    }

    protected void gvResultados_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
            return;

        Feed Sensor = (Feed)e.Row.DataItem;
        GridView gvDatastreams = (GridView)e.Row.FindControl("gvDatastreams");

        Dictionary<string, Datastream> ListaDts = new Dictionary<string, Datastream>();

        //Agregar el codigo del feed al datastream para facilitar su tratamiento
        for(int i=0; i < Sensor.datastreams.Length; i++)
        {
            Sensor.datastreams[i].feedid=Sensor.id;
        }

        gvDatastreams.DataSource = Sensor.datastreams;
        gvDatastreams.DataBind();

        IDSensor = Sensor.id;
    }
}