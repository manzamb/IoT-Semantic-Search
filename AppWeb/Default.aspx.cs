using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Por defecto inicia en Español
            Session["idiomaBuscar"] = "Español";
        }
        else
        {
            //Establecer en los controles el valor actual
            rdblistIdioma.Items.FindByValue(rdblistIdioma.SelectedItem.Text).Selected = true;
        }
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        //Obtenemos la consulta del usuario
        string consulta = TxtConsulta.Text;

        //Se almacena la consulta para su uso posterior en la aplicación
        Session["QUERRY_TEXT"] = consulta;

        //El usuario decide consultar directamente
        Response.Redirect("SemanticIndexResult.aspx");
    }

    protected void rdblistIdioma_SelectedIndexChanged(object sender, EventArgs e)
    {
        rdblistIdioma.Items.FindByValue(rdblistIdioma.SelectedItem.Text).Selected = true;
        Session["idiomaBuscar"] = rdblistIdioma.SelectedItem.Text;
    }
}