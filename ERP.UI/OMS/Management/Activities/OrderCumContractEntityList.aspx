﻿<%@ Page Title="" Language="C#" MasterPageFile="~/OMS/MasterPage/ERP.Master" AutoEventWireup="true" CodeBehind="OrderCumContractEntityList.aspx.cs" Inherits="ERP.OMS.Management.Activities.OrderCumContractEntityList" %>
<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Data.Linq" TagPrefix="dx" %>
<%@ Register Src="~/OMS/Management/Activities/UserControls/VehicleDetailsControl.ascx" TagPrefix="uc1" TagName="VehicleDetailsControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <%--<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/jquery-confirm/3.1.0/jquery-confirm.min.css">--%>
    <link href="../../../assests/css/custom/jquery.confirm.css" rel="stylesheet" />
    <%--Code Added By Sandip For Approval Detail Section Start--%>
      <script src="JS/OrderCumContractEntityList.js?v=1.6"></script>
    <link href="CSS/SalesOrderEntityList.css" rel="stylesheet" />
    <script>
        $(document).ready(function () {
            $("#btntransporter").hide();
        });

        function OnApproveClick(keyValue, visibleIndex) {
            //document.getElementById("forfeitTable2").style.display = "block";
            //document.getElementById("forfeitTable2").style.display = "block";
            cGrdOrder.SetFocusedRowIndex(visibleIndex);
            var IsBalMapQtyExists = cGrdOrder.GetRow(cGrdOrder.GetFocusedRowIndex()).children[17].innerHTML; //cGrdOrder.GetRow(cGrdOrder.GetFocusedRowIndex()).children[18].innerText;
            //if (IsBalMapQtyExists.trim() != "0") {


            var IsCancelFlag = cGrdOrder.GetRow(cGrdOrder.GetFocusedRowIndex()).children[12].innerText;

            if (IsCancelFlag.trim() != 'Cancelled' && IsCancelFlag.trim() != 'Closed') {
                var ActiveUser = '<%=Session["userid"]%>'
                if (ActiveUser != null) {
                    $.ajax({
                        type: "POST",
                        url: "OrderCumContractEntityList.aspx/GetEditablePermission",
                        data: "{'ActiveUser':'" + ActiveUser + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: false,//Added By:Subhabrata
                        success: function (msg) {
                            //debugger;
                            var status = msg.d;
                            var url = 'ServiceContact.aspx?key=' + keyValue + '&Permission=' + status + '&type=SO' + '&type1=PO';
                            window.location.href = url;
                        }
                    });
                }
            }
            else {
                jAlert("Project Service Contract is " + IsCancelFlag.trim() + ".Edit is not allowed.");
            }
        }



        function OnMoreInfoClick(keyValue, visibleIndex) {
            //  debugger;
            cGrdOrder.SetFocusedRowIndex(visibleIndex);
            var IsBalMapQtyExists = cGrdOrder.GetRow(cGrdOrder.GetFocusedRowIndex()).children[17].innerHTML; //cGrdOrder.GetRow(cGrdOrder.GetFocusedRowIndex()).children[18].innerText;
            //if (IsBalMapQtyExists.trim() != "0") {


            var IsCancelFlag = cGrdOrder.GetRow(cGrdOrder.GetFocusedRowIndex()).children[12].innerText;

            if (IsCancelFlag.trim() != 'Cancelled' && IsCancelFlag.trim() != 'Closed') {
                var ActiveUser = '<%=Session["userid"]%>'
                if (ActiveUser != null) {
                    $.ajax({
                        type: "POST",
                        url: "OrderCumContractEntityList.aspx/GetEditablePermission",
                        data: "{'ActiveUser':'" + ActiveUser + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: false,//Added By:Subhabrata
                        success: function (msg) {
                            //debugger;
                            var status = msg.d;
                            var url = 'ServiceContact.aspx?key=' + keyValue + '&Permission=' + status + '&type=SO';
                            window.location.href = url;
                        }
                    });
                }
            }
            else {
                jAlert("Service Contract is " + IsCancelFlag.trim() + ".Edit is not allowed.");
            }
        }

        $(document).ready(function () {
            $("#expandCgvPurchaseOrder").click(function (e) {
                e.preventDefault();

                var $this = $(this);

                if ($this.children('i').hasClass('fa-expand')) {
                    $this.removeClass('hovered half').addClass('full');
                    $this.attr('title', 'Minimize Grid');
                    $this.children('i').removeClass('fa-expand');
                    $this.children('i').addClass('fa-arrows-alt');
                    var gridId = $(this).attr('data-instance');
                    $(this).closest('.makeFullscreen').addClass('panel-fullscreen');
                    var cntWidth = $(this).parent('.makeFullscreen').width();
                    var browserHeight = document.documentElement.clientHeight;
                    var browserWidth = document.documentElement.clientWidth;


                    CgvPurchaseOrder.SetHeight(browserHeight - 150);
                    CgvPurchaseOrder.SetWidth(cntWidth);
                }
                else if ($this.children('i').hasClass('fa-arrows-alt')) {
                    $this.children('i').removeClass('fa-arrows-alt');
                    $this.removeClass('full').addClass('hovered half');
                    $this.attr('title', 'Maximize Grid');
                    $this.children('i').addClass('fa-expand');
                    var gridId = $(this).attr('data-instance');
                    $(this).closest('.makeFullscreen').removeClass('panel-fullscreen');

                    var browserHeight = document.documentElement.clientHeight;
                    var browserWidth = document.documentElement.clientWidth;


                    CgvPurchaseOrder.SetHeight(450);

                    var cntWidth = $this.parent('.makeFullscreen').width();
                    CgvPurchaseOrder.SetWidth(cntWidth);

                }

            });
        });
    </script>

  <script>
      function UpdateTransporter(values) {
          $("#hdnOrderID").val(values);
          callTransporterControl(values, "SO");
          $("#exampleModal").modal('show');
      }

      function InsertTransporterControlDetails(data) {
          var orderid = $("#hdnOrderID").val();
          $.ajax({
              type: "POST",
              url: "SalesOrderEntityList.aspx/InsertTransporterControlDetails",
              data: "{'id':'" + orderid + "','hfControlData':'" + data + "'}",
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              async: false,//Added By:Subhabrata
              success: function (msg) {

              }
          });
      }
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dxe:ASPxPopupControl ID="Popup_OrderStatus" runat="server" ClientInstanceName="cOrderStatus"
        Width="500px" HeaderText="Approvers Configuration" PopupHorizontalAlign="WindowCenter" HeaderStyle-CssClass="wht"
        PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
        Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True"
        ContentStyle-CssClass="pad">
        <ContentCollection>
            <dxe:PopupControlContentControl runat="server">
                <%--<div style="Width:400px;background-color:#FFFFFF;margin:0px;border:1px solid red;">--%>
                <div class="row">
                    <div class="col-md-12">
                        <div class="col-md-12">
                            <table width="100%">
                                <tr>
                                    <td style="padding-right: 20px">
                                        <label style="margin-bottom: 5px">Proforma</label>
                                    </td>
                                    <td>
                                        <%--<dxe:ASPxTextBox ID="txt_Proforma" MaxLength="80" ClientInstanceName="cProforma" TabIndex="1" 
                                                runat="server" Width="100%"> 
                                            </dxe:ASPxTextBox>--%>
                                        <dxe:ASPxLabel ID="lbl_Proforma" runat="server" ClientInstanceName="cProforma" Text="ASPxLabel"></dxe:ASPxLabel>
                                    </td>
                                    <td style="padding-right: 20px; padding-left: 8px">
                                        <label style="margin-bottom: 5px">Customer</label>
                                    </td>
                                    <td>
                                        <%-- <dxe:ASPxTextBox ID="txt_Customer" ClientInstanceName="cCustomer"  runat="server" MaxLength="100" TabIndex="2"
                                            Width="100%"> 
                                        </dxe:ASPxTextBox>--%>
                                        <dxe:ASPxLabel ID="lbl_Customer" runat="server" ClientInstanceName="cCustomer" Text="ASPxLabel"></dxe:ASPxLabel>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="col-md-6">
                        </div>
                        <div class="col-md-6">
                        </div>
                        <div class="clear"></div>
                    </div>
                </div>
                <div class="col-md-12">
                    <table>
                        <tr>
                            <td style="width: 70px; padding: 13px 0;">Status </td>
                            <td>
                                <asp:RadioButtonList ID="rbl_OrderStatus" runat="server" Width="250px" CssClass="mTop5" RepeatDirection="Horizontal">
                                    <asp:ListItem Text="Pending" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Accepted" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Pending" Value="3"></asp:ListItem>
                                </asp:RadioButtonList></td>
                        </tr>
                    </table>
                </div>
                <div class="clear"></div>
                <div class="col-md-12">

                    <div class="" style="margin-bottom: 5px;">
                        Reason 
                    </div>

                    <div>
                        <dxe:ASPxMemo ID="txt_OrderRemarks" runat="server" ClientInstanceName="cOrderRemarks" Height="71px" Width="100%"></dxe:ASPxMemo>
                    </div>
                </div>

                <div class="col-md-12" style="padding-top: 10px;">
                    <dxe:ASPxButton ID="btn_PrpformaStatus" CausesValidation="true" ClientInstanceName="cbtn_PrpformaStatus" runat="server"
                        AutoPostBack="False" Text="Save" CssClass="btn btn-primary">
                        <ClientSideEvents Click="function (s, e) {SavePrpformaStatus();}" />
                    </dxe:ASPxButton>
                </div>
            </dxe:PopupControlContentControl>
        </ContentCollection>
    </dxe:ASPxPopupControl>
    <div class="panel-heading clearfix">
        <div class="panel-title pull-left">
            <h3>Service Contract</h3>
        </div>
        <table class="padTab pull-right" style="margin-top: 7px">
            <tr>
                <td>
                    <label>From </label>
                </td>
                <td style="width: 150px">
                    <dxe:ASPxDateEdit ID="FormDate" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" ClientInstanceName="cFormDate" Width="100%">
                        <ButtonStyle Width="13px">
                        </ButtonStyle>
                    </dxe:ASPxDateEdit>
                </td>
                <td>
                    <label>To </label>
                </td>
                <td style="width: 150px">
                    <dxe:ASPxDateEdit ID="toDate" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" ClientInstanceName="ctoDate" Width="100%">
                        <ButtonStyle Width="13px">
                        </ButtonStyle>
                    </dxe:ASPxDateEdit>
                </td>
                <td>Unit</td>
                <td>
                    <dxe:ASPxComboBox ID="cmbBranchfilter" runat="server" ClientInstanceName="ccmbBranchfilter" Width="100%">
                    </dxe:ASPxComboBox>
                </td>
                <td>
                    <input type="button" value="Show" class="btn btn-primary" onclick="updateGridByDate()" />
                </td>

            </tr>

        </table>
    </div>
    <div class="form_main">
        <div class="clearfix">

            <a href="javascript:void(0);" onclick="OnAddButtonClick()" class="btn btn-success btn-radius"><span class="btn-icon"><i class="fa fa-plus"></i></span><span>New</span> </a>
            
            <% if (rights.CanExport)
               { %>
            <asp:DropDownList ID="drdExport" runat="server" CssClass="btn btn-primary btn-radius" OnSelectedIndexChanged="cmbExport_SelectedIndexChanged" AutoPostBack="true" OnChange="if(!AvailableExportOption()){return false;}">
                <asp:ListItem Value="0">Export to</asp:ListItem>
                <asp:ListItem Value="1">PDF</asp:ListItem>
                <asp:ListItem Value="2">XLS</asp:ListItem>
                <asp:ListItem Value="3">RTF</asp:ListItem>
                <asp:ListItem Value="4">CSV</asp:ListItem>
            </asp:DropDownList>
            <% } %>
            <a href="javascript:void(0);" onclick="OpenWaitingOrder()" class="btn btn-warning hide typeNotificationBtn btn-radius"><span><u>O</u>rder Waiting </span>
                <span class="typeNotification">
                    <dxe:ASPxLabel runat="server" Text="" ID="lblQuoteweatingCount" ClientInstanceName="ClblQuoteweatingCount"></dxe:ASPxLabel>
                </span>
            </a>
            <%--Sandip Section for Approval Section in Design Start --%>

            <span id="spanStatus" runat="server">
                <a href="javascript:void(0);" onclick="OpenPopUPUserWiseQuotaion()" class="btn btn-primary btn-radius">
                    <span>My Service Contract Status</span>
                    <%--<asp:Label ID="Label1" runat="server" Text=""></asp:Label>--%>                   
                </a>
            </span>
            <span id="divPendingWaiting" runat="server">
                <a href="javascript:void(0);" onclick="OpenPopUPApprovalStatus()" class="btn btn-primary btn-radius">
                    <span>Approval Waiting</span>

                    <asp:Label ID="lblWaiting" runat="server" Text=""></asp:Label>
                </a>
                <i class="fa fa-reply blink" style="font-size: 20px; margin-right: 10px;" aria-hidden="true"></i>

            </span>

            <%--Sandip Section for Approval Section in Design End --%>
        </div>
    </div>
    <div class="GridViewArea relative">
         <div class="makeFullscreen ">
            <span class="fullScreenTitle">Service Contract</span>
            <span class="makeFullscreen-icon half hovered " data-instance="cGrdOrder" title="Maximize Grid" id="expandCgvPurchaseOrder"><i class="fa fa-expand"></i></span>
        <dxe:ASPxGridView ID="GrdOrder" runat="server" KeyFieldName="SlNo" AutoGenerateColumns="False" DataSourceID="EntityServerModeDataSource" SettingsDataSecurity-AllowEdit="false"
            SettingsDataSecurity-AllowInsert="false" SettingsDataSecurity-AllowDelete="false"
            Width="100%" ClientInstanceName="cGrdOrder" OnCustomCallback="GrdOrder_CustomCallback"
            SettingsBehavior-AllowFocusedRow="true" OnHtmlRowPrepared="AvailableStockgrid_HtmlRowPrepared"
            HorizontalScrollBarMode="Auto" Settings-VerticalScrollableHeight="250" Settings-VerticalScrollBarMode="Auto">
            <SettingsSearchPanel Visible="True" Delay="5000" />
            <Columns>
                <dxe:GridViewDataTextColumn Caption="Document No." FieldName="OrderNo"
                    VisibleIndex="1" FixedStyle="Left" Width="140px" Settings-ShowFilterRowMenu="True">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Posting Date" FieldName="Order_Date"
                    VisibleIndex="2" Width="150px" Settings-ShowFilterRowMenu="True">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Customer" FieldName="CustomerName"
                    VisibleIndex="3" Width="300px" Settings-ShowFilterRowMenu="True">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Unit" FieldName="BranchName"
                    VisibleIndex="4" Width="250px" Settings-ShowFilterRowMenu="True">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Project Name" FieldName="Proj_Name"
                    VisibleIndex="5" Width="250px" Settings-ShowFilterRowMenu="True" Settings-AllowAutoFilter="True">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="True" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Amount" FieldName="Amount"
                    VisibleIndex="6" Width="100" Settings-ShowFilterRowMenu="True">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                    <PropertiesTextEdit DisplayFormatString="0.00"></PropertiesTextEdit>
                </dxe:GridViewDataTextColumn>

                <%--<dxe:GridViewDataTextColumn Caption="Challan No" FieldName="ChallanNo"
                    VisibleIndex="6"  Width="150">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Challan Date" FieldName="Challan_Date"
                    VisibleIndex="7"  Width="150" Settings-ShowFilterRowMenu="True">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                </dxe:GridViewDataTextColumn>--%>
                <%--<dxe:GridViewDataTextColumn Caption="Vehicle No" FieldName="VehicleNos"
                    VisibleIndex="6" Width="120">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Vehicle Out Date" FieldName="VehicleOutDateTime"
                    VisibleIndex="7" Width="120">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>--%>
                <%--   <dxe:GridViewDataTextColumn FieldName="VehicleNos" Caption="Vehicle No" VisibleIndex="7" Width="150px">
                    <DataItemTemplate>
                        <a href="javascript:void(0);" onclick="OnAddEditClick(this,'Show~'+'<%# Eval("Order_Id") %>'+'~'+'<%# Eval("InvoiceNo") %>')" style='<%#Eval("MultipleStatusV")%>'>
                            <dxe:ASPxLabel ID="ASPxLabel1" runat="server" Text='<%# Eval("VehicleNos")%>'
                                ToolTip="Invoice Number">
                            </dxe:ASPxLabel>
                        </a>
                        <a href="javascript:void(0);" onclick="OnAddEditClick(this,'Show~'+'<%# Eval("Order_Id") %>'+'~'+'<%# Eval("InvoiceNo") %>')" style='<%#Eval("SingleStatusV")%>'>
                            <dxe:ASPxLabel ID="ASPxLabel2" runat="server" Text='<%# Eval("VehicleNos")%>'
                                ToolTip="Invoice Number">
                            </dxe:ASPxLabel>
                        </a>
                    </DataItemTemplate>
                    <EditFormSettings Visible="False" />
                    <CellStyle Wrap="False" CssClass="text-center">
                    </CellStyle>
                  
                     <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AllowAutoFilter="False" />
                    <HeaderStyle Wrap="False" CssClass="text-center" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn FieldName="VehicleOutDateTime" Caption="Vehicle Date" VisibleIndex="8" Width="120px">
                    <DataItemTemplate>
                        <a href="javascript:void(0);" onclick="OnAddEditDateClick(this,'Show~'+'<%# Eval("Order_Id") %>'+'~'+'<%# Eval("InvoiceDate") %>')" style='<%#Eval("MultipleStatusV")%>'>
                            <dxe:ASPxLabel ID="ASPxLabel3" runat="server" Text='<%# Eval("VehicleOutDateTime")%>'
                                ToolTip="Invoice Date">
                            </dxe:ASPxLabel>
                        </a>
                        <a href="javascript:void(0);" onclick="OnAddEditDateClick(this,'Show~'+'<%# Eval("Order_Id") %>'+'~'+'<%# Eval("InvoiceDate") %>')" style='<%#Eval("SingleStatusV")%>'>
                            <dxe:ASPxLabel ID="ASPxLabel4" runat="server" Text='<%# Eval("VehicleOutDateTime")%>'
                                ToolTip="Invoice Date">
                            </dxe:ASPxLabel>
                        </a>
                    </DataItemTemplate>
                    <EditFormSettings Visible="False" />
                    <CellStyle Wrap="False" CssClass="text-center">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AllowAutoFilter="False" />
                    <HeaderStyle Wrap="False" CssClass="text-center" />
                </dxe:GridViewDataTextColumn>--%>
                <dxe:GridViewDataTextColumn FieldName="InvoiceNo" Caption="Invoice Details" VisibleIndex="7" Width="150px">
                    <DataItemTemplate>
                        <a href="javascript:void(0);" onclick="OnAddEditClick(this,'Show~'+'<%# Eval("Order_Id") %>'+'~'+'<%# Eval("InvoiceNo") %>')" style='<%#Eval("MultipleStatus")%>'>
                            <dxe:ASPxLabel ID="ASPxTextBox2" runat="server" Text='See Invoice'
                                ToolTip="Invoice Number">
                            </dxe:ASPxLabel>
                        </a>
                        <a href="javascript:void(0);" onclick="OnAddEditClick(this,'Show~'+'<%# Eval("Order_Id") %>'+'~'+'<%# Eval("InvoiceNo") %>')" style='<%#Eval("SingleStatus")%>'>
                            <%-- <dxe:ASPxLabel ID="ASPxTextBox3" runat="server" Text='<%# Eval("InvoiceNo")%>'
                                ToolTip="Invoice Number">
                            </dxe:ASPxLabel>--%>
                            <dxe:ASPxLabel ID="ASPxTextBox3" runat="server" Text='See Invoice' ToolTip="Invoice Number">
                            </dxe:ASPxLabel>
                        </a>
                        <a href="javascript:void(0);" style='<%#Eval("NOInvoice")%>'>
                            <dxe:ASPxLabel ID="ASPxLabel1" runat="server" Text='No Invoice'
                                ToolTip="Invoice Number">
                            </dxe:ASPxLabel>
                        </a>
                        <%--  <asp:Label ID="lblSN" runat="server" Text='<%# Eval("InvoiceNo") %>' style='<%#Eval("SingleStatus")%>'></asp:Label>--%>
                    </DataItemTemplate>
                    <EditFormSettings Visible="False" />
                    <CellStyle Wrap="False" CssClass="text-center">
                    </CellStyle>
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                    <%-- <HeaderTemplate>
                        Status
                    </HeaderTemplate>--%>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AllowAutoFilter="False" />
                    <HeaderStyle Wrap="False" CssClass="text-center" />
                </dxe:GridViewDataTextColumn>
                <%--   <dxe:GridViewDataTextColumn FieldName="InvoiceDate" Caption="Invoice Date" VisibleIndex="10" Width="80px">
                    <DataItemTemplate>
                        <a href="javascript:void(0);" onclick="OnAddEditDateClick(this,'Show~'+'<%# Eval("Order_Id") %>'+'~'+'<%# Eval("InvoiceDate") %>')" style='<%#Eval("MultipleStatus")%>'>
                            <dxe:ASPxLabel ID="ASPxTextBox2" runat="server" Text='<%# Eval("InvoiceDate")%>'
                                ToolTip="Invoice Date">
                            </dxe:ASPxLabel>
                        </a>
                        <a href="javascript:void(0);" onclick="OnAddEditDateClick(this,'Show~'+'<%# Eval("Order_Id") %>'+'~'+'<%# Eval("InvoiceDate") %>')" style='<%#Eval("SingleStatus")%>'>
                            <dxe:ASPxLabel ID="ASPxTextBox3" runat="server" Text='<%# Eval("InvoiceDate")%>'
                                ToolTip="Invoice Date">
                            </dxe:ASPxLabel>
                        </a>
                    </DataItemTemplate>
                    <EditFormSettings Visible="False" />
                    <CellStyle Wrap="False" CssClass="text-center">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AllowAutoFilter="False" />
                    <HeaderStyle Wrap="False" CssClass="text-center" />
                </dxe:GridViewDataTextColumn>--%>
                <%--  <dxe:GridViewDataTextColumn Caption="Invoice Net Amount" FieldName="InvoiceNetAmount"
                    VisibleIndex="11" Width="150">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                    <PropertiesTextEdit DisplayFormatString="0.00"></PropertiesTextEdit>
                </dxe:GridViewDataTextColumn>--%>
                <%-- <dxe:GridViewDataTextColumn Caption="Invoice Amount Received" FieldName="InvoiceAmountReceived"
                    VisibleIndex="12" Width="150">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                    <PropertiesTextEdit DisplayFormatString="0.00"></PropertiesTextEdit>
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Invoice Balance Amount" FieldName="InvoiceBalanceAmount"
                    VisibleIndex="13" Width="150">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                    <PropertiesTextEdit DisplayFormatString="0.00"></PropertiesTextEdit>
                </dxe:GridViewDataTextColumn>--%>
                <dxe:GridViewDataTextColumn Caption="Place of Supply[GST]" FieldName="PosState"
                    VisibleIndex="8" Width="150">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                    <PropertiesTextEdit DisplayFormatString="0.00"></PropertiesTextEdit>
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Entered By" FieldName="EnteredBy"
                    VisibleIndex="9" Width="80">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Last Modified On" FieldName="LastModifiedOn"
                    VisibleIndex="10" Width="110">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Updated By" FieldName="UpdatedBy"
                    VisibleIndex="11" Width="100">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Created From" FieldName="CreatedFrom"
                    VisibleIndex="12" Width="120">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Status" FieldName="Doc_status"
                    VisibleIndex="13" Width="120">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>

                <dxe:GridViewDataTextColumn Caption="Bal Quantity" FieldName="BalQty"
                    VisibleIndex="20" Width="0">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Status" FieldName="Status" Width="0"
                    VisibleIndex="21">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="IsCancel" FieldName="IsCancel" Width="0"
                    VisibleIndex="22">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="IsClosed" FieldName="IsClosed" Width="0"
                    VisibleIndex="23">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                 <dxe:GridViewDataTextColumn Caption="Revision No." FieldName="SO_RevisionNo" Width="120"
                    VisibleIndex="24">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                 <dxe:GridViewDataTextColumn Caption="Revision date" FieldName="SO_RevisionDate"
                    VisibleIndex="25" Width="100">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
                 <dxe:GridViewDataTextColumn Caption="Approval Status" FieldName="SO_ApproveStatus" Width="150"
                    VisibleIndex="26">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>

                <%--  End of Rev Sayantani --%>
                <dxe:GridViewDataTextColumn HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="center" VisibleIndex="27" Width="0">
                    <DataItemTemplate>
                        <div class="floatedIcons">
                            <div class='floatedBtnArea'>
                                <% if (rights.CanView)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnViewClick('<%#Eval("Order_Id")%>')" class="" title="">
                                    <span class='ico ColorSix'><i class='fa fa-eye'></i></span><span class='hidden-xs'>View</span></a>
                                <% } %>
                                <% if (rights.CanEdit)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnMoreInfoClick('<%#Eval("Order_Id")%>',<%# Container.VisibleIndex %>)" class="" title=""  style='<%#Eval("SOLastEntryStaus")%>'>

                                    <span class='ico editColor'><i class='fa fa-pencil' aria-hidden='true'></i></span><span class='hidden-xs'>Modify</span></a>  <% } %>


                                   <% if (rights.CanApproved && isApprove)
                           { %>
                          <a href="javascript:void(0);" onclick="OnApproveClick('<%#Eval("Order_Id")%>',<%# Container.VisibleIndex %>)" class="" title="">
                            
                            <span class='ico editColor'><i class='fa fa-check' aria-hidden='true'></i></span><span class='hidden-xs'>Approve/Reject</span>

                        </a><% } %>

                                <% if (rights.CanDelete)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnClickDelete('<%#Eval("Order_Id")%>',<%# Container.VisibleIndex %>)" class="" title=""  style='<%#Eval("SOLastEntryStaus")%>'>
                                    <span class='ico deleteColor'><i class='fa fa-trash' aria-hidden='true'></i></span><span class='hidden-xs'>Delete</span></a>
                                <% } %>

                                <% if (rights.CanCancel)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnCancelClick('<%#Eval("Order_Id")%>',<%# Container.VisibleIndex %>)" class="" title=""  style='<%#Eval("SOLastEntryStaus")%>'>

                                    <span class='ico deleteColor'><i class='fa fa-times' aria-hidden='true'></i></span><span class='hidden-xs'>Cancel Order</span>

                                </a><% } %>

                                <% if (rights.CanClose)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnClosedClick('<%#Eval("Order_Id")%>',<%# Container.VisibleIndex %>)" class="" title=""  style='<%#Eval("SOLastEntryStaus")%>'>

                                    <span class='ico deleteColor'><i class='fa fa-times' aria-hidden='true'></i></span><span class='hidden-xs'>Closed Order</span>

                                </a><% } %>

                                <a href="javascript:void(0);" onclick="OnClickCopy('<%#Eval("Order_Id")%>')" class="" title=" " style="display: none">
                                    <span class='ico ColorFour'><i class='fa fa-files-o'></i></span><span class='hidden-xs'>Copy</span>
                                </a>
                                <a href="javascript:void(0);" onclick="OnClickStatus('<%#Eval("Order_Id")%>')" class="" title="" style="display: none">
                                    <span class='ico ColorFive'><i class='fa fa-check'></i></span><span class='hidden-xs'>Status</span></a>
                                <% if (rights.CanView)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnclickViewAttachment('<%#Eval("Order_Id")%>')" class="" title="">
                                    <span class='ico ColorSix'><i class='fa fa-paperclip'></i></span><span class='hidden-xs'>Add/View Attachment</span>
                                </a>
                                <% } %>
                                <% if (rights.CanPrint)
                                   { %>
                                <a href="javascript:void(0);" onclick="onPrintJv('<%#Eval("Order_Id")%>')" class="" title="">
                                    <span class='ico ColorSeven'><i class='fa fa-print'></i></span><span class='hidden-xs'>Print</span>
                                </a><%} %>

                                 <% if (rights.CanUpdateTransporter)
                                { %>
                            <a href="javascript:void(0);" onclick="UpdateTransporter('<%#Eval("Order_Id")%>')" class="" title="" >
                                <span class='ico editColor'><i class='fa fa-pencil' aria-hidden='true'></i></span><span class='hidden-xs'>Update Transporter</span></a>
                            <% } %>
                            </div>
                        </div>
                    </DataItemTemplate>
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                    <CellStyle HorizontalAlign="Center"></CellStyle>
                    <HeaderTemplate><span>Actions</span></HeaderTemplate>
                    <EditFormSettings Visible="False"></EditFormSettings>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                </dxe:GridViewDataTextColumn>
            </Columns>
            <%-- --Rev Sayantani--%>
            <SettingsBehavior ConfirmDelete="true" EnableCustomizationWindow="true" EnableRowHotTrack="true" />
            <SettingsCookies Enabled="true" StorePaging="true" StoreColumnsVisiblePosition="true" Version="3.2" />
            <%-- -- End of Rev Sayantani --%>
            <SettingsContextMenu Enabled="true"></SettingsContextMenu>
            <ClientSideEvents EndCallback="function (s, e) {grid_EndCallBack();}" RowClick="gridRowclick" />
            <SettingsPager NumericButtonCount="10" PageSize="10" ShowSeparators="True" Mode="ShowPager">
                <PageSizeItemSettings Visible="true" ShowAllItem="false" Items="10,50,100,150,200" />
                <FirstPageButton Visible="True">
                </FirstPageButton>
                <LastPageButton Visible="True">
                </LastPageButton>
            </SettingsPager>
            <%--<SettingsSearchPanel Visible="True" />--%>
            <Settings ShowGroupPanel="True" ShowFooter="true" ShowGroupFooter="VisibleIfExpanded" ShowStatusBar="Hidden" ShowHorizontalScrollBar="true" ShowFilterRow="true" ShowFilterRowMenu="false" />
            <SettingsLoadingPanel Text="Please Wait..." />
            <TotalSummary>
                <dxe:ASPxSummaryItem FieldName="Amount" SummaryType="Sum" />
            </TotalSummary>
        </dxe:ASPxGridView>
        <asp:HiddenField ID="hiddenedit" runat="server" />
        <dx:LinqServerModeDataSource ID="EntityServerModeDataSource" runat="server" OnSelecting="EntityServerModeDataSource_Selecting"
            ContextTypeName="ERPDataClassesDataContext" TableName="v_GetSalesOrderEntityList" />

              <uc1:VehicleDetailsControl runat="server" ID="VehicleDetailsControl" />
        </div>
    </div>
    <div style="display: none">
        <dxe:ASPxGridViewExporter ID="exporter" runat="server" Landscape="false" PaperKind="A4" PageHeader-Font-Size="Larger" PageHeader-Font-Bold="true">
        </dxe:ASPxGridViewExporter>
    </div>

    <%--DEBASHIS--%>
    <div class="PopUpArea">

        <dxe:ASPxPopupControl ID="Popup_Feedback" runat="server" ClientInstanceName="cPopup_Feedback"
            Width="400px" HeaderText="Reason For Cancel" PopupHorizontalAlign="WindowCenter"
            BackColor="white" Height="100px" PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
            Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                    <%--<div style="Width:400px;background-color:#FFFFFF;margin:0px;border:1px solid red;">--%>
                    <div class="Top clearfix">

                        <table style="width: 94%">

                            <tr>
                                <td>Reason<span style="color: red">*</span></td>
                                <td class="relative">
                                    <dxe:ASPxMemo ID="txtInstFeedback" runat="server" Width="100%" Height="50px" ClientInstanceName="txtFeedback"></dxe:ASPxMemo>
                                    <span id="MandatoryRemarksFeedback" style="display: none">
                                        <img id="gridHistory_DXPEForm_efnew_DXEFL_DXEditor1234_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory"></span>
                                </td>
                            </tr>

                            <tr>
                                <td></td>
                                <td colspan="2" style="padding-top: 10px;">
                                    <input id="btnFeedbackSave" class="btn btn-primary" onclick="CallFeedback_save()" type="button" value="Save" />&nbsp;&nbsp;
                                    <input id="btnFeedbackCancel" class="btn btn-danger" onclick="CancelFeedback_save()" type="button" value="Cancel" />
                                </td>

                            </tr>
                        </table>


                    </div>

                </dxe:PopupControlContentControl>
            </ContentCollection>
            <HeaderStyle BackColor="LightGray" ForeColor="Black" />


        </dxe:ASPxPopupControl>


        <dxe:ASPxPopupControl ID="Popup_Closed" runat="server" ClientInstanceName="cPopup_Closed"
            Width="400px" HeaderText="Reason For Close" PopupHorizontalAlign="WindowCenter"
            BackColor="white" Height="100px" PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
            Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                    <%--<div style="Width:400px;background-color:#FFFFFF;margin:0px;border:1px solid red;">--%>
                    <div class="Top clearfix">
                        <table style="width: 94%">
                            <tr>
                                <td>Reason<span style="color: red">*</span></td>
                                <td class="relative">
                                    <dxe:ASPxMemo ID="ASPxMemo1" runat="server" Width="100%" Height="50px" ClientInstanceName="txtClosed"></dxe:ASPxMemo>
                                    <span id="MandatoryRemarksFeedback1" style="display: none">
                                        <img id="gridHistory_DXPEForm_efnew_DXEFL_DXEditor1234_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory"></span>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td colspan="2" style="padding-top: 10px;">
                                    <input id="btnClosedSave" class="btn btn-primary" onclick="CallClosed_save()" type="button" value="Save" />&nbsp;&nbsp;
                                    <input id="btnClosedCancel" class="btn btn-danger" onclick="CancelClosed_save()" type="button" value="Cancel" />
                                </td>
                            </tr>
                        </table>
                    </div>

                </dxe:PopupControlContentControl>
            </ContentCollection>
            <HeaderStyle BackColor="LightGray" ForeColor="Black" />


        </dxe:ASPxPopupControl>


        <dxe:ASPxPopupControl ID="ASPxDocumentsPopup" runat="server" ClientInstanceName="cDocumentsPopup"
            Width="350px" HeaderText="Select Design(s)" PopupHorizontalAlign="WindowCenter"
            PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
            Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                    <dxe:ASPxCallbackPanel runat="server" ID="SelectPanel" ClientInstanceName="cSelectPanel" OnCallback="SelectPanel_Callback" ClientSideEvents-EndCallback="cSelectPanelEndCall">
                        <PanelCollection>
                            <dxe:PanelContent runat="server">
                                <dxe:ASPxComboBox ID="CmbDesignName" ClientInstanceName="cCmbDesignName" runat="server" ValueType="System.String" Width="100%" EnableSynchronization="True">
                                </dxe:ASPxComboBox>
                                <div class="text-center pTop10">
                                    <dxe:ASPxButton ID="btnOK" ClientInstanceName="cbtnOK" runat="server" AutoPostBack="False" Text="OK" CssClass="btn btn-primary" meta:resourcekey="btnSaveRecordsResource1" UseSubmitBehavior="False">
                                        <ClientSideEvents Click="function(s, e) {return PerformCallToGridBind();}" />
                                    </dxe:ASPxButton>
                                </div>
                            </dxe:PanelContent>
                        </PanelCollection>
                    </dxe:ASPxCallbackPanel>
                </dxe:PopupControlContentControl>
            </ContentCollection>
        </dxe:ASPxPopupControl>
    </div>
    <%-- Sandip Approval Dtl Section Start--%>

    <div class="PopUpArea">

        <dxe:ASPxPopupControl ID="popup_OrderWait" runat="server" Width="1200"
            CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="Cpopup_OrderWait"
            HeaderText="Order Waiting" AllowResize="false" ResizingMode="Postponed">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">

                    <div onkeypress="OnWaitingGridKeyPress(event)">

                        <dxe:ASPxGridView ID="watingOrdergrid" runat="server" KeyFieldName="SBMain_Id" AutoGenerateColumns="False"
                            Width="100%" ClientInstanceName="CwatingOrdergrid" OnCustomCallback="watingOrdergrid_CustomCallback" KeyboardSupport="true"
                            SettingsBehavior-AllowSelectSingleRowOnly="true" OnDataBinding="watingOrdergrid_DataBinding" SettingsBehavior-AllowFocusedRow="true"
                            Settings-VerticalScrollBarMode="auto" Settings-VerticalScrollableHeight="320">
                            <Columns>
                                <%--<dxe:GridViewCommandColumn ShowSelectCheckbox="true" VisibleIndex="0" Width="60" Caption="Select" />--%>

                                <dxe:GridViewDataTextColumn Caption="Salesman" FieldName="Salesman"
                                    VisibleIndex="0" FixedStyle="Left">
                                    <CellStyle CssClass="gridcellleft" Wrap="true">
                                    </CellStyle>
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataTextColumn>

                                <dxe:GridViewDataTextColumn Caption="Unit" FieldName="Branch"
                                    VisibleIndex="0" FixedStyle="Left">
                                    <CellStyle CssClass="gridcellleft" Wrap="true">
                                    </CellStyle>
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataTextColumn>


                                <dxe:GridViewDataTextColumn Caption="Customer" FieldName="Name"
                                    VisibleIndex="1" FixedStyle="Left">
                                    <CellStyle CssClass="gridcellleft" Wrap="true">
                                    </CellStyle>
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataTextColumn>


                                <dxe:GridViewDataTextColumn Caption="Product List" FieldName="ProductList"
                                    VisibleIndex="1" FixedStyle="Left" Width="180px">
                                    <CellStyle CssClass="gridcellleft" Wrap="true">
                                    </CellStyle>
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataTextColumn>




                                <dxe:GridViewDataTextColumn Caption="Billing Amount" FieldName="finalAmount"
                                    VisibleIndex="2" FixedStyle="Left">
                                    <CellStyle CssClass="gridcellleft" Wrap="true">
                                    </CellStyle>
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataTextColumn>


                                <dxe:GridViewDataTextColumn Caption="Billing Date" FieldName="Billingdate"
                                    VisibleIndex="2" FixedStyle="Left">
                                    <CellStyle CssClass="gridcellleft" Wrap="true">
                                    </CellStyle>
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataTextColumn>

                                <dxe:GridViewDataTextColumn Caption="Payment Type" FieldName="Paymenttype"
                                    VisibleIndex="2" FixedStyle="Left">
                                    <CellStyle CssClass="gridcellleft" Wrap="true">
                                    </CellStyle>
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataTextColumn>

                                <dxe:GridViewDataTextColumn HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="center" VisibleIndex="17" Width="60">
                                    <DataItemTemplate>
                                        <% if (rights.CanDelete)
                                           { %>
                                        <a href="javascript:void(0);" onclick="RemoveQuote('<%# Container.KeyValue %>')" class="pad" title="Remove">
                                            <%--   <img src="../../../assests/images/Delete.png" />--%>
                                            <i class="fa fa-close" aria-hidden="true" id="CloseRemoveWattingBtn" style="font-size: 19px; color: #f35248;"></i>
                                        </a>
                                        <%} %>
                                    </DataItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    <CellStyle HorizontalAlign="Center"></CellStyle>
                                    <HeaderTemplate><span>Actions</span></HeaderTemplate>
                                    <EditFormSettings Visible="False"></EditFormSettings>
                                </dxe:GridViewDataTextColumn>

                            </Columns>

                            <ClientSideEvents RowClick="ListRowClicked" EndCallback="watingOrdergridEndCallback" />

                            <SettingsPager NumericButtonCount="20" PageSize="10" ShowSeparators="True" Mode="ShowPager">
                                <FirstPageButton Visible="True">
                                </FirstPageButton>
                                <LastPageButton Visible="True">
                                </LastPageButton>
                            </SettingsPager>
                            <SettingsSearchPanel Visible="True" />
                            <Settings ShowGroupPanel="False" ShowStatusBar="Hidden" ShowHorizontalScrollBar="False" ShowFilterRow="true" ShowFilterRowMenu="true" />
                            <SettingsLoadingPanel Text="Please Wait..." />
                        </dxe:ASPxGridView>
                    </div>


                    <dxe:ASPxButton ID="OrderWattingOk" runat="server" AutoPostBack="false" Text="O&#818;k" CssClass="btn btn-primary okClass"
                        ClientSideEvents-Click="OrderWattingOkClick" UseSubmitBehavior="False" />
                </dxe:PopupControlContentControl>
            </ContentCollection>
        </dxe:ASPxPopupControl>

    </div>
    <dxe:ASPxTimer runat="server" Interval="3600000" ClientInstanceName="Timer1">
        <ClientSideEvents Tick="timerTick" />
    </dxe:ASPxTimer>
    <asp:HiddenField ID="waitingOrderCount" runat="server" />
    <div class="PopUpArea">
        <%-- <button class="btn btn-primary" onclick="ApproveAll();">Approve Selection</button>
        <button class="btn btn-primary" onclick="RejectAll();">Reject Selection</button>--%>

        <dxe:ASPxPopupControl ID="popupApproval" runat="server" ClientInstanceName="cpopupApproval"
            Width="900px" HeaderText="Pending Approvals" PopupHorizontalAlign="WindowCenter" HeaderStyle-CssClass="wht"
            PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
            Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True"
            ContentStyle-CssClass="pad">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                    <%--<div style="Width:400px;background-color:#FFFFFF;margin:0px;border:1px solid red;">--%>
                    <div class="row">
                        <div class="col-md-12">
                            <dxe:ASPxGridView ID="gridPendingApproval" runat="server" KeyFieldName="ID" AutoGenerateColumns="False" OnPageIndexChanged="gridPendingApproval_PageIndexChanged"
                                Width="100%" ClientInstanceName="cgridPendingApproval" OnCustomCallback="gridPendingApproval_CustomCallback"
                                SettingsBehavior-ColumnResizeMode="Control" Settings-VerticalScrollBarMode="Auto" Settings-VerticalScrollableHeight="350">
                                <Columns>
                                    <%-- <dxe:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0" Width="60" Caption=" " />--%>
                                    <dxe:GridViewDataTextColumn Caption="Document No." FieldName="Number"
                                        VisibleIndex="0" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="Party Name" FieldName="customer"
                                        VisibleIndex="0" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="Posting Date" FieldName="CreateDate"
                                        VisibleIndex="1" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>
                                    <dxe:GridViewDataTextColumn Caption="Unit" FieldName="branch_description"
                                        VisibleIndex="2" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>
                                    <dxe:GridViewDataTextColumn Caption="Entered By" FieldName="craetedby"
                                        VisibleIndex="3" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataCheckColumn UnboundType="Boolean" Caption="Approved">
                                        <DataItemTemplate>
                                            <dxe:ASPxCheckBox ID="chkapprove" runat="server" AllowGrayed="false" OnInit="chkapprove_Init" ValueType="System.Boolean" ValueChecked="true" ValueUnchecked="false">
                                                <%--<ClientSideEvents CheckedChanged="function (s, e) {ch_fnApproved();}" />--%>
                                            </dxe:ASPxCheckBox>
                                        </DataItemTemplate>
                                        <Settings ShowFilterRowMenu="False" AllowFilterBySearchPanel="False" AllowAutoFilter="False" />
                                    </dxe:GridViewDataCheckColumn>

                                    <dxe:GridViewDataCheckColumn UnboundType="Boolean" Caption="Rejected">
                                        <DataItemTemplate>
                                            <dxe:ASPxCheckBox ID="chkreject" runat="server" AllowGrayed="false" OnInit="chkreject_Init" ValueType="System.Boolean" ValueChecked="true" ValueUnchecked="false">
                                                <%--<ClientSideEvents CheckedChanged="function (s, e) {ch_fnApproved();}" />--%>
                                            </dxe:ASPxCheckBox>
                                        </DataItemTemplate>
                                        <Settings ShowFilterRowMenu="False" AllowFilterBySearchPanel="False" AllowAutoFilter="False" />
                                    </dxe:GridViewDataCheckColumn>
                                </Columns>
                                <SettingsBehavior AllowFocusedRow="true" />
                                <SettingsPager NumericButtonCount="20" PageSize="50" ShowSeparators="True">
                                    <FirstPageButton Visible="True">
                                    </FirstPageButton>
                                    <LastPageButton Visible="True">
                                    </LastPageButton>
                                </SettingsPager>
                                <SettingsEditing Mode="Inline">
                                </SettingsEditing>
                                <SettingsSearchPanel Visible="True" />
                                <Settings ShowGroupPanel="True" ShowStatusBar="Hidden" ShowHorizontalScrollBar="False" ShowFilterRow="true" ShowFilterRowMenu="true" />
                                <SettingsLoadingPanel Text="Please Wait..." />
                                <ClientSideEvents EndCallback="OnApprovalEndCall" />
                            </dxe:ASPxGridView>
                        </div>
                        <div class="clear"></div>


                        <%--<div class="col-md-12" style="padding-top: 10px;">
                            <dxe:ASPxButton ID="ASPxButton1" CausesValidation="true" ClientInstanceName="cbtn_PrpformaStatus" runat="server"
                                AutoPostBack="False" Text="Save" CssClass="btn btn-primary">
                                <ClientSideEvents Click="function (s, e) {SaveApprovalStatus();}" />
                            </dxe:ASPxButton>
                        </div>--%>
                    </div>
                </dxe:PopupControlContentControl>
            </ContentCollection>
        </dxe:ASPxPopupControl>
        <dxe:ASPxPopupControl ID="ASPXPopupControl" runat="server" ClientInstanceName="popup"
            CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Height="630px"
            Width="1200px" HeaderText="Quotation Approval" Modal="true" AllowResize="true" ResizingMode="Postponed">
            <HeaderTemplate>
                <span>User Approval</span>
                <div class="closeApprove" onclick="closeUserApproval();"><i class="fa fa-close"></i></div>
            </HeaderTemplate>
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                </dxe:PopupControlContentControl>
            </ContentCollection>
        </dxe:ASPxPopupControl>
        <dxe:ASPxPopupControl ID="PopupUserWiseQuotation" runat="server" ClientInstanceName="cPopupUserWiseQuotation"
            Width="900px" HeaderText="User Wise Service Contract Status" PopupHorizontalAlign="WindowCenter" HeaderStyle-CssClass="wht"
            PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
            Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True"
            ContentStyle-CssClass="pad">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                    <div class="row">
                        <div class="col-md-12">

                            <dxe:ASPxGridView ID="gridUserWiseQuotation" runat="server" KeyFieldName="ID" AutoGenerateColumns="False"
                                Width="100%" ClientInstanceName="cgridUserWiseQuotation" OnCustomCallback="gridUserWiseQuotation_CustomCallback" OnPageIndexChanged="gridUserWiseQuotation_PageIndexChanged">
                                <Columns>
                                    <dxe:GridViewDataTextColumn Caption="Branch" FieldName="Branch"
                                        VisibleIndex="0" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                        <Settings AllowAutoFilterTextInputTimer="true" />
                                    </dxe:GridViewDataTextColumn>
                                    <dxe:GridViewDataTextColumn Caption="Service Contract No." FieldName="number"
                                        VisibleIndex="1" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                        <Settings AllowAutoFilterTextInputTimer="true" />
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="Date" FieldName="OrderedDate"
                                        VisibleIndex="2" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                        <Settings AllowAutoFilterTextInputTimer="true" />
                                    </dxe:GridViewDataTextColumn>
                                    <dxe:GridViewDataTextColumn Caption="Customer" FieldName="Customer"
                                        VisibleIndex="2" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                        <Settings AllowAutoFilterTextInputTimer="true" />
                                    </dxe:GridViewDataTextColumn>
                                    <dxe:GridViewDataTextColumn Caption="Approval User" FieldName="approvedby"
                                        VisibleIndex="3" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                        <Settings AllowAutoFilterTextInputTimer="false" />
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="User Level" FieldName="UserLevel"
                                        VisibleIndex="4" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="Status" FieldName="status"
                                        VisibleIndex="5" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                        <Settings AllowAutoFilterTextInputTimer="true" />
                                    </dxe:GridViewDataTextColumn>

                                </Columns>
                                <SettingsBehavior AllowFocusedRow="true" />
                                <SettingsPager NumericButtonCount="20" PageSize="10" ShowSeparators="True">
                                    <FirstPageButton Visible="True">
                                    </FirstPageButton>
                                    <LastPageButton Visible="True">
                                    </LastPageButton>
                                </SettingsPager>
                                <SettingsEditing Mode="Inline">
                                </SettingsEditing>
                                <SettingsSearchPanel Visible="True" />
                                <Settings ShowGroupPanel="True" ShowStatusBar="Hidden" ShowHorizontalScrollBar="False" ShowFilterRow="true" ShowFilterRowMenu="true" />
                                <SettingsLoadingPanel Text="Please Wait..." />

                            </dxe:ASPxGridView>
                        </div>
                        <div class="clear"></div>
                    </div>
                </dxe:PopupControlContentControl>
            </ContentCollection>
        </dxe:ASPxPopupControl>
    </div>
    <%-- Sandip Approval Dtl Section End--%>

    <%--Kaushik--%>

    <div class="PopUpArea">
        <dxe:ASPxPopupControl ID="ASPxInvoiceDocumentsPopup" runat="server" ClientInstanceName="cInvoiceDocumentsPopup"
            Width="350px" HeaderText="Select Design(s)" PopupHorizontalAlign="WindowCenter"
            PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
            Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">

                    <dxe:ASPxCallbackPanel runat="server" ID="SelectInvoicePanel" ClientInstanceName="cInvoiceSelectPanel" OnCallback="SelectInvoicePanel_Callback" ClientSideEvents-EndCallback="cInvoiceSelectPanelEndCall">
                        <PanelCollection>
                            <dxe:PanelContent runat="server">

                                <dxe:ASPxCheckBox ID="selectOriginal" Text="Original For Recipient" runat="server" ToolTip="Select Original"
                                    ClientInstanceName="CselectOriginal">
                                </dxe:ASPxCheckBox>
                                <dxe:ASPxCheckBox ID="selectDuplicate" Text="Duplicate For Transporter" runat="server" ToolTip="Select Duplicate"
                                    ClientInstanceName="CselectDuplicate">
                                </dxe:ASPxCheckBox>
                                <dxe:ASPxCheckBox ID="selectTriplicate" Text="Triplicate For Supplier" runat="server" ToolTip="Select Triplicate"
                                    ClientInstanceName="CselectTriplicate">
                                </dxe:ASPxCheckBox>
                                <dxe:ASPxCheckBox ID="selectOfficecopy" Text="Extra/Office Copy" runat="server" ToolTip="Select Office Copy"
                                    ClientInstanceName="CselectOfficecopy">
                                </dxe:ASPxCheckBox>

                                <dxe:ASPxComboBox ID="CmbInvoiceDesignName" ClientInstanceName="cInvoiceCmbDesignName" runat="server" ValueType="System.String" Width="100%" EnableSynchronization="True">
                                </dxe:ASPxComboBox>

                                <div class="text-center pTop10">
                                    <dxe:ASPxButton ID="btnInvoiceOK" ClientInstanceName="cInvoicebtnOK" runat="server" AutoPostBack="False" Text="OK" CssClass="btn btn-primary" meta:resourcekey="btnSaveRecordsResource1" UseSubmitBehavior="False">
                                        <ClientSideEvents Click="function(s, e) {return PerformInvoiceCallToGridBind();}" />
                                    </dxe:ASPxButton>

                                </div>




                            </dxe:PanelContent>
                        </PanelCollection>
                    </dxe:ASPxCallbackPanel>



                </dxe:PopupControlContentControl>

            </ContentCollection>
        </dxe:ASPxPopupControl>
    </div>
    <%--Kaushik--%>

    <dxe:ASPxGlobalEvents ID="GlobalEvents" runat="server">
        <ClientSideEvents ControlsInitialized="AllControlInitilize" />
    </dxe:ASPxGlobalEvents>


    <dxe:ASPxPopupControl ID="InvoiceNumberpopup" ClientInstanceName="cInvoiceNumberpopup" runat="server"
        AllowDragging="False" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" HeaderText="Invoice Number Detail"
        EnableHotTrack="False" BackColor="#DDECFE" Width="850px" CloseAction="CloseButton">
        <ContentCollection>
            <dxe:PopupControlContentControl runat="server">
                <dxe:ASPxCallbackPanel ID="InvoiceNumberpanel" runat="server" Width="650px" ClientInstanceName="popInvoiceNumberPanel"
                    OnCallback="InvoiceNumberpanel_Callback">
                    <PanelCollection>
                        <dxe:PanelContent runat="server">
                            <div>
                                <dxe:ASPxGridView ID="grdInvoiceNumber" runat="server" KeyFieldName="id" AutoGenerateColumns="False"
                                    Width="100%" ClientInstanceName="cpbInvoiceNumber" OnDataBinding="InvoiceNumberpanel_DataBinding">
                                    <Columns>
                                        <dxe:GridViewDataTextColumn Caption="Invoice Number" FieldName="Invoice_Number" HeaderStyle-CssClass="text-left"
                                            VisibleIndex="0" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-left" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>
                                        <dxe:GridViewDataTextColumn Caption="Invoice Date" FieldName="Invoice_Date" HeaderStyle-CssClass="text-center"
                                            VisibleIndex="1" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>
                                        <dxe:GridViewDataTextColumn Caption="Invoice Net Amt" FieldName="NetAmount" HeaderStyle-CssClass="right"
                                            VisibleIndex="2" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>

                                        <dxe:GridViewDataTextColumn Caption="Invoice Amt Received" FieldName="AmountReceived" HeaderStyle-CssClass="right"
                                            VisibleIndex="3" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>

                                        <dxe:GridViewDataTextColumn Caption="Invoice Balance Amt" FieldName="BalanceAmount" HeaderStyle-CssClass="right"
                                            VisibleIndex="4" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>
                                        <dxe:GridViewDataTextColumn Caption="Vehicle No" FieldName="VehicleNos" HeaderStyle-CssClass="right"
                                            VisibleIndex="5" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>
                                        <dxe:GridViewDataTextColumn Caption="Vehicle Out Date" FieldName="VehicleOutDateTime" HeaderStyle-CssClass="right"
                                            VisibleIndex="6" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>
                                        <dxe:GridViewDataTextColumn Caption="Type" FieldName="Type" HeaderStyle-CssClass="right"
                                            VisibleIndex="7" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>


                                        <dxe:GridViewDataTextColumn Caption="Print" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="center" VisibleIndex="8" Width="240px">
                                            <DataItemTemplate>

                                                <a href="javascript:void(0);" onclick="onPrintSi('<%#Eval("Invoice_Id")%>','<%#Eval("Type")%>')" class="pad" title="print" style='<%#Eval("PrintButton")%>'>
                                                    <img src="../../../assests/images/Print.png" />
                                                </a>
                                            </DataItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <CellStyle HorizontalAlign="Center"></CellStyle>
                                            <HeaderTemplate><span>Print</span></HeaderTemplate>
                                            <EditFormSettings Visible="False"></EditFormSettings>
                                        </dxe:GridViewDataTextColumn>
                                    </Columns>
                                </dxe:ASPxGridView>
                            </div>
                        </dxe:PanelContent>
                    </PanelCollection>
                </dxe:ASPxCallbackPanel>
            </dxe:PopupControlContentControl>
        </ContentCollection>
        <HeaderStyle HorizontalAlign="Left">
            <Paddings PaddingRight="6px" />
        </HeaderStyle>
        <SizeGripImage Height="16px" Width="16px" />
        <CloseButtonImage Height="12px" Width="13px" />
        <ClientSideEvents CloseButtonClick="function(s, e) {
