using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Drawing;
using System.Globalization;
using srvIdexSemanticIoT;

public partial class AnalizarContaminacion : System.Web.UI.Page
{
    private List<GeonameNode> Biotiponodos = new List<GeonameNode>();
    private string radio = string.Empty;
    private string idioma = string.Empty;
    private DateTime fechainicio;
    private DateTime fechaFin;
    private CultureInfo culture = CultureInfo.InvariantCulture;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Cargamos los datos de los biotipos
            if (Session["BiotiposNodos"] != null)
            {
                Biotiponodos = (Session["BiotiposNodos"] as List<GeonameNode>);
                foreach (GeonameNode nodo in Biotiponodos)
                {
                    ListItem li = new ListItem();

                    li.Value = nodo.Latitud + "," + nodo.Longitud;
                    li.Text = nodo.Nombre_lugar_Jerarquico;
                    drplLugares.Items.Add(li);
                }

                //Leer del biotipo seleccionado
                string valor = Convert.ToString(Request.QueryString["id"]);
                drplLugares.SelectedIndex = Convert.ToInt32(valor);

                //Establecer las preserencias del usuario
                if (Session["radio"] != null)
                {
                    radio = Session["radio"].ToString();
                    txtradio.Text = radio;
                }
                else
                {
                    radio = "100"; //Valor por defecto
                    Session["radio"] = radio;
                }
                idioma = (Session["idiomaBuscar"] as string);
                //Ajustamos las fechas de analisis de datos para el dia presente por defecto separadas por 6 horas
                DateTime ahora = DateTime.Now;
                AsignarRangoFechas(ahora.AddHours(-6), ahora);

                //Iniciamos la busqueda de sesnsores
                BuscaryAnalizarBiotipo();
            }
            else
            {
                Response.Write("No puede utilizar esta página hasta que no realice una consulta");
            }
        }
        else
        {
            radio = Session["radio"].ToString();
            idioma = (Session["idiomaBuscar"] as string);
            Biotiponodos = (Session["BiotiposNodos"] as List<GeonameNode>);
        }
    }

    //Cuando seleccione un biotipo en el drop, se debe buscar toida la información relacionada
    protected void drplLugares_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Se analiza el Biotipo actual
        BuscaryAnalizarBiotipo();
    }

    protected void txtradio_TextChanged(object sender, EventArgs e)
    {
        Session["radio"] = txtradio.Text;
    }

    protected void btnBuscarSensores_Click(object sender, EventArgs e)
    {
        BuscaryAnalizarBiotipo();
    }

    private void MostrarBiotipo(Biotipo biotipo)
    {
        //Cargando las fechas
        LeerRangoFechas();

        //Almacenar las variables temporales
        List<Variable> variables = new List<Variable>();

        //Colocar los datos generales
        lblBiotipoId.Text = biotipo.GeonameId;
        lblNombre.Text = biotipo.name;
        lblPais.Text = biotipo.CountryName;
        lblLatitud.Text = biotipo.AreaImpacto.lat.ToString();
        lblLongitud.Text = biotipo.AreaImpacto.lon.ToString();
        lblArea.Text = biotipo.AreaImpacto.Area.ToString();

        //Asignar los datasets de sensores de cada característica
        //Contaminación Aire
        foreach (Variable varCaracteristica in biotipo.ContaminacionAire.Variables)
        {
            Literal lit = new Literal();
            lit.Text = CrearEncabezadoVariable(varCaracteristica);
            pnAire.Controls.Add(lit);

            //Obtenemos el arreglo de feeds
            List<Feed> feeds = new List<Feed>();
            foreach (FeedXively feedx in varCaracteristica.sensores)
            {
                feeds.Add(feedx.feed);
            }

            //Creando el control grid para presentar los resultados
            Control gridSen = LoadControl("Controles/GridSensores.ascx");
            ((Controles_GridSensores)gridSen).ID = varCaracteristica.Id.Replace(" ","_");
            ((Controles_GridSensores)gridSen).FeedLista = feeds;
            ((Controles_GridSensores)gridSen).FechaInicio = fechainicio;
            ((Controles_GridSensores)gridSen).FechaFin = fechaFin;
            pnAire.Controls.Add(gridSen);
        }

        //Contaminación Suelo
        foreach (Variable varCaracteristica in biotipo.ContaminacionSuelo.Variables)
        {
            Literal lit = new Literal();
            lit.Text = CrearEncabezadoVariable(varCaracteristica);
            pnSuelo.Controls.Add(lit);

            List<Feed> feeds = new List<Feed>();
            foreach (FeedXively feedx in varCaracteristica.sensores)
            {
                feeds.Add(feedx.feed);
            }

            //Creando el control grid para presentar los resultados
            Control gridSen = LoadControl("Controles/GridSensores.ascx");
            ((Controles_GridSensores)gridSen).ID = varCaracteristica.Id.Replace(" ", "_");
            ((Controles_GridSensores)gridSen).FeedLista = feeds;
            ((Controles_GridSensores)gridSen).FechaInicio = fechainicio;
            ((Controles_GridSensores)gridSen).FechaFin = fechaFin;
            pnSuelo.Controls.Add(gridSen);
        }

        //Contaminación Agua
        foreach (Variable varCaracteristica in biotipo.ContaminacionAgua.Variables)
        {
            Literal lit = new Literal();
            lit.Text = CrearEncabezadoVariable(varCaracteristica);
            pnAgua.Controls.Add(lit);

            //Obtenemos el arreglo de feeds
            List<Feed> feeds = new List<Feed>();
            foreach (FeedXively feedx in varCaracteristica.sensores)
            {
                feeds.Add(feedx.feed);
            }

            //Creando el control grid para presentar los resultados
            Control gridSen = LoadControl("Controles/GridSensores.ascx");
            ((Controles_GridSensores)gridSen).ID = varCaracteristica.Id.Replace(" ", "_");
            ((Controles_GridSensores)gridSen).FeedLista = feeds;
            ((Controles_GridSensores)gridSen).FechaInicio = fechainicio;
            ((Controles_GridSensores)gridSen).FechaFin = fechaFin;
            pnAgua.Controls.Add(gridSen);
        }

        //Contaminación Sonora
        foreach (Variable varCaracteristica in biotipo.ContaminacionSonora.Variables)
        {
            Literal lit = new Literal();
            lit.Text = CrearEncabezadoVariable(varCaracteristica);
            pnSonora.Controls.Add(lit);

            //Obtenemos el arreglo de feeds
            List<Feed> feeds = new List<Feed>();
            foreach (FeedXively feedx in varCaracteristica.sensores)
            {
                feeds.Add(feedx.feed);
            }

            //Creando el control grid para presentar los resultados
            Control gridSen = LoadControl("Controles/GridSensores.ascx");
            ((Controles_GridSensores)gridSen).ID = varCaracteristica.Id.Replace(" ", "_");
            ((Controles_GridSensores)gridSen).FeedLista = feeds;
            ((Controles_GridSensores)gridSen).FechaInicio = fechainicio;
            ((Controles_GridSensores)gridSen).FechaFin = fechaFin;
            pnSonora.Controls.Add(gridSen);
        }

        //Contaminación Térmica
        foreach (Variable varCaracteristica in biotipo.ContaminacionTermica.Variables)
        {
            Literal lit = new Literal();
            lit.Text = CrearEncabezadoVariable(varCaracteristica);
            pnTermica.Controls.Add(lit);

            //Obtenemos el arreglo de feeds
            List<Feed> feeds = new List<Feed>();
            foreach (FeedXively feedx in varCaracteristica.sensores)
            {
                feeds.Add(feedx.feed);
            }

            //Creando el control grid para presentar los resultados
            Control gridSen = LoadControl("Controles/GridSensores.ascx");
            ((Controles_GridSensores)gridSen).ID = varCaracteristica.Id.Replace(" ", "_");
            ((Controles_GridSensores)gridSen).FeedLista = feeds;
            ((Controles_GridSensores)gridSen).FechaInicio = fechainicio;
            ((Controles_GridSensores)gridSen).FechaFin = fechaFin;
            pnTermica.Controls.Add(gridSen);
        }

        //Contaminación Visual
        foreach (Variable varCaracteristica in biotipo.ContaminacionVisual.Variables)
        {
            Literal lit = new Literal();
            lit.Text = CrearEncabezadoVariable(varCaracteristica);
            pnVisual.Controls.Add(lit);

            //Obtenemos el arreglo de feeds
            List<Feed> feeds = new List<Feed>();
            foreach (FeedXively feedx in varCaracteristica.sensores)
            {
                feeds.Add(feedx.feed);
            }

            //Creando el control grid para presentar los resultados
            Control gridSen = LoadControl("Controles/GridSensores.ascx");
            ((Controles_GridSensores)gridSen).ID = varCaracteristica.Id.Replace(" ", "_");
            ((Controles_GridSensores)gridSen).FeedLista = feeds;
            ((Controles_GridSensores)gridSen).FechaInicio = fechainicio;
            ((Controles_GridSensores)gridSen).FechaFin = fechaFin;
            pnVisual.Controls.Add(gridSen);
        }
        
    }

    private string CrearEncabezadoVariable(Variable variable)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("<br/>");
        sb.Append("<h3>" + variable.Id + "</h3>");
        sb.Append("<p>" + variable.Maximo + "</p>");
        sb.Append("<p>" + variable.Minimo  + "</p>");
        sb.Append("<p>" + variable.Normal  + "</p>");
        return sb.ToString();
    }

    private void BuscaryAnalizarBiotipo()
    {
        //Proxy de negocio de biotipo
        BiotipoManager bm = new BiotipoManager();

        //Obtener los datos y sensores del Biotipo
        Biotipo biotipo = bm.ObtenerInfoBiotipoContaminacion(Biotiponodos[drplLugares.SelectedIndex], radio, idioma);

        MostrarBiotipo(biotipo);
    }

    private void AsignarRangoFechas(DateTime fInicio, DateTime fFin)
    {
        //Definimos el formato de parseo que es mismo del datepicker
        string formatHora = "MM/dd/yyyy";
        string formatMinutos= "HH:mm:ss";

        //Asignar las fechas y horas a los controles
        txtfrom.Text = fInicio.ToString(formatHora);
        txtHoraInicio.Text = fInicio.ToString(formatMinutos);
        txtto.Text = fFin.ToString(formatHora);
        txtHoraFin.Text = fFin.ToString(formatMinutos);
    }

    private void LeerRangoFechas()
    {
        //Definimos el formato de parseo que es mismo del datepicker
        string format = "MM/dd/yyyy HH:mm:ss";
        string dateInicioString = txtfrom.Text + " " + txtHoraInicio.Text;
        string dateFinString = txtto.Text + " " + txtHoraFin.Text;

        //Lee las fechas almacenadas en los controles
        fechainicio = DateTime.ParseExact(dateInicioString, format, culture);
        fechaFin = DateTime.ParseExact(dateFinString, format, culture);
    }
    protected void txtfrom_TextChanged(object sender, EventArgs e)
    {
        LeerRangoFechas();
    }
    protected void txtHoraInicio_TextChanged(object sender, EventArgs e)
    {
        LeerRangoFechas();
    }
    protected void txtto_TextChanged(object sender, EventArgs e)
    {
        LeerRangoFechas();
    }
    protected void txtHoraFin_TextChanged(object sender, EventArgs e)
    {
        LeerRangoFechas();
    }
}