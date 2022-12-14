﻿<%@ Page Title="Estimate / Cost Sheet" Language="C#" MasterPageFile="~/OMS/MasterPage/ERP.Master" AutoEventWireup="true"
    CodeBehind="EstimateCostSheetList.aspx.cs" Inherits="ERP.OMS.Management.Activities.frm_EstimateCostSheetMain" %>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Data.Linq" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="JS/EstimateCostSheetList.js?v?3.1"></script>
    <link href="CSS/SalesQuotaion.css?1.0.1" rel="stylesheet" />
    <link href="CSS/PosSalesInvoice.css" rel="stylesheet" />
    <script>
        function OnMoreInfoClick(keyValue) {
            var url = 'EstimateCostSheet.aspx?key=' + keyValue + '&type=ECS';
            window.location.href = url;
        }


        function OnApproveClick(keyValue, visibleIndex) {
           
            cGrdEstimateCost.SetFocusedRowIndex(visibleIndex);
            var url = 'EstimateCostSheet.aspx?key=' + keyValue + '&type=ECS&type1=ACECS';
            window.location.href = url;
           
            <%--var IsClosedFlag = cGrdEstimateCost.GetRow(cGrdEstimateCost.GetFocusedRowIndex()).children[5].innerText;
            if (IsClosedFlag.trim() != 'Closed') {
                var ActiveUser = '<%=Session["userid"]%>'
                if (ActiveUser != null) {
                    $.ajax({
                        type: "POST",
                        url: "SalesQuotationList.aspx/GetEditablePermissions",
                        data: "{'ActiveUser':'" + ActiveUser + "','SalesDocId':'" + keyValue + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: false,//Added By:Subhabrata	
                        success: function (msg) {
                            //debugger;	
                            var status = msg.d;
                            var url = 'SalesQuotation.aspx?key=' + keyValue + '&Permission=' + status + '&type=QO' + '&type1=QN';
                            window.location.href = url;
                        }
                    });
                }
            }
            else {
                jAlert("Sales Quotation is " + IsClosedFlag.trim() + ".Approve is not allowed.");
            }--%>
        }

        //Rev Rajdip
        //Start For Copy
        function fn_CopySalesQuotation(keyValue) {
            var ActiveUser = '<%=Session["userid"]%>'
            if (ActiveUser != null) {
                $.ajax({
                    type: "POST",
                    url: "SalesQuotationList.aspx/GetEditablePermissions",
                    //data: "{'ActiveUser':'" + ActiveUser + "'}",
                    data: "{'ActiveUser':'" + ActiveUser + "','SalesDocId':'" + keyValue + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        var status = msg.d;
                        var url = 'SalesQuotation.aspx?key=' + keyValue + '&Permission=' + status + '&type=QO&Typenew=Copy';
                        window.location.href = url;
                    }
                });
            }

        }
    </script>
    <style>
        strong label {
            font-weight: bold !important;
        }

        input[type="radio"] {
            webkit-transform: translateY(3px);
            -moz-transform: translateY(3px);
            transform: translateY(3px);
        }
    </style>
    <style>
        .smllpad > tbody > tr > td {
            padding-right: 25px;
        }

        .errorField {
            position: absolute;
            right: 5px;
            top: 9px;
        }

        .padTab {
            margin-bottom: 4px;
        }

            .padTab > tbody > tr > td {
                padding-right: 15px;
                vertical-align: middle;
            }

                .padTab > tbody > tr > td > label, .padTab > tbody > tr > td > input[type="button"] {
                    margin-bottom: 0 !important;
                }

        .backBranch {
            font-weight: 600;
            background: #75c1f5;
            padding: 5px;
        }

        .btn.typeNotificationBtn {
            position: relative;
            padding-right: 16px !important;
        }

        .typeNotification {
            position: absolute;
            width: 22px;
            height: 22px;
            background: #ff5140;
            display: block;
            font-size: 12px;
            border-radius: 50%;
            right: -7px;
            top: -9px;
            line-height: 22px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="panel-heading clearfix">
        <div class="panel-title pull-left">
            <h3>Estimate / Cost Sheet</h3>
        </div>
        <table class="padTab pull-right" style="margin-top: 7px;">
            <tr>
                <td>
                    <label>From </label>
                </td>
                <td style="width: 150px">
                    <dxe:ASPxDateEdit ID="FormDate" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" ClientInstanceName="cFormDate" Width="100%" UseMaskBehavior="True">
                        <ButtonStyle Width="13px">
                        </ButtonStyle>
                    </dxe:ASPxDateEdit>
                </td>
                <td>
                    <label>To </label>
                </td>
                <td style="width: 150px">
                    <dxe:ASPxDateEdit ID="toDate" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" ClientInstanceName="ctoDate" Width="100%" UseMaskBehavior="True">
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
            <% if (rights.CanAdd)
               { %>
            <a href="javascript:void(0);" onclick="OnAddButtonClick()" class="btn btn-success btn-radius"><span class="btn-icon"><i class="fa fa-plus"></i></span><span>New</span> </a><%} %>

            <%--<dxe:ASPxButton ID="btn_Approval" runat="server" class="btn btn-primary" Text="Pending Approval" ClientInstanceName="cbtn_Approval">
                <ClientSideEvents Click="function (s, e) {OpenPopUPApprovalStatus();}" />
            </dxe:ASPxButton>--%>
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
            <%-- <dxe:ASPxButton ID="btn_WaitQuote" ClientInstanceName="Cbtn_WaitQuote" runat="server" AutoPostBack="False" Text="Edit" CssClass="btn btn-primary">
                <ClientSideEvents Click="function(s, e) {OpenWaitingQuote();}" />
            </dxe:ASPxButton>
            <a href="javascript:void(0);" onclick="OpenWaitingQuote()" class="btn btn-warning typeNotificationBtn btn-radius"><span><u>E</u>stimate Waiting </span>
                <span class="typeNotification">
                    <dxe:ASPxLabel runat="server" Text="" ID="lblQuoteweatingCount" ClientInstanceName="ClblQuoteweatingCount"></dxe:ASPxLabel>
                </span>
            </a>--%>
            <%-- <span id="spanStatus" runat="server">
                <a href="javascript:void(0);" onclick="OpenPopUPUserWiseQuotaion()" class="btn btn-primary btn-radius">
                    <span>My Estimate Status</span>
                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                </a>
            </span>
            <span id="divPendingWaiting" runat="server">
                <a href="javascript:void(0);" onclick="OpenPopUPApprovalStatus()" class="btn btn-info btn-radius">
                    <span>Approval Waiting</span>
                    <asp:Label ID="lblWaiting" runat="server" Text=""></asp:Label>
                </a>
                <i class="fa fa-reply blink" style="font-size: 20px; margin-right: 10px;" aria-hidden="true"></i>
            </span>--%>
        </div>
    </div>
    <div class="GridViewArea relative">
        <div class="makeFullscreen ">
            <span class="fullScreenTitle">Estimate / Cost Sheet</span>
            <span class="makeFullscreen-icon half hovered " data-instance="cGrdEstimateCost" title="Maximize Grid" id="expandcGrdEstimateCost">
                <i class="fa fa-expand"></i>
            </span>

            <dxe:ASPxGridView ID="GrdQuotation" runat="server" KeyFieldName="EstimateCost_Id" AutoGenerateColumns="False" SettingsBehavior-AllowFocusedRow="true" Settings-VerticalScrollableHeight="250" Settings-VerticalScrollBarMode="Auto"
                Width="100%" ClientInstanceName="cGrdEstimateCost" OnCustomCallback="GrdQuotation_CustomCallback" DataSourceID="EntityServerModeDataSource" SettingsDataSecurity-AllowEdit="false" SettingsDataSecurity-AllowInsert="false"
                SettingsDataSecurity-AllowDelete="false" OnHtmlRowPrepared="AvailableStockgrid_HtmlRowPrepared" Settings-HorizontalScrollBarMode="Auto">
                <SettingsSearchPanel Visible="True" Delay="5000" />
                <Columns>

                    <dxe:GridViewDataTextColumn FieldName="EstimateCost_Id" Visible="false" ShowInCustomizationForm="false" SortOrder="Descending" VisibleIndex="1">
                        <CellStyle Wrap="False" CssClass="gridcellleft"></CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                    </dxe:GridViewDataTextColumn>

                    <dxe:GridViewDataTextColumn Caption="Document No." FieldName="EstimateCost_Number"
                        VisibleIndex="2" FixedStyle="Left" Width="150px">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                        <Settings AutoFilterCondition="Contains" />
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn Caption="Posting Date" FieldName="EstimateCost_Date"
                        VisibleIndex="3" FixedStyle="Left" Width="100px">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <PropertiesTextEdit DisplayFormatString="dd-MM-yyyy"></PropertiesTextEdit>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                        <Settings AutoFilterCondition="Contains" />
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn Caption="Customer" FieldName="CustomerName"
                        VisibleIndex="4" FixedStyle="Left" Width="200px">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                        <Settings AutoFilterCondition="Contains" />
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn Caption="Ref.Inquiry" FieldName="RefInquiry"
                        VisibleIndex="5" FixedStyle="Left" Width="200px">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                        <Settings AutoFilterCondition="Contains" />
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn Caption="Unit" FieldName="BranchDescription"
                        VisibleIndex="6" FixedStyle="Left" Width="200px">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                        <Settings AutoFilterCondition="Contains" />
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn Caption="Project Name" FieldName="Proj_Name"
                        VisibleIndex="7" Width="250px" Settings-ShowFilterRowMenu="True" Settings-AllowAutoFilter="True">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="True" />
                    </dxe:GridViewDataTextColumn>

                    <dxe:GridViewDataTextColumn Caption="Inquiry Status" FieldName="ApproveStatus"
                        VisibleIndex="8" Width="250px" Settings-ShowFilterRowMenu="True" Settings-AllowAutoFilter="True">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="True" />
                    </dxe:GridViewDataTextColumn>

                    <dxe:GridViewDataTextColumn VisibleIndex="9" FieldName="CreatedBy"
                        Caption="Entered By" Width="120px">
                        <CellStyle HorizontalAlign="left" CssClass="gridcellleft">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn VisibleIndex="10" FieldName="CreatedDate" Settings-AllowAutoFilter="False"
                        Caption="Entered On" Width="180px">
                        <Settings AllowAutoFilterTextInputTimer="False" />
                        <PropertiesTextEdit DisplayFormatString="dd-MM-yyyy"></PropertiesTextEdit>
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn VisibleIndex="11" FieldName="ModifyBy" Settings-AllowAutoFilter="False"
                        Caption="Updated By" Width="120px">
                        <CellStyle HorizontalAlign="left" CssClass="gridcellleft">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn VisibleIndex="11" FieldName="ModifiedDate" Settings-AllowAutoFilter="False"
                        Caption="Updated On" Width="180px">
                        <Settings AllowAutoFilterTextInputTimer="False" />
                        <PropertiesTextEdit DisplayFormatString="dd-MM-yyyy"></PropertiesTextEdit>
                    </dxe:GridViewDataTextColumn>

                    <dxe:GridViewDataTextColumn Caption="Revision No." FieldName="ECS_RevisionNo" Width="120"
                        VisibleIndex="12">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn Caption="Revision date" FieldName="ECS_RevisionDate"
                        VisibleIndex="13" Width="100">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                        <PropertiesTextEdit DisplayFormatString="dd-MM-yyyy"></PropertiesTextEdit>
                    </dxe:GridViewDataTextColumn>
                    <dxe:GridViewDataTextColumn Caption="Approval Status" FieldName="ECS_ApproveStatus" Width="150"
                        VisibleIndex="14">
                        <CellStyle CssClass="gridcellleft" Wrap="true">
                        </CellStyle>
                        <Settings AllowAutoFilterTextInputTimer="False" />
                    </dxe:GridViewDataTextColumn>



                    <dxe:GridViewDataTextColumn HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="center" VisibleIndex="15" Width="1">
                        <DataItemTemplate>
                            <div class='floatedBtnArea'>
                                <% if (rights.CanView)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnViewClick('<%# Container.KeyValue %>')" class="" title="">
                                    <span class='ico ColorFour'><i class='fa fa-eye'></i></span><span class='hidden-xs'>View</span></a>
                                <% } %>
                                <% if (rights.CanEdit)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnMoreInfoClick('<%# Container.KeyValue %>')" title="" style='<%#Eval("IsShowEdit")%>'>
                                    <span class='ico editColor'><i class='fa fa-pencil' aria-hidden='true'></i></span><span class='hidden-xs'>Edit</span></a><%} %>
                                <% if (rights.CanDelete)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnClickDelete('<%# Container.KeyValue %>')" class="" title="" style='<%#Eval("IsShowEdit")%>'>
                                    <span class='ico deleteColor'><i class='fa fa-trash' aria-hidden='true'></i></span><span class='hidden-xs'>Delete</span></a><%} %>

                                <% if (rights.CanEdit && IsInquiryStatus)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnClickStatus('<%# Container.KeyValue %>')" class="" title="" style='<%#Eval("Inquiry_Status")%>'>
                                    <span class='ico ColorSeven'><i class='fa fa-check'></i></span><span class='hidden-xs'>Status</span></a><%} %>

                                <% if (rights.CanView)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnclickLinePreview('<%# Container.KeyValue %>')" class="" title="">
                                    <span class='ico ColorFive'><i class='fa fa-share-alt'></i></span><span class='hidden-xs'>Preview Line Items</span>
                                </a><%} %>
                                 <% if (rights.CanApproved && IsApprove)
                                       { %>
                                <a href="javascript:void(0);" onclick="OnApproveClick('<%# Container.KeyValue %>',<%# Container.VisibleIndex %>)" class="" title="">
                                    <span class='ico editColor'><i class='fa fa-check' aria-hidden='true'></i></span><span class='hidden-xs'>Approve/Reject</span>
                                </a>
                                <% } %>
                                <%-- <% if (rights.CanEdit)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnClickStatus('<%# Container.KeyValue %>')" class="" title="">
                                    <span class='ico ColorSeven'><i class='fa fa-check'></i></span><span class='hidden-xs'>Status</span></a><%} %>--%>
                                <%--  <% if (rights.CanView)
                                   { %>
                                <a href="javascript:void(0);" onclick="OnclickViewAttachment('<%# Container.KeyValue %>')" class="" title="">
                                    <span class='ico ColorFive'><i class='fa fa-share-alt'></i></span><span class='hidden-xs'>Add/View Attachment</span>
                                </a><%} %>--%>
                                <%--<% if (rights.CanPrint)
                                   { %>
                                <a href="javascript:void(0);" onclick="onPrintJv('<%# Container.KeyValue %>')" classa="" title="">
                                    <span class='ico ColorSix'><i class='fa fa-print'></i></span><span class='hidden-xs'>Print</span>
                                </a><%} %>--%>
                            </div>
                        </DataItemTemplate>
                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                        <CellStyle HorizontalAlign="Center"></CellStyle>
                        <%-- Rev RAJDIP --%>
                        <%--<HeaderTemplate><span>Actions</span></HeaderTemplate>--%>
                        <EditFormSettings Visible="False"></EditFormSettings>
                        <%--END Rev RAJDIP --%>

                        <Settings AllowAutoFilterTextInputTimer="False" />
                    </dxe:GridViewDataTextColumn>
                </Columns>
                <%-- --Rev Sayantani--%>
                <SettingsBehavior ConfirmDelete="true" EnableCustomizationWindow="true" EnableRowHotTrack="true" />
                <SettingsCookies Enabled="true" StorePaging="true" Version="16" />
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

                <Settings ShowGroupPanel="True" ShowStatusBar="Hidden" ShowHorizontalScrollBar="False" ShowFilterRow="true" ShowFilterRowMenu="true" />
                <SettingsLoadingPanel Text="Please Wait..." />
            </dxe:ASPxGridView>
            <dx:LinqServerModeDataSource ID="EntityServerModeDataSource" runat="server" OnSelecting="EntityServerModeDataSource_Selecting"
                ContextTypeName="ERPDataClassesDataContext" TableName="EstimateCostSheetList" />
            <asp:HiddenField ID="hiddenedit" runat="server" />
        </div>
    </div>
    <div style="display: none">
        <dxe:ASPxGridViewExporter ID="exporter" GridViewID="GrdQuotation" runat="server" Landscape="false" PaperKind="A4" PageHeader-Font-Size="Larger" PageHeader-Font-Bold="true">
        </dxe:ASPxGridViewExporter>
    </div>
    <div class="PopUpArea">

        <dxe:ASPxPopupControl ID="popup_QuoteWait" runat="server" Width="1200"
            CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="Cpopup_QuoteWait"
            HeaderText="Estimate Waiting" AllowResize="false" ResizingMode="Postponed">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">

                    <div onkeypress="OnWaitingGridKeyPress(event)">

                        <dxe:ASPxGridView ID="watingQuotegrid" runat="server" KeyFieldName="SBMain_Id" AutoGenerateColumns="False"
                            Width="100%" ClientInstanceName="CwatingQuotegrid" OnCustomCallback="watingQuotegrid_CustomCallback" KeyboardSupport="true"
                            SettingsBehavior-AllowSelectSingleRowOnly="true" OnDataBinding="watingQuotegrid_DataBinding" SettingsBehavior-AllowFocusedRow="true"
                            Settings-VerticalScrollBarMode="auto" Settings-VerticalScrollableHeight="320">
                            <Columns>

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

                            <ClientSideEvents RowClick="ListRowClicked" EndCallback="watingQuotegridEndCallback" />

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


                    <dxe:ASPxButton ID="InvoiceWattingOk" runat="server" AutoPostBack="false" Text="O&#818;k" CssClass="btn btn-primary okClass"
                        ClientSideEvents-Click="QuotationWattingOkClick" UseSubmitBehavior="False" />
                </dxe:PopupControlContentControl>
            </ContentCollection>
        </dxe:ASPxPopupControl>

    </div>
    <div class="PopUpArea">
        <%--Client Wise Quotation Status Section Start--%>

        <dxe:ASPxPopupControl ID="Popup_QuotationStatus" runat="server" ClientInstanceName="cQuotationStatus"
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
                                <table width="">
                                    <tr>
                                        <td style="padding-right: 8px; vertical-align: top">
                                            <strong>
                                                <label style="margin-bottom: 5px">Estimate Cost Sheet:-</label></strong>
                                        </td>
                                        <td style="padding-top: 0; vertical-align: top; padding-right: 35px">
                                            <%--<dxe:ASPxTextBox ID="txt_Proforma" MaxLength="80" ClientInstanceName="cProforma" TabIndex="1" 
                                                runat="server" Width="100%"> 
                                            </dxe:ASPxTextBox>--%>
                                            <strong>
                                                <dxe:ASPxLabel ID="lbl_Proforma" runat="server" ClientInstanceName="cProforma" Text="ASPxLabel"></dxe:ASPxLabel>
                                            </strong>
                                        </td>
                                        <td style="padding-right: 8px; padding-left: 8px; vertical-align: top">
                                            <strong>
                                                <label style="margin-bottom: 5px">Customer:-</label></strong>
                                        </td>
                                        <td style="padding-top: 0; vertical-align: top">
                                            <%-- <dxe:ASPxTextBox ID="txt_Customer" ClientInstanceName="cCustomer"  runat="server" MaxLength="100" TabIndex="2"
                                            Width="100%"> 
                                        </dxe:ASPxTextBox>--%>
                                            <strong>
                                                <dxe:ASPxLabel ID="lbl_Customer" runat="server" ClientInstanceName="cCustomer" Text="ASPxLabel"></dxe:ASPxLabel>
                                            </strong>
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
                                <td style="width: 70px; padding: 13px 0;"><strong>Status </strong></td>
                                <td>
                                    <asp:RadioButtonList ID="rbl_QuoteStatus" runat="server" Width="172px" CssClass="mTop5" RepeatDirection="Horizontal">
                                        <asp:ListItem Text="Accepted" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Declined" Value="2"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="clear"></div>
                    <div class="col-md-12">
                        <div class="" style="margin-bottom: 5px;">
                            <strong>Reason </strong>
                        </div>
                        <div>
                            <dxe:ASPxMemo ID="txt_QuotationRemarks" runat="server" ClientInstanceName="cQuotationRemarks" Height="71px" Width="100%"></dxe:ASPxMemo>
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

        <%--Client Wise Quotation Status Section END--%>

        <%-- Sandip Approval Dtl Section Start--%>


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
                            <dxe:ASPxGridView ID="gridPendingApproval" runat="server" KeyFieldName="Quote_Id" AutoGenerateColumns="False" OnPageIndexChanged="gridPendingApproval_PageIndexChanged"
                                Width="100%" ClientInstanceName="cgridPendingApproval" OnCustomCallback="gridPendingApproval_CustomCallback">
                                <Columns>
                                    <dxe:GridViewDataTextColumn Caption="Proforma/Quotation No." FieldName="Quote_Number"
                                        VisibleIndex="0" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>
                                    <dxe:GridViewDataTextColumn Caption="Created On" FieldName="Quote_Date"
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
                                                <%-- <ClientSideEvents CheckedChanged="function (s, e) {ch_fnApproved();}" />--%>
                                            </dxe:ASPxCheckBox>
                                        </DataItemTemplate>
                                        <Settings ShowFilterRowMenu="False" AllowFilterBySearchPanel="False" AllowAutoFilter="False" />
                                    </dxe:GridViewDataCheckColumn>

                                    <dxe:GridViewDataCheckColumn UnboundType="Boolean" Caption="Rejected">
                                        <DataItemTemplate>
                                            <dxe:ASPxCheckBox ID="chkreject" runat="server" AllowGrayed="false" OnInit="chkreject_Init" ValueType="System.Boolean" ValueChecked="true" ValueUnchecked="false">
                                                <%-- <ClientSideEvents CheckedChanged="function (s, e) {ch_fnApproved();}" />--%>
                                            </dxe:ASPxCheckBox>
                                        </DataItemTemplate>
                                        <Settings ShowFilterRowMenu="False" AllowFilterBySearchPanel="False" AllowAutoFilter="False" />
                                    </dxe:GridViewDataCheckColumn>
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

        <dxe:ASPxPopupControl ID="ASPXPopupControl" runat="server"
            CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="popup" Height="630px"
            Width="1200px" HeaderText="Quotation Approval" Modal="true" AllowResize="true" ResizingMode="Postponed">
            <HeaderTemplate>
                <span>User Approval</span>
            </HeaderTemplate>
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                </dxe:PopupControlContentControl>
            </ContentCollection>
        </dxe:ASPxPopupControl>
        <dxe:ASPxPopupControl ID="PopupUserWiseQuotation" runat="server" ClientInstanceName="cPopupUserWiseQuotation"
            Width="900px" HeaderText="User Wise Quotation Status" PopupHorizontalAlign="WindowCenter" HeaderStyle-CssClass="wht"
            PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
            Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True"
            ContentStyle-CssClass="pad">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                    <div class="row">
                        <div class="col-md-12">
                            <dxe:ASPxGridView ID="gridUserWiseQuotation" runat="server" KeyFieldName="Quote_Id" AutoGenerateColumns="False"
                                Width="100%" ClientInstanceName="cgridUserWiseQuotation" OnCustomCallback="gridUserWiseQuotation_CustomCallback">
                                <Columns>
                                    <dxe:GridViewDataTextColumn Caption="Proforma/Quotation No." FieldName="Quote_Number"
                                        VisibleIndex="0" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="Entered On" FieldName="createddate"
                                        VisibleIndex="1" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="Approval User" FieldName="approvedby"
                                        VisibleIndex="2" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="User Level" FieldName="UserLevel"
                                        VisibleIndex="3" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="Status" FieldName="status"
                                        VisibleIndex="4" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
                                    </dxe:GridViewDataTextColumn>

                                    <dxe:GridViewDataTextColumn Caption="Approved On" FieldName="ApprovedOn"
                                        VisibleIndex="5" FixedStyle="Left">
                                        <CellStyle CssClass="gridcellleft" Wrap="true">
                                        </CellStyle>
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

        <%-- Sandip Approval Dtl Section End--%>
    </div>

    <%--   <dxe:ASPxTimer runat="server" Interval="10000" ClientInstanceName="Timer1">
        <ClientSideEvents Tick="timerTick" />
    </dxe:ASPxTimer>--%>
    <asp:HiddenField ID="waitingQuotationCount" runat="server" />
    <div class="PopUpArea">
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

    <dxe:ASPxGlobalEvents ID="GlobalEvents" runat="server">
        <ClientSideEvents ControlsInitialized="AllControlInitilize" />
    </dxe:ASPxGlobalEvents>

    <div>
        <asp:HiddenField ID="hfIsFilter" runat="server" />
        <asp:HiddenField ID="hfFromDate" runat="server" />
        <asp:HiddenField ID="hfToDate" runat="server" />
        <asp:HiddenField ID="hfBranchID" runat="server" />
        <asp:HiddenField ID="hfIsUserwise" runat="server" />
    </div>
    <%-- Rev Rajdip --%>
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
    <asp:HiddenField ID="hddnKeyValue" runat="server" />
    <%--End Rev Rajdip --%>

    <dxe:ASPxPopupControl ID="ASPxProductsPopup" runat="server" ClientInstanceName="cProductsPopup"
        Width="900px" HeaderText="" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
        Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True">
        <HeaderTemplate>
            <strong><span style="color: #fff">Preview Line Items</span></strong>
            <dxe:ASPxImage ID="ASPxImage3" runat="server" ImageUrl="/assests/images/closePop.png" Cursor="pointer" CssClass="popUpHeader pull-right">
                <ClientSideEvents Click="function(s, e){ 
                                                            cProductsPopup.Hide();
                                                        }" />
            </dxe:ASPxImage>
        </HeaderTemplate>
        <ContentCollection>
            <dxe:PopupControlContentControl runat="server">
                <% if (rights.CanExport)
                   { %>
                <asp:DropDownList ID="ExportProduct" runat="server" CssClass="btn btn-primary btn-radius" OnSelectedIndexChanged="ExportProduct_SelectedIndexChanged" AutoPostBack="true">
                    <asp:ListItem Value="0">Export to</asp:ListItem>
                    <asp:ListItem Value="1">PDF</asp:ListItem>
                    <asp:ListItem Value="2">XLS</asp:ListItem>
                    <asp:ListItem Value="3">RTF</asp:ListItem>
                    <asp:ListItem Value="4">CSV</asp:ListItem>
                </asp:DropDownList>
                <% } %>

                <dxe:ASPxGridView runat="server" KeyFieldName="EstCostDetails_Id" ClientInstanceName="cgridproducts" ID="grid_Products"
                    Width="100%" SettingsBehavior-AllowSort="false" SettingsBehavior-AllowDragDrop="false" SettingsPager-Mode="ShowAllRecords"
                    OnCustomCallback="grid_Products_CustomCallback" OnDataBinding="grid_Products_DataBinding"
                    Settings-ShowFooter="false" AutoGenerateColumns="False"
                    Settings-VerticalScrollableHeight="300" Settings-VerticalScrollBarMode="Visible" Settings-HorizontalScrollBarMode="Visible">

                    <SettingsBehavior AllowDragDrop="False" AllowSort="False"></SettingsBehavior>
                    <SettingsPager Visible="false"></SettingsPager>
                    <Columns>

                        <dxe:GridViewDataTextColumn VisibleIndex="1" FieldName="SrlNo" Width="45" ReadOnly="true" Caption="Sl. No." FixedStyle="Left">
                            <Settings AutoFilterCondition="Contains" />
                        </dxe:GridViewDataTextColumn>
                        <%-- <dxe:GridViewDataTextColumn VisibleIndex="36" FieldName="ProductID" ReadOnly="true" Caption="Product" Width="0">
                            <Settings AutoFilterCondition="Contains" />
                        </dxe:GridViewDataTextColumn>--%>
                        <dxe:GridViewDataTextColumn VisibleIndex="4" FieldName="ProductName" ReadOnly="true" Width="200" Caption="Product Name" FixedStyle="Left">
                            <Settings AutoFilterCondition="Contains" />
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn VisibleIndex="5" FieldName="Description" Width="200" ReadOnly="true" Caption="Product Description" FixedStyle="Left">
                            <Settings AutoFilterCondition="Contains" />
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn VisibleIndex="7" FieldName="Inquiry_Number" ReadOnly="true" Caption="Inquiry Number" Width="200">
                            <Settings AutoFilterCondition="Contains" />
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="Quantity" Caption="Quantity" VisibleIndex="8" Width="100px" PropertiesTextEdit-MaxLength="14" HeaderStyle-HorizontalAlign="Right" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="UOM" Caption="UOM" VisibleIndex="9" ReadOnly="true" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn Caption="Multi Qty" CellStyle-HorizontalAlign="Right" FieldName="Order_AltQuantity" PropertiesTextEdit-MaxLength="14" VisibleIndex="11" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn Caption="Multi Unit" FieldName="Order_AltUOM" ReadOnly="true" VisibleIndex="12" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_NB" Caption="NB" VisibleIndex="13" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_OD" Caption="OD" VisibleIndex="14" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Thk" Caption="Thk" VisibleIndex="15" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Tol_thk" Caption="Tolerance Thk" VisibleIndex="16" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Wt_Mtr" Caption="Wt/Mtr" VisibleIndex="17" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Tol_wt" Caption="Tolerance Wt/ Mtr" VisibleIndex="18" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Qty_Mtr" Caption="Qty(Mtr)" VisibleIndex="19" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Sqmtr" Caption="Total Sq. Mtr" VisibleIndex="20" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_T_WT" Caption="Total Wt" VisibleIndex="21" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Tol_t_wt" Caption="Tolerance Total Wt" VisibleIndex="22" Width="200px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Rate_kg" Caption="Cost/ Kg" VisibleIndex="23" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Frt" Caption="Freight/ Kg" VisibleIndex="24" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Surface_prep" Caption="Paint Cost/ Sq. Mtr" VisibleIndex="25" Width="200px">
                        </dxe:GridViewDataTextColumn>

                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Rate_sqm" Caption="Paint Cost/ Mtr" VisibleIndex="26" Width="200px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Ratekg" Caption="Paint Cost/ Kg" VisibleIndex="27" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_CP_MT" Caption="Total Cost/ Kg" VisibleIndex="28" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_FOR_CP_MTR" Caption="Total Cost/ Mtr" VisibleIndex="29" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_FOR_CP_VAL" Caption="Cost Value" VisibleIndex="30" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_Gross_Margin" Caption="Profit Margin" VisibleIndex="31" Width="100px">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="EstCostDetails_SP_Mtr" Caption="Selling Price/ Mtr" VisibleIndex="32" Width="200px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn FieldName="Value" Caption="Sales Value" VisibleIndex="33" Width="100px" ReadOnly="true">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn Caption="Remarks" FieldName="Remarks" Width="150px" VisibleIndex="34" PropertiesTextEdit-MaxLength="5000">
                        </dxe:GridViewDataTextColumn>

                    </Columns>
                    <Settings ShowFilterRow="true" ShowFilterRowMenu="true" />
                    <ClientSideEvents EndCallback="OnEndCallback" />

                    <Settings ShowFilterRow="true" ShowFilterRowMenu="true" />
                    <SettingsDataSecurity AllowEdit="true" />

                </dxe:ASPxGridView>

                <div class="content reverse horizontal-images clearfix " style="width: 100%; margin-right: 0; padding: 8px; height: auto; border-top: 1px solid #ccc; border-bottom: 1px solid #ccc; border-radius: 0;">
                    <ul>
                        <li class="clsbnrLblTotalQty">
                            <div class="horizontallblHolder">
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <dxe:ASPxLabel ID="ASPxLabel10" runat="server" Text="Qty (mtr)" ClientInstanceName="cbnrLblTotalQty" />
                                            </td>
                                            <td>
                                                <dxe:ASPxLabel ID="lbl_Total_Qty_Mtr" runat="server" Text="0.00" ClientInstanceName="clbl_Total_Qty_Mtr" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                        </li>
                        <li class="clsbnrLblTaxableAmt">
                            <div class="horizontallblHolder">
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrLblTaxableAmt" runat="server" Text="Total Sq. Mtr" ClientInstanceName="cbnrLblTaxableAmt" />
                                            </td>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrLblTotalSqMtr" runat="server" Text="0.00" ClientInstanceName="cbnrLblTotalSqMtr" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                        </li>
                        <li class="clsbnrLblTaxAmt">
                            <div class="horizontallblHolder" id="">
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrLblTaxAmt" runat="server" Text="Total Wt" ClientInstanceName="cbnrLblTaxAmt" />
                                            </td>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrLblTotalWt" runat="server" Text="0.00" ClientInstanceName="cbnrLblTotalWt" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                        </li>
                        <li class="clsbnrLblAmtWithTax" runat="server" id="oldUnitBanerLbl">
                            <div class="horizontallblHolder">
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrLblAmtWithTax" runat="server" Text="Tolerance Total Wt" ClientInstanceName="cbnrLblAmtWithTax" />
                                            </td>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrlblToleranceTotalWt" runat="server" Text="0.00" ClientInstanceName="cbnrlblToleranceTotalWt" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                        </li>
                        <li class="clsbnrLblInvVal" id="otherChargesId">
                            <div class="horizontallblHolder">
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <dxe:ASPxLabel ID="cbnrOtherCharges" runat="server" Text="Cost Value" ClientInstanceName="cbnrOtherCharges" />
                                            </td>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrCOSTVALUE" runat="server" Text="0.00" ClientInstanceName="cbnrCOSTVALUE" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                        </li>
                        <li class="clsbnrLblLessAdvance" id="idclsbnrLblLessAdvance" runat="server">
                            <div class="horizontallblHolder">
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrLblLessAdvance" runat="server" Text="Profit Margin" ClientInstanceName="cbnrLblLessAdvance" />
                                            </td>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrLblProfitMargin" runat="server" Text="0.00" ClientInstanceName="cbnrLblProfitMargin" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                        </li>

                        <li class="clsbnrLblInvVal">
                            <div class="horizontallblHolder">
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrLblInvVal" runat="server" Text="Sales Value" ClientInstanceName="cbnrLblInvVal" />
                                            </td>
                                            <td>
                                                <dxe:ASPxLabel ID="bnrSalesValue" runat="server" Text="0.00" ClientInstanceName="cbnrSalesValue" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                        </li>







                    </ul>
                </div>
                <div style="display: none">
                    <dxe:ASPxGridViewExporter ID="productGVExporter" runat="server" Landscape="false" PaperKind="A4" PageHeader-Font-Size="Larger" PageHeader-Font-Bold="true">
                    </dxe:ASPxGridViewExporter>
                </div>
            </dxe:PopupControlContentControl>
        </ContentCollection>
        <ContentStyle VerticalAlign="Top" CssClass="pad"></ContentStyle>
        <HeaderStyle BackColor="LightGray" ForeColor="Black" />
    </dxe:ASPxPopupControl>

    <dxe:ASPxCallbackPanel runat="server" ID="CallbackPanel" ClientInstanceName="cCallbackPanel" OnCallback="CallbackPanel_Callback">
        <PanelCollection>
            <dxe:PanelContent runat="server">           
            </dxe:PanelContent>
        </PanelCollection>
        <ClientSideEvents EndCallback="CallbackPanelEndCall" />
    </dxe:ASPxCallbackPanel>
     <asp:HiddenField ID="hFilterType" runat="server" />
</asp:Content>
