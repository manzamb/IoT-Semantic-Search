using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using srvIdexSemanticIoT;
using System.IO;

public partial class SemanticIndexResult : System.Web.UI.Page
{
    //Porpiedad de la página para manejar la consulta    
    public string consulta { get; set; }

    #region "Manejo de Eventos de la Página"

    //Establece si indexa y la primera consulta del usuario
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Recupera la consulta realizada por el usuario
            TxtConsulta.Text = (Session["QUERRY_TEXT"] == null) ? null : Session["QUERRY_TEXT"].ToString();
            this.consulta = TxtConsulta.Text;

            //Establecer en los controles el valor actual del idioma preferido de búsqueda
            rdblistIdioma.Items.FindByValue((Session["idiomaBuscar"] as string)).Selected = true;

            //Posteriormente realiza la consulta al índice
            BuscarSemanticIndex();
        }
        else
        {
            //Establecer en los controles el valor actual
            rdblistIdioma.Items.FindByValue(rdblistIdioma.SelectedItem.Text).Selected = true;
            this.consulta = TxtConsulta.Text;
        }
        Session["idiomaBuscar"] = rdblistIdioma.SelectedItem.Text;
        RegistrarScript();
        //Cambiar la aparuencia dependiendo si el mapa esta visible
        if (pnMapa.Visible)
        {
            Resultados.Style.Clear();
            Resultados.Style.Add("max-height", "400px");
            Resultados.Style.Add("border", "thin solid #000000");
            Resultados.Style.Add("width", "980px");
            Resultados.Style.Add("overflow", "scroll");
        }
        else
        {
            Resultados.Style.Clear();
            Resultados.Style.Add("width", "980px");
        }
        //Mensaje de espera predeterminado
        //(udpProgreso.FindControl("LitEspera") as Literal).Text = "Cargando...";
    }

    //El usuario realiza otra busqueda si lo desea
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        //Obtenemos la consulta del usuario
        this.consulta = TxtConsulta.Text;

        //Se almacena la consulta para su uso posterior en la aplicación
        Session["QUERRY_TEXT"] = consulta;

        //El usuario decide consultar
        BuscarSemanticIndex();
    }

    //Almacena la selección de idioma prefderido
    protected void rdblistIdioma_SelectedIndexChanged(object sender, EventArgs e)
    {
        rdblistIdioma.Items.FindByValue(rdblistIdioma.SelectedItem.Text).Selected = true;
        Session["idiomaBuscar"] = rdblistIdioma.SelectedItem.Text;
    }

    //Cuando ecoge un nuevo biotipo, selecciona los sensores correspondientes
    protected void drplLugares_SelectedIndexChanged(object sender, EventArgs e)
    {
        MostrarBiotipoenMapa();
    }

    //Si ha cambiado el radio o desea refrescar el geoposicionamiento
    protected void btnBuscarSensores_Click(object sender, EventArgs e)
    {
        MostrarBiotipoenMapa();
    }

    //Presenta el Mapa y las opciones de Geolocalización
    protected void btnGeopocisionar_Click(object sender, EventArgs e)
    {
        GeoposicionarElementos();
    }

    //Oculta el Mapa para acelerar la búsqueda
    protected void btnCerrarMapa_Click(object sender, EventArgs e)
    {
        pnMapa.Visible = false;
        btnGeopocisionar.Visible = true;
        btnCerrarMapa.Visible = false;
        //Presentamos el resultado de la consulta general original
        MostrarResumenResultadosFeed((List<FeedXively>)Session["ResultadosFeedGeneral"]);
        Resultados.Style.Clear();
        Resultados.Style.Add("width", "980px");
    }


    protected void gvResultados_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
            return;

        Feed Sensor = (Feed)e.Row.DataItem;

        GridView gvDatastreams = (GridView)e.Row.FindControl("gvDatastreams");

        gvDatastreams.DataSource = Sensor.datastreams;
        gvDatastreams.DataBind();
    }


    #endregion

    #region "Funciones Auxiliares de la Página"

    //Realiza la búsqueda al Índice Semántico y la presenta al usuario
    private void BuscarSemanticIndex()
    {
        //Definir el objeto de negocio de la aplicación
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

        //Verificar si el buscador esta disponible
        if (sm.ObtenerConfiguracion("BusquedaBloqueada") == "No")
        {
            //Calculamos el tiempo de demora en el proceso
            DateTime inicio = DateTime.Now;
      
            //Realizamos el proceso de expanción implícito
            string consultaexpandida = sm.ExpandirConsulta(TxtConsulta.Text, rdblistIdioma.SelectedItem.Text);

            //Obtener el resultado de la busqueda y presentarlo en la pagina
            List<FeedXively> FeedResultados = sm.BuscarFeeds(consultaexpandida, rdblistIdioma.SelectedItem.Text);

            Session["ConsultaExpandida"] = consultaexpandida;

            MostrarResumenResultadosFeed(FeedResultados);
            //MostrarResumenResultados(sm.Buscar(TxtConsulta.Text, rdblistIdioma.SelectedItem.Text));

            //Si el mapa esta visible, geoposicionar
            if(pnMapa.Visible)
                GeoposicionarElementos();

            //Tiempo final
            DateTime fin = DateTime.Now;

            //Mostramos el resultado
            TimeSpan intervalo = new TimeSpan(fin.Ticks - inicio.Ticks);
            LblResultados.Text += " - Tiempo Consulta: " + ((float)intervalo.TotalMilliseconds/1000).ToString() + " segundos";
        }
        else
            LblResultados.Text += "En este momento el buscador esta realizando indexación. Por favor intente más tarde.";

    }

    //Busca los Biotipos y Geolocaliza los sensores relacionados
    private void GeoposicionarElementos()
    {
        //Calculamos el tiempo de demora en el proceso
        DateTime inicio = DateTime.Now;

        //Mostramos el mapa
        pnMapa.Visible = true;
        //btnGeopocisionar.Visible = false;
        //btnCerrarMapa.Visible = true;
      
        //Borrar los valores anteriores del drop
        drplLugares.Items.Clear();

        //Obtenemos la lista de lugares posibles a partir de la consulta
        List<GeonameNode> geonodes = new List<GeonameNode>();
        List<GeonameNode> geonodesinv = new List<GeonameNode>();
        //Copia el arreglo de lugares encontrados a partir de la consulta original
        //foreach (GeonameNode geonode in srvIndex.ObtenerCiudadesConsulta(Consulta))
        //{
        //    geonodes.Add(geonode);
        //}

        List<FeedXively> dtsResultados = (Session["ResultadosFeedGeneral"] as List<FeedXively>);

        //Añadir los lugares que estan en los resultados retornados
        geonodes = BuscarBiotiposConsulta(dtsResultados, geonodes);

        if (geonodes != null)
        {
            //Llenar el dropdown con los lugares encontrados en la consulta y en los resultados
            foreach (GeonameNode geonode in geonodes)
            {
                ListItem li = new ListItem();
                string nombrelugar = string.Empty;

                //Invertir jerarquia y obtener un string que ubique google maps facilmente
                if (!string.IsNullOrEmpty(geonode.Nombre_lugar_Jerarquico))
                    nombrelugar = ObtenerNombreBuscarMapa(geonode.Nombre_lugar_Jerarquico);
                else
                    nombrelugar = geonode.Nombre_lugar;   //Nombres de la consulta

                li.Value = geonode.Latitud + "," + geonode.Longitud;
                li.Text = nombrelugar;
                drplLugares.Items.Add(li);
                //Almacenar copia con los cambios
                geonode.Nombre_lugar_Jerarquico = nombrelugar;
                geonodesinv.Add(geonode);

            }
            //Guardar en una variable de sesion los nodos para mapas
            Session["BiotiposNodos"] = geonodesinv;
        }
        else
            Session["BitoiposNodos"] = null;
        
        //Finalmente se presenta el primer Biotipo detectado
        MostrarBiotipoenMapa();

        //Tiempo final
        DateTime fin = DateTime.Now;

        //Mostramos el resultado
        TimeSpan intervalo = new TimeSpan(fin.Ticks - inicio.Ticks);
        LblResultados.Text += " - Tiempo Consulta: " + ((float)intervalo.TotalMilliseconds / 1000).ToString() + " segundos";
    }

    //Permite presentar el Biotipo seleccionado en el mapa
    private void MostrarBiotipoenMapa()
    {
        string[] lugar = drplLugares.SelectedValue.Split(',');
        //Damos formato a la cadena para una mejor visualización
        latlng.Text = string.Format("Lat: {0}, Lng: {1}", lugar[0], lugar[1]);

        //Filtrar los resultados por los más cercanos al lugar escogido
        //Definir el objeto del negocio de la aplicación
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

        //Recupoeramos los resultados actuales de la consulta
        List<FeedXively> dtsResultados = (Session["ResultadosFeedGeneral"] as List<FeedXively>);

        //El método retorna los sensores que cumplen la condicion de cercanía
        List<FeedXively> dtsGeopos = sm.RetornarMapaLugarListaSensores(lugar[0].ToString(),
                                                              lugar[1].ToString(),
                                                              dtsResultados, rdblistIdioma.SelectedItem.Text, txtradio.Text);
        //Cargamos el datgrid con los sensores correspondientes
        MostrarResumenResultadosFeed(dtsGeopos);

        //Los marcadores son ubicados mediante javascript en el script del page load
    }

    //Busca los Biotipos que estan relacionados a los sensores retornados en la consulta
    private List<GeonameNode> BuscarBiotiposConsulta(List<FeedXively> dts, List<GeonameNode> geonodes)
    {
        //Definir el objeto del negocio
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

        //Variables temporales
        Boolean encontrado = false;
        string ciudadesadicionales = string.Empty;
        List<GeonameNode> geonodestmp = new List<GeonameNode>();

        //Recorrer el dataset para la busqueda de nuevos lugares
        foreach (FeedXively dtr in dts)
        {
            GeonameNode gn = new GeonameNode();

            //En caso de no tener Coordenadas Descartarlo
            if (dtr.feed.location != null)
            {
                //Leemos la localización de registro correspondiente
                gn.Nombre_lugar = "";  //Este lugar hay que averiguarlo con el Servicio Web enviando las coordenadas
                gn.Nombre_lugar_usuario = dtr.feed.location.name;
                gn.Latitud = dtr.feed.location.lat;
                gn.Longitud = dtr.feed.location.lon;

                geonodestmp.Add(gn);
            }
        }

        //Obtenemos los Biotipos (lugares) encontrados
        List<GeonameNode> gns = sm.ObtenerBiotiposConsulta(geonodestmp);
        if (gns != null)
        {
            foreach (GeonameNode gn in gns)
            {
                geonodestmp.Add(gn);
            }

            //Añadimos los nuevos lugares a la lista actual
            foreach (GeonameNode geonode in geonodestmp)
            {
                //Verificamos si los nuevos lugares ya fueron detectados en la consulta
                foreach (GeonameNode geon in geonodes)
                    if (geon.Nombre_lugar_Jerarquico == geonode.Nombre_lugar_Jerarquico)
                    {
                        encontrado = true;
                        break;
                    }

                //Si no estaba el lugar adicionarlo, verificando nulos
                if (!encontrado && !string.IsNullOrEmpty(geonode.Nombre_lugar_Jerarquico))
                    geonodes.Add(geonode);
                encontrado = false;
            }

            return geonodes;
            
        }
        return null;
    }

    //Invierte el Nombre retornado para una mejor lectura por el Usuario
    private string ObtenerNombreBuscarMapa(string nobrejerarquico)
    {
        string[] nombrejterminos = ObtenerTerminos(nobrejerarquico);
        string nombrej = string.Empty;

        //Al quitarle 3 eliminamos Earth y el continente
        for (int i = nombrejterminos.Count() - 1; i > 1; i--)
        {
            if (string.IsNullOrEmpty(nombrej))
                nombrej = nombrejterminos[i];
            else
                nombrej += " , " + nombrejterminos[i];
        }

        return nombrej;
    }

    //Obtiene un arreglo de cadenas a partir de una cadena con términos separados por comas
    private string[] ObtenerTerminos(string cadena)
    {
        char[] delimitadores = { ',' };

        string[] terminos = cadena.Split(delimitadores);

        return terminos;
    }

    //Registra el JavaScript que implementa toda la funcionalidad con el Mapa
    private void RegistrarScript()
    {
        const string ScriptKey = "ScriptKey";
        if (!ClientScript.IsStartupScriptRegistered(this.GetType(), ScriptKey))
        {
            StringBuilder fn = new StringBuilder();
            //fn.Append("function fnAceptar() { ");
            fn.Append("codeLatLng();");
            //fn.Append("}");
            //ClientScript.RegisterStartupScript(this.GetType(), ScriptKey, fn.ToString(), true);
            ScriptManager.RegisterStartupScript(this, typeof(Page), ScriptKey, fn.ToString(), true);
        }
    }

    //Presneta los resultados de la búsqueda
    private void MostrarResumenResultadosFeed(List<FeedXively> ResultadosFeed)
    {
        //Establecer la consulta hecha
        string Consulta = string.Empty;

        if(Session["consultaexpandida"] != null)
            Consulta = Session["consultaexpandida"].ToString();
        else
            Consulta = TxtConsulta.Text;

        //Obtenemos el arreglo de feeds
        List<Feed> feeds = new List<Feed>();

        foreach (FeedXively feedx in ResultadosFeed)
        {
            feeds.Add(feedx.feed);
        }

        gridResultados.DataSource = feeds;
        gridResultados.DataBind();

        //Generar un SesnsorRangking para mejorar los primeros resultados


        //Contar totales de resultados y habilitar mapa
        if (gridResultados.Rows.Count > 0)
        {
            if (pnMapa.Visible == false) //Resultados de la Consulta general
            {
                LblResultados.Text = string.Format("Resultados de la busqueda ({1}). Total {0}",
                                                    gridResultados.Rows.Count,
                                                    Consulta);
                btnGeopocisionar.Visible = true;
                btnCerrarMapa.Visible = false;

                Session["ResultadosFeedGeneral"] = ResultadosFeed;
            }
            else //Resultados con geoposición
            {
                LblResultados.Text = string.Format("Resultados de la busqueda en el Biotipo: ({1}). Total {0}",
                                                    gridResultados.Rows.Count,
                                                    drplLugares.SelectedItem.Text);
                btnGeopocisionar.Visible = false;
                btnCerrarMapa.Visible = true;

                Session["ResultadosFeedGeoposicion"] = ResultadosFeed;
            }

        }
        else
        {
            if (pnMapa.Visible == false) //Sin resultados Consulta general
            {
                LblResultados.Text = string.Format("No se encontraron resultados de la busqueda ({0})." +
                                                    "Por favor revise que su consulta pertenezca al dominio de la ontología cargada",
                                                    TxtConsulta.Text);
                btnGeopocisionar.Visible = true;
                btnCerrarMapa.Visible = false;

                Session["ResultadosFeedGeneral"] = ResultadosFeed;
            }
            else //Sin resultados con Geoposición
            {
                LblResultados.Text = string.Format("No se encontraron resultados en el Biotipo: ({0}). " +
                                                    "Si aumenta el radio del área del Biotipo, es posible que se encuentre los sensores relacoonados",
                                                    drplLugares.SelectedItem.Text);
                btnGeopocisionar.Visible = false;
                btnCerrarMapa.Visible = true;

                //Almacenamos copia de los resultados
                Session["ResultadosFeedGeoposicion"] = ResultadosFeed;
            }
        }
    }

    #endregion

    protected void btnExplorarBiotipo_Click(object sender, EventArgs e)
    {
        string redirectURL = "AnalizarBiotipo.aspx?id=" + drplLugares.SelectedIndex;
        //Response.Redirect(redirectURL);
        Response.Redirect(redirectURL, "_blank", "menubar=0,scrollbars=1,width=780,height=900,top=10");
    }

    protected void txtradio_TextChanged(object sender, EventArgs e)
    {
        Session["radio"] = txtradio.Text;
    }
    protected void btnExplorarContaminacion_Click(object sender, EventArgs e)
    {
        string redirectURL = "AnalizarContaminacion.aspx?id=" + drplLugares.SelectedIndex;
        //Response.Redirect(redirectURL);
        Response.Redirect(redirectURL, "_blank", "menubar=0,scrollbars=1,width=780,height=900,top=10");
    }
    protected void btnRegistrar_Click(object sender, EventArgs e)
    {
        //Recuperar los valores generales a aregistrar
        string consultaoriginal = TxtConsulta.Text;
        string consultaexpandida = Session["consultaexpandida"].ToString();
        DateTime fechaconsulta = DateTime.Now;

        //Verificar que se haya iniciado sesion
        if (User.Identity.IsAuthenticated)
        {
            //Definir el objeto de negocio de la aplicación
            ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

            //Guardamos la consulta del usuario
            int consultaId = sm.SaveConsulta(consultaoriginal, consultaexpandida, fechaconsulta, User.Identity.Name);

            //Recorrer el grid para registrar cada resultado
            foreach (GridViewRow row in gridResultados.Rows)
            {
                CheckBox check = row.FindControl("chkRelevancia") as CheckBox;
                string feedid = row.Cells[1].Text;
                if (check.Checked)
                {
                    //La calificacion es 1 porque esta chequeado el feed
                    sm.SaveCalificacion(consultaId, feedid, 1);
                }
                else
                {
                    sm.SaveCalificacion(consultaId, feedid, 0);
                }
            }
            RegistrarScriptMensaje("Las calificaciones han sido registradas correctamente. !Muchas Gracias¡", true);
        }
        else
        {
            RegistrarScriptMensaje("Debe Iniciar sesion para poder registrar sus calificaciones",false);
        }
    }

    private void RegistrarScriptMensaje(string mensaje, Boolean redireccionar)
    {
        const string ScriptKey = "Errores";
        if (!ClientScript.IsStartupScriptRegistered(this.GetType(), ScriptKey))
        {
            StringBuilder fn = new StringBuilder();
            if (redireccionar)
            {
                fn.Append("alert('" + mensaje + "');");
                fn.Append("window.location.href = 'Default.aspx';");
            }
            else
            {
                fn.Append("alert('" + mensaje + "');");
            }
            ScriptManager.RegisterStartupScript(this,typeof(Page), ScriptKey, fn.ToString(), true);
        }
    }
}