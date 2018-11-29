using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

public partial class Registrarse: System.Web.UI.Page
{
    protected void crearUsuario_CreatedUser(object sender, EventArgs e)
    {
        //Por defecto el nuevo usuario es un cliente
        Roles.AddUserToRole(crearUsuario.UserName, "Cliente");
        Response.Redirect("Default.aspx", true);
    }
}