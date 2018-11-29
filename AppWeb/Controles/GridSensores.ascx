<%@ Control ClassName="Controles_GridSensores" Language="C#" AutoEventWireup="true" CodeFile="GridSensores.ascx.cs" Inherits="Controles_GridSensores" %>

 <script src="Scripts/jquery-2.1.0.min.js" type="text/javascript"></script>
<script type="text/javascript">
   
    function load_Data_<%=this.ID %>(feedId, opcion) {
        if (opcion == -1) {
            var img = $('#' + feedId);
            var tr = $('#<%=this.ID%>_' + '<%=gridResultados.ID%>' + ' tr[Id =' + feedId + ']');
            tr.toggle();

            //Cambiar la imagen
            if (tr.is(':visible'))
                img.attr('src', 'MasterPages/MasterResult/images/minus.gif');
            else
                img.attr('src', 'MasterPages/MasterResult/images/plus.gif');
        }
        else {
            //obtenemos la fila a mostrar u ocultar
            var tr = $('#<%=this.ID%>_' + '<%=gridResultados.ID%>' + ' tr[Id =' + opcion + ']').find(' tr[Id =' + feedId + ']');
            var img = tr.parent().find('img[id = ' + feedId + ']');
            if (tr.is(':visible')) {
                img.attr('src', 'MasterPages/MasterResult/images/plus.gif');
                tr.toggle();
            }
            else {
                var Idfeed = opcion;
                var dtsID = feedId;
                var fechaInicio = $('#<%=hidfechaInicio.ClientID %>');
                var fechaFin = $('#<%=hidFechaFin.ClientID %>');
                var dataString = "{'feedID':'" + Idfeed + "','DatastreamId':'" + dtsID + "','fechaInicio':'" + fechaInicio.attr("value") + "','fechaFin':'" + fechaFin.attr("value") + "'}";
                var pageUrl = 'wsDataPoints.asmx'
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: dataString,
                    url: pageUrl + "/RetornarDatapointsFeed",
                    dataType: "json",
                    success: function (data) {
                        if (data.d != null) {
                            var resultadosdiv = tr.find('div[id = divDatapoints_' + feedId + ']');
                            var stringmostrar = '<hr/>';
                            //Establecer si tiene datos
                            if (data.d != "") {
                                var datastream = $.parseJSON(data.d);
                                if (datastream.datapoints != undefined) {
                                    stringmostrar += '<table id="hor-minimalist-a" summary="Mediciones del Sensor">';
                                    stringmostrar += '<thead>';
                                    stringmostrar += '<tr>';
                                    stringmostrar += '<th scope="col">Medición</th>';
                                    stringmostrar += '<th scope="col">Fecha</th>';
                                    stringmostrar += '</tr>';
                                    stringmostrar += '</thead>';
                                    stringmostrar += '<tbody>';
                                    $.each(datastream.datapoints, function () {
                                        stringmostrar += '<tr>';
                                        stringmostrar += '<td>' + this.value + '</td>';
                                        stringmostrar += '<td>' + this.at + '</td>';
                                        stringmostrar += '</tr>';
                                    });
                                    stringmostrar += '</tbody>';
                                    stringmostrar += '</table>';
                                    stringmostrar += '<hr/> <br/>';
                                }
                                else {
                                    stringmostrar += 'No hay datos en este sensor';
                                    stringmostrar += '<hr/> <br/>';
                                }
                            }
                            else {
                                stringmostrar += 'No hay datos en este sensor';
                                stringmostrar += '<hr/> <br/>';
                            }

                            resultadosdiv.html(stringmostrar);
                            tr.toggle();
                            img.attr('src', 'MasterPages/MasterResult/images/minus.gif');
                        }
                    },
                    error: function OnErrorCall(response) {
                        alert(response.status + " " + response.statusText);
                    }
                });
            }
        }
    }

    </script>
<style type="text/css">
* {
    margin: 0;
    padding: 0;
}

img {
    border-style: none;
    border-color: inherit;
    border-width: 0;
    vertical-align: top;
    text-align: left;
    /*height: 130px;
    width: 224px;*/
}

</style>
<div id="Grid">
    <asp:HiddenField ID="hidfechaInicio" runat="server" />
    <asp:HiddenField ID="hidFechaFin" runat="server" />
    <asp:GridView ID="gridResultados" runat="server" AutoGenerateColumns="False" Width="955px" 
                    CellPadding="4" ForeColor="#333333" GridLines="None" DataKeyNames="id"
                    onrowdatabound="gvResultados_RowDataBound">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <img id="<%# Eval("id") %>" alt="" src="MasterPages/MasterResult/images/plus.gif" onclick ="load_Data_<%=this.ID %>(<%# Eval("id") %>, -1)"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="id" HeaderText="ID Dispositivo" ReadOnly="True" />
            <asp:BoundField DataField="TitleHTML" HeaderText="Nombre del Sensor" HtmlEncode="False" HtmlEncodeFormatString="False" />
            <asp:BoundField DataField="location.name" HeaderText="Localización" />
            <asp:BoundField DataField="description" HeaderText="Descripción" />
            <asp:BoundField DataField="location.lat" HeaderText="Lat" />
            <asp:BoundField DataField="location.lon" HeaderText="Lng" />
            <asp:TemplateField>
            <ItemTemplate>
                <tr style="display:none;" Id='<%# Eval("Id") %>'>
                    <td colspan="100%">
                        <div style="position:relative;left:25px;">
                            <asp:GridView ID="gvDatastreams" runat="server" AutoGenerateColumns="False"
                                BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px"
                                CellPadding="3" ForeColor="Black" GridLines="Vertical" DataKeyNames="Feedid, Id">
                                <FooterStyle BackColor="#CCCCCC" />
                                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                                <AlternatingRowStyle BackColor="#CCCCCC" />
                                <Columns>
                                    <asp:TemplateField>
                                    <ItemTemplate>
                                        <img id='<%# Eval("id") %>' alt="" src="MasterPages/MasterResult/images/plus.gif" onclick ="load_Data_<%=this.ID %>('<%# Eval("id") %>', '<%# Eval("Feedid") %>')"/>
                                    </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Feedid" HeaderText="Dispositivo" />
                                    <asp:BoundField DataField="Id" HeaderText="Datastream" />
                                    <asp:BoundField DataField="at" HeaderText="Fecha" />
                                    <asp:BoundField DataField="current_value" HeaderText="Valor Actual" />
                                    <asp:BoundField DataField="min_value" HeaderText="Valor Mínimo" />
                                    <asp:BoundField DataField="max_value" HeaderText="Valor Máximo" />
                                    <asp:TemplateField>
                                    <ItemTemplate>
                                        <tr style="display:none;" Id='<%# Eval("Id") %>'>
                                            <td colspan="100%">
                                                <div style="position:relative;left:25px;">
                                                    <div id="divDatapoints_<%# Eval("Id") %>"></div>
                                                </div>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </td>
                </tr>
            </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EditRowStyle BackColor="#7C6F57" />
        <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#E3EAEB" />
        <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#F8FAFA" />
        <SortedAscendingHeaderStyle BackColor="#246B61" />
        <SortedDescendingCellStyle BackColor="#D4DFE1" />
        <SortedDescendingHeaderStyle BackColor="#15524A" />
    </asp:GridView>
</div>

                        
