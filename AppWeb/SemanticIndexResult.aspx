<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterResult/MasterPageResult.master" AutoEventWireup="true" CodeFile="SemanticIndexResult.aspx.cs" Inherits="SemanticIndexResult" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .indent1 {
            height: 225px;
        }
        
        </style>
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?v=3.exp&key=AIzaSyCmVnaqWoK3x-Lb56Yud7T3jbw1E13TWnM&sensor=false"></script>
    <script type="text/javascript" src="mapas/map.js"></script>
    <%--Para el grid anidado--%>

    <script src="Scripts/jquery-2.1.0.min.js" type="text/javascript"></script>

    <script type="text/javascript">

        $(function () {

            $('#<%=gridResultados.ClientID %> img').click(function () {

                var img = $(this)
                var feedId = $(this).attr('Id');

                var tr = $('#<%=gridResultados.ClientID %> tr[Id =' + feedId + ']')
                tr.toggle();

                if (tr.is(':visible'))
                    img.attr('src', 'MasterPages/MasterResult/images/minus.gif');
                else
                    img.attr('src', 'MasterPages/MasterResult/images/plus.gif');

            });

        });


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <div id="General">
            <asp:Image ID="Image10" runat="server" Height="16px" ImageUrl="~/imagenes/linea1.bmp" Width="955px"/>
            Soportado en el Servidor de Objetos
            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="https://xively.com/">Xively</asp:HyperLink>
            .<br />
            <br />
                <asp:TextBox ID="TxtConsulta" runat="server" Width="544px" ToolTip="Digite el Texto a Buscar"></asp:TextBox>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" onclick="btnBuscar_Click" />
                &nbsp;
                <asp:Button ID="btnGeopocisionar" runat="server" Text="Detectar Biotipos" OnClick="btnGeopocisionar_Click" />
                &nbsp;
            <asp:Button ID="btnCerrarMapa" runat="server" OnClick="btnCerrarMapa_Click" Text="Cerrar Mapa" Visible="False" />
                <asp:Panel ID="pnPreferencias" runat="server" Height="21px">
                    <asp:LoginStatus ID="loginEstado" runat="server" />
                    &nbsp;<asp:LoginName ID="loginUser" runat="server" Font-Bold="True" />
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
                <br />
                <asp:Label ID="LblResultados" runat="server" Font-Italic="True" Font-Size="Small" Font-Bold="True"></asp:Label>
            <br />
        </div>

        <div id="Resultados" runat="server">
            <br />
            <asp:UpdatePanel ID="UdpGridResultados" runat="server" Visible="true">
                <ContentTemplate>
                    <fieldset>
                    <legend>Tabla Resultados Sensores de la Consulta</legend>
                    <asp:GridView ID="gridResultados" runat="server" AutoGenerateColumns="False" Width="955px" 
                                    CellPadding="4" ForeColor="#333333" GridLines="None" DataKeyNames="Id"
                                    onrowdatabound="gvResultados_RowDataBound">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <img alt="" src="MasterPages/MasterResult/images/plus.gif" Id="<%# Eval("Id") %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="id" HeaderText="ID Dispositivo" ReadOnly="True" />
                            <asp:BoundField DataField="TitleHTML" HeaderText="Nombre del Sensor" HtmlEncode="False" HtmlEncodeFormatString="False" />
                            <asp:BoundField DataField="location.name" HeaderText="Localización" />
                            <asp:BoundField DataField="description" HeaderText="Descripción" />
                            <asp:BoundField DataField="location.lat" HeaderText="Lat" />
                            <asp:BoundField DataField="location.lon" HeaderText="Lng" />

                            <asp:TemplateField HeaderText="¿Relevante?">
                                <EditItemTemplate>
                                    <asp:CheckBox ID="chkRelevancia" runat="server" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkRelevancia" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>

                <asp:TemplateField>
                <ItemTemplate>
                    <tr style="display:none;" Id="<%# Eval("Id") %>">
                        <td colspan="100%">
                            <div style="position:relative;left:25px;">
                                <asp:GridView ID="gvDatastreams" runat="server" AutoGenerateColumns="False" 
                                    BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" 
                                    CellPadding="3" ForeColor="Black" GridLines="Vertical" DataKeyNames="Id">
                                    <FooterStyle BackColor="#CCCCCC" />
                                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                    <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                                    <AlternatingRowStyle BackColor="#CCCCCC" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Datastream">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" 
                                                    Text='<%# Bind("Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="at" HeaderText="Fecha"  />
                                        <asp:BoundField DataField="current_value" HeaderText="Valor Actual"  />
                                        <asp:BoundField DataField="min_value" HeaderText="Valor Mínimo"  />
                                        <asp:BoundField DataField="max_value" HeaderText="Valor Máximo"  />
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
                    </fieldset>
                    <asp:Button ID="btnRegistrar" runat="server" Text="Enviar Respuestas" OnClick="btnRegistrar_Click" />
                </ContentTemplate>
                <Triggers></Triggers>
            </asp:UpdatePanel>
            </div>
        <div id="mapa">
            <br />
            <asp:UpdatePanel ID="pnMapa" runat="server" Visible="false">
                <ContentTemplate>
                    <fieldset>
                        <legend>Biotipos y Ubicación de los Sensores</legend>
                        <%--<asp:Label ID="lblLugaresConsulta" runat="server" Text="Lugares en Consulta:"></asp:Label>--%>
                        <asp:DropDownList ID="drplLugares" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drplLugares_SelectedIndexChanged" Width ="220px"></asp:DropDownList>
                        <%--<input id="address" type="text" runat="server" visible="True" style="border-style: hidden; border-color: #FFFFFF;"/>--%>
                        <asp:Label ID="latlng" runat="server" style="border-style: hidden; border-color: #FFFFFF;" width="206px"></asp:Label>
                        <asp:Label ID="lblradio" runat="server" Text="Radio (km):"></asp:Label>
                        <asp:TextBox ID="txtradio" Text="100" runat="server" Width="62px" OnTextChanged="txtradio_TextChanged"></asp:TextBox>
                        <%--<input id="latlng" type="text" value ="" runat="server" visible="True" style="border-style: hidden; border-color: #FFFFFF;" width="180"/>--%>
                        <asp:Button ID="btnBuscarSensores" runat="server" OnClick="btnBuscarSensores_Click" Text="Buscar Sensores" Width="133px" />
                        &nbsp;&nbsp;<asp:Button ID="btnExplorarBiotipo" runat="server" Text="Medio Ambiente" OnClick="btnExplorarBiotipo_Click" Width="129px" />
                        &nbsp;
                        <asp:Button ID="btnExplorarContaminacion" runat="server" Text="Contaminación" OnClick="btnExplorarContaminacion_Click" Width="121px" />
                        <%--<input type="button" value="Reverse Geocode" onclick="codeLatLng()"/><br />--%>
                        <div id="map">
                        </div>
                    </fieldset>
                </ContentTemplate>
                <Triggers>
                        <asp:PostBackTrigger  ControlID="drplLugares" />
                        <asp:PostBackTrigger ControlID="btnBuscarSensores" />
                        <asp:PostBackTrigger ControlID="btnExplorarBiotipo" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
</asp:Content>

