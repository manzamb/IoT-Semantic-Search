using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;
using srvIdexSemanticIoT;

public partial class Contenpages_Administrador_Default : System.Web.UI.Page
{
    //Crea una propiedad para el manejo de los mensajes de procesamiento
    public string mensaje;
    public Boolean indexando = false;

    public string Mensaje
    {
        get
        {
            return mensaje;
        }
        set
        {
            mensaje = value;
            //Construir el mensaje en el literal
            string msg = string.Empty;
            msg += "<b>RESULTADOS: </b>";
            msg += "<hr width=100%/>";
            msg += "<p>" + mensaje + "</p>";
            msg += "<hr width=100%/>";
            litMensaje.Text = msg;
            //(udpProgreso.FindControl("LitEspera") as Literal).Text = mensaje;
        }
    }

    //Establece variables de aplicación si es la primera vez que se carga la página
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Definir el objeto de negocio de la aplicación
            ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

            //Establecer en los controles el valor actual
            ddlAnalizador.Items.FindByValue(sm.ObtenerConfiguracion("tipoAnalizador")).Selected = true;
            ddlBDD.Items.FindByValue(sm.ObtenerConfiguracion("BDDfuente")).Selected = true;
            drpExpansion.Items.FindByValue(sm.ObtenerConfiguracion("utilizarExpansion")).Selected = true;
            drpOntologías.Items.Add(sm.ObtenerConfiguracion("FileOntology"));
            Session["Indexado"] = null;

            //Verifica si se esta cargando el archivo de ontología o eliminando uno cargado
            if (fuOntologia.IsPosting)
            {
                CargarOntologia();
                //Establecer que aun no ha indexado
                Session["Indexado"] = false;
            }
            else if (fuOntologia.IsDeleting)
                fuOntologia.historial.Clear();

        }

        //Mensaje de espera predeterminado
        (udpProgreso.FindControl("LitEspera") as Literal).Text = "Cargando...";
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        if (Session["Indexado"] != null)
        {
            if ((Boolean)Session["Indexado"] == true)
            {
                //Definir el objeto de negocio de la aplicación
                ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

                if (Session["OntologiaAnterior"]!= null)
                    //En cso de haber subido una ontologí pero haberla indexado se pasa a la configuracíón anterior
                    sm.AsignarConfiguracion("OntologiaAnterior", (string)Session["OntologiaAnterior"]);
            }
        }
    }


    //Crea el índice semántico desde cero
    private void CrearSemanticIndex(string ontologia)
    {
        string resultado;

        try
        {
            //Definir el objeto de negocio de la aplicación
            ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

            //Calculamos el tiempo de demora en el proceso
            DateTime inicio = DateTime.Now;

            Mensaje = "Creando el Indice. Puede demorar varios minutos";
            resultado = sm.CrearIndiceSemantico(ontologia);

            //Tiempo final
            DateTime fin = DateTime.Now;

            //Mostramos el resultado
            TimeSpan intervalo = new TimeSpan(fin.Ticks - inicio.Ticks);
            Mensaje = resultado + " - Tiempo Indexación : " + ((float)intervalo.TotalMilliseconds / 1000 / 60).ToString() + " minutos.";

            //Termino la indexación correctamente
            Session["Indexado"] = true;
            Session["OntologiaAnterior"] = null;
        }
        catch(Exception ex)
        {
            Mensaje = "ERROR: " + ex.Message.ToString();
        }
    }

    protected void ddlBDD_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Definir el objeto de negocio de la aplicación
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

        Mensaje = "Asignando la Fuente de Documentos";
        sm.AsignarConfiguracion("BDDfuente", ddlBDD.SelectedItem.Text);
        Mensaje = "La Base de Datos Documental como fuete de datos se ha cambiado a: <b>" + ddlBDD.SelectedItem.Text + "</b>";
    }

    protected void ddlAnalizador_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Definir el objeto de negocio de la aplicación
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

        Mensaje = "Asignando el Tipo de Analizador";
        sm.AsignarConfiguracion("tipoAnalizador", ddlAnalizador.SelectedItem.Text);
        Mensaje = "El analizador de cadenas se ha cambiado a: <b>" + ddlAnalizador.SelectedItem.Text + "</b>";
    }

    protected void drpExpansion_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Definir el objeto de negocio de la aplicación
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

        Mensaje = "Asignando la Opción de Expanción de Consulta";
        sm.AsignarConfiguracion("utilizarExpansion", drpExpansion.SelectedItem.Text);
        Mensaje = "La opción de expandir la consulta del usuario se ha cambiado a: <b>" + drpExpansion.SelectedItem.Text + "</b>";
    }

    protected void btnCargareIndexar_Click(object sender, EventArgs e)
    {
        //Definir el objeto de negocio de la aplicación
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

        //Verificar que ha subido los archivos de ontología correctos
        if (fuOntologia.historial!= null)
            if (fuOntologia.historial.Count > 0)
            {
                try
                {
                    //Se solicita crear el índice semántico
                    CrearSemanticIndex(fuOntologia.historial[0].FileName);
                }
                catch (Exception ex)
                {
                    litMensaje.Text = "ERROR: " + ex.Message.ToString();
                }
            }
            else
            {
                Mensaje = "ERROR: No ha seleccionado un archivo aún";
            }
        else
            Mensaje = "ERROR: No ha seleccionado un archivo aún";
   }

    protected void btnIndexarFeed_Click(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(txtFeedId.Text))
        {
            //Definir el objeto de negocio de la aplicación
            ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

            Mensaje = "Indexando Feed de Xively";
            Mensaje = sm.CargarFeedXivelyBDD(txtFeedId.Text);
        }
        else
        {
            Mensaje = "Para indexar un feed debe proveer un identificador de feed válido de Xively";
        }
    }

    protected void CargarOntologia()
    {
        //Definir el objeto de negocio de la aplicación
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();
        
        //Seleccionar y convertir archivo en byte array
        if (fuOntologia.IsPosting)
            try
            {
                string pathstr = Server.MapPath("~/App_Data/Ontologias/");
                string nombrefile = fuOntologia.PostedFile.FileName;
                string filenameontologialocal = pathstr + nombrefile;

                //Se almacena en la aplicación web. Si existe lo sobreescribe
                fuOntologia.SaveAs("~/App_Data/Ontologias", fuOntologia.PostedFile.FileName);

                //Se prepara para envarlo al servicio web
                StreamReader reader = new StreamReader(filenameontologialocal);
                BinaryReader binReader = new BinaryReader(reader.BaseStream);

                //Crea una variable byte array del archivo para sus uso posterior
                byte[] binFile = binReader.ReadBytes(Convert.ToInt32(binReader.BaseStream.Length));

                //close reader
                binReader.Close();
                reader.Close();

                //Llamar al servicio Web para cargar el archivo
                Mensaje = "Cargando archivo de Ontología";
                Mensaje = sm.CargarOntologia(fuOntologia.PostedFile.FileName, binFile);
                
                //Reflejar el cambio en el drop de ontologias
                drpOntologías.Items.Add(sm.ObtenerConfiguracion("FileOntology"));
            }
            catch (Exception ex)
            {
               Mensaje= "ERROR: " + ex.Message.ToString();
            }
        else
        {
            Mensaje = "Usted no ha especificado un archivo";
        }
    }

    protected void btnReindexar_Click(object sender, EventArgs e)
    {
        //Se solicita crear el índice semántico
        CrearSemanticIndex(drpOntologías.SelectedValue);
    }


}