popup.Hide();
}" />
    </dxe:ASPxPopupControl>

    <dxe:ASPxPopupControl ID="InvoiceDatepopup" ClientInstanceName="cInvoiceDatepopup" runat="server"
        AllowDragging="True" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" HeaderText="Invoice Date Detail"
        EnableHotTrack="False" BackColor="#DDECFE" Width="850px" CloseAction="CloseButton">
        <ContentCollection>
            <dxe:PopupControlContentControl runat="server">
                <dxe:ASPxCallbackPanel ID="InvoiceDatepanel" runat="server" Width="650px" ClientInstanceName="popInvoiceDatePanel"
                    OnCallback="InvoiceDatepanel_Callback">
                    <PanelCollection>
                        <dxe:PanelContent runat="server">
                            <div>
                                <dxe:ASPxGridView ID="grdInvoiceDate" runat="server" KeyFieldName="id" AutoGenerateColumns="False"
                                    Width="100%" ClientInstanceName="cpbInvoiceDate" OnDataBinding="InvoiceDatepanel_DataBinding">
                                    <Columns>
                                        <dxe:GridViewDataTextColumn Caption="Invoice Number" FieldName="Invoice_Number" HeaderStyle-CssClass="text-left"
                                            VisibleIndex="0" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-left" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>
                                        <dxe:GridViewDataTextColumn Caption="Invoice Date" FieldName="Invoice_Date" HeaderStyle-CssClass="text-center"
                                            VisibleIndex="1" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>

                                        <dxe:GridViewDataTextColumn Caption="Invoice Net Amt" FieldName="NetAmount" HeaderStyle-CssClass="right"
                                            VisibleIndex="2" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>

                                        <dxe:GridViewDataTextColumn Caption="Invoice Amt Received" FieldName="AmountReceived" HeaderStyle-CssClass="right"
                                            VisibleIndex="3" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>

                                        <dxe:GridViewDataTextColumn Caption="Invoice Balance Amt" FieldName="BalanceAmount" HeaderStyle-CssClass="right"
                                            VisibleIndex="4" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>
                                        <dxe:GridViewDataTextColumn Caption="Vehicle No" FieldName="VehicleNos" HeaderStyle-CssClass="right"
                                            VisibleIndex="5" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>
                                        <dxe:GridViewDataTextColumn Caption="Vehicle Out Date" FieldName="VehicleOutDateTime" HeaderStyle-CssClass="right"
                                            VisibleIndex="6" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>
                                        <dxe:GridViewDataTextColumn Caption="Type" FieldName="Type" HeaderStyle-CssClass="right"
                                            VisibleIndex="7" FixedStyle="Left" Width="100px">
                                            <CellStyle CssClass="text-center" Wrap="true">
                                            </CellStyle>
                                            <Settings AutoFilterCondition="Contains" />
                                        </dxe:GridViewDataTextColumn>

                                        <dxe:GridViewDataTextColumn Caption="Print" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="center" VisibleIndex="8" Width="240px">
                                            <DataItemTemplate>
                                                <a href="javascript:void(0);" onclick="onPrintSi('<%#Eval("Invoice_Id")%>','<%#Eval("Type")%>')" class="pad" title="print">
                                                    <img src="../../../assests/images/Print.png" />
                                                </a>
                                            </DataItemTemplate>
                                        </dxe:GridViewDataTextColumn>
                                    </Columns>
                                </dxe:ASPxGridView>
                            </div>
                        </dxe:PanelContent>
                    </PanelCollection>
                </dxe:ASPxCallbackPanel>
            </dxe:PopupControlContentControl>
        </ContentCollection>
        <HeaderStyle HorizontalAlign="Left">
            <Paddings PaddingRight="6px" />
        </HeaderStyle>
        <SizeGripImage Height="16px" Width="16px" />
        <CloseButtonImage Height="12px" Width="13px" />
        <ClientSideEvents CloseButtonClick="function(s, e) {
popup.Hide();
}" />
    </dxe:ASPxPopupControl>

    <div>
        <asp:HiddenField ID="hfIsFilter" runat="server" />
        <asp:HiddenField ID="hfFromDate" runat="server" />
        <asp:HiddenField ID="hfToDate" runat="server" />
        <asp:HiddenField ID="hfBranchID" runat="server" />
        <asp:HiddenField ID="hdnIsUserwiseFilter" runat="server" />
        <asp:HiddenField ID="hddnKeyValue" runat="server" />
        <asp:HiddenField ID="hddnCancelCloseFlag" runat="server" />
        <asp:HiddenField ID="hddnPrintButton" runat="server" />

         <asp:HiddenField ID="hdnOrderID" runat="server" />
    </div>
</asp:Content>

