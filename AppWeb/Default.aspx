<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterDefecto/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="indent">
        <h2>Iniciar Búsqueda</h2>
            <asp:TextBox ID="TxtConsulta" runat="server" Width="544px" ToolTip="Digite el Texto a Buscar" Height="19px"></asp:TextBox>
            &nbsp;
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Panel ID="pnPreferencias" runat="server" Height="21px">
                <asp:LoginStatus ID="LoginStatus1" runat="server" />
                &nbsp;<asp:LoginName ID="LoginName1" runat="server" Font-Bold="True" />
                &nbsp;-
                <asp:Label ID="lblIdioma" runat="server" Font-Size="Small" Text="Idioma Preferido:" Font-Bold="False"></asp:Label>
                &nbsp;<asp:RadioButtonList ID="rdblistIdioma" runat="server" Font-Size="Small" OnSelectedIndexChanged="rdblistIdioma_SelectedIndexChanged" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Selected="True">Español</asp:ListItem>
                    <asp:ListItem>Ingles</asp:ListItem>
                    <asp:ListItem>Ambos</asp:ListItem>
                </asp:RadioButtonList>
                <br />
                <asp:RequiredFieldValidator ID="rfvConsultavacia" runat="server" ControlToValidate="TxtConsulta" ErrorMessage="La consulta no puede ser vacia" Font-Size="Small"></asp:RequiredFieldValidator>
                <br />
            </asp:Panel>
            <br/>
        <h3>Introducción</h3>
        <p>El buscador semántico tiene por objeto facilitar la búsqueda e identificación de servicios
            de los sesnores de la Web de las Cosas, para ayudar a los usuarios a crear aplicaciones que
            integren estas técnologías, facilitándo el desarrollo de nuevos servicios.</p>
        <h3>Domimio de Búsqueda Actual</h3>
        <p>El dominio de búsqueda actual es de CONTAMINACION AMBIENTAL. Para iniciar la búsqueda solo
            digite su consulta en el cuadro siguiente. Puede seleccionar el idioma preferido en las opciones debajo
            del cuadro de consulta. Evite en lo posible errores de ortografía y especifique en lo posible en la consulta una 
            ubicación o lugar, con el fin de establecer los sensores más cercanos a la misma.</p>
      </div>
    
</asp:Content>

