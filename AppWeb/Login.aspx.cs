using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
    {
        if (e.Authenticated) Response.Redirect("Default.aspx", true);
    }
}