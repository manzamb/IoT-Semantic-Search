<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterDefecto/MasterPage.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div id="indent1" align="center">
        <asp:Image ID="Image5" runat="server" Height="16px" 
            ImageUrl="~/imagenes/linea1.bmp" />
        <br />
        Soportado en el Servidor de Objetos
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://xively.com/">Xively</asp:HyperLink>
        <br />
        <br />
        <br />
        <div style="width: 400px; margin-left: auto; margin-right:auto;">
        <asp:Login ID="Login1" runat="server" BorderStyle="Solid" Height="198px" Width="398px">
        </asp:Login>
        </div>

        <br />
        <asp:HyperLink ID="hplnkregistro" runat="server" NavigateUrl="~/Registrarse.aspx">Registrarse</asp:HyperLink>
        <br />
        
        &nbsp;<br />
        <br />    
    </div>
</asp:Content>

