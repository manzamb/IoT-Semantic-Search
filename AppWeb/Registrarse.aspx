<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterDefecto/MasterPage.master" AutoEventWireup="true" CodeFile="Registrarse.aspx.cs" Inherits="Registrarse" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <div  id="indent1" style ="text-align: center">
        <asp:Image ID="Image5" runat="server" Height="16px" 
            ImageUrl="~/imagenes/linea1.bmp" />
        <br />
        Soportado en el Servidor de Objetos
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://xively.com/">Xively</asp:HyperLink>
        <br />
        <br />
        <br />
        <div style="width: 388px; margin-left: auto; margin-right:auto;">
            <asp:CreateUserWizard ID="crearUsuario" runat="server" BorderStyle="Solid" OnCreatedUser="crearUsuario_CreatedUser">
                <WizardSteps>
                    <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                    </asp:CreateUserWizardStep>
                    <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                    </asp:CompleteWizardStep>
                </WizardSteps>
            </asp:CreateUserWizard>
        </div>

        <br />
        <br />
        
        &nbsp;<br />    
       </div>
</asp:Content>

