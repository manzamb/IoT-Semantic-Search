<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterDefecto/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Contenpages_Administrador_Default" %>

<%@ Register Assembly="FUA" Namespace="Subgurim.Controles" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
        <style type="text/css">
            .auto-style1 {
            width: 100%;
        }
            .auto-style11 {
            }
            .auto-style16 {
                width: 626px;
            }
            .auto-style19 {
                width: 626px;
                height: 61px;
            }
            .auto-style20 {
                width: 900px;
                height: 61px;
            }
            .auto-style25 {
                width: 900px;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <div  id = "indent1" style ="text-align: center; height: 384px;">
        <asp:Image ID="Image5" runat="server" Height="16px" 
            ImageUrl="~/imagenes/linea1.bmp" />
        <br />
        Soportado en el Servidor de Objetos
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://xively.com/">Xively</asp:HyperLink>
        <br />
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnAdministrador" runat="server" HorizontalAlign="Center">
                    <table class="auto-style1" style="padding: 2px; border: thin solid #000000; margin-left: 1; margin-right: 1;">
                        <tr>
                            <td class="auto-style16" style="border: thin solid #000000; font-weight: bold; text-align: center;">Ajuste</td>
                            <td class="auto-style25" style="border: thin solid #000000; font-weight: bold; text-align: center;">Configuración</td>
                        </tr>
                        <tr>
                            <td class="auto-style16" style="text-align: left">
                                Ontología Actual:</td>
                            <td class="auto-style25" style="text-align: left">
                                <asp:DropDownList ID="drpOntologías" runat="server" Width="238px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style16" style="text-align: left">
                                <asp:Label ID="lblanalizador" runat="server" Text="Usar Analizador:"></asp:Label>
                            </td>
                            <td class="auto-style25" style="text-align: left">
                                <asp:DropDownList ID="ddlAnalizador" runat="server" AutoPostBack="True" Height="16px" OnSelectedIndexChanged="ddlAnalizador_SelectedIndexChanged" Width="106px">
                                    <asp:ListItem>Español</asp:ListItem>
                                    <asp:ListItem>Defecto</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style16" style="text-align: left">
                                <asp:Label ID="lblBDD" runat="server" Text="Base de Datos de Indexación:"></asp:Label>
                            </td>
                            <td class="auto-style25" style="text-align: left">
                                <asp:DropDownList ID="ddlBDD" runat="server" AutoPostBack="True" Height="19px" OnSelectedIndexChanged="ddlBDD_SelectedIndexChanged" Width="106px">
                                    <asp:ListItem>Local</asp:ListItem>
                                    <asp:ListItem>Xively</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style16" style="text-align: left">Utilizar Expanción de Consulta:</td>
                            <td class="auto-style25" style="text-align: left">
                                <asp:DropDownList ID="drpExpansion" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpExpansion_SelectedIndexChanged" Width="106px">
                                    <asp:ListItem>Si</asp:ListItem>
                                    <asp:ListItem>No</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left">Reconstruír el Índice:</td>
                            <td class="auto-style11" style="text-align: left"><asp:Button ID="btnReindexar" runat="server" OnClick="btnReindexar_Click" Text="Reindexar" Width="103px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style19" style="text-align: left">Cargar e Indexar Nueva Ontología:</td>
                            <td class="auto-style20" style="text-align: left">
                                <cc1:FileUploaderAJAX ID="fuOntologia" runat="server" File_RenameIfAlreadyExists="False" text_Uploading="Indexando... Puede demorar varios minutos..." />
                                <asp:Button ID="btnCargareIndexar" runat="server" Height="25px" OnClick="btnCargareIndexar_Click" Text="Indexar Nueva" Width="103px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style16" style="text-align: left">Indexar Feed Xively</td>
                            <td class="auto-style25" style="text-align: left">
                                <asp:TextBox ID="txtFeedId" runat="server" Width="146px"></asp:TextBox>
                                &nbsp;
                                <asp:Button ID="btnIndexarFeed" runat="server" Height="25px" OnClick="btnIndexarFeed_Click" Text="Indexar Feed" Width="102px" />
                                </td>
                        </tr>
                    </table>
                    &nbsp;<asp:Literal ID="litMensaje" runat="server"></asp:Literal>
                    &nbsp;<asp:HyperLink ID="hplBuscar" runat="server" NavigateUrl="~/Default.aspx">Regresar a Buscar</asp:HyperLink>
                    <br />
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
<%--            <asp:AsyncPostBackTrigger ControlID="lnkIndexar" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="ddlAnalizador" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlBDD"  EventName ="SelectedIndexChanged"/>
                <asp:AsyncPostBackTrigger ControlID="drpExpansion" EventName="SelectedIndexChanged" />--%>
            </Triggers>
        </asp:UpdatePanel>
        <br />
        <asp:UpdateProgress ID="udpProgreso" runat="server" DisplayAfter="0" AssociatedUpdatePanelID = "UpdatePanel2">
        <ProgressTemplate>
            <div class="overlay" />
            <div class="overlayContent" >
                <h2>
                    <asp:Literal ID="LitEspera" runat="server"></asp:Literal>
                </h2>
                <asp:Image ID="Image1" runat="server" ImageUrl="~/MasterPages/MasterDefecto/images/ajax-loader.gif" AlternateText="Cargando" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
        <br />
         
        <br />

        <br />    
    </div>
</asp:Content>

