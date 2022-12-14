﻿<%@ Page Title="Approver's Configuration" Language="C#" MasterPageFile="~/OMS/MasterPage/ERP.Master" AutoEventWireup="true" CodeBehind="Moduleapprovalconfig.aspx.cs" Inherits="ERP.OMS.Management.Master.Moduleapprovalconfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .dxgvEditFormDisplayRow_PlasticBlue td.dxgv, .dxgvDataRow_PlasticBlue td.dxgv, .dxgvDataRowAlt_PlasticBlue td.dxgv, .dxgvSelectedRow_PlasticBlue td.dxgv, .dxgvFocusedRow_PlasticBlue td.dxgv {
            overflow: visible !important;
        }

        .dxgvControl_PlasticBlue td.dxgvBatchEditModifiedCell_PlasticBlue {
            background: #fff !important;
        }
        #approvalGrid_DXMainTable>tbody>tr>td:last-child {
            display:none !important;
        }
    </style>
    <script type="text/javascript">


        $(document).ready(function () {
            capprovalGrid.batchEditApi.StartEdit(-1, 1);
            // cCmblevel1.PerformCallback();
        })

        function OnInit(s, e) {

            //cbtnSaveRecords.SetClientEnabled(false);
            //IntializeGlobalVariables(s);
        }
        function ShowError(obj) {

            //IntializeGlobalVariables(s);
            if (capprovalGrid.cpupdatemssg != null) {
                jAlert(capprovalGrid.cpupdatemssg);
                capprovalGrid.cpupdatemssg = null;
            }
            if (capprovalGrid.cpDelete != null) {
                if (capprovalGrid.cpDelete == 'Success') {
                    jAlert('Deleted Successfully');
                    capprovalGrid.cpDelete = null;
                }
                else {
                    jAlert('Used in other module.Can not delete');
                    capprovalGrid.cpDelete = null;
                }

            }
        }


        function OnCmblevel1SelectedIndexChanged(level1id) {

            // var SelectedValue = level1id.GetValue();
            // var SelectedText = level1id.GetText();

            //cCmblevel2.PerformCallback("populate|" + SelectedValue + "|" + SelectedText);
            //capprovalGrid.PerformCallback("cCmblevel3|" + SelectedValue + "|" + SelectedText);
            // capprovalGrid.GetEditor("level2userids").PerformCallback("cCmblevel3|" + SelectedValue + "|" + SelectedText);

            //var currentValue = capprovalGrid.GetEditor('level1userids').GetValue();
            ////var currentValue = s.GetValue();
            //if (lastCountryID == currentValue) {
            //    if (level2ID.GetSelectedIndex() < 0)
            //        level2ID.SetSelectedIndex(0);
            //    return;
            //}
            //lastCountryID = currentValue;
            //level2ID.PerformCallback(currentValue);
            //cbtnSaveRecords.SetEnabled(true);
            capprovalGrid.GetEditor("level2userids").PerformCallback(level1id.GetValue());
            capprovalGrid.GetEditor("level3userids").PerformCallback(level1id.GetValue());
        }

        function OnCmblevel2SelectedIndexChanged(lebel2) {

            //var currentValue = capprovalGrid.GetEditor('level1userids').GetValue();
            //var currentlevel2Value = lebel2.GetValue();
            ////var currentValue = s.GetValue();
            //if (lastCountryID == currentValue) {
            //    if (level2ID.GetSelectedIndex() < 0)
            //        level2ID.SetSelectedIndex(0);
            //    return;
            //}
            //lastCountryID = currentValue;
            //var ids = currentValue + "," + currentlevel2Value;
            //level3ID.PerformCallback(ids);

            var current1Value = capprovalGrid.GetEditor('level1userids').GetValue();
            var current2Value = capprovalGrid.GetEditor('level2userids').GetValue();
            var ids = lebel2.GetValue() + "," + current1Value;

            //alert(current1Value);
            var count = 0;
            if (current1Value == "" || current1Value == null) {
                count++;
            } else if (current1Value == "" || current1Value == null) {
                count++;
            }
            if (count > 0) {
                lebel2.SetSelectedIndex(-1);
                jAlert("You must select first level before selecting third level.");
            } else {
                
                capprovalGrid.GetEditor("level3userids").PerformCallback(ids);
            }



        }

        function OnCmblevel3SelectedIndexChanged(cmbOnlevel2user) {

            var current1Value = capprovalGrid.GetEditor('level1userids').GetValue();
            var current2Value = capprovalGrid.GetEditor('level2userids').GetValue();
            var count = 0;
            cbtnSaveRecords.SetEnabled(true);
            if (current1Value == "" || current1Value == null) {
                count++;
            } else if (current2Value == "" || current2Value == null) {
                count++;
            }
            if (count > 0) {
                cmbOnlevel2user.SetSelectedIndex(-1);
                jAlert("You must select first level and 2nd level before selecting 3rd level.");
            }

        }
        function saveandupdate() {

            capprovalGrid.UpdateEdit();
            capprovalGrid.PerformCallback();
        }

        //var allowEdit = false;

        function OnAllowEditChanged(s, e) {
            //alert();
            var active1 = capprovalGrid.GetEditor('active1').GetValue();
            capprovalGrid.GetEditor('level1userids').SetSelectedIndex(-1);
            capprovalGrid.GetEditor('level2userids').SetSelectedIndex(-1);
            capprovalGrid.GetEditor('level3userids').SetSelectedIndex(-1);


            //if (s.GetValue() == false && active1 == "true") {
            //    capprovalGrid.GetEditor('active').SetValue(true);
            //} else {

            //    }
        }
        function OnBatchStartEdit(s, e) {
            
            //var Keyval = capprovalGrid.GetRowValues(e.VisibleIndex, "moduleids"); 
            //var Keyval = capprovalGrid.GetEditor("moduleids");
            //var active1 = capprovalGrid.GetEditor("active1");
            //alert(String(active1).split(",")[0]);
            //if (Keyval == "1" && active1.split(',')[0] == "QO") {

            //    capprovalGrid.GetEditor('active').SetEnabled(false);
            
            //}
            //cbtnSaveRecords.ClientEnabled(true);
        }
        function chnagedcombo(s) {
           
            $('#<%=hdnselectedbranch.ClientID %>').val(s.GetValue());
            capprovalGrid.PerformCallback("branchwise~" + s.GetValue());
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="panel-heading">
        <div class="panel-title" id="td_contact1" runat="server">

            <h3>
                <asp:Label ID="lblHeadTitle" runat="server" Text="Approver's Configuration"></asp:Label>
            </h3>
        </div>

    </div>
    <div class="form_main">
        <div class="SearchArea">
            <div class="FilterSide">

                <div class="pull-left">
                    <% if (rights.CanExport)
                                               { %>
                    <asp:DropDownList ID="drdExport" runat="server" CssClass="btn btn-sm btn-primary" OnChange="if(!AvailableExportOption()){return false;}" OnSelectedIndexChanged="cmbExport_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="0">Export to</asp:ListItem>
                        <asp:ListItem Value="1">PDF</asp:ListItem>
                        <asp:ListItem Value="2">XLS</asp:ListItem>
                        <asp:ListItem Value="3">RTF</asp:ListItem>
                        <asp:ListItem Value="4">CSV</asp:ListItem>

                    </asp:DropDownList>
                     <% } %>
                </div>
            </div>
             <div class="FilterSide">
                <div style="width: 60px; float: left; padding-top: 5px">Branch: </div>
                <div class="col-sm-4">
                    <dxe:ASPxComboBox ID="cmbbranch" runat="server" ClientInstanceName="ccmbbranch" TextField="branchName" ValueField="branchID">
                        <ClientSideEvents SelectedIndexChanged="function(s,e) { chnagedcombo(s);}" />
                    </dxe:ASPxComboBox>
                </div>
            </div>
            <div class="clear">
                <br />
            </div>
        </div>
        <dxe:ASPxGridView ID="approvalGrid" runat="server" KeyFieldName="moduleids" AutoGenerateColumns="False" SettingsBehavior-AllowSort="false"
            Width="100%" ClientInstanceName="capprovalGrid" OnRowInserting="Grid_RowInserting" OnRowUpdating="Grid_RowUpdating" OnRowDeleting="Grid_RowDeleting"
            OnCustomCallback="approvalGrid_CustomCallback" OnDataBinding="approvalGrid_DataBinding" OnCellEditorInitialize="approvalGrid_CellEditorInitialize" OnBatchUpdate="approvalGrid_BatchUpdate">

            <ClientSideEvents EndCallback="function(s,e) { ShowError(s.cpInsertError);}" BatchEditStartEditing="OnBatchStartEdit" Init="OnInit" />
            <SettingsEditing Mode="Batch">
                <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="row" />
            </SettingsEditing>
            <Settings ShowGroupPanel="false" ShowStatusBar="Hidden" ShowTitlePanel="false" />
            <SettingsDataSecurity AllowDelete="False" />
            <Columns>

                <dxe:GridViewDataTextColumn FieldName="moduleids" VisibleIndex="1" Visible="false" Caption="Entries">
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn FieldName="acid" VisibleIndex="2" Width="1" CellStyle-CssClass="hide">
                    <EditFormSettings Visible="False" />
                    <HeaderStyle CssClass="hide" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataCheckColumn Caption="Active" VisibleIndex="3" FieldName="active">
                    <PropertiesCheckEdit>
                        <ClientSideEvents CheckedChanged="OnAllowEditChanged" />

                    </PropertiesCheckEdit>
                   <%-- <DataItemTemplate>
                        <dxe:ASPxCheckBox ID="ASPxCheckBox1" runat="server"
                            Value='<%#Eval("active")%>' ReadOnly="<%#SetVisibility(Container)%>" AutoPostBack="false" >
                            <ClientSideEvents CheckedChanged="OnAllowEditChanged" />
                        </dxe:ASPxCheckBox>
                    </DataItemTemplate>--%>

                </dxe:GridViewDataCheckColumn>

                <dxe:GridViewDataTextColumn FieldName="modulenames" Visible="true" VisibleIndex="5" Caption="Entries">
                    <EditFormSettings Visible="False" />
                </dxe:GridViewDataTextColumn>

                <dxe:GridViewDataComboBoxColumn Caption="1st Level User" FieldName="level1userids" VisibleIndex="6" Width="220px">
                    <PropertiesComboBox ValueField="LevelID" TextField="LevelName" ClearButton-DisplayMode="Always">
                        <%--<ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" ErrorDisplayMode="ImageWithTooltip">
                            <RequiredField ErrorText="Mandetary" />
                        </ValidationSettings>--%>
                        <ClientSideEvents SelectedIndexChanged="function(s, e) { OnCmblevel1SelectedIndexChanged(s); }" />
                    </PropertiesComboBox>
                </dxe:GridViewDataComboBoxColumn>
                <%--<dxe:GridViewDataComboBoxColumn FieldName="level2userids" Caption="2nd Level User" VisibleIndex="5" Width="220px">
                            <PropertiesComboBox TextField="LevelName" ValueField="LevelID">
                            </PropertiesComboBox>
                            <EditItemTemplate>
                                <dxe:ASPxComboBox runat="server" Width="100%" OnInit="level2userids_Init" EnableIncrementalFiltering="true" TextField="LevelName" OnCallback="level2userids_Callback" EnableCallbackMode="true" ValueField="LevelID" ID="level2userids" ClientInstanceName="level2ID">
                                    <ClientSideEvents EndCallback="level2Combo_EndCallback" SelectedIndexChanged="function(s, e) { OnCmblevel2SelectedIndexChanged(s); }"/>
                                  
                                </dxe:ASPxComboBox>
                            </EditItemTemplate>
                        </dxe:GridViewDataComboBoxColumn>
                    <dxe:GridViewDataComboBoxColumn FieldName="level3userids" Caption="3rd Level User" VisibleIndex="6" Width="220px">
                            <PropertiesComboBox TextField="LevelName" ValueField="LevelID">
                            </PropertiesComboBox>
                            <EditItemTemplate>
                                <dxe:ASPxComboBox runat="server" Width="100%" OnInit="level3userids_Init" EnableIncrementalFiltering="true" TextField="LevelName" OnCallback="level3userids_Callback" EnableCallbackMode="true" ValueField="LevelID" ID="level3userids" ClientInstanceName="level3ID">
                                    <ClientSideEvents EndCallback="level3Combo_EndCallback" />
                                   
                                </dxe:ASPxComboBox>
                            </EditItemTemplate>
                        </dxe:GridViewDataComboBoxColumn>--%>
                <dxe:GridViewDataComboBoxColumn Caption="2nd Level User" FieldName="level2userids" VisibleIndex="7" Width="220px">
                    <PropertiesComboBox ValueField="LevelID" TextField="LevelName" ClearButton-DisplayMode="Always">

                        <ClientSideEvents SelectedIndexChanged="function(s, e) { OnCmblevel2SelectedIndexChanged(s); }" />
                    </PropertiesComboBox>
                </dxe:GridViewDataComboBoxColumn>

                <dxe:GridViewDataComboBoxColumn Caption="3rd Level User" FieldName="level3userids" VisibleIndex="8" Width="220px">
                    <PropertiesComboBox ValueField="LevelID" TextField="LevelName" ClientInstanceName="clvl3" ClearButton-DisplayMode="Always">

                        <ClientSideEvents SelectedIndexChanged="function(s, e) { OnCmblevel3SelectedIndexChanged(s); }" />
                    </PropertiesComboBox>
                </dxe:GridViewDataComboBoxColumn>
                <dxe:GridViewDataTextColumn VisibleIndex="9" Caption="Active145" FieldName="active1" Width="0%">
                    <EditFormSettings Visible="True"  />
                    <PropertiesTextEdit >
                    </PropertiesTextEdit>
                </dxe:GridViewDataTextColumn>
            </Columns>
        </dxe:ASPxGridView>


    </div>
    <br />
    <div>
        <% if (rights.CanAdd && rights.CanEdit)
           { %>



        <dxe:ASPxButton ID="btnSaveRecords" ClientInstanceName="cbtnSaveRecords" runat="server" 
            AccessKey="S" AutoPostBack="false" CssClass="btn btn-primary" TabIndex="0" Text="Save & Update"
            UseSubmitBehavior="False">
            <ClientSideEvents Click="function(s, e) {saveandupdate();}" />
        </dxe:ASPxButton>
        <% } %>
    </div>
    <dxe:ASPxGridViewExporter ID="exporter" runat="server" Landscape="false" PaperKind="A4" PageHeader-Font-Size="Larger" PageHeader-Font-Bold="true">
    </dxe:ASPxGridViewExporter>

    <asp:HiddenField ID="hdnselectedbranch" runat="server" Value="0" />
</asp:Content>
