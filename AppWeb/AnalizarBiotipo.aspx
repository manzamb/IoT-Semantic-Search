<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterResult/MasterPageResult.master" AutoEventWireup="true" CodeFile="AnalizarBiotipo.aspx.cs" Inherits="AnalizarBiotipo" %>

<%@ Reference Control="~/Controles/GridSensores.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="Scripts/jquery-2.1.0.min.js" type="text/javascript"></script>
    <link href="Scripts/jquery-ui-1.10.4.custom/development-bundle/themes/base/jquery.ui.all.css" rel="stylesheet" />
<%--    <script src="Scripts/jquery-ui-1.10.4.custom/js/jquery-1.10.2.js"></script>--%>
    <script src="Scripts/jquery-ui-1.10.4.custom/development-bundle/ui/jquery.ui.core.js"></script>
    <script src="Scripts/jquery-ui-1.10.4.custom/development-bundle/ui/jquery.ui.widget.js"></script>
    <script src="Scripts/jquery-ui-1.10.4.custom/development-bundle/ui/jquery.ui.datepicker.js"></script>
    <%--Para el selector de tiempo--%>
    <script src="Scripts/Timepicker/jquery.mousewheel.js"></script>
    <script src="Scripts/Timepicker/jquery.timepickerinputmask.js"></script>

	<script type="text/javascript">
	    jQuery.noConflict();
	    jQuery(document).ready(function (jQuery) {
	        jQuery('#<%=txtHoraInicio.ClientID %>').TimepickerInputMask({
	            spinners: true,
	            bgcolor: '#006600',
	            arrowColor: '#fff'
	        });
	        jQuery('#<%=txtHoraFin.ClientID %>').TimepickerInputMask({
	            spinners: true,
	            bgcolor: '#006600',
	            arrowColor: '#fff'
	        });
	    });
    </script>

    <script type="text/javascript">
	    var q = jQuery.noConflict();
        q(function (q) {
	        q('#<%=txtfrom.ClientID %>').datepicker({
	            defaultDate: "+1w",
	            maxDate: "today",
	            dateTimeFormat: 'yy-mm-dd',
	            changeMonth: true,
	            numberOfMonths: 3,
	            onClose: function (selectedDate) {
	                q('#<%=txtto.ClientID %>').datepicker("option", "minDate", selectedDate);
	            }
	        });
            q('#<%=txtto.ClientID %>').datepicker({
	            defaultDate: "+1w",
	            maxDate: "today",
	            dateTimeFormat: 'yy-mm-dd',
	            changeMonth: true,
	            numberOfMonths: 3,
	            onClose: function (selectedDate) {
	                q('#<%=txtfrom.ClientID %>').datepicker("option", "maxDate", selectedDate);
	            }
	        });
	    });
	</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <div "indent1">
        <asp:Image ID="Image10" runat="server" Height="16px" ImageUrl="~/imagenes/linea1.bmp" Width="955px"/>
        <h3>Selección del Biotipo y Area de Acción:</h3>
        <asp:Label ID="lblBiotipos" runat="server" Text="Lista de Biotipos Encontrados: "></asp:Label>
        <asp:DropDownList ID="drplLugares" runat="server" AutoPostBack="true" Width ="220px" OnSelectedIndexChanged="drplLugares_SelectedIndexChanged"></asp:DropDownList>
        &nbsp;&nbsp;
        <asp:Label ID="lblradio" runat="server" Text="Radio (km):"></asp:Label>
        <asp:TextBox ID="txtradio" Text="100" runat="server" Width="62px" OnTextChanged="txtradio_TextChanged"></asp:TextBox>
        &nbsp;
        <asp:Button ID="btnAnalizarBiotipo" runat="server" OnClick="btnBuscarSensores_Click" Text="Analizar Biotipo" />
        <br />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtradio" ErrorMessage="rfvradio">El radio que define el área del Biotipo no puede ser vacio o cero</asp:RequiredFieldValidator>
        <br />
        <h3>Rango de Análisis de Datos (MM-dd-yyyy hh:mm:ss):</h3>
        <asp:Label ID="fromlbl" runat="server" Text="Desde"></asp:Label>
        <asp:TextBox ID="txtfrom" runat="server" OnTextChanged="txtfrom_TextChanged"></asp:TextBox>
        <asp:TextBox ID="txtHoraInicio" runat="server" OnTextChanged="txtHoraInicio_TextChanged"></asp:TextBox>
        <asp:Label ID="tolbl" runat="server" Text="hasta"></asp:Label>
        <asp:TextBox ID="txtto" runat="server" OnTextChanged="txtto_TextChanged"></asp:TextBox>
        <asp:TextBox ID="txtHoraFin" runat="server" OnTextChanged="txtHoraFin_TextChanged"></asp:TextBox>

    </div>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtfrom" ErrorMessage="rfvfechainicio">La fecha Inicial no puede ser vacia</asp:RequiredFieldValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtto" ErrorMessage="rfvfechafin">La fecha Final no puede ser Vacia</asp:RequiredFieldValidator>
    <br />
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <br />
                <h1>DATOS GENERALES DEL BIOTIPO</h1>
                <asp:Panel ID="pnEncabezado" runat="server">
                    <table style="margin: 5px; padding: 10px;">
                  <tr>
                    <td style="font-weight: bold">
                        Id de Biotipo: </td>
                    <td>
                        <asp:Label ID="lblBiotipoId" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="font-weight: bold">
                        Nombre:</td>
                    <td>
                        <asp:Label ID="lblNombre" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="font-weight: bold">
                        Pais:</td>
                    <td>
                        <asp:Label ID="lblPais" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="font-weight: bold">
                        Latitud:</td>
                    <td>
                        <asp:Label ID="lblLatitud" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="font-weight: bold">
                        Longitud:</td>
                    <td>
                        <asp:Label ID="lblLongitud" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="font-weight: bold">
                        Radio:</td>
                    <td>
                        <asp:Label ID="lblArea" runat="server" Text=""></asp:Label>
                    </td>
                </tr>

                </table>
                </asp:Panel>
                <br />
                <h1>MEDICIÓN DE VARIABLES MEDIOAMBIENTALES</h1>
                <asp:Panel ID="pnVariables" runat="server">
                    <br />
                    <hr style="width: 960px" />
                    <h2>Edatopo</h2>
                    <asp:Panel ID="pnEdatopo" runat="server">
                    </asp:Panel>
                    <br />
                    <hr style="width: 960px" />
                    <h2>Hidrotopo</h2>
                    <asp:Panel ID="pnHidrotopo" runat="server"></asp:Panel>
                    <br />
                    <hr style="width: 960px" />
                    <h2>ClimAtopo</h2>
                    <asp:Panel ID="pnClimatopo" runat="server"></asp:Panel>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:Literal ID="LitResultados" runat="server"></asp:Literal>
    </div>
</asp:Content>

