﻿#region//====================================================Revision History=========================================================================
// 1.0   v2.0.39	Priti	05-09-2023	0026786:DB-CR mismatch in Project Sales Invoice
#endregion//====================================================End Revision History=====================================================================


using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using BusinessLogicLayer;
using ClsDropDownlistNameSpace;
using EntityLayer.CommonELS;
using System.Web.Services;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Web.Data;
using DataAccessLayer;
using System.ComponentModel;
using System.Linq;
using EntityLayer;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Drawing;
using ERP.OMS.Tax_Details.ClassFile;
using ERP.Models;
using EntityLayer.MailingSystem;
using UtilityLayer;
using BusinessLogicLayer.EmailDetails;

namespace ERP.OMS.Management.Activities
{
    public partial class ProjectInvoice : System.Web.UI.Page
    {
        BusinessLogicLayer.Converter objConverter = new BusinessLogicLayer.Converter();
        //   BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
        BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();


        clsDropDownList oclsDropDownList = new clsDropDownList();
        BusinessLogicLayer.Contact oContactGeneralBL = new BusinessLogicLayer.Contact();
        public EntityLayer.CommonELS.UserRightsForPage rights = new UserRightsForPage();
        public EntityLayer.CommonELS.UserRightsForPage SlInvoiceRights = new UserRightsForPage();
        static string ForJournalDate = null;
        SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
        ProjectInvoiceBL objProjectInvoiceBL = new ProjectInvoiceBL();
        CRMSalesDtlBL objCRMSalesDtlBL = new CRMSalesDtlBL();
        GSTtaxDetails gstTaxDetails = new GSTtaxDetails();

        BusinessLogicLayer.RemarkCategoryBL reCat = new BusinessLogicLayer.RemarkCategoryBL();
        PosSalesInvoiceBl pos = new PosSalesInvoiceBl();
        string UniqueQuotation = string.Empty;
        public string pageAccess = "";
        string userbranch = "";
        DataTable Remarks = null;
        string QuotationIds = string.Empty;
        string QuoteId = string.Empty;
        string Doctype = string.Empty;
        public EntityLayer.CommonELS.UserRightsForPage rightsProd = new UserRightsForPage();

        protected void Page_Init(object sender, EventArgs e)
        {

            CountrySelect.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
            StateSelect.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
            SelectCity.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
            SelectArea.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
            SelectPin.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
            sqltaxDataSource.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            SlInvoiceRights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/management/master/CustomerMasterList.aspx");
            rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/management/Activities/ProjectInvoice.aspx");
            rightsProd = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/management/store/Master/sProducts.aspx");
            if (HttpContext.Current.Session["userid"] == null)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "SighOff", "<script>SignOff();</script>");
            }
            CommonBL ComBL = new CommonBL();
            string ProjectSelectInEntryModule = ComBL.GetSystemSettingsResult("ProjectSelectInEntryModule");
            if (!String.IsNullOrEmpty(ProjectSelectInEntryModule))
            {
                if (ProjectSelectInEntryModule == "Yes")
                {
                    hdnProjectSelectInEntryModule.Value = "1";
                    lookup_Project.ClientVisible = true;
                    lblProject.Visible = true;
                }
                else if (ProjectSelectInEntryModule.ToUpper().Trim() == "NO")
                {
                    hdnProjectSelectInEntryModule.Value = "0";
                    lookup_Project.ClientVisible = false;
                    lblProject.Visible = false;
                }
            }
            string ProjectMandatoryInEntry = ComBL.GetSystemSettingsResult("ProjectMandatoryInEntry");

            if (!String.IsNullOrEmpty(ProjectMandatoryInEntry))
            {
                if (ProjectMandatoryInEntry == "Yes")
                {
                    hdnProjectMandatory.Value = "1";
                }
                else if (ProjectMandatoryInEntry.ToUpper().Trim() == "NO")
                {
                    hdnProjectMandatory.Value = "0";
                }
            }
            string ProjectTermsCondition = ComBL.GetSystemSettingsResult("ShowProjectTCProjectSalesInvoice");
            if (!String.IsNullOrEmpty(ProjectTermsCondition))
            {
                if (ProjectTermsCondition == "Yes")
                {
                    dvSOTC.Style.Add("display", "inline-block");
                    hdnProjectSOTC.Value = "1";
                }
                else if (ProjectTermsCondition.ToUpper().Trim() == "NO")
                {
                    dvSOTC.Style.Add("display", "none");
                    hdnProjectSOTC.Value = "0";
                }
            }

            //Tanmoy Hierarchy
            string HierarchySelectInEntryModule = ComBL.GetSystemSettingsResult("Show_Hierarchy");
            if (!String.IsNullOrEmpty(HierarchySelectInEntryModule))
            {
                if (HierarchySelectInEntryModule.ToUpper().Trim() == "YES")
                {
                    ddlHierarchy.Visible = true;
                    lblHierarchy.Visible = true;
                    lookup_Project.Columns[3].Visible = true;
                }
                else if (HierarchySelectInEntryModule.ToUpper().Trim() == "NO")
                {
                    ddlHierarchy.Visible = false;
                    lblHierarchy.Visible = false;
                    lookup_Project.Columns[3].Visible = false;
                }
            }
            //Tanmoy Hierarchy End
            hdnSettingsShowRetention.Value = "";
            if (!IsPostBack)
            {
                string SalesOrderItemNegative = ComBL.GetSystemSettingsResult("SalesOrderItemNegative");
                if (!String.IsNullOrEmpty(SalesOrderItemNegative))
                {
                    if (SalesOrderItemNegative == "Yes")
                    {
                        hdnSalesOrderItemNegative.Value = "1";

                    }
                    else if (SalesOrderItemNegative.ToUpper().Trim() == "NO")
                    {
                        hdnSalesOrderItemNegative.Value = "0";

                    }
                }
                string ProjAlertForRetention = ComBL.GetSystemSettingsResult("ProjAlertForRetention");
                if (!String.IsNullOrEmpty(ProjAlertForRetention))
                {
                    if (ProjAlertForRetention == "Yes")
                    {
                        hdnAlertRetention.Value = "1";

                    }
                    else if (ProjAlertForRetention.ToUpper().Trim() == "NO")
                    {
                        hdnAlertRetention.Value = "0";

                    }
                }

                Session["PSI_ProductDetails"] = null;
                #region New Tax Block

                string ItemLevelTaxDetails = string.Empty; string HSNCodewisetaxSchemid = string.Empty; string BranchWiseStateTax = string.Empty; string StateCodeWiseStateIDTax = string.Empty;
                gstTaxDetails.GetTaxData(ref ItemLevelTaxDetails, ref HSNCodewisetaxSchemid, ref BranchWiseStateTax, ref StateCodeWiseStateIDTax, "S");
                HDItemLevelTaxDetails.Value = ItemLevelTaxDetails;
                HDHSNCodewisetaxSchemid.Value = HSNCodewisetaxSchemid;
                HDBranchWiseStateTax.Value = BranchWiseStateTax;
                HDStateCodeWiseStateIDTax.Value = StateCodeWiseStateIDTax;

                #endregion
                //txt_PLQuoteNo.Enabled = false;
                //ddl_numberingScheme.Focus();
                ddlInventory.Focus();
                #region Sandip Section For Checking User Level for Allow Edit to logging User or Not
                GetEditablePermission();
                //PopulateCustomerDetail();
                SetFinYearCurrentDate();
                GetFinacialYearBasedQouteDate();
                if (Session["userbranchHierarchy"] != null)
                {
                    userbranch = Convert.ToString(Session["userbranchHierarchy"]);
                }
                GetAllDropDownDetailForSalesQuotation(userbranch);
                //Tanmoy Hierarchy
                bindHierarchy();
                ddlHierarchy.Enabled = false;
                //Tanmoy Hierarchy End

                if (Request.QueryString.AllKeys.Contains("status"))
                {
                    divcross.Visible = false;
                    btn_SaveRecords.Visible = false;
                    ApprovalCross.Visible = true;
                    ddl_Branch.Enabled = false;
                }
                else
                {
                    divcross.Visible = true;
                    btn_SaveRecords.Visible = true;
                    ApprovalCross.Visible = false;
                    ddl_Branch.Enabled = false; // Change Due to Numbering Schema
                }

                dt_PlQuoteExpiry.Value = Convert.ToDateTime(oDBEngine.GetDate().ToString());
                string finyear = Convert.ToString(Session["LastFinYear"]);
                IsUdfpresent.Value = Convert.ToString(getUdfCount());
                hdnAddressDtl.Value = "0";
                #endregion Sandip Section
                #region Session Null Initialization Start
                //Session["SI_QuotationAddressDtl"] = null;
                Session["SI_BillingAddressLookup"] = null;
                Session["SI_ShippingAddressLookup"] = null;
                #endregion Session Null Initialization End
                //Purpose : Binding Batch Edit Grid
                //Name : Sudip 
                // Dated : 21-01-2017
                IsUdfpresent.Value = Convert.ToString(getUdfCount());

                Session["SI_InvoiceID"] = "";
                Session["SI_CustomerDetail"] = null;
                Session["SI_QuotationDetails"] = null;
                Session["SI_WarehouseData"] = null;
                Session["SI_QuotationTaxDetails"] = null;
                Session["SI_LoopWarehouse"] = 1;
                Session["SI_TaxDetails"] = null;
                Session["SI_ActionType"] = "";
                Session["SI_ComponentData"] = null;
                Session["InlineRemarks"] = null;
                Session["requesttype"] = "Customer/Client";
                Session["Contactrequesttype"] = "customer";
                PopulateGSTCSTVATCombo(DateTime.Now.ToString("yyyy-MM-dd"));
                PopulateChargeGSTCSTVATCombo(DateTime.Now.ToString("yyyy-MM-dd"));
                string strQuotationId = "";
                hdnCustomerId.Value = "";
                hdnPageStatus.Value = "first";
                //LoadGSTCSTVATCombo();
                Session["SI_QuotationAddressDtl"] = null;

                if (Request.QueryString["key"] != null)
                {
                    if (Convert.ToString(Request.QueryString["key"]) != "ADD")
                    {
                        chkSendMail.Visible = false;
                        chkSendMail.Checked = false;
                        //ASPxButton1.ClientEnabled = false;
                        //btn_SaveRecords.ClientEnabled = false;
                        lblHeadTitle.Text = "Modify Project Sales Invoice";
                        hdnPageStatus.Value = "update";
                        divScheme.Style.Add("display", "none");
                        strQuotationId = Convert.ToString(Request.QueryString["key"]);
                        Session["SI_KeyVal_InternalID"] = "PIQUOTE" + strQuotationId;
                        hdAddOrEdit.Value = "Edit";
                        Keyval_internalId.Value = "SaleInvoice" + strQuotationId;
                        hdnProjPageStat.Value = "update";
                        hdnEditID.Value = strQuotationId;
                        #region Subhra Section
                        hdnEditIdForTerms.Value = strQuotationId;
                        Session["SI_key_QutId"] = strQuotationId;
                        if (hdnProjectSOTC.Value == "1")
                        {
                            QuoteId = strQuotationId;
                            Doctype = "SI";
                            ProjectTermsConditionsControl.SetEditTermsCoditionData(Convert.ToString(QuoteId), Doctype);
                        }


                        if (Request.QueryString["status"] == null)
                        {
                            IsExistsDocumentInERPDocApproveStatus(strQuotationId);
                        }

                        #endregion End Section
                        #region Product, Quotation Tax, Warehouse

                        Session["SI_InvoiceID"] = strQuotationId;
                        Session["SI_ActionType"] = "Edit";
                        SetInvoiceDetails(strQuotationId);
                        //  DateTime quoteDate = Convert.ToDateTime(dt_PLQuote.Date.ToString("dd/MM/yyyy"));
                        Session["SI_TaxDetails"] = GetTaxDataWithGST(GetTaxData(dt_PLQuote.Date.ToString("yyyy-MM-dd")));

                        Session["SI_WarehouseData"] = GetQuotationWarehouseData();

                        DataTable Productdt = objProjectInvoiceBL.GetProjectInvoiceProductData(strQuotationId).Tables[0];
                        Session["SI_QuotationDetails"] = Productdt;
                        grid.DataSource = GetQuotation(Productdt);
                        grid.DataBind();

                        #region ##### Existing BillingShipping Code : ############

                        //Session["SI_QuotationAddressDtl"] = objProjectInvoiceBL.GetInvoiceBillingAddress(strQuotationId);
                        //if (Session["SI_QuotationAddressDtl"] != null)
                        //{
                        //    DataTable dt = (DataTable)Session["SI_QuotationAddressDtl"];
                        //    if (dt != null && dt.Rows.Count > 0)
                        //    {
                        //        if (dt.Rows.Count == 2)
                        //        {
                        //            if (Convert.ToString(dt.Rows[0]["QuoteAdd_addressType"]) == "Shipping")
                        //            {
                        //                string countryid = Convert.ToString(dt.Rows[0]["QuoteAdd_countryId"]);
                        //                CmbCountry1.Value = countryid;
                        //                FillStateCombo(CmbState1, Convert.ToInt32(countryid));
                        //                string stateid = Convert.ToString(dt.Rows[0]["QuoteAdd_stateId"]);
                        //                CmbState1.Value = stateid;
                        //            }
                        //            else if (Convert.ToString(dt.Rows[1]["QuoteAdd_addressType"]) == "Shipping")
                        //            {
                        //                string countryid = Convert.ToString(dt.Rows[0]["QuoteAdd_countryId"]);
                        //                CmbCountry1.Value = countryid;
                        //                FillStateCombo(CmbState1, Convert.ToInt32(countryid));
                        //                string stateid = Convert.ToString(dt.Rows[0]["QuoteAdd_stateId"]);
                        //                CmbState1.Value = stateid;
                        //            }
                        //        }
                        //        else if (dt.Rows.Count == 1)
                        //        {
                        //            if (Convert.ToString(dt.Rows[0]["QuoteAdd_addressType"]) == "Shipping")
                        //            {
                        //                string countryid = Convert.ToString(dt.Rows[0]["QuoteAdd_countryId"]);
                        //                CmbCountry1.Value = countryid;
                        //                FillStateCombo(CmbState1, Convert.ToInt32(countryid));
                        //                string stateid = Convert.ToString(dt.Rows[0]["QuoteAdd_stateId"]);
                        //                CmbState1.Value = stateid;
                        //            }

                        //        }
                        //    }
                        //}
                        #endregion

                        #endregion
                        #region Debjyoti Get Tax Details in Edit Mode

                        Session["SI_QuotationTaxDetails"] = GetQuotationTaxData().Tables[0];
                        DataTable quotetable = GetQuotationEditedTaxData().Tables[0];
                        if (quotetable == null)
                        {
                            CreateDataTaxTable();
                        }
                        else
                        {
                            Session["SI_FinalTaxRecord"] = quotetable;
                        }

                        #endregion Debjyoti Get Tax Details in Edit Mode
                        int IsInvoiceUsed = objProjectInvoiceBL.CheckInvoice(strQuotationId);
                        if (IsInvoiceUsed == -99)
                        {
                            lbl_quotestatusmsg.Text = "*** Used in other module.";
                            btn_SaveRecords.Visible = false;
                            ASPxButton1.Visible = false;
                            lbl_quotestatusmsg.Visible = true;
                        }
                        #region Samrat Roy -- Hide Save Button in Edit Mode
                        if (Request.QueryString["req"] != null && Request.QueryString["req"] == "V")
                        {
                            lblHeadTitle.Text = "View Project Sales Invoice";
                            lbl_quotestatusmsg.Text = "*** View Mode Only";
                            btn_SaveRecords.Visible = false;
                            ASPxButton1.Visible = false;
                            lbl_quotestatusmsg.Visible = true;
                        }
                        #endregion
                    }
                    else
                    {
                        Employee_BL objemployeebal = new Employee_BL();
                        DataTable dt2 = new DataTable();
                        dt2 = objemployeebal.GetSystemsettingmail("Show Email in SI");
                        if (Convert.ToString(dt2.Rows[0]["Variable_Value"]) == "Yes")
                        {
                            chkSendMail.Visible = true;
                            chkSendMail.Checked = true;
                        }
                        else
                        {
                            chkSendMail.Visible = false;
                            chkSendMail.Checked = false;
                        }

                        #region To Show By Default Cursor after SAVE AND NEW
                        if (Session["SI_SaveMode"] != null)  // it has been removed from coding side of Quotation list 
                        {
                            if (Session["SI_schemavalue"] != null)  // it has been removed from coding side of Quotation list 
                            {
                                ddl_numberingScheme.SelectedValue = Convert.ToString(Session["SI_schemavalue"]); // it has been removed from coding side of Quotation list 
                                ddl_Branch.SelectedValue = Convert.ToString(Session["SI_schemavalue"]).Split('~')[3];
                            }

                            if (Convert.ToString(Session["SI_SaveMode"]) == "A")
                            {
                                dt_PLQuote.Focus();
                                txt_PLQuoteNo.ClientEnabled = false;
                                txt_PLQuoteNo.Text = "Auto";
                            }
                            else if (Convert.ToString(Session["SI_SaveMode"]) == "M")
                            {
                                txt_PLQuoteNo.ClientEnabled = true;
                                txt_PLQuoteNo.Text = "";
                                txt_PLQuoteNo.Focus();

                                string MaxLenth = Convert.ToString(Session["SI_schemavalue"]).Split('~')[2];
                                txt_PLQuoteNo.MaxLength = Convert.ToInt32(MaxLenth);
                            }
                        }
                        else
                        {
                            ddlInventory.Focus();
                        }
                        #endregion To Show By Default Cursor after SAVE AND NEW

                        //Debjyoti: Inventory Type Should Select from Listing Page
                        ddlInventory.SelectedValue = Request.QueryString["InvType"];
                        ddlInventory.Enabled = false;
                        ddl_numberingScheme.Focus();
                        //Debjyoti End here
                        hdnProjPageStat.Value = "Add";
                        Keyval_internalId.Value = "Add";
                        Session["SI_ActionType"] = "Add";
                        hdAddOrEdit.Value = "Add";
                        //ASPxButton2.Enabled = false;
                        Session["SI_TaxDetails"] = null;
                        CreateDataTaxTable();
                        lblHeadTitle.Text = "Add Project Sales Invoice";
                        ddl_AmountAre.Value = "1";
                        ddl_VatGstCst.SelectedIndex = 0;
                        ddl_VatGstCst.ClientEnabled = false;
                    }
                }

                string strSQL = "Select 'Y' From Config_SystemSettings Where Variable_Name='IsSIDiscountPercentage' AND Variable_Value='Yes'";
                DataTable dtSQL = oDBEngine.GetDataTable(strSQL);
                if (dtSQL != null && dtSQL.Rows.Count > 0)
                {
                    IsDiscountPercentage.Value = "Y";
                    grid.Columns[12].Caption = "Add/Less (%)";
                }
                else
                {
                    grid.Columns[12].Caption = "Add/Less Amt";
                    IsDiscountPercentage.Value = "N";
                }

                string strBranch = Convert.ToString(ddl_Branch.SelectedValue);
                string sqlQuery = " Select * from tbl_master_ApprovalConfiguration where Active=1 AND Entries_Id=4 AND BranchId='" + strBranch + "'";
                DataTable dtC = oDBEngine.GetDataTable(sqlQuery);
                if (dtC != null && dtC.Rows.Count > 0)
                {
                    ddlCashBank.ClientEnabled = false;
                }
                else
                {
                    ddlCashBank.ClientEnabled = true;
                }
                ddlCashBank.ClientEnabled = false;
                if (Request.QueryString.AllKeys.Contains("IsTagged"))
                {
                    ApprovalCross.Visible = false;
                    divcross.Visible = false;
                }

                if (ddlInventory.SelectedValue != "Y")
                {
                    foreach (ListItem item in rdl_SaleInvoice.Items)
                    {
                        if (item.Value == "SC")
                        {
                            item.Enabled = false;
                            item.Attributes.Add("style", "color:#999;");
                            break;
                        }
                    }
                }

                if (ddlInventory.SelectedValue == "S")
                {
                    foreach (ListItem item in rdl_SaleInvoice.Items)
                    {
                        if (item.Value == "SC" || item.Value == "SO" || item.Value == "QO")
                        {
                            item.Enabled = false;
                            item.Attributes.Add("style", "color:#999;");
                            break;
                        }
                    }
                }

                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "GridCallBack()", true);
                MasterSettings objmaster = new MasterSettings();
                hdnConvertionOverideVisible.Value = objmaster.GetSettings("ConvertionOverideVisible");
                hdnShowUOMConversionInEntry.Value = objmaster.GetSettings("ShowUOMConversionInEntry");
            }
            else
            {
                //PopulateCustomerDetail();
            }
        }
        protected void grid_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("Item #{0}", e.ListSourceRowIndex);
            }
            if (e.Column.FieldName == "Warehouse")
            {
            }
        }

        #region Sandip Section For Approval Section Start
        ERPDocPendingApprovalBL objERPDocPendingApproval = new ERPDocPendingApprovalBL();
        #endregion Sandip Section For Approval Dtl Section End

        #region Sandip Section For Approval Section Start
        public void IsExistsDocumentInERPDocApproveStatus(string orderId)
        {
            string editable = "";
            string status = "";
            DataTable dt = new DataTable();
            int Id = Convert.ToInt32(orderId);
            dt = objERPDocPendingApproval.IsExistsDocumentInERPDocApproveStatus(Id, 4); // 2 for Sale Invoice
            if (dt.Rows.Count > 0)
            {
                editable = Convert.ToString(dt.Rows[0]["editable"]);
                if (editable == "0")
                {
                    lbl_quotestatusmsg.Visible = true;
                    status = Convert.ToString(dt.Rows[0]["Status"]);
                    if (status == "Approved")
                    {
                        lbl_quotestatusmsg.Text = "Document already Approved";
                    }
                    if (status == "Rejected")
                    {
                        lbl_quotestatusmsg.Text = "Document already Rejected";
                    }
                    btn_SaveRecords.Visible = false;
                    ASPxButton1.Visible = false;
                }
                else
                {
                    lbl_quotestatusmsg.Visible = false;
                    btn_SaveRecords.Visible = true;
                    ASPxButton1.Visible = true;
                }
            }
        }

        #endregion Sandip Section For Approval Dtl Section End

        #region Other Details
        public void GetEditablePermission()
        {
            if (Request.QueryString["Permission"] != null)
            {
                if (Convert.ToString(Request.QueryString["Permission"]) == "1")
                {

                    btn_SaveRecords.Visible = false;
                    ASPxButton1.Visible = false;
                }
                else if (Convert.ToString(Request.QueryString["Permission"]) == "2")
                {

                    btn_SaveRecords.Visible = true;
                    ASPxButton1.Visible = true;
                }
                else if (Convert.ToString(Request.QueryString["Permission"]) == "3")
                {

                    btn_SaveRecords.Visible = true;
                    ASPxButton1.Visible = true;
                }
            }
        }
        protected int getUdfCount()
        {
            DataTable udfCount = oDBEngine.GetDataTable("select 1 from tbl_master_remarksCategory rc where cat_applicablefor='SI'  and ( exists (select * from tbl_master_udfGroup where id=rc.cat_group_id and grp_isVisible=1) or rc.cat_group_id=0)");
            return udfCount.Rows.Count;
        }

        #endregion

        #region Code Checked and Modified
        protected void Page_PreInit(object sender, EventArgs e) // lead add
        {
            #region Sandip Section For Approval Section Start
            if (Request.QueryString.AllKeys.Contains("status") || Request.QueryString.AllKeys.Contains("IsTagged"))
            {
                this.Page.MasterPageFile = "~/OMS/MasterPage/PopUp.Master";
                //divcross.Visible = false;
            }
            else
            {
                this.Page.MasterPageFile = "~/OMS/MasterPage/ERP.Master";
                //divcross.Visible = true;
            }
            #endregion Sandip Section For Approval Dtl Section End

            if (!IsPostBack)
            {
                string sPath = Convert.ToString(HttpContext.Current.Request.Url);
                oDBEngine.Call_CheckPageaccessebility(sPath);
            }
        }

        #endregion

        #region Product Details, Warehouse

        protected void CmbProduct_Init(object sender, EventArgs e)
        {
            ASPxComboBox cityCombo = sender as ASPxComboBox;
            cityCombo.DataSource = GetProduct();
        }
        public void SetInvoiceDetails(string strInvoiceId)
        {

            //Chinmoy added Below Line
            DataSet dsQuotationEditdt = objProjectInvoiceBL.GetDetailsOfInvoicedata(strInvoiceId);
            DataTable QuotationEditdt = dsQuotationEditdt.Tables[0];
            if (QuotationEditdt != null && QuotationEditdt.Rows.Count > 0)
            {
                string Branch_Id = Convert.ToString(QuotationEditdt.Rows[0]["Invoice_BranchId"]);
                string Quote_Number = Convert.ToString(QuotationEditdt.Rows[0]["Invoice_Number"]);
                string Quote_Date = Convert.ToString(QuotationEditdt.Rows[0]["Invoice_Date"]);
                string Customer_Id = Convert.ToString(QuotationEditdt.Rows[0]["Customer_Id"]);
                string Customer_Name = Convert.ToString(QuotationEditdt.Rows[0]["CustomerName"]);
                string Contact_Person_Id = Convert.ToString(QuotationEditdt.Rows[0]["Contact_Person_Id"]);
                string Quote_Reference = Convert.ToString(QuotationEditdt.Rows[0]["Invoice_Reference"]);
                string Currency_Id = Convert.ToString(QuotationEditdt.Rows[0]["Currency_Id"]);
                string Currency_Conversion_Rate = Convert.ToString(QuotationEditdt.Rows[0]["Currency_Conversion_Rate"]);
                string Tax_Option = Convert.ToString(QuotationEditdt.Rows[0]["Tax_Option"]);
                string Tax_Code = Convert.ToString(QuotationEditdt.Rows[0]["Tax_Code"]);
                string Quote_SalesmanId = Convert.ToString(QuotationEditdt.Rows[0]["Invoice_SalesmanId"]);
                string Quote_SalesmanName = Convert.ToString(QuotationEditdt.Rows[0]["SalesmanName"]);
                string IsUsed = Convert.ToString(QuotationEditdt.Rows[0]["IsUsed"]);

                string CashBank_Code = Convert.ToString(QuotationEditdt.Rows[0]["CashBank_Code"]);
                string InvoiceCreatedFromDoc = Convert.ToString(QuotationEditdt.Rows[0]["InvoiceCreatedFromDoc"]);
                string InvoiceCreatedFromDoc_Ids = Convert.ToString(QuotationEditdt.Rows[0]["InvoiceCreatedFromDoc_Ids"]);
                string InvoiceCreatedFromDocDate = Convert.ToString(QuotationEditdt.Rows[0]["InvoiceCreatedFromDocDate"]);
                string DueDate = Convert.ToString(QuotationEditdt.Rows[0]["DueDate"]);

                string VehicleNumber = Convert.ToString(QuotationEditdt.Rows[0]["VehicleNumber"]);
                string TransporterName = Convert.ToString(QuotationEditdt.Rows[0]["TransporterName"]);
                string TransporterPhone = Convert.ToString(QuotationEditdt.Rows[0]["TransporterPhone"]);
                string IsInventory = Convert.ToString(QuotationEditdt.Rows[0]["IsInventory"]);
                string Remarks = Convert.ToString(QuotationEditdt.Rows[0]["Remarks"]);

                string invoice_ret_Percentage = Convert.ToString(QuotationEditdt.Rows[0]["invoice_ret_Percentage"]);
                string invoice_ret_amount = Convert.ToString(QuotationEditdt.Rows[0]["invoice_ret_amount"]);
                string invoice_ret_Remarks = Convert.ToString(QuotationEditdt.Rows[0]["invoice_ret_Remarks"]);
                string invoice_ret_GL = Convert.ToString(QuotationEditdt.Rows[0]["invoice_ret_GL"]);
                string invoice_ret_GL_Name = Convert.ToString(QuotationEditdt.Rows[0]["invoice_ret_GL_Name"]);

                if (!(QuotationEditdt.Rows[0]["invoice_ret_duedate"] is DBNull))
                    dtDueDate.Date = Convert.ToDateTime(QuotationEditdt.Rows[0]["invoice_ret_duedate"]);

                if (InvoiceCreatedFromDoc_Ids != "")
                {
                    lookup_Project.ClearButton.Visibility = AutoBoolean.False;
                    lookup_Project.ClientEnabled = false;
                }
                string TransCategory = Convert.ToString(QuotationEditdt.Rows[0]["TransCategory"]);
                drdTransCategory.SelectedValue = TransCategory;

                crtxtRetAmount.Text = invoice_ret_amount;
                crtxtRetPercentage.Text = invoice_ret_Percentage;
                btnGL.Text = invoice_ret_GL_Name;
                crtxtRemarks.Text = invoice_ret_Remarks;
                hdnROMainAc.Value = invoice_ret_GL;

                ddl_PosGst.DataSource = dsQuotationEditdt.Tables[2];
                ddl_PosGst.ValueField = "ID";
                ddl_PosGst.TextField = "Name";
                ddl_PosGst.DataBind();

                Session["InlineRemarks"] = dsQuotationEditdt.Tables[3];






                //////////////////  TCS section  /////////////////////////
                string strTCScode = Convert.ToString(QuotationEditdt.Rows[0]["TCSSection"]);
                string strTCSappl = Convert.ToString(QuotationEditdt.Rows[0]["TCSApplAmount"]);
                string strTCSpercentage = Convert.ToString(QuotationEditdt.Rows[0]["TCSPercentage"]);
                string strTCSamout = Convert.ToString(QuotationEditdt.Rows[0]["TCSAmount"]);

                txtTCSSection.Text = Convert.ToString(strTCScode);
                txtTCSapplAmount.Text = Convert.ToString(strTCSappl);
                txtTCSpercentage.Text = Convert.ToString(strTCSpercentage);
                txtTCSAmount.Text = Convert.ToString(strTCSamout);
                //////////////////////////////////////////////////////////



                string PosForGst = Convert.ToString(QuotationEditdt.Rows[0]["PosForGst"]);

                ddl_PosGst.Value = PosForGst;
                txtRemarks.Text = Convert.ToString(QuotationEditdt.Rows[0]["Remarks"]);
                txtCreditDays.Text = Convert.ToString(QuotationEditdt.Rows[0]["CreditDays"]);
                Sales_BillingShipping.SetBillingShippingTable(dsQuotationEditdt.Tables[1]);

                //ASPxTextBox txtDriverName = (ASPxTextBox)VehicleDetailsControl.FindControl("txtDriverName");
                //ASPxTextBox txtPhoneNo = (ASPxTextBox)VehicleDetailsControl.FindControl("txtPhoneNo");
                //DropDownList ddl_VehicleNo = (DropDownList)VehicleDetailsControl.FindControl("ddl_VehicleNo");

                //txtDriverName.Text = TransporterName;
                //txtPhoneNo.Text = TransporterPhone;
                //ddl_VehicleNo.SelectedValue = VehicleNumber;


                DataTable dtt = objProjectInvoiceBL.GetProjectEditData(Convert.ToString(Session["SI_InvoiceID"]));
                if (dtt != null)
                {

                    lookup_Project.GridView.Selection.SelectRowByKey(Convert.ToInt64(dtt.Rows[0]["Proj_Id"]));

                    //Tanmoy  Hierarchy
                    BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                    DataTable dt2 = oDBEngine.GetDataTable("Select Hierarchy_ID from V_ProjectList where Proj_Id='" + Convert.ToInt64(dtt.Rows[0]["Proj_Id"]) + "'");
                    if (dt2.Rows.Count > 0)
                    {
                        ddlHierarchy.SelectedValue = Convert.ToString(dt2.Rows[0]["Hierarchy_ID"]);
                    }
                    //Tanmoy  Hierarchy End
                }


                ddlCashBank.Value = CashBank_Code;
                if (InvoiceCreatedFromDoc != "") rdl_SaleInvoice.SelectedValue = InvoiceCreatedFromDoc;
                txt_InvoiceDate.Text = InvoiceCreatedFromDocDate;
                dt_SaleInvoiceDue.Date = Convert.ToDateTime(DueDate);

                if (!String.IsNullOrEmpty(InvoiceCreatedFromDoc_Ids))
                {
                    string[] eachQuo = InvoiceCreatedFromDoc_Ids.Split(',');
                    if (eachQuo.Length > 1)//More tha one quotation
                    {
                        BindLookUp(Customer_Id, Quote_Date, InvoiceCreatedFromDoc, Branch_Id);
                        foreach (string val in eachQuo)
                        {
                            lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                        }
                    }
                    else if (eachQuo.Length == 1)//Single Quotation
                    {
                        BindLookUp(Customer_Id, Quote_Date, InvoiceCreatedFromDoc, Branch_Id);
                        foreach (string val in eachQuo)
                        {
                            lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                        }
                    }
                    else//No Quotation selected
                    {
                        BindLookUp(Customer_Id, Quote_Date, InvoiceCreatedFromDoc, Branch_Id);
                    }
                }

                txt_PLQuoteNo.Text = Quote_Number;
                dt_PLQuote.Date = Convert.ToDateTime(Quote_Date);
                PopulateGSTCSTVATCombo(Convert.ToDateTime(Quote_Date).ToString("yyyy-MM-dd"));
                PopulateChargeGSTCSTVATCombo(Convert.ToDateTime(Quote_Date).ToString("yyyy-MM-dd"));

                //lookup_Customer.GridView.Selection.SelectRowByKey(Customer_Id);
                txtCustName.Text = Customer_Name;
                hdnCustomerId.Value = Customer_Id;

                TabPage page = ASPxPageControl1.TabPages.FindByName("Billing/Shipping");
                page.ClientEnabled = true;

                ddlInventory.SelectedValue = IsInventory;
                ddlInventory.Enabled = false;

                PopulateContactPersonOfCustomer(Customer_Id);
                cmbContactPerson.SelectedItem = cmbContactPerson.Items.FindByValue(Contact_Person_Id);
                txt_Refference.Text = Quote_Reference;
                ddl_Branch.SelectedValue = Branch_Id;
                //ddl_SalesAgent.SelectedValue = Quote_SalesmanId;
                hdnSalesManAgentId.Value = Quote_SalesmanId;
                txtSalesManAgent.Text = Quote_SalesmanName;
                ddl_Currency.SelectedValue = Currency_Id;
                txt_Rate.Value = Currency_Conversion_Rate;
                txt_Rate.Text = Currency_Conversion_Rate;
                if (Tax_Option != "0") ddl_AmountAre.Value = Tax_Option;
                if (Tax_Code != "0")
                {
                    PopulateGSTCSTVAT("2");
                    setValueForHeaderGST(ddl_VatGstCst, Tax_Code);
                }
                else
                {
                    PopulateGSTCSTVAT("2");
                    ddl_VatGstCst.SelectedIndex = 0;
                }
                ddl_AmountAre.ClientEnabled = false;
                ddl_VatGstCst.ClientEnabled = false;

                if (IsUsed == "Y")
                {
                    dt_PLQuote.ClientEnabled = false;
                    txtCustName.ClientEnabled = false;
                    //lookup_Customer.ClientEnabled = false;
                }
                else
                {
                    dt_PLQuote.ClientEnabled = true;
                    txtCustName.ClientEnabled = true;
                    //lookup_Customer.ClientEnabled = true;
                }
                txtCustName.ClientEnabled = false;
                dt_PLQuote.ClientEnabled = false;
            }
        }
        protected void BindLookUp(string Customer, string OrderDate, string ComponentType, string BranchID)
        {
            string Action = "";
            if (ComponentType == "QO")
            {
                Action = "GetQuotation";
            }
            else if (ComponentType == "SO")
            {
                Action = "GetOrder";
            }
            else if (ComponentType == "SC")
            {
                Action = "GetChallan";
            }

            string strInvoiceID = Convert.ToString(Session["SI_InvoiceID"]);
            string FinYear = Convert.ToString(Session["LastFinYear"]);
            DataTable ComponentTable = objProjectInvoiceBL.GetComponent(Customer, OrderDate, ComponentType, FinYear, BranchID, Action, strInvoiceID);
            lookup_quotation.GridView.Selection.CancelSelection();

            lookup_quotation.GridView.Selection.CancelSelection();
            lookup_quotation.DataSource = ComponentTable;
            lookup_quotation.DataBind();

            Session["SI_ComponentData"] = ComponentTable;
        }

        #region Page Classes
        public class Product
        {
            public string ProductID { get; set; }
            public string ProductName { get; set; }
        }
        public class Taxes
        {
            public string TaxID { get; set; }
            public string TaxName { get; set; }
            public string Percentage { get; set; }
            public string Amount { get; set; }
            public decimal calCulatedOn { get; set; }
        }
        public class ProjectQuotation
        {
            public string SrlNo { get; set; }
            public string QuotationID { get; set; }
            public string ProductID { get; set; }
            public string Description { get; set; }
            public string Quantity { get; set; }
            public string UOM { get; set; }
            public string Warehouse { get; set; }
            public string StockQuantity { get; set; }
            public string StockUOM { get; set; }
            public string SalePrice { get; set; }
            public string Discount { get; set; }
            public string Amount { get; set; }
            public string TaxAmount { get; set; }
            public string TotalAmount { get; set; }
            public string ProductName { get; set; }
            public string ComponentID { get; set; }
            public string ComponentNumber { get; set; }
            public string TotalQty { get; set; }
            public string BalanceQty { get; set; }
            public string IsComponentProduct { get; set; }
            public string IsLinkedProduct { get; set; }
            public string Remarks { get; set; }
            public string SONetAmount { get; set; }
            public string ComponentDetailsID { get; set; }
        }
        #endregion

        #region Product Details

        #region Code Checked and Modified

        public IEnumerable GetProduct()
        {
            List<Product> ProductList = new List<Product>();
            //  BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
            BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine();


            DataTable DT = GetProductData();
            //DataTable DT = GetProductData().Tables[0];
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                Product Products = new Product();
                Products.ProductID = Convert.ToString(DT.Rows[i]["Products_ID"]);
                Products.ProductName = Convert.ToString(DT.Rows[i]["Products_Name"]);
                ProductList.Add(Products);
            }

            return ProductList;
        }
        //public IEnumerable GetFilterProduct(string filter)
        //{
        //    List<Product> ProductList = new List<Product>();
        //    BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
        //    DataTable DT = GetFilterProductData(filter);
        //    //DataTable DT = GetProductData().Tables[0];
        //    for (int i = 0; i < DT.Rows.Count; i++)
        //    {
        //        Product Products = new Product();
        //        Products.ProductID = Convert.ToString(DT.Rows[i]["Products_ID"]);
        //        Products.ProductName = Convert.ToString(DT.Rows[i]["Products_Name"]);
        //        ProductList.Add(Products);
        //    }

        //    return ProductList;
        //}
        public DataTable GetProductData()
        {
            DataTable ds = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "ProductDetails");
            ds = proc.GetTable();
            return ds;
        }
        //public DataTable GetFilterProductData(string filter)
        //{
        //    DataTable ds = new DataTable();
        //    ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
        //    proc.AddVarcharPara("@Action", 500, "ProductFilterDetails");
        //    proc.AddVarcharPara("@Filter", 2000, filter);
        //    ds = proc.GetTable();
        //    return ds;
        //}

        #endregion Code Checked and Modified

        //public DataSet GetProductData()
        //{
        //    DataSet ds = new DataSet();
        //    ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
        //    proc.AddVarcharPara("@Action", 500, "ProductDetails");
        //    ds = proc.GetDataSet();
        //    return ds;
        //}
        public DataTable GetWarehouseData()
        {

            MasterSettings masterBl = new MasterSettings();
            string multiwarehouse = Convert.ToString(masterBl.GetSettings("IaMultilevelWarehouse"));

            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "GetWareHouseDtlByProductID");
            proc.AddVarcharPara("@QuotationID", 500, Convert.ToString(Session["SI_InvoiceID"]));
            proc.AddVarcharPara("@ProductID", 500, Convert.ToString(hdfProductID.Value));
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 2000, Convert.ToString(ddl_Branch.SelectedValue));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            proc.AddVarcharPara("@Multiwarehouse", 500, Convert.ToString(multiwarehouse));
            dt = proc.GetTable();
            return dt;
        }
        public DataTable GetProductDetailsData(string ProductID)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "ProductDetailsSearch");
            proc.AddVarcharPara("@ProductID", 500, ProductID);
            dt = proc.GetTable();
            return dt;
        }
        public DataTable GetBatchData(string WarehouseID)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "GetBatchByProductIDWarehouse");
            proc.AddVarcharPara("@QuotationID", 500, Convert.ToString(Session["SI_InvoiceID"]));
            proc.AddVarcharPara("@ProductID", 500, Convert.ToString(hdfProductID.Value));
            proc.AddVarcharPara("@WarehouseID", 500, WarehouseID);
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 2000, Convert.ToString(ddl_Branch.SelectedValue));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            dt = proc.GetTable();
            return dt;
        }
        public DataTable GetSerialata(string WarehouseID, string BatchID)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "GetSerialByProductIDWarehouseBatch");
            proc.AddVarcharPara("@QuotationID", 500, Convert.ToString(Session["SI_InvoiceID"]));
            proc.AddVarcharPara("@ProductID", 500, Convert.ToString(hdfProductID.Value));
            proc.AddVarcharPara("@BatchID", 500, BatchID);
            proc.AddVarcharPara("@WarehouseID", 500, WarehouseID);
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 2000, Convert.ToString(ddl_Branch.SelectedValue));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            dt = proc.GetTable();
            return dt;
        }
        public IEnumerable GetQuotation(DataTable Quotationdt)
        {
            List<ProjectQuotation> QuotationList = new List<ProjectQuotation>();

            if (Quotationdt != null && Quotationdt.Rows.Count > 0)
            {
                for (int i = 0; i < Quotationdt.Rows.Count; i++)
                {
                    ProjectQuotation Quotations = new ProjectQuotation();

                    Quotations.SrlNo = Convert.ToString(Quotationdt.Rows[i]["SrlNo"]);
                    Quotations.QuotationID = Convert.ToString(Quotationdt.Rows[i]["QuotationID"]);
                    Quotations.ProductID = Convert.ToString(Quotationdt.Rows[i]["ProductID"]);
                    Quotations.Description = Convert.ToString(Quotationdt.Rows[i]["Description"]);
                    Quotations.Quantity = Convert.ToString(Quotationdt.Rows[i]["Quantity"]);
                    Quotations.UOM = Convert.ToString(Quotationdt.Rows[i]["UOM"]);
                    Quotations.Warehouse = "";
                    Quotations.StockQuantity = Convert.ToString(Quotationdt.Rows[i]["StockQuantity"]);
                    Quotations.StockUOM = Convert.ToString(Quotationdt.Rows[i]["StockUOM"]);
                    Quotations.SalePrice = Convert.ToString(Quotationdt.Rows[i]["SalePrice"]);
                    Quotations.Discount = Convert.ToString(Quotationdt.Rows[i]["Discount"]);
                    Quotations.Amount = Convert.ToString(Quotationdt.Rows[i]["Amount"]);
                    Quotations.TaxAmount = Convert.ToString(Quotationdt.Rows[i]["TaxAmount"]);
                    Quotations.TotalAmount = Convert.ToString(Quotationdt.Rows[i]["TotalAmount"]);
                    Quotations.ProductName = Convert.ToString(Quotationdt.Rows[i]["ProductName"]);
                    Quotations.ComponentID = Convert.ToString(Quotationdt.Rows[i]["ComponentID"]);
                    Quotations.ComponentNumber = Convert.ToString(Quotationdt.Rows[i]["ComponentNumber"]);
                    Quotations.TotalQty = Convert.ToString(Quotationdt.Rows[i]["TotalQty"]);
                    Quotations.BalanceQty = Convert.ToString(Quotationdt.Rows[i]["BalanceQty"]);
                    Quotations.IsComponentProduct = Convert.ToString(Quotationdt.Rows[i]["IsComponentProduct"]);
                    Quotations.IsLinkedProduct = Convert.ToString(Quotationdt.Rows[i]["IsLinkedProduct"]);
                    Quotations.Remarks = Convert.ToString(Quotationdt.Rows[i]["Remarks"]);
                    Quotations.SONetAmount = Convert.ToString(Quotationdt.Rows[i]["SONetAmount"]);

                    Quotations.ComponentDetailsID = Convert.ToString(Quotationdt.Rows[i]["ComponentDetailsID"]);

                    QuotationList.Add(Quotations);
                }
            }

            return QuotationList;
        }
        protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            if (e.Column.FieldName == "Description")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "UOM")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "StkUOM")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "TaxAmount")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "ComponentNumber")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "TotalAmount")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "SrlNo")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "ProductName")
            {
                e.Editor.Enabled = true;
            }
            else
            {
                e.Editor.ReadOnly = false;
            }
        }
        protected void grid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            DataTable Quotationdt = new DataTable();
            string IsDeleteFrom = Convert.ToString(hdfIsDelete.Value);
            string SchemeID = "";


            string validate = "";
            grid.JSProperties["cpQuotationNo"] = "";
            grid.JSProperties["cpQuotationID"] = "";
            grid.JSProperties["cpSaveSuccessOrFail"] = "";

            if (Session["SI_QuotationDetails"] != null)
            {
                Quotationdt = (DataTable)Session["SI_QuotationDetails"];
            }
            else
            {
                Quotationdt.Columns.Add("SrlNo", typeof(string));
                Quotationdt.Columns.Add("QuotationID", typeof(string));
                Quotationdt.Columns.Add("ProductID", typeof(string));
                Quotationdt.Columns.Add("Description", typeof(string));
                Quotationdt.Columns.Add("Quantity", typeof(string));
                Quotationdt.Columns.Add("UOM", typeof(string));
                Quotationdt.Columns.Add("Warehouse", typeof(string));
                Quotationdt.Columns.Add("StockQuantity", typeof(string));
                Quotationdt.Columns.Add("StockUOM", typeof(string));
                Quotationdt.Columns.Add("SalePrice", typeof(string));
                Quotationdt.Columns.Add("Discount", typeof(string));
                Quotationdt.Columns.Add("Amount", typeof(string));
                Quotationdt.Columns.Add("TaxAmount", typeof(string));
                Quotationdt.Columns.Add("TotalAmount", typeof(string));
                Quotationdt.Columns.Add("Status", typeof(string));
                Quotationdt.Columns.Add("ProductName", typeof(string));
                Quotationdt.Columns.Add("ComponentID", typeof(string));
                Quotationdt.Columns.Add("ComponentNumber", typeof(string));
                Quotationdt.Columns.Add("TotalQty", typeof(string));
                Quotationdt.Columns.Add("BalanceQty", typeof(string));
                Quotationdt.Columns.Add("IsComponentProduct", typeof(string));
                Quotationdt.Columns.Add("IsLinkedProduct", typeof(string));
                Quotationdt.Columns.Add("Remarks", typeof(string));
                Quotationdt.Columns.Add("SONetAmount", typeof(string));
                Quotationdt.Columns.Add("ComponentDetailsID", typeof(string));
            }

            foreach (var args in e.InsertValues)
            {
                string ProductDetails = Convert.ToString(args.NewValues["ProductID"]);

                if (ProductDetails != "" && ProductDetails != "0")
                {
                    string SrlNo = Convert.ToString(args.NewValues["SrlNo"]);
                    string[] ProductDetailsList = ProductDetails.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string ProductID = ProductDetailsList[0];

                    string ProductName = Convert.ToString(args.NewValues["ProductName"]);
                    string Description = Convert.ToString(args.NewValues["Description"]);
                    string Quantity = Convert.ToString(args.NewValues["Quantity"]);
                    string UOM = Convert.ToString(args.NewValues["UOM"]);
                    string Warehouse = Convert.ToString(args.NewValues["Warehouse"]);

                    decimal strMultiplier = Convert.ToDecimal(ProductDetailsList[7]);
                    string StockQuantity = Convert.ToString(Convert.ToDecimal(Quantity) * strMultiplier);
                    string StockUOM = Convert.ToString(ProductDetailsList[4]);
                    //string StockQuantity = Convert.ToString(args.NewValues["StockQuantity"]);
                    //string StockUOM = Convert.ToString(args.NewValues["StockUOM"]);

                    string SalePrice = Convert.ToString(args.NewValues["SalePrice"]);
                    string Discount = (Convert.ToString(args.NewValues["Discount"]) != "") ? Convert.ToString(args.NewValues["Discount"]) : "0";
                    string Amount = (Convert.ToString(args.NewValues["Amount"]) != "") ? Convert.ToString(args.NewValues["Amount"]) : "0";
                    string TaxAmount = (Convert.ToString(args.NewValues["TaxAmount"]) != "") ? Convert.ToString(args.NewValues["TaxAmount"]) : "0";
                    string TotalAmount = (Convert.ToString(args.NewValues["TotalAmount"]) != "") ? Convert.ToString(args.NewValues["TotalAmount"]) : "0";

                    string ComponentID = (Convert.ToString(args.NewValues["ComponentID"]) != "") ? Convert.ToString(args.NewValues["ComponentID"]) : "0";
                    string ComponentName = (Convert.ToString(args.NewValues["ComponentNumber"]) != "") ? Convert.ToString(args.NewValues["ComponentNumber"]) : "";
                    string TotalQty = (Convert.ToString(args.NewValues["TotalQty"]) != "") ? Convert.ToString(args.NewValues["TotalQty"]) : "0";
                    string BalanceQty = (Convert.ToString(args.NewValues["BalanceQty"]) != "") ? Convert.ToString(args.NewValues["BalanceQty"]) : "0";
                    string IsComponentProduct = (Convert.ToString(args.NewValues["IsComponentProduct"]) != "") ? Convert.ToString(args.NewValues["IsComponentProduct"]) : "N";
                    string IsLinkedProduct = (Convert.ToString(args.NewValues["IsLinkedProduct"]) != "") ? Convert.ToString(args.NewValues["IsLinkedProduct"]) : "N";
                    string Remarks = (Convert.ToString(args.NewValues["Remarks"]) != "") ? Convert.ToString(args.NewValues["Remarks"]) : "";
                    string SONetAmount = (Convert.ToString(args.NewValues["SONetAmount"]) != "") ? Convert.ToString(args.NewValues["SONetAmount"]) : "0";

                    string ComponentDetailsID = (Convert.ToString(args.NewValues["ComponentDetailsID"]) != "") ? Convert.ToString(args.NewValues["ComponentDetailsID"]) : "0";

                    Quotationdt.Rows.Add(SrlNo, "0", ProductDetails, Description, Quantity, UOM, Warehouse, StockQuantity, StockUOM, SalePrice, Discount, Amount, TaxAmount, TotalAmount, "I", ProductName, ComponentID, ComponentName, TotalQty, BalanceQty, "", IsLinkedProduct, Remarks, SONetAmount, ComponentDetailsID);
                  


                    if (IsComponentProduct == "Y")
                    {
                        DataTable ComponentTable = objProjectInvoiceBL.GetLinkedProductList("LinkedProduct", ProductID);
                        foreach (DataRow drr in ComponentTable.Rows)
                        {
                            string sProductsID = Convert.ToString(drr["sProductsID"]);
                            string Products_Description = Convert.ToString(drr["Products_Description"]);
                            string Sales_UOM_Name = Convert.ToString(drr["Sales_UOM_Name"]);
                            string Conversion_Multiplier = Convert.ToString(drr["Conversion_Multiplier"]);
                            string Stk_UOM_Name = Convert.ToString(drr["Stk_UOM_Name"]);
                            string Product_SalePrice = Convert.ToString(drr["Product_SalePrice"]);
                            string Products_Name = Convert.ToString(drr["Products_Name"]);
                            string StkQty = Convert.ToString(Convert.ToDecimal(Quantity) * Convert.ToDecimal(Conversion_Multiplier));

                            Quotationdt.Rows.Add(Convert.ToString(Convert.ToInt32(SrlNo) + 1), "0", sProductsID, Products_Description, Quantity, Sales_UOM_Name, "", StkQty, Stk_UOM_Name, Product_SalePrice, "0.0", "0.0", "0.0", "0.0", "I", Products_Name, ComponentID, ComponentName, "0.0", "0.0", "", "Y", Remarks, SONetAmount, ComponentDetailsID);
                        }
                    }
                }
            }

            foreach (var args in e.UpdateValues)
            {
                string SrlNo = Convert.ToString(args.NewValues["SrlNo"]);
                string QuotationID = Convert.ToString(args.Keys["QuotationID"]);
                string ProductDetails = Convert.ToString(args.NewValues["ProductID"]);

                bool isDeleted = false;
                foreach (var arg in e.DeleteValues)
                {
                    string DeleteID = Convert.ToString(arg.Keys["QuotationID"]);
                    if (DeleteID == QuotationID)
                    {
                        isDeleted = true;
                        break;
                    }
                }

                if (isDeleted == false)
                {
                    if (ProductDetails != "" && ProductDetails != "0")
                    {
                        string[] ProductDetailsList = ProductDetails.Split(new string[] { "||@||" }, StringSplitOptions.None);
                        string ProductID = Convert.ToString(ProductDetailsList[0]);

                        string ProductName = Convert.ToString(args.NewValues["ProductName"]);
                        string Description = Convert.ToString(args.NewValues["Description"]);
                        string Quantity = Convert.ToString(args.NewValues["Quantity"]);
                        string UOM = Convert.ToString(args.NewValues["UOM"]);
                        string Warehouse = Convert.ToString(args.NewValues["Warehouse"]);

                        decimal strMultiplier = Convert.ToDecimal(ProductDetailsList[7]);
                        string StockQuantity = Convert.ToString(Convert.ToDecimal(Quantity) * strMultiplier);
                        string StockUOM = Convert.ToString(ProductDetailsList[4]);
                        //string StockQuantity = Convert.ToString(args.NewValues["StockQuantity"]);
                        //string StockUOM = Convert.ToString(args.NewValues["StockUOM"]);

                        string SalePrice = Convert.ToString(args.NewValues["SalePrice"]);
                        string Discount = (Convert.ToString(args.NewValues["Discount"]) != "") ? Convert.ToString(args.NewValues["Discount"]) : "0";
                        string Amount = (Convert.ToString(args.NewValues["Amount"]) != "") ? Convert.ToString(args.NewValues["Amount"]) : "0";
                        string TaxAmount = (Convert.ToString(args.NewValues["TaxAmount"]) != "") ? Convert.ToString(args.NewValues["TaxAmount"]) : "0";
                        string TotalAmount = (Convert.ToString(args.NewValues["TotalAmount"]) != "") ? Convert.ToString(args.NewValues["TotalAmount"]) : "0";

                        string ComponentID = (Convert.ToString(args.NewValues["ComponentID"]) != "") ? Convert.ToString(args.NewValues["ComponentID"]) : "0";
                        string ComponentName = (Convert.ToString(args.NewValues["ComponentNumber"]) != "") ? Convert.ToString(args.NewValues["ComponentNumber"]) : "";
                        string TotalQty = (Convert.ToString(args.NewValues["TotalQty"]) != "") ? Convert.ToString(args.NewValues["TotalQty"]) : "0";
                        string BalanceQty = (Convert.ToString(args.NewValues["BalanceQty"]) != "") ? Convert.ToString(args.NewValues["BalanceQty"]) : "0";
                        string IsComponentProduct = (Convert.ToString(args.NewValues["IsComponentProduct"]) != "") ? Convert.ToString(args.NewValues["IsComponentProduct"]) : "N";
                        string IsLinkedProduct = (Convert.ToString(args.NewValues["IsLinkedProduct"]) != "") ? Convert.ToString(args.NewValues["IsLinkedProduct"]) : "N";
                        string Remarks = (Convert.ToString(args.NewValues["Remarks"]) != "") ? Convert.ToString(args.NewValues["Remarks"]) : "";
                        string SONetAmount = (Convert.ToString(args.NewValues["SONetAmount"]) != "") ? Convert.ToString(args.NewValues["SONetAmount"]) : "0";

                        string ComponentDetailsID = (Convert.ToString(args.NewValues["ComponentDetailsID"]) != "") ? Convert.ToString(args.NewValues["ComponentDetailsID"]) : "0";

                        bool Isexists = false;


                        foreach (DataRow drr in Quotationdt.Rows)
                        {
                            string OldQuotationID = Convert.ToString(drr["QuotationID"]);

                            if (OldQuotationID == QuotationID)
                            {
                                Isexists = true;

                                drr["ProductID"] = ProductDetails;
                                drr["Description"] = Description;
                                drr["Quantity"] = Quantity;
                                drr["UOM"] = UOM;
                                drr["Warehouse"] = Warehouse;
                                drr["StockQuantity"] = StockQuantity;
                                drr["StockUOM"] = StockUOM;
                                drr["SalePrice"] = SalePrice;
                                drr["Discount"] = Discount;
                                drr["Amount"] = Amount;
                                drr["TaxAmount"] = TaxAmount;
                                drr["TotalAmount"] = TotalAmount;
                                drr["Status"] = "U";
                                drr["ProductName"] = ProductName;
                                drr["ComponentID"] = ComponentID;
                                drr["ComponentNumber"] = ComponentName;
                                drr["TotalQty"] = TotalQty;
                                drr["BalanceQty"] = BalanceQty;
                                drr["IsComponentProduct"] = IsComponentProduct;
                                drr["IsLinkedProduct"] = IsLinkedProduct;
                                drr["Remarks"] = Remarks;
                                drr["SONetAmount"] = SONetAmount;
                                drr["ComponentDetailsID"] = ComponentDetailsID;
                                break;
                            }
                        }

                        if (Isexists == false)
                        {
                            Quotationdt.Rows.Add(SrlNo, QuotationID, ProductDetails, Description, Quantity, UOM, Warehouse, StockQuantity, StockUOM, SalePrice, Discount, Amount, TaxAmount, TotalAmount, "U", ProductName, ComponentID, ComponentName, TotalQty, BalanceQty, IsComponentProduct, IsLinkedProduct, Remarks, SONetAmount, ComponentDetailsID);
                        }

                        if (IsComponentProduct == "Y")
                        {
                            DataTable ComponentTable = objProjectInvoiceBL.GetLinkedProductList("LinkedProduct", ProductID);
                            foreach (DataRow drr in ComponentTable.Rows)
                            {
                                string sProductsID = Convert.ToString(drr["sProductsID"]);
                                string Products_Description = Convert.ToString(drr["Products_Description"]);
                                string Sales_UOM_Name = Convert.ToString(drr["Sales_UOM_Name"]);
                                string Conversion_Multiplier = Convert.ToString(drr["Conversion_Multiplier"]);
                                string Stk_UOM_Name = Convert.ToString(drr["Stk_UOM_Name"]);
                                string Product_SalePrice = Convert.ToString(drr["Product_SalePrice"]);
                                string Products_Name = Convert.ToString(drr["Products_Name"]);
                                string StkQty = Convert.ToString(Convert.ToDecimal(Quantity) * Convert.ToDecimal(Conversion_Multiplier));

                                Quotationdt.Rows.Add(Convert.ToString(Convert.ToInt32(SrlNo) + 1), "0", sProductsID, Products_Description, Quantity, Sales_UOM_Name, "", StkQty, Stk_UOM_Name, Product_SalePrice, "0.0", "0.0", "0.0", "0.0", "I", Products_Name, ComponentID, ComponentName, "0.0", "0.0", "", "Y", "", SONetAmount, ComponentDetailsID);
                            }
                        }
                    }
                }
            }

            foreach (var args in e.DeleteValues)
            {
                string QuotationID = Convert.ToString(args.Keys["QuotationID"]);
                string SrlNo = "";

                for (int i = Quotationdt.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = Quotationdt.Rows[i];
                    string delQuotationID = Convert.ToString(dr["QuotationID"]);

                    if (delQuotationID == QuotationID)
                    {
                        SrlNo = Convert.ToString(dr["SrlNo"]);
                        dr.Delete();
                    }
                }
                Quotationdt.AcceptChanges();

                DeleteWarehouse(SrlNo);
                DeleteTaxDetails(SrlNo);

                if (QuotationID.Contains("~") != true)
                {
                    Quotationdt.Rows.Add("0", QuotationID, "0", "", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "D", "", "0", "", "0", "0", "", "", "","0","0");
                }
            }

            ///////////////////////

            string strDeleteSrlNo = Convert.ToString(hdnDeleteSrlNo.Value);
            if (strDeleteSrlNo != "" && strDeleteSrlNo != "0")
            {
                DeleteWarehouse(strDeleteSrlNo);
                DeleteTaxDetails(strDeleteSrlNo);

                hdnDeleteSrlNo.Value = "";
            }

            int j = 1;
            foreach (DataRow dr in Quotationdt.Rows)
            {
                string Status = Convert.ToString(dr["Status"]);
                string oldSrlNo = Convert.ToString(dr["SrlNo"]);
                string newSrlNo = j.ToString();

                dr["SrlNo"] = j.ToString();

                UpdateWarehouse(oldSrlNo, newSrlNo);
                UpdateTaxDetails(oldSrlNo, newSrlNo);

                if (Status != "D")
                {
                    if (Status == "I")
                    {
                        string strID = Convert.ToString("Q~" + j);
                        dr["QuotationID"] = strID;
                    }
                    j++;
                }
            }
            Quotationdt.AcceptChanges();

            Session["SI_QuotationDetails"] = Quotationdt;

            //////////////////////


            if (IsDeleteFrom != "D" && IsDeleteFrom != "C")
            {
                string InvoiceComponentDate = "", InvoiceComponent = "";

                string ActionType = Convert.ToString(Session["SI_ActionType"]);
                string MainInvoiceID = Convert.ToString(Session["SI_InvoiceID"]);

                string strSchemeType = Convert.ToString(ddl_numberingScheme.SelectedValue);
                string strInvoiceNo = Convert.ToString(txt_PLQuoteNo.Text);
                string strInvoiceDate = Convert.ToString(dt_PLQuote.Date);
                string strBranch = Convert.ToString(ddl_Branch.SelectedValue);

                string strCustomer = Convert.ToString(hdfLookupCustomer.Value);
                string strContactName = Convert.ToString(cmbContactPerson.Value);
                string Reference = Convert.ToString(txt_Refference.Text);
                //string strAgents = Convert.ToString(ddl_SalesAgent.SelectedValue);
                string strAgents = hdnSalesManAgentId.Value;

                string strIsInventory = Convert.ToString(ddlInventory.SelectedValue);

                string strComponenyType = Convert.ToString(rdl_SaleInvoice.SelectedValue);
                List<object> QuoList = lookup_quotation.GridView.GetSelectedFieldValues("ComponentID");
                foreach (object Quo in QuoList)
                {
                    InvoiceComponent += "," + Quo;
                }
                InvoiceComponent = InvoiceComponent.TrimStart(',');
                string[] eachInvoice = InvoiceComponent.Split(',');
                if (eachInvoice.Length == 1)
                {
                    InvoiceComponentDate = Convert.ToString(txt_InvoiceDate.Text);
                }
                else
                {
                    InvoiceComponentDate = "";
                }

                string strCashBank = Convert.ToString(ddlCashBank.Value);
                string strDueDate = Convert.ToString(dt_SaleInvoiceDue.Date);

                string strCurrency = Convert.ToString(ddl_Currency.SelectedValue);
                string PosForGst = Convert.ToString(ddl_PosGst.Value);
                string strRate = Convert.ToString(txt_Rate.Value);
                string strTaxType = Convert.ToString(ddl_AmountAre.Value);
                string strTaxCode = Convert.ToString(ddl_VatGstCst.Value).Split('~')[0];

                string CompID = Convert.ToString(HttpContext.Current.Session["LastCompany"]);
                string currency = Convert.ToString(HttpContext.Current.Session["LocalCurrency"]);
                string[] ActCurrency = currency.Split('~');
                int BaseCurrencyId = Convert.ToInt32(ActCurrency[0]);
                int ConvertedCurrencyId = Convert.ToInt32(strCurrency);

                DataTable tempQuotation = Quotationdt.Copy();
                foreach (DataRow dr in tempQuotation.Rows)
                {
                    string Status = Convert.ToString(dr["Status"]);

                    if (Status == "I")
                    {
                        dr["QuotationID"] = "0";

                        string[] list = Convert.ToString(dr["ProductID"]).Split(new string[] { "||@||" }, StringSplitOptions.None);
                        dr["ProductID"] = Convert.ToString(list[0]);
                        dr["UOM"] = Convert.ToString(list[3]);
                        dr["StockUOM"] = Convert.ToString(list[5]);
                    }
                    else if (Status == "U" || Status == "")
                    {
                        if (Convert.ToString(dr["QuotationID"]).Contains("~") == true)
                        {
                            dr["QuotationID"] = 0;
                        }

                        string[] list = Convert.ToString(dr["ProductID"]).Split(new string[] { "||@||" }, StringSplitOptions.None);
                        dr["ProductID"] = Convert.ToString(list[0]);
                        dr["UOM"] = Convert.ToString(list[3]);
                        dr["StockUOM"] = Convert.ToString(list[5]);
                    }
                }
                tempQuotation.AcceptChanges();

                //DataTable TaxDetailTable = getAllTaxDetails(e);

                DataTable TaxDetailTable = new DataTable();
                if (Session["SI_FinalTaxRecord"] != null)
                {
                    TaxDetailTable = (DataTable)Session["SI_FinalTaxRecord"];
                }

                // DataTable of Warehouse

                DataTable tempWarehousedt = new DataTable();
                if (Session["SI_WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["SI_WarehouseData"];
                    tempWarehousedt = Warehousedt.DefaultView.ToTable(false, "Product_SrlNo", "LoopID", "WarehouseID", "TotalQuantity", "BatchID", "SerialID");
                }
                else
                {
                    tempWarehousedt.Columns.Add("Product_SrlNo", typeof(string));
                    tempWarehousedt.Columns.Add("SrlNo", typeof(string));
                    tempWarehousedt.Columns.Add("WarehouseID", typeof(string));
                    tempWarehousedt.Columns.Add("TotalQuantity", typeof(string));
                    tempWarehousedt.Columns.Add("BatchID", typeof(string));
                    tempWarehousedt.Columns.Add("SerialID", typeof(string));
                }

                // End

                // DataTable Of Quotation Tax Details 

                DataTable TaxDetailsdt = new DataTable();
                if (Session["SI_TaxDetails"] != null)
                {
                    TaxDetailsdt = (DataTable)Session["SI_TaxDetails"];
                }
                else
                {
                    TaxDetailsdt.Columns.Add("Taxes_ID", typeof(string));
                    TaxDetailsdt.Columns.Add("Taxes_Name", typeof(string));
                    TaxDetailsdt.Columns.Add("Percentage", typeof(string));
                    TaxDetailsdt.Columns.Add("Amount", typeof(string));
                    TaxDetailsdt.Columns.Add("AltTax_Code", typeof(string));
                }

                DataTable tempTaxDetailsdt = new DataTable();
                tempTaxDetailsdt = TaxDetailsdt.DefaultView.ToTable(false, "Taxes_ID", "Percentage", "Amount", "AltTax_Code");

                tempTaxDetailsdt.Columns.Add("SlNo", typeof(string));
                //    tempTaxDetailsdt.Columns.Add("AltTaxCode", typeof(string));

                tempTaxDetailsdt.Columns["SlNo"].SetOrdinal(0);
                tempTaxDetailsdt.Columns["Taxes_ID"].SetOrdinal(1);
                tempTaxDetailsdt.Columns["AltTax_Code"].SetOrdinal(2);
                tempTaxDetailsdt.Columns["Percentage"].SetOrdinal(3);
                tempTaxDetailsdt.Columns["Amount"].SetOrdinal(4);

                foreach (DataRow d in tempTaxDetailsdt.Rows)
                {
                    d["SlNo"] = "0";
                    //d["AltTaxCode"] = "0";
                }

                // End

                // DataTable Of Billing Address

                #region ##### Added By : Samrat Roy -- to get BillingShipping user control data
                DataTable tempBillAddress = new DataTable();
                tempBillAddress = Sales_BillingShipping.GetBillingShippingTable();



                #region #### Old_Process ####
                ////// DataTable Of Billing Address

                //DataTable tempBillAddress = new DataTable();
                //if (Session["SI_QuotationAddressDtl"] != null)
                //{
                //    tempBillAddress = (DataTable)Session["SI_QuotationAddressDtl"];
                //}
                //else
                //{
                //    tempBillAddress = StoreQuotationAddressDetail();
                //}

                #endregion

                #endregion

                // End

                string approveStatus = "";
                if (Request.QueryString["status"] != null)
                {
                    approveStatus = Convert.ToString(Request.QueryString["status"]);
                }

                if (ActionType == "Add")
                {
                    string[] SchemeList = strSchemeType.Split(new string[] { "~" }, StringSplitOptions.None);
                    //Chinmoy Added Below Line
                    SchemeID = Convert.ToString(SchemeList[0]);


                    //validate = checkNMakeJVCode(strInvoiceNo, Convert.ToInt32(SchemeList[0]));
                }

                foreach (DataRow dr in tempQuotation.Rows)
                {
                    string strSrlNo = Convert.ToString(dr["SrlNo"]);
                    decimal strProductQuantity = Convert.ToDecimal(dr["Quantity"]);

                    decimal strWarehouseQuantity = 0;
                    GetQuantityBaseOnProduct(strSrlNo, ref strWarehouseQuantity);

                    if (strWarehouseQuantity != 0)
                    {
                        if (strProductQuantity != strWarehouseQuantity)
                        {
                            validate = "checkWarehouse";
                            grid.JSProperties["cpProductSrlIDCheck"] = strSrlNo;
                            break;
                        }
                    }
                }

                DataView dvData = new DataView(tempQuotation);
                dvData.RowFilter = "Status<>'D'";
                DataTable _tempQuotation = dvData.ToTable();

                var duplicateRecords = _tempQuotation.AsEnumerable()
               .GroupBy(r => r["ProductID"]) //coloumn name which has the duplicate values
               .Where(gr => gr.Count() > 1)
               .Select(g => g.Key);

                foreach (var d in duplicateRecords)
                {
                    validate = "duplicateProduct";
                }

                //if (ddlInventory.SelectedValue != "N" && ddlInventory.SelectedValue != "S")
                //{
                //    foreach (DataRow dr in tempQuotation.Rows)
                //    {
                //        decimal ProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                //        string Status = Convert.ToString(dr["Status"]);

                //        if (Status != "D")
                //        {
                //            if (ProductQuantity == 0)
                //            {
                //                validate = "nullQuantity";
                //                break;
                //            }
                //        }
                //    }
                //}

                if (hdnSalesOrderItemNegative.Value == "1")
                {
                    if (InvoiceComponent != "" && strComponenyType == "SO")
                    {
                        foreach (DataRow dr in tempQuotation.Rows)
                        {
                            decimal SONetAmount = Convert.ToDecimal(dr["SONetAmount"]);
                            decimal TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                            string Status = Convert.ToString(dr["Status"]);

                            if (Status != "D")
                            {
                                if (TotalAmount > SONetAmount)
                                {
                                    validate = "NetAmountExceed";
                                    break;
                                }
                            }
                        }
                    }

                }

                if (Convert.ToInt32(txtCreditDays.Text) == 0)
                {
                    validate = "nullCredit";
                }

                //decimal ProductAmount = 0;
                //foreach (DataRow dr in tempQuotation.Rows)
                //{
                //    ProductAmount = ProductAmount + Convert.ToDecimal(dr["Amount"]);
                //}

                //if (ProductAmount == 0)
                //{
                //    validate = "nullAmount";
                //}

                //// ############# Added By : Samrat Roy -- 02/05/2017 -- To check Transporter Mandatory Control 
                //    BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);




                if (hdnProjPageStat.Value == "Add")
                {

                    if (lookup_quotation.GridView.GetSelectedFieldValues("ComponentID").Count > 0)
                    {
                        if (tempQuotation.Rows.Count > 0)
                        {
                            foreach (DataRow dr in tempQuotation.Rows)
                            {
                                Int64 DetailsId = 0;
                                string ProductID = Convert.ToString(dr["ProductID"]);
                                decimal ProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                                string Status = Convert.ToString(dr["Status"]);
                                //QuoteOrderDetails_id = Convert.ToString(Session["QuoteOrderDetails_id"]);
                                //Int64 i = 0;
                                //for ( i =(Convert.ToInt64(dr["SrlNo"])-1); i <Convert.ToInt64(dr["SrlNo"]); i++)
                                //{
                                //     DetailsId = Convert.ToInt64(QuoteOrderDetails_id.Split(',')[i]);
                                //}

                                DataTable dtq = oDBEngine.GetDataTable("select isnull(BalanceQty,0) TotQty from tbl_trans_SalesOrderBalanceMapForChallan where  SalesOrderId='" + Convert.ToInt32(dr["ComponentID"]) + "' and  ProductId='" + ProductID + "'");

                                if (ProductID != "")
                                {
                                    if (ProductQuantity > Convert.ToDecimal(dtq.Rows[0]["TotQty"]))
                                    {
                                        validate = "ExceedQuantity";
                                        break;
                                    }
                                }


                            }

                        }
                    }
                }

                if (hdnProjPageStat.Value == "update")
                {

                    if (lookup_quotation.GridView.GetSelectedFieldValues("ComponentID").Count > 0)
                    {
                        if (tempQuotation.Rows.Count > 0)
                        {
                            foreach (DataRow dr in tempQuotation.Rows)
                            {
                                Int64 DetailsId = 0;
                                string ProductID = Convert.ToString(dr["ProductID"]);
                                decimal ProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                                string Status = Convert.ToString(dr["Status"]);
                                //QuoteOrderDetails_id = Convert.ToString(Session["QuoteOrderDetails_id"]);
                                //Int64 i = 0;
                                //for ( i =(Convert.ToInt64(dr["SrlNo"])-1); i <Convert.ToInt64(dr["SrlNo"]); i++)
                                //{
                                //     DetailsId = Convert.ToInt64(QuoteOrderDetails_id.Split(',')[i]);
                                //}

                                DataTable dtq = oDBEngine.GetDataTable("select (ISNULL(TotalQty,0)+isnull(BalanceQty,0)) TotQty from tbl_trans_SalesOrderBalanceMapForChallan where  SalesOrderId='" + Convert.ToInt32(dr["ComponentID"]) + "' and  ProductId='" + ProductID + "'");

                                if (ProductID != "")
                                {
                                    if (ProductQuantity > Convert.ToDecimal(dtq.Rows[0]["TotQty"]))
                                    {
                                        validate = "ExceedQuantity";
                                        break;
                                    }
                                }


                            }

                        }
                    }
                }







                BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine();


                DataTable DT = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='Transporter_SIMandatory' AND IsActive=1");
                if (DT != null && DT.Rows.Count > 0)
                {
                    string IsMandatory = Convert.ToString(DT.Rows[0]["Variable_Value"]).Trim();

                    if (Convert.ToString(Session["TransporterVisibilty"]).Trim() == "Yes")
                    {
                        if (IsMandatory == "Yes")
                        {
                            //if (VehicleDetailsControl.GetControlValue("cmbTransporter") == "")
                            if (hfControlData.Value.Trim() == "")
                            {
                                validate = "transporteMandatory";
                            }
                        }
                    }
                }

                //// ############# Added By : Samrat Roy -- 02/05/2017 -- To check Transporter Mandatory Control 

                //----------Start-------------------------
                //Data: 31-05-2017 Author: Sayan Dutta
                //Details:To check T&C Mandatory Control
                #region TC

                if (ddlInventory.SelectedValue != "N" && ddlInventory.SelectedValue != "S")
                {
                    DataTable DT_TC = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='TC_SIMandatory' AND IsActive=1");
                    if (DT_TC != null && DT_TC.Rows.Count > 0)
                    {
                        string IsMandatory = Convert.ToString(DT_TC.Rows[0]["Variable_Value"]).Trim();

                        // objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);

                        objEngine = new BusinessLogicLayer.DBEngine();

                        DataTable DTVisible = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='Show_TC_SI' AND IsActive=1");
                        //chinmoy comment for terms condition check start
                        if (Convert.ToString(DTVisible.Rows[0]["Variable_Value"]).Trim() == "Yes")
                        {
                            if (IsMandatory == "Yes")
                            {
                                if (TermsConditionsControl.GetControlValue("dtDeliveryDate") == "" || TermsConditionsControl.GetControlValue("dtDeliveryDate") == "@")
                                {
                                    validate = "TCMandatory";
                                }
                            }
                        }
                        //chinmoy comment for terms condition check End
                    }
                }

                #endregion
                //----------End-------------------------

                #region Salesman Mandatory

                DataTable Salesman_DT = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='SalesManAgentsMandatory' AND IsActive=1");
                if (Salesman_DT != null && Salesman_DT.Rows.Count > 0)
                {
                    string IsMandatory = Convert.ToString(Salesman_DT.Rows[0]["Variable_Value"]).Trim();
                    if (IsMandatory == "Yes")
                    {
                        if (hdnSalesManAgentId.Value.Trim() == "")
                        {
                            validate = "SalesmanMandatory";
                        }
                    }
                }

                #endregion

                //Checking for minimum Sale Price

                //----------Invoice with Order tagging is mandatory----------

                if (strIsInventory == "Y")
                {
                    DataTable dtmandatory = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='Invoice_With_Order_Tagging' AND IsActive=1");
                    if (dtmandatory != null && dtmandatory.Rows.Count > 0)
                    {
                        string IsMandatory = Convert.ToString(dtmandatory.Rows[0]["Variable_Value"]).Trim();

                        if (IsMandatory == "Yes")
                        {
                            if (Convert.ToString(rdl_SaleInvoice.SelectedValue) != "SO")
                            {
                                validate = "OrderTaggingMandatory";
                            }
                            else if (Convert.ToString(rdl_SaleInvoice.SelectedValue) == "SO" && InvoiceComponent == "")
                            {
                                validate = "OrderTaggingMandatory";
                            }
                        }
                    }
                }
                //----------Invoice with Order tagging is mandatory----------

                string ProductMinSalePriceList = "";
                foreach (DataRow dr in tempQuotation.Rows)
                {
                    ProductMinSalePriceList = ProductMinSalePriceList + Convert.ToString(dr["ProductID"]) + ",";
                }
                ProductMinSalePriceList = ProductMinSalePriceList.TrimEnd(',');
                string validateminSalePrice = IsMinSalePriceOk(ProductMinSalePriceList, tempQuotation);
                if (validateminSalePrice == "MinSalePriceGreater")
                {
                    validate = "minSalePriceMust";
                }
                if (validateminSalePrice == "MRPLess")
                {
                    validate = "MRPLess";
                }

                if (dt_PLQuote.Value != null && dt_SaleInvoiceDue.Value != null)
                {
                    if (Convert.ToDateTime(dt_SaleInvoiceDue.Value) < Convert.ToDateTime(dt_PLQuote.Value))
                    {
                        validate = "DueDateLess";
                    }
                }
                string TaxType = "";
                string ShippingState = "0";

                if (ddl_PosGst.Value.ToString() == "S")
                {
                    ShippingState = Convert.ToString(Sales_BillingShipping.GetShippingStateId());
                }
                else
                {
                    ShippingState = Convert.ToString(Sales_BillingShipping.GetBillingStateId());
                }

                if (ShippingState == "0")
                {
                    validate = "BillingShippingNotLoaded";
                }


                if (Convert.ToString(Sales_BillingShipping.GeteShippingStateCode()) == "0")
                {
                    validate = "BillingShippingNull";
                }
                if (Convert.ToString(Sales_BillingShipping.GetBillingStateCode()) == "0")
                {
                    validate = "BillingShippingNull";
                }



                object sumObject;
                //sumObject = _tempQuotation.Compute("Sum(Amount)", string.Empty);
                sumObject = _tempQuotation.AsEnumerable()
                    .Sum(x => Convert.ToDecimal(x["Amount"]));
                object TotalsumObject;
                //TotalsumObject = _tempQuotation.Compute("Sum(TotalAmount)", string.Empty);
                TotalsumObject = _tempQuotation.AsEnumerable()
    .Sum(x => Convert.ToDecimal(x["TotalAmount"]));

                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("prc_TCSDetails");
                proc.AddVarcharPara("@CustomerID", 500, hdnCustomerId.Value);
                proc.AddVarcharPara("@invoice_id", 500, MainInvoiceID);
                proc.AddVarcharPara("@Action", 500, "ShowTDSDetails");
                proc.AddVarcharPara("@date", 500, dt_PLQuote.Text);
                proc.AddVarcharPara("@totalAmount", 500, Convert.ToString(TotalsumObject));
                proc.AddVarcharPara("@taxableAmount", 500, Convert.ToString(sumObject));
                proc.AddVarcharPara("@branch_id", 500, strBranch);
                dt = proc.GetTable();

                if (dt != null && dt.Rows.Count > 0 && ddlInventory.SelectedValue == "I")
                {
                    if (MainInvoiceID == "" && Convert.ToDecimal(dt.Rows[0]["Amount"]) > 0 && Convert.ToDecimal(txtTCSAmount.Text) == 0)
                    {
                        validate = "TCSMandatory";
                    }
                }



                if (validate == "outrange" || validate == "duplicate" || validate == "checkWarehouse" || validate == "duplicateProduct" || validate == "nullAmount" || validate == "nullQuantity" || validate == "transporteMandatory" || validate == "TCMandatory" || validate == "minSalePriceMust" || validate == "MRPLess"
                    || validate == "DueDateLess" || validate == "BillingShippingNotLoaded" || validate == "SalesmanMandatory" || validate == "OrderTaggingMandatory"
                    || validate == "BillingShippingNull" || validate == "ExceedQuantity" || validate == "NetAmountExceed")
                {
                    grid.JSProperties["cpSaveSuccessOrFail"] = validate;
                }
                else
                {
                    grid.JSProperties["cpQuotationNo"] = UniqueQuotation;

                    #region To Show By Default Cursor after SAVE AND NEW
                    if (Convert.ToString(Session["SI_ActionType"]) == "Add") // session has been removed from quotation list page working good
                    {
                        string[] schemaid = new string[] { };
                        string schemavalue = ddl_numberingScheme.SelectedValue;
                        Session["SI_schemavalue"] = schemavalue;        // session has been removed from quotation list page working good
                        schemaid = ddl_numberingScheme.SelectedValue.Split('~');

                        string schematype = schemaid[1];
                        if (hdnRefreshType.Value == "N")
                        {
                            if (schematype == "1")
                            {
                                Session["SI_SaveMode"] = "A";
                            }
                            else
                            {
                                Session["SI_SaveMode"] = "M";
                            }
                        }
                        else
                        {
                            Session["SI_SaveMode"] = null;
                        }
                    }

                    #endregion

                    #region Subhabrata Section Start

                    int strIsComplete = 0, strQuoteID = 0;
                    //Chinmoy Added Below Line
                    string strInvoiceNumber = "";
                    UniqueQuotation = strInvoiceNo;
                    if (Convert.ToString(Request.QueryString["key"]) == "ADD")
                    {
                        if (!reCat.isAllMandetoryDone((DataTable)Session["UdfDataOnAdd"], "SI"))
                        {
                            grid.JSProperties["cpinsert"] = "UDFMandatory";

                            return;

                        }
                    }



                    if (Convert.ToString(ddl_AmountAre.Value).Trim() == "1" || Convert.ToString(ddl_AmountAre.Value).Trim() == "01")
                    {
                        TaxType = "E";
                    }
                    else if (Convert.ToString(ddl_AmountAre.Value).Trim() == "2" || Convert.ToString(ddl_AmountAre.Value).Trim() == "02")
                    {
                        TaxType = "I";
                    }

                    // TaxDetailTable = gstTaxDetails.SetTaxTableDataWithProductSerialRoundOff(ref tempQuotation, "SrlNo", "ProductID", "Amount", "TaxAmount", TaxDetailTable, "S", dt_PLQuote.Date.ToString("yyyy-MM-dd"), strBranch, ShippingState, TaxType);
                    TaxDetailTable = gstTaxDetails.SetTaxTableDataWithProductSerialWithException(tempQuotation, "SrlNo", "ProductID", "Amount", "TaxAmount", TaxDetailTable, "S", dt_PLQuote.Date.ToString("yyyy-MM-dd"), strBranch, ShippingState, TaxType, hdnCustomerId.Value, "Quantity", "SQ");


                    #region Add New Filed To Check from Table

                    DataTable duplicatedt2 = new DataTable();
                    duplicatedt2.Columns.Add("productid", typeof(Int64));
                    duplicatedt2.Columns.Add("slno", typeof(Int32));
                    duplicatedt2.Columns.Add("Quantity", typeof(Decimal));
                    duplicatedt2.Columns.Add("packing", typeof(Decimal));
                    duplicatedt2.Columns.Add("PackingUom", typeof(Int32));
                    duplicatedt2.Columns.Add("PackingSelectUom", typeof(Int32));

                    if (HttpContext.Current.Session["SessionPackingDetails"] != null)
                    {
                        List<ProductQuantity> obj = new List<ProductQuantity>();
                        obj = (List<ProductQuantity>)HttpContext.Current.Session["SessionPackingDetails"];
                        foreach (var item in obj)
                        {
                            duplicatedt2.Rows.Add(item.productid, item.slno, item.Quantity, item.packing, item.PackingUom, item.PackingSelectUom);
                        }
                    }
                    HttpContext.Current.Session["SessionPackingDetails"] = null;
                    #endregion

                    CommonBL cbl = new CommonBL();
                    string ProjectSelectcashbankModule = cbl.GetSystemSettingsResult("ProjectSelectInEntryModule");
                    Int64 ProjId = 0;
                    if (lookup_Project.Text != "")
                    {
                        string projectCode = lookup_Project.Text;
                        DataTable dtSlOrd = objProjectInvoiceBL.GetProjectCode(projectCode);
                        //oDbEngine.GetDataTable("select Proj_Id from Master_ProjectManagement where Proj_Code='" + projectCode + "'");
                        ProjId = Convert.ToInt64(dtSlOrd.Rows[0]["Proj_Id"]);
                    }
                    else if (lookup_Project.Text == "")
                    {
                        ProjId = 0;
                    }

                    else
                    {
                        ProjId = 0;
                    }

                    DataTable dtAddlDesc = (DataTable)Session["InlineRemarks"];


                    string RetPercentage = Convert.ToString(crtxtRetPercentage.Text);
                    string RetAmount = Convert.ToString(crtxtRetAmount.Text);
                    string GL = Convert.ToString(hdnROMainAc.Value);
                    string Remarks = Convert.ToString(crtxtRemarks.Text);
                    DateTime? dtDuedt = null;

                    if (dtDueDate.Text != null && dtDueDate.Text != "")
                    {
                        dtDuedt = dtDueDate.Date;
                    }

                    //////////////////  TCS section  /////////////////////////
                    string strTCScode = Convert.ToString(txtTCSSection.Text);
                    string strTCSappl = Convert.ToString(txtTCSapplAmount.Text);
                    string strTCSpercentage = Convert.ToString(txtTCSpercentage.Text);
                    string strTCSamout = Convert.ToString(txtTCSAmount.Text);
                    //////////////////////////////////////////////////////////

                    DataColumnCollection dtDetailscheck = tempQuotation.Columns;

                    if (dtDetailscheck.Contains("SONetAmount"))
                    {
                        dtDetailscheck.Remove("SONetAmount");
                    }


                    ModifyQuatation(strIsInventory, MainInvoiceID, strSchemeType, UniqueQuotation, strInvoiceDate, strCustomer, strContactName, ProjId,
                                    Reference, strBranch, strAgents, strCurrency, strRate, strTaxType, strTaxCode,
                                    strComponenyType, InvoiceComponent, InvoiceComponentDate, strCashBank, strDueDate,
                                    tempQuotation, TaxDetailTable, tempWarehousedt, tempTaxDetailsdt, tempBillAddress, SchemeID,
                                    approveStatus, ActionType, PosForGst, ref strIsComplete, ref strQuoteID, ref strInvoiceNumber, duplicatedt2, dtAddlDesc, RetPercentage, RetAmount, GL, Remarks, dtDuedt, strTCScode, strTCSappl
                                    , strTCSpercentage, strTCSamout,drdTransCategory.SelectedValue);

                    //Chinmoy added Below Line
                    grid.JSProperties["cpQuotationNo"] = strInvoiceNumber;
                    //Udf Add mode
                    DataTable udfTable = (DataTable)Session["UdfDataOnAdd"];
                    if (udfTable != null)
                        Session["UdfDataOnAdd"] = reCat.insertRemarksCategoryAddMode("SI", "SaleInvoice" + Convert.ToString(strQuoteID), udfTable, Convert.ToString(Session["userid"]));

                    if (strIsComplete == 1)
                    {
                        if (ActionType == "Add")
                        {
                            grid.JSProperties["cpQuotationID"] = Convert.ToString(strQuoteID);
                        }
                        else
                        {
                            grid.JSProperties["cpQuotationID"] = "";
                            grid.JSProperties["cpQuotationNo"] = "";
                        }

                        if (approveStatus != "")
                        {
                            if (approveStatus == "2")
                            {
                                grid.JSProperties["cpApproverStatus"] = "approve";
                            }
                            else
                            {
                                grid.JSProperties["cpApproverStatus"] = "rejected";
                            }
                        }

                        #region Approval related Mail

                        //Employee_BL objemployeebal = new Employee_BL();
                        //int MailStatus = 0;
                        //string AssignedTo_Email = string.Empty;
                        //string ReceiverEmail = string.Empty;
                        //decimal Level1_User = Convert.ToDecimal(Session["userid"]);
                        //decimal Level2User = Convert.ToDecimal(Session["userid"]);
                        //decimal Level3User = Convert.ToDecimal(Session["userid"]);
                        //bool L1 = false;
                        //bool L2 = false;
                        //bool L3 = false;
                        //string ActivityName = string.Empty;

                        //DataTable dtbl_AssignedTo = new DataTable();
                        //DataTable dtbl_AssignedBy = new DataTable();
                        //DataTable dtEmail_To = new DataTable();
                        //DataTable dt_EmailConfig = new DataTable();
                        //DataTable dt_ActivityName = new DataTable();
                        //DataTable Dt_LevelUser = new DataTable();
                        //DataTable dtbl_InvoiceTo = new DataTable();
                        //dt_EmailConfig = objemployeebal.GetEmailAccountConfigDetails("", 1); // Checked fetch datatatable of email setup with action 1 param
                        ////  Dt_LevelUser = objemployeebal.GetAllLevelUsers("1", 12);

                        //ActivityName = UniqueQuotation;
                        //dtEmail_To = objemployeebal.GetEmailInvoice(strCustomer, 15);
                        //dtbl_InvoiceTo = objemployeebal.GetEmailAccountConfigDetails(Convert.ToString(strQuoteID), 14);

                        ////if (Dt_LevelUser != null && string.IsNullOrEmpty(approveStatus))
                        ////{
                        ////    L1 = true;
                        ////   // dtEmail_To = objemployeebal.GetEmailLevelUsersWise(1, 11, 1);
                        ////    dtEmail_To = objemployeebal.GetEmailLevelUsersWise(4, 11, 1);
                        ////}
                        ////else if (Dt_LevelUser != null && Dt_LevelUser.Rows[0].Field<decimal>("Level1_user_id") == Level1_User && approveStatus == "2")
                        ////{
                        ////    L2 = true;
                        ////   // dtEmail_To = objemployeebal.GetEmailLevelUsersWise(1, 11, 2);
                        ////    dtEmail_To = objemployeebal.GetEmailLevelUsersWise(4, 11, 2);
                        ////}
                        ////else if (Dt_LevelUser != null && Dt_LevelUser.Rows[0].Field<decimal>("Level2_user_id") == Level2User && approveStatus == "2")
                        ////{
                        ////    L3 = true;
                        ////   // dtEmail_To = objemployeebal.GetEmailLevelUsersWise(1, 11, 3);
                        ////    dtEmail_To = objemployeebal.GetEmailLevelUsersWise(4, 11, 3);
                        ////}

                        //if (dtEmail_To != null && dtEmail_To.Rows.Count > 0)
                        //{
                        //    if (!string.IsNullOrEmpty(dtEmail_To.Rows[0].Field<string>("Email_Id")))
                        //    {
                        //        ReceiverEmail = Convert.ToString(dtEmail_To.Rows[0].Field<string>("Email_Id"));
                        //    }
                        //    else
                        //    {
                        //        ReceiverEmail = "";
                        //    }
                        //}

                        //ListDictionary replacements = new ListDictionary();
                        //if (dtbl_InvoiceTo.Rows.Count > 0)
                        //{
                        //    replacements.Add("@InvoiceNumber@", Convert.ToString(dtbl_InvoiceTo.Rows[0].Field<string>("Invoice_Number")));
                        //}
                        //else
                        //{
                        //    replacements.Add("@InvoiceNumber@", "");
                        //}

                        //if (dtbl_InvoiceTo.Rows.Count > 0)
                        //{
                        //    replacements.Add("@InvoiceDate@", Convert.ToString(dtbl_InvoiceTo.Rows[0].Field<string>("Invoice_Date")));
                        //}
                        //else
                        //{
                        //    replacements.Add("@InvoiceDate@", "");
                        //}

                        ////ExceptionLogging.SendExceptionMail(ex, Convert.ToInt32(lineNumber));

                        //if (!string.IsNullOrEmpty(ReceiverEmail))
                        //{
                        //    //ExceptionLogging.SendEmailToAssigneeByUser(ReceiverEmail, "", replacements, "~/OMS/EmailTemplates/EmailSendToAssigneeByUserID.html", dt_EmailConfig, ActivityName,4);
                        //    MailStatus = ExceptionLogging.SendEmailToAssigneeByUser(ReceiverEmail, "", replacements, dt_EmailConfig, ActivityName, 17);
                        //}

                        #endregion
                    }
                    else if (strIsComplete == -50)
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "duplicate";
                    }
                    else if (strIsComplete == -60)
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "MultiTax";
                    }
                    else if (strIsComplete == 2)
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "quantityTagged";
                    }

                    else if (strIsComplete == -12)
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "EmptyProject";
                    }
                    else if (strIsComplete == -80)
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "QuantityorAmountnegative";
                    }
                    //else if (strIsComplete == -3)
                    //{
                    //    grid.JSProperties["cpSaveSuccessOrFail"] = "-3";
                    //}
                    else
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "errorInsert";
                    }
                    
                    #endregion Subhabrata Section End
                }
            }
            else
            {
                DataView dvData = new DataView(Quotationdt);
                dvData.RowFilter = "Status <> 'D'";

                grid.DataSource = GetQuotation(dvData.ToTable());
                grid.DataBind();
            }
        }
        protected void grid_DataBinding(object sender, EventArgs e)
        {
            //string IsDeleteFrom = Convert.ToString(hdfIsDelete.Value);
            //if (IsDeleteFrom == "D")
            //{
            //    DataTable Quotationdt = (DataTable)Session["SI_QuotationDetails"];
            //    grid.DataSource = GetQuotation(Quotationdt);
            //}
            //else
            //{
            //    grid.DataSource = GetQuotation();
            //}

            if (Session["SI_QuotationDetails"] != null)
            {
                DataTable Quotationdt = (DataTable)Session["SI_QuotationDetails"];
                DataView dvData = new DataView(Quotationdt);
                dvData.RowFilter = "Status <> 'D'";
                grid.DataSource = GetQuotation(dvData.ToTable());
            }
        }

        #region grid_CustomCallback
        protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            grid.JSProperties["cpSaveSuccessOrFail"] = "";
            string strSplitCommand = e.Parameters.Split('~')[0];

            if (strSplitCommand == "Display")
            {
                string IsDeleteFrom = Convert.ToString(hdfIsDelete.Value);
                if (IsDeleteFrom == "D" || IsDeleteFrom == "C")
                {
                    DataTable Quotationdt = (DataTable)Session["SI_QuotationDetails"];
                    grid.DataSource = GetQuotation(Quotationdt);
                    grid.DataBind();
                }

                if (e.Parameters.Split('~').Length > 1)
                {
                    if (e.Parameters.Split('~')[1] == "fromComponent")
                    {
                        grid.JSProperties["cpComponent"] = "true";
                    }
                }
            }
            else if (strSplitCommand == "GridBlank")
            {
                Session["SI_QuotationDetails"] = null;
                grid.DataSource = null;
                grid.DataBind();
            }
            else if (strSplitCommand == "CurrencyChangeDisplay")
            {
                if (Session["SI_QuotationDetails"] != null)
                {
                    DataTable Quotationdt = (DataTable)Session["SI_QuotationDetails"];

                    string strCurrRate = Convert.ToString(txt_Rate.Text);
                    decimal strRate = 1;

                    if (strCurrRate != "")
                    {
                        strRate = Convert.ToDecimal(strCurrRate);
                        if (strRate == 0) strRate = 1;
                    }

                    foreach (DataRow dr in Quotationdt.Rows)
                    {
                        string Status = Convert.ToString(dr["Status"]);
                        if (Status != "D")
                        {
                            string ProductDetails = Convert.ToString(dr["ProductID"]);
                            string QuantityValue = Convert.ToString(dr["Quantity"]);
                            string Discount = Convert.ToString(dr["Discount"]);
                            string[] ProductDetailsList = ProductDetails.Split(new string[] { "||@||" }, StringSplitOptions.None);

                            string strMultiplier = ProductDetailsList[7];
                            string strFactor = ProductDetailsList[8];
                            string strSalePrice = ProductDetailsList[6];

                            decimal SalePrice = Convert.ToDecimal(strSalePrice) / strRate;
                            string Amount = Convert.ToString(Math.Round((Convert.ToDecimal(QuantityValue) * Convert.ToDecimal(strFactor) * SalePrice), 2));
                            string amountAfterDiscount = Convert.ToString(Math.Round((Convert.ToDecimal(Amount) - ((Convert.ToDecimal(Discount) * Convert.ToDecimal(Amount)) / 100)), 2));


                            dr["SalePrice"] = Convert.ToString(Math.Round(SalePrice, 2));
                            dr["Amount"] = amountAfterDiscount;
                            dr["TaxAmount"] = ReCalculateTaxAmount(Convert.ToString(dr["SrlNo"]), Convert.ToDouble(amountAfterDiscount));
                            dr["TotalAmount"] = Convert.ToDecimal(dr["Amount"]) + Convert.ToDecimal(dr["TaxAmount"]);
                        }
                    }

                    Session["SI_QuotationDetails"] = Quotationdt;

                    DataView dvData = new DataView(Quotationdt);
                    dvData.RowFilter = "Status <> 'D'";

                    grid.DataSource = GetQuotation(dvData.ToTable());
                    grid.DataBind();
                }
            }
            else if (strSplitCommand == "DateChangeDisplay")
            {
                if (Session["SI_QuotationDetails"] != null)
                {
                    DataTable Quotationdt = (DataTable)Session["SI_QuotationDetails"];

                    string strCurrRate = Convert.ToString(txt_Rate.Text);
                    decimal strRate = 1;

                    if (strCurrRate != "")
                    {
                        strRate = Convert.ToDecimal(strCurrRate);
                        if (strRate == 0) strRate = 1;
                    }

                    foreach (DataRow dr in Quotationdt.Rows)
                    {
                        string Status = Convert.ToString(dr["Status"]);
                        if (Status != "D")
                        {
                            dr["TaxAmount"] = 0;
                            dr["TotalAmount"] = dr["Amount"];
                        }
                    }

                    Session["SI_QuotationDetails"] = Quotationdt;

                    DataView dvData = new DataView(Quotationdt);
                    dvData.RowFilter = "Status <> 'D'";

                    grid.DataSource = GetQuotation(dvData.ToTable());
                    grid.DataBind();
                }
            }



            else if (strSplitCommand == "BindGridOnQuotation")
            {
                string command = e.Parameters.ToString();
                string State = Convert.ToString(e.Parameters.Split('~')[1]);
                string strType = Convert.ToString(rdl_SaleInvoice.SelectedValue);
                string ComponentDetailsIDs = string.Empty;
                string strAction = "";
                string strTaxCountAction = "";

                for (int i = 0; i < grid_Products.GetSelectedFieldValues("ComponentDetailsID").Count; i++)
                {
                    ComponentDetailsIDs += "," + Convert.ToString(grid_Products.GetSelectedFieldValues("ComponentDetailsID")[i]);
                }
                ComponentDetailsIDs = ComponentDetailsIDs.TrimStart(',');


                if (strType == "QO")
                {
                    strAction = "GetSeletedQuotationProducts";
                    strTaxCountAction = "GetSeletedQuotationTaxCount";
                }
                else if (strType == "SO")
                {
                    strAction = "GetSeletedOrderProducts";
                    strTaxCountAction = "GetSeletedOrderTaxCount";
                }
                else if (strType == "SC")
                {
                    strAction = "GetSeletedChallanProducts";
                    strTaxCountAction = "GetSeletedChallanTaxCount";
                }

                string strInvoiceID = Convert.ToString(Session["SI_InvoiceID"]);
                lookup_Project.ClearButton.Visibility = AutoBoolean.False;
                lookup_Project.ClientEnabled = false;
                if (objProjectInvoiceBL.GetSelectedComponentProductList(strTaxCountAction, ComponentDetailsIDs, strInvoiceID).Tables[0].Rows.Count <= 2)
                {
                    DataSet dt_QuotationDetails = objProjectInvoiceBL.GetSelectedComponentProductList(strAction, ComponentDetailsIDs, strInvoiceID);
                    Session["SI_QuotationDetails"] = dt_QuotationDetails.Tables[0];

                    Session["InlineRemarks"] = dt_QuotationDetails.Tables[1];

                    grid.DataSource = GetQuotation(dt_QuotationDetails.Tables[0]);
                    grid.DataBind();

                    Session["SI_WarehouseData"] = GetTaggingWarehouseData(ComponentDetailsIDs, strType);
                    Session["SI_FinalTaxRecord"] = GetComponentEditedTaxData(ComponentDetailsIDs, strType);


                    #region ##### Existing BillingShipping Code : Samrat Roy ############
                    //Session["SI_QuotationAddressDtl"] = GetComponentEditedAddressData(ComponentDetailsIDs, strType);
                    //if (Session["SI_QuotationAddressDtl"] != null)
                    //{
                    //    DataTable dt = (DataTable)Session["SI_QuotationAddressDtl"];
                    //    if (dt != null && dt.Rows.Count > 0)
                    //    {
                    //        if (dt.Rows.Count == 2)
                    //        {
                    //            if (Convert.ToString(dt.Rows[0]["QuoteAdd_addressType"]) == "Shipping")
                    //            {
                    //                string countryid = Convert.ToString(dt.Rows[0]["QuoteAdd_countryId"]);
                    //                CmbCountry1.Value = countryid;
                    //                FillStateCombo(CmbState1, Convert.ToInt32(countryid));
                    //                string stateid = Convert.ToString(dt.Rows[0]["QuoteAdd_stateId"]);
                    //                CmbState1.Value = stateid;
                    //            }
                    //            else if (Convert.ToString(dt.Rows[1]["QuoteAdd_addressType"]) == "Shipping")
                    //            {
                    //                string countryid = Convert.ToString(dt.Rows[0]["QuoteAdd_countryId"]);
                    //                CmbCountry1.Value = countryid;
                    //                FillStateCombo(CmbState1, Convert.ToInt32(countryid));
                    //                string stateid = Convert.ToString(dt.Rows[0]["QuoteAdd_stateId"]);
                    //                CmbState1.Value = stateid;
                    //            }
                    //        }
                    //        else if (dt.Rows.Count == 1)
                    //        {
                    //            if (Convert.ToString(dt.Rows[0]["QuoteAdd_addressType"]) == "Shipping")
                    //            {
                    //                string countryid = Convert.ToString(dt.Rows[0]["QuoteAdd_countryId"]);
                    //                CmbCountry1.Value = countryid;
                    //                FillStateCombo(CmbState1, Convert.ToInt32(countryid));
                    //                string stateid = Convert.ToString(dt.Rows[0]["QuoteAdd_stateId"]);
                    //                CmbState1.Value = stateid;
                    //            }

                    //        }
                    //    }
                    //}
                    #endregion
                }
                else
                {
                    grid.JSProperties["cpSaveSuccessOrFail"] = "MultiTax";
                }
            }
        }
        #endregion

        protected string IsMinSalePriceOk(string list, DataTable DetailsTable)
        {
            string validate = "";
            DataTable minSalePriceTable = objProjectInvoiceBL.IsMinSalePriceOk(list);

            if (minSalePriceTable != null)
            {

                foreach (DataRow dr in minSalePriceTable.Rows)
                {
                    DataRow[] productRow = DetailsTable.Select("ProductID='" + Convert.ToString(dr["sProducts_ID"]) + "'");
                    if (Convert.ToDecimal(dr["sProduct_MinSalePrice"]) > Convert.ToDecimal(productRow[0]["SalePrice"]))
                    {
                        validate = "MinSalePriceGreater";
                        break;
                    }
                    if (Convert.ToDecimal(dr["sProduct_MRP"]) != 0 && Convert.ToDecimal(dr["sProduct_MRP"]) < Convert.ToDecimal(productRow[0]["SalePrice"]))
                    {
                        validate = "MRPLess";
                        break;
                    }
                }

                //validate = "MinSalePriceGreater";

            }

            return validate;
        }

        public void ModifyQuatation(string strIsInventory, string QuotationID, string strSchemeType, string strQuoteNo, string strQuoteDate, string strCustomer, string strContactName, Int64 ProjId,
                                    string Reference, string strBranch, string strAgents, string strCurrency, string strRate, string strTaxType, string strTaxCode,
                                    string strComponenyType, string strInvoiceComponent, string strInvoiceComponentDate, string strCashBank, string strDueDate,
                                    DataTable Productdt, DataTable TaxDetailTable, DataTable Warehousedt, DataTable InvoiceTaxdt, DataTable BillAddressdt,
                                    string strSchemeID, string approveStatus, string ActionType, string PosForGst, ref int strIsComplete, ref int strInvoiceID, ref string strInvoiceNumber,
            DataTable QuotationPackingDetailsdt, DataTable dtAddlDesc, string RetPercentage, string RetAmount, string GL, string Remarks, DateTime? dtDuedate
            , string strTCScode, string strTCSappl, string strTCSpercentage, string strTCSamout,string TransacCategory)
        {
            try
            {
                
                DataSet dsInst = new DataSet();
                //    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["DBConnectionDefault"]);
                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));


                SqlCommand cmd = new SqlCommand("prc_CRMProjectInvoice_AddEdit", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", ActionType);
                cmd.Parameters.AddWithValue("@approveStatus", approveStatus);
                cmd.Parameters.AddWithValue("@IsInventory", strIsInventory);
                //Chinmoy added below Line
                cmd.Parameters.AddWithValue("@SchemeID", strSchemeID);
                cmd.Parameters.AddWithValue("@InvoiceID", QuotationID);
                cmd.Parameters.AddWithValue("@InvoiceNo", strQuoteNo);
                cmd.Parameters.AddWithValue("@InvoiceDate", strQuoteDate);
                cmd.Parameters.AddWithValue("@BranchID", strBranch);
                cmd.Parameters.AddWithValue("@CustomerID", strCustomer);
                cmd.Parameters.AddWithValue("@ContactPerson", strContactName);
                cmd.Parameters.AddWithValue("@Reference", Reference);
                cmd.Parameters.AddWithValue("@PosForGst", PosForGst);
                cmd.Parameters.AddWithValue("@RetRemarks", Remarks);
                cmd.Parameters.AddWithValue("@GL", GL);
                cmd.Parameters.AddWithValue("@RetPercentage", RetPercentage);
                cmd.Parameters.AddWithValue("@RetAmount", RetAmount);
                cmd.Parameters.AddWithValue("@dtDuedt", dtDuedate);
                cmd.Parameters.AddWithValue("@Agents", strAgents);
                cmd.Parameters.AddWithValue("@ComponenyType", strComponenyType);
                cmd.Parameters.AddWithValue("@InvoiceComponent", strInvoiceComponent);
                cmd.Parameters.AddWithValue("@InvoiceComponentDate", strInvoiceComponentDate);
                cmd.Parameters.AddWithValue("@CashBank", strCashBank);
                cmd.Parameters.AddWithValue("@DueDate", strDueDate);
                cmd.Parameters.AddWithValue("@Currency", strCurrency);
                cmd.Parameters.AddWithValue("@Rate", strRate);
                cmd.Parameters.AddWithValue("@TaxType", strTaxType);
                cmd.Parameters.AddWithValue("@TaxCode", "0");
                cmd.Parameters.AddWithValue("@Remarks", Convert.ToString(txtRemarks.Text));               
                cmd.Parameters.AddWithValue("@CompanyID", Convert.ToString(Session["LastCompany"]));
                cmd.Parameters.AddWithValue("@FinYear", Convert.ToString(Session["LastFinYear"]));
                cmd.Parameters.AddWithValue("@UserID", Convert.ToString(Session["userid"]));
                cmd.Parameters.AddWithValue("@ProductDetails", Productdt);
                cmd.Parameters.AddWithValue("@TaxDetail", TaxDetailTable);
                cmd.Parameters.AddWithValue("@WarehouseDetail", Warehousedt);
                cmd.Parameters.AddWithValue("@InvoiceTax", InvoiceTaxdt);
                cmd.Parameters.AddWithValue("@BillAddress", BillAddressdt);
                cmd.Parameters.AddWithValue("@invoicefor", "CL");
                cmd.Parameters.AddWithValue("@Project_Id", ProjId);
                cmd.Parameters.AddWithValue("@QuotationPackingDetails", QuotationPackingDetailsdt);
                cmd.Parameters.AddWithValue("@udt_Addldesc", dtAddlDesc);                
                cmd.Parameters.AddWithValue("@TCScode", strTCScode);
                cmd.Parameters.AddWithValue("@TCSappAmount", strTCSappl);
                cmd.Parameters.AddWithValue("@TCSpercentage", strTCSpercentage);
                cmd.Parameters.AddWithValue("@TCSamount", strTCSamout);                
                cmd.Parameters.AddWithValue("@TransacCategory", TransacCategory);               
                cmd.Parameters.Add("@ReturnValue", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@ReturnInvoiceID", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@ReturnInvoiceNumber", SqlDbType.VarChar, 50);
                //REV 1.0
                cmd.Parameters.Add("@ReturnLedgerAmt", SqlDbType.VarChar, 50);
                //REV 1.0 END
                cmd.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
                cmd.Parameters["@ReturnInvoiceID"].Direction = ParameterDirection.Output;
                //Chinmoy Added Below Line
                cmd.Parameters["@ReturnInvoiceNumber"].Direction = ParameterDirection.Output;
                //REV 1.0
                cmd.Parameters["@ReturnLedgerAmt"].Direction = ParameterDirection.Output;
                //REV 1.0 END
                cmd.CommandTimeout = 0;
                SqlDataAdapter Adap = new SqlDataAdapter();
                Adap.SelectCommand = cmd;
                Adap.Fill(dsInst);
                strIsComplete = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value.ToString());
                strInvoiceID = Convert.ToInt32(cmd.Parameters["@ReturnInvoiceID"].Value.ToString());
                //Chinmoy added below line
                strInvoiceNumber = Convert.ToString(cmd.Parameters["@ReturnInvoiceNumber"].Value.ToString());
                //REV 1.0 
                string ReturnLedgerAmt = "";
                if (strIsComplete == -3)
                {
                    ReturnLedgerAmt = Convert.ToString(cmd.Parameters["@ReturnLedgerAmt"].Value);
                    grid.JSProperties["cpReturnLedgerAmt"] = strIsComplete;
                    string[] ReturnLedgerAmtList = new string[] { };
                    ReturnLedgerAmtList = ReturnLedgerAmt.Split('~');
                    grid.JSProperties["cpDRAmt"] = Convert.ToString(ReturnLedgerAmtList[0]);
                    grid.JSProperties["cpCRAmt"] = Convert.ToString(ReturnLedgerAmtList[1]);
                }
                //REV 1.0 END
                if (strInvoiceID > 0)
                {
                    //####### Coded By Samrat Roy For Custom Control Data Process #########
                    if (!string.IsNullOrEmpty(hfControlData.Value))
                    {
                        CommonBL objCommonBL = new CommonBL();
                        objCommonBL.InsertTransporterControlDetails(strInvoiceID, "SI", hfControlData.Value, Convert.ToString(HttpContext.Current.Session["userid"]));
                    }
                }
                if (strInvoiceID > 0)
                {                    
                    if (hdnProjectSOTC.Value == "1")
                    {
                        ProjectTermsConditionsControl.Save_ProjectTerms(Convert.ToString(strInvoiceID), "SI");
                    }
                    if (!string.IsNullOrEmpty(hfTermsConditionData.Value))
                    {
                        TermsConditionsControl.SaveTC(hfTermsConditionData.Value, Convert.ToString(strInvoiceID), "SI");
                    }
                    //}
                    //chinmoy comment for terms condition check End
                }

                if (strInvoiceID > 0)
                {
                    if (chkSendMail.Checked)
                    {
                        if (Convert.ToString(Request.QueryString["key"]) == "ADD")
                        {
                            Export.ExportToPDF exportToPDF = new Export.ExportToPDF();
                            exportToPDF.ExportToPdfforEmail("ProjectInvoice~D", "Invoice", Server.MapPath("~"), "1", Convert.ToString(strInvoiceID));
                            SendMail(Convert.ToString(strInvoiceID));
                        }
                    }
                }
                cmd.Dispose();
                con.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
        protected void Grid_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void Grid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void Grid_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void grid_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
        {
            if (e.RowType != GridViewRowType.Data)
                return;

            string IsLinkedProduct = Convert.ToString(e.GetValue("IsLinkedProduct"));
            if (IsLinkedProduct == "Y")
                e.Row.ForeColor = Color.Blue;
        }


        protected void EntityServerModeDataSalesInvoice_Selecting(object sender, DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs e)
        {
            e.KeyExpression = "Proj_Id";

            //string connectionString = ConfigurationManager.ConnectionStrings["crmConnectionString"].ConnectionString;
            string connectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);

            ERPDataClassesDataContext dc = new ERPDataClassesDataContext(connectionString);

            string Userid = Convert.ToString(HttpContext.Current.Session["userid"]);
            BusinessLogicLayer.DBEngine BEngine = new BusinessLogicLayer.DBEngine();




            var q = from d in dc.V_ProjectLists
                    where d.ProjectStatus == "Approved" && d.ProjBracnchid == Convert.ToInt64(ddl_Branch.SelectedValue) && d.CustId == Convert.ToString(hdnCustomerId.Value)
                    orderby d.Proj_Id descending
                    select d;

            e.QueryableSource = q;

        }





        #endregion

        #region Quotation Tax Details

        public IEnumerable GetTaxes()
        {
            List<Taxes> TaxList = new List<Taxes>();
            //BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
            BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine();


            DataTable DT = GetTaxData(dt_PLQuote.Date.ToString("yyyy-MM-dd"));
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                Taxes Taxes = new Taxes();
                Taxes.TaxID = Convert.ToString(DT.Rows[i]["Taxes_ID"]);
                Taxes.TaxName = Convert.ToString(DT.Rows[i]["Taxes_Name"]);
                Taxes.Percentage = Convert.ToString(DT.Rows[i]["Percentage"]);
                Taxes.Amount = Convert.ToString(DT.Rows[i]["Amount"]);
                TaxList.Add(Taxes);
            }

            return TaxList;
        }
        public IEnumerable GetTaxes(DataTable DT)
        {
            List<Taxes> TaxList = new List<Taxes>();

            decimal totalParcentage = 0;
            foreach (DataRow dr in DT.Rows)
            {
                if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                {
                    totalParcentage += Convert.ToDecimal(dr["Percentage"]);
                }
            }




            for (int i = 0; i < DT.Rows.Count; i++)
            {
                if (Convert.ToString(DT.Rows[i]["Taxes_ID"]) != "0")
                {
                    Taxes Taxes = new Taxes();
                    Taxes.TaxID = Convert.ToString(DT.Rows[i]["Taxes_ID"]);
                    Taxes.TaxName = Convert.ToString(DT.Rows[i]["Taxes_Name"]);
                    Taxes.Percentage = Convert.ToString(DT.Rows[i]["Percentage"]);
                    Taxes.Amount = Convert.ToString(DT.Rows[i]["Amount"]);
                    if (Convert.ToString(DT.Rows[i]["ApplicableOn"]) == "G")
                    {
                        Taxes.calCulatedOn = Convert.ToDecimal(HdChargeProdAmt.Value);
                    }
                    else if (Convert.ToString(DT.Rows[i]["ApplicableOn"]) == "N")
                    {
                        Taxes.calCulatedOn = Convert.ToDecimal(HdChargeProdNetAmt.Value);
                    }
                    else
                    {
                        Taxes.calCulatedOn = 0;
                    }

                    //Set Amount Value 
                    if (Taxes.Amount == "0.00")
                    {
                        Taxes.Amount = Convert.ToString(Taxes.calCulatedOn * (Convert.ToDecimal(Taxes.Percentage) / 100));
                    }


                    if (Convert.ToString(ddl_AmountAre.Value) == "2")
                    {
                        if (Convert.ToString(ddl_VatGstCst.Value) == "0~0~X")
                        {
                            if (Convert.ToString(DT.Rows[i]["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(DT.Rows[i]["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(DT.Rows[i]["TaxTypeCode"]).Trim() == "SGST")
                            {
                                decimal finalCalCulatedOn = 0;
                                decimal backProcessRate = (1 + (totalParcentage / 100));
                                finalCalCulatedOn = Taxes.calCulatedOn / backProcessRate;
                                Taxes.calCulatedOn = Math.Round(finalCalCulatedOn);
                            }
                        }
                    }


                    TaxList.Add(Taxes);
                }
            }

            return TaxList;
        }
        public DataTable GetTaxData(string quoteDate)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
            proc.AddVarcharPara("@Action", 500, "TaxDetails");
            proc.AddVarcharPara("@InvoiceID", 500, Convert.ToString(Session["SI_InvoiceID"]));
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@BranchID", 500, Convert.ToString(ddl_Branch.SelectedValue));
            proc.AddVarcharPara("@CompanyID", 500, Convert.ToString(Session["LastCompany"]));
            proc.AddVarcharPara("@S_quoteDate", 10, quoteDate);
            dt = proc.GetTable();
            return dt;
        }
        protected void gridTax_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string strSplitCommand = e.Parameters.Split('~')[0];
            if (strSplitCommand == "Display")
            {
                DataTable TaxDetailsdt = new DataTable();
                if (Session["SI_TaxDetails"] == null)
                {
                    Session["SI_TaxDetails"] = GetTaxData(dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                }

                if (Session["SI_TaxDetails"] != null)
                {
                    TaxDetailsdt = (DataTable)Session["SI_TaxDetails"];

                    #region Delete Igst,Cgst,Sgst respectively
                    //Get Company Gstin 09032017
                    string CompInternalId = Convert.ToString(Session["LastCompany"]);
                    string BranchId = Convert.ToString(ddl_Branch.SelectedValue);
                    string[] compGstin = oDBEngine.GetFieldValue1("tbl_master_company", "cmp_gstin", "cmp_internalid='" + CompInternalId + "'", 1);
                    string[] branchGstin = oDBEngine.GetFieldValue1("tbl_master_branch", "isnull(branch_GSTIN,'')branch_GSTIN", "branch_id='" + BranchId + "'", 1);
                    String GSTIN = "";
                    if (compGstin.Length > 0)
                    {
                        if (branchGstin[0].Trim() != "")
                        {
                            GSTIN = branchGstin[0].Trim();
                        }
                        else
                        {
                            GSTIN = compGstin[0].Trim();
                        }
                    }

                    string ShippingState = "";

                    #region ##### Added By : Samrat Roy -- For BillingShippingUserControl ######
                    string sstateCode = "";

                    if (ddl_PosGst.Value.ToString() == "S")
                    {
                        sstateCode = Sales_BillingShipping.GeteShippingStateCode();
                    }
                    else
                    {
                        sstateCode = Sales_BillingShipping.GetBillingStateCode();
                    }

                    ShippingState = sstateCode;
                    if (ShippingState.Trim() != "")
                    {
                        ShippingState = ShippingState;
                    }
                    #region ##### Old Code -- BillingShipping ######
                    ////if (chkBilling.Checked)
                    ////{
                    ////    if (CmbState.Value != null)
                    ////    {
                    ////        ShippingState = CmbState.Text;
                    ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                    ////    }
                    ////}
                    ////else
                    ////{
                    ////    if (CmbState1.Value != null)
                    ////    {
                    ////        ShippingState = CmbState1.Text;
                    ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                    ////    }
                    ////}

                    #endregion

                    #endregion


                    if (ShippingState.Trim() != "" && GSTIN.Trim() != "")
                    {

                        if (GSTIN.Substring(0, 2) == ShippingState)
                        {
                            //Check if the state is in union territories then only UTGST will apply
                            //   Chandigarh     Andaman and Nicobar Islands    DADRA & NAGAR HAVELI    DAMAN & DIU        Lakshadweep              PONDICHERRY
                            if (ShippingState == "4" || ShippingState == "26" || ShippingState == "25" || ShippingState == "35" || ShippingState == "31" || ShippingState == "34")
                            {
                                foreach (DataRow dr in TaxDetailsdt.Rows)
                                {
                                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                                    {
                                        dr.Delete();
                                    }
                                }

                            }
                            else
                            {
                                foreach (DataRow dr in TaxDetailsdt.Rows)
                                {
                                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                                    {
                                        dr.Delete();
                                    }
                                }
                            }
                            TaxDetailsdt.AcceptChanges();
                        }
                        else
                        {
                            foreach (DataRow dr in TaxDetailsdt.Rows)
                            {
                                if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                                {
                                    dr.Delete();
                                }
                            }
                            TaxDetailsdt.AcceptChanges();

                        }

                    }

                    //If Company GSTIN is blank then Delete All CGST,UGST,IGST,CGST
                    if (GSTIN.Trim() == "")
                    {
                        foreach (DataRow dr in TaxDetailsdt.Rows)
                        {
                            if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST")
                            {
                                dr.Delete();
                            }
                        }
                        TaxDetailsdt.AcceptChanges();
                    }

                    #endregion






                    //gridTax.DataSource = GetTaxes();
                    var taxlist = (List<Taxes>)GetTaxes(TaxDetailsdt);
                    var taxChargeDataSource = setChargeCalculatedOn(taxlist, TaxDetailsdt);
                    gridTax.DataSource = taxChargeDataSource;
                    gridTax.DataBind();
                    gridTax.JSProperties["cpJsonChargeData"] = createJsonForChargesTax(TaxDetailsdt);
                    gridTax.JSProperties["cpTotalCharges"] = ClculatedTotalCharge(taxChargeDataSource);
                }
            }
            else if (strSplitCommand == "SaveGst")
            {
                DataTable TaxDetailsdt = new DataTable();
                if (Session["SI_TaxDetails"] != null)
                {
                    TaxDetailsdt = (DataTable)Session["SI_TaxDetails"];
                }
                else
                {
                    TaxDetailsdt.Columns.Add("Taxes_ID", typeof(string));
                    TaxDetailsdt.Columns.Add("Taxes_Name", typeof(string));
                    TaxDetailsdt.Columns.Add("Percentage", typeof(string));
                    TaxDetailsdt.Columns.Add("Amount", typeof(string));
                    TaxDetailsdt.Columns.Add("TaxTypeCode", typeof(string));
                    //ForGst
                    TaxDetailsdt.Columns.Add("AltTax_Code", typeof(string));
                }
                DataRow[] existingRow = TaxDetailsdt.Select("Taxes_ID='0'");
                if (Convert.ToString(cmbGstCstVatcharge.Value) == "0")
                {
                    if (existingRow.Length > 0)
                    {
                        TaxDetailsdt.Rows.Remove(existingRow[0]);
                    }
                }
                else
                {
                    if (existingRow.Length > 0)
                    {
                        existingRow[0]["Percentage"] = (cmbGstCstVatcharge.Value != null) ? Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[1] : "0";
                        existingRow[0]["Amount"] = txtGstCstVatCharge.Text;
                        existingRow[0]["AltTax_Code"] = (cmbGstCstVatcharge.Value != null) ? Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[0] : "0";

                    }
                    else
                    {
                        string GstTaxId = (cmbGstCstVatcharge.Value != null) ? Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[0] : "0";
                        string GstPerCentage = (cmbGstCstVatcharge.Value != null) ? Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[1] : "0";

                        string GstAmount = txtGstCstVatCharge.Text;
                        DataRow gstRow = TaxDetailsdt.NewRow();
                        gstRow["Taxes_ID"] = 0;
                        gstRow["Taxes_Name"] = cmbGstCstVatcharge.Text;
                        gstRow["Percentage"] = GstPerCentage;
                        gstRow["Amount"] = GstAmount;
                        gstRow["AltTax_Code"] = GstTaxId;
                        TaxDetailsdt.Rows.Add(gstRow);
                    }

                    Session["SI_TaxDetails"] = TaxDetailsdt;
                }
            }
        }
        protected decimal ClculatedTotalCharge(List<Taxes> taxChargeDataSource)
        {
            decimal totalCharges = 0;
            foreach (Taxes txObj in taxChargeDataSource)
            {

                if (Convert.ToString(txObj.TaxName).Contains("(+)"))
                {
                    totalCharges += Convert.ToDecimal(txObj.Amount);
                }
                else
                {
                    totalCharges -= Convert.ToDecimal(txObj.Amount);
                }

            }
            totalCharges += Convert.ToDecimal(txtGstCstVatCharge.Text);

            return totalCharges;

        }
        protected void gridTax_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void gridTax_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void gridTax_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void gridTax_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            DataTable TaxDetailsdt = new DataTable();
            if (Session["SI_TaxDetails"] != null)
            {
                TaxDetailsdt = (DataTable)Session["SI_TaxDetails"];
            }
            else
            {
                TaxDetailsdt.Columns.Add("Taxes_ID", typeof(string));
                TaxDetailsdt.Columns.Add("Taxes_Name", typeof(string));
                TaxDetailsdt.Columns.Add("Percentage", typeof(string));
                TaxDetailsdt.Columns.Add("Amount", typeof(string));
                TaxDetailsdt.Columns.Add("TaxTypeCode", typeof(string));
                //ForGst
                TaxDetailsdt.Columns.Add("AltTax_Code", typeof(string));
            }

            foreach (var args in e.UpdateValues)
            {
                string TaxID = Convert.ToString(args.Keys["TaxID"]);
                string TaxName = Convert.ToString(args.NewValues["TaxName"]);
                string Percentage = Convert.ToString(args.NewValues["Percentage"]);
                string Amount = Convert.ToString(args.NewValues["Amount"]);

                bool Isexists = false;
                foreach (DataRow drr in TaxDetailsdt.Rows)
                {
                    string OldTaxID = Convert.ToString(drr["Taxes_ID"]);

                    if (OldTaxID == TaxID)
                    {
                        Isexists = true;

                        drr["Percentage"] = Percentage;
                        drr["Amount"] = Amount;

                        break;
                    }
                }

                if (Isexists == false)
                {
                    TaxDetailsdt.Rows.Add(TaxID, TaxName, Percentage, Amount, 0);
                }
            }

            if (cmbGstCstVatcharge.Value != null)
            {
                DataRow[] existingRow = TaxDetailsdt.Select("Taxes_ID='0'");
                if (Convert.ToString(cmbGstCstVatcharge.Value) == "0")
                {
                    if (existingRow.Length > 0)
                    {
                        TaxDetailsdt.Rows.Remove(existingRow[0]);
                    }
                }
                else
                {
                    if (existingRow.Length > 0)
                    {

                        existingRow[0]["Percentage"] = Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[1];
                        existingRow[0]["Amount"] = txtGstCstVatCharge.Text;
                        existingRow[0]["AltTax_Code"] = Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[0]; ;

                    }
                    else
                    {
                        string GstTaxId = Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[0];
                        string GstPerCentage = Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[1];
                        string GstAmount = txtGstCstVatCharge.Text;
                        DataRow gstRow = TaxDetailsdt.NewRow();
                        gstRow["Taxes_ID"] = 0;
                        gstRow["Taxes_Name"] = cmbGstCstVatcharge.Text;
                        gstRow["Percentage"] = GstPerCentage;
                        gstRow["Amount"] = GstAmount;
                        gstRow["AltTax_Code"] = GstTaxId;
                        TaxDetailsdt.Rows.Add(gstRow);
                    }
                }
            }

            Session["SI_TaxDetails"] = TaxDetailsdt;

            gridTax.DataSource = GetTaxes(TaxDetailsdt);
            gridTax.DataBind();
        }
        protected void gridTax_DataBinding(object sender, EventArgs e)
        {
            if (Session["SI_TaxDetails"] != null)
            {
                DataTable TaxDetailsdt = (DataTable)Session["SI_TaxDetails"];

                //gridTax.DataSource = GetTaxes();
                var taxlist = (List<Taxes>)GetTaxes(TaxDetailsdt);
                var taxChargeDataSource = setChargeCalculatedOn(taxlist, TaxDetailsdt);
                gridTax.DataSource = taxChargeDataSource;
            }
        }


        #endregion

        #region Warehouse Details

        public DataTable GetQuotationWarehouseData()
        {
            try
            {

                MasterSettings masterBl = new MasterSettings();
                string multiwarehouse = Convert.ToString(masterBl.GetSettings("IaMultilevelWarehouse"));

                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
                proc.AddVarcharPara("@Action", 500, "InvoiceWarehouse");
                proc.AddVarcharPara("@InvoiceID", 500, Convert.ToString(Session["SI_InvoiceID"]));
                proc.AddVarcharPara("@Multiwarehouse", 500, Convert.ToString(multiwarehouse));
                dt = proc.GetTable();

                string strNewVal = "", strOldVal = "";
                DataTable tempdt = dt.Copy();
                foreach (DataRow drr in tempdt.Rows)
                {
                    strNewVal = Convert.ToString(drr["QuoteWarehouse_Id"]);

                    if (strNewVal == strOldVal)
                    {
                        drr["WarehouseName"] = "";
                        drr["BatchNo"] = "";
                        drr["SalesUOMName"] = "";
                        drr["SalesQuantity"] = "";
                        drr["StkUOMName"] = "";
                        drr["StkQuantity"] = "";
                        drr["ConversionMultiplier"] = "";
                        drr["AvailableQty"] = "";
                        drr["BalancrStk"] = "";
                        drr["MfgDate"] = "";
                        drr["ExpiryDate"] = "";
                    }

                    strOldVal = strNewVal;
                }

                Session["SI_LoopWarehouse"] = Convert.ToString(Convert.ToInt32(strNewVal) + 1);
                tempdt.Columns.Remove("QuoteWarehouse_Id");
                return tempdt;
            }
            catch
            {
                return null;
            }
        }
        public DataTable GetTaggingWarehouseData(string ComponentDetailsIDs, string strType)
        {
            try
            {

                MasterSettings masterBl = new MasterSettings();
                string multiwarehouse = Convert.ToString(masterBl.GetSettings("IaMultilevelWarehouse"));

                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
                proc.AddVarcharPara("@Action", 500, "ComponentWarehouse");
                proc.AddVarcharPara("@SelectedComponentList", 2000, ComponentDetailsIDs);
                proc.AddVarcharPara("@ComponentType", 10, strType);
                proc.AddVarcharPara("@Multiwarehouse", 500, Convert.ToString(multiwarehouse));
                dt = proc.GetTable();

                string strNewVal = "", strOldVal = "";
                DataTable tempdt = dt.Copy();
                foreach (DataRow drr in tempdt.Rows)
                {
                    strNewVal = Convert.ToString(drr["QuoteWarehouse_Id"]);

                    if (strNewVal == strOldVal)
                    {
                        drr["WarehouseName"] = "";
                        drr["Quantity"] = "";
                        drr["BatchNo"] = "";
                        drr["SalesUOMName"] = "";
                        drr["SalesQuantity"] = "";
                        drr["StkUOMName"] = "";
                        drr["StkQuantity"] = "";
                        drr["ConversionMultiplier"] = "";
                        drr["AvailableQty"] = "";
                        drr["BalancrStk"] = "";
                        drr["MfgDate"] = "";
                        drr["ExpiryDate"] = "";
                    }

                    strOldVal = strNewVal;
                }

                Session["SI_LoopWarehouse"] = Convert.ToString(Convert.ToInt32(strNewVal) + 1);
                tempdt.Columns.Remove("QuoteWarehouse_Id");
                return tempdt;
            }
            catch
            {
                return null;
            }
        }
        protected void CmbWarehouse_Callback(object sender, CallbackEventArgsBase e)
        {
            string WhichCall = e.Parameter.Split('~')[0];
            if (WhichCall == "BindWarehouse")
            {
                DataTable dt = GetWarehouseData();

                CmbWarehouse.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CmbWarehouse.Items.Add(Convert.ToString(dt.Rows[i]["WarehouseName"]), Convert.ToString(dt.Rows[i]["WarehouseID"]));
                }
            }
        }
        protected void CmbBatch_Callback(object sender, CallbackEventArgsBase e)
        {
            string WhichCall = e.Parameter.Split('~')[0];
            if (WhichCall == "BindBatch")
            {
                string WarehouseID = Convert.ToString(e.Parameter.Split('~')[1]);
                DataTable dt = GetBatchData(WarehouseID);

                CmbBatch.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CmbBatch.Items.Add(Convert.ToString(dt.Rows[i]["BatchName"]), Convert.ToString(dt.Rows[i]["BatchID"]));
                }
            }
        }

        [WebMethod]
        public static string GetSerialId(string id, string wareHouseStr, string BatchID, string ProducttId)
        {
            CRMSalesOrderDtlBL objCRMSalesOrderDtlBL = new CRMSalesOrderDtlBL();

            string[] Serials = id.Split(';');
            string Serial = Serials[0].TrimStart(';');
            string ispermission = string.Empty;
            string LastSerial = string.Empty;
            for (int i = 0; i < Serials.Length; i++)
            {
                LastSerial = Serials[Serials.Length - 1].TrimStart(';');

            }
            //string SerialLast=
            DataTable dt = new DataTable();
            //ispermission = objCRMSalesOrderDtlBL.GetInvoiceCustomerId(Convert.ToInt32(KeyVal));
            if (!string.IsNullOrEmpty(LastSerial))
            {
                dt = objCRMSalesOrderDtlBL.GetSerialataOnFIFOBasis(wareHouseStr, BatchID, Serial, ProducttId, id, LastSerial);
            }
            else
            {
                dt = objCRMSalesOrderDtlBL.GetSerialataOnFIFOBasis(wareHouseStr, BatchID, Serial, ProducttId, id, Serial);
            }


            ispermission = Convert.ToString(dt.Rows[0].Field<Int32>("ResturnVal"));
            return Convert.ToString(ispermission);

        }

        protected void CmbSerial_Callback(object sender, CallbackEventArgsBase e)
        {
            string WhichCall = e.Parameter.Split('~')[0];
            if (WhichCall == "BindSerial")
            {
                string WarehouseID = Convert.ToString(e.Parameter.Split('~')[1]);
                string BatchID = Convert.ToString(e.Parameter.Split('~')[2]);
                DataTable dt = GetSerialata(WarehouseID, BatchID);

                if (Session["SI_WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["SI_WarehouseData"];
                    DataTable tempdt = Warehousedt.DefaultView.ToTable(false, "SerialID");

                    foreach (DataRow dr in dt.Rows)
                    {
                        string SerialID = Convert.ToString(dr["SerialID"]);
                        DataRow[] rows = tempdt.Select("SerialID = '" + SerialID + "' AND SerialID<>'0'");

                        if (rows != null && rows.Length > 0)
                        {
                            dr.Delete();
                        }

                        //foreach (DataRow drr in tempdt.Rows)
                        //{
                        //    string oldSerialID = Convert.ToString(drr["SerialID"]);
                        //    if (newSerialID == oldSerialID)
                        //    {
                        //        dr.Delete();
                        //    }
                        //}

                    }
                    dt.AcceptChanges();

                }

                ASPxListBox lb = sender as ASPxListBox;
                lb.DataSource = dt;
                lb.ValueField = "SerialID";
                lb.TextField = "SerialName";
                lb.ValueType = typeof(string);
                lb.DataBind();
            }
            else if (WhichCall == "EditSerial")
            {
                string WarehouseID = Convert.ToString(e.Parameter.Split('~')[1]);
                string BatchID = Convert.ToString(e.Parameter.Split('~')[2]);
                string editSerialID = Convert.ToString(e.Parameter.Split('~')[3]);
                DataTable dt = GetSerialata(WarehouseID, BatchID);

                if (Session["SI_WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["SI_WarehouseData"];
                    DataTable tempdt = Warehousedt.DefaultView.ToTable(false, "SerialID");

                    foreach (DataRow dr in dt.Rows)
                    {
                        string SerialID = Convert.ToString(dr["SerialID"]);
                        DataRow[] rows = tempdt.Select("SerialID = '" + SerialID + "' AND SerialID not in ('0','" + editSerialID + "')");

                        if (rows != null && rows.Length > 0)
                        {
                            dr.Delete();
                        }

                        //foreach (DataRow drr in tempdt.Rows)
                        //{
                        //    string oldSerialID = Convert.ToString(drr["SerialID"]);
                        //    if (newSerialID == oldSerialID)
                        //    {
                        //        dr.Delete();
                        //    }
                        //}

                    }
                    dt.AcceptChanges();

                }

                ASPxListBox lb = sender as ASPxListBox;
                lb.DataSource = dt;
                lb.ValueField = "SerialID";
                lb.TextField = "SerialName";
                lb.ValueType = typeof(string);
                lb.DataBind();
            }
        }
        protected void listBox_Init(object sender, EventArgs e)
        {
            ASPxListBox lb = sender as ASPxListBox;
            DataTable dt = GetSerialata("", "");

            lb.DataSource = dt;
            lb.ValueField = "SerialID";
            lb.TextField = "SerialName";
            lb.ValueType = typeof(string);
            lb.DataBind();
        }
        protected void GrdWarehouse_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            GrdWarehouse.JSProperties["cpIsSave"] = "";
            string strSplitCommand = e.Parameters.Split('~')[0];
            string Type = "";

            if (strSplitCommand == "Display")
            {
                GetProductType(ref Type);
                string ProductID = Convert.ToString(hdfProductID.Value);
                string SerialID = Convert.ToString(e.Parameters.Split('~')[1]);

                DataTable Warehousedt = new DataTable();
                if (Session["SI_WarehouseData"] != null)
                {
                    Warehousedt = (DataTable)Session["SI_WarehouseData"];
                }
                else
                {
                    Warehousedt.Columns.Add("Product_SrlNo", typeof(string));
                    Warehousedt.Columns.Add("SrlNo", typeof(string));
                    Warehousedt.Columns.Add("WarehouseID", typeof(string));
                    Warehousedt.Columns.Add("WarehouseName", typeof(string));
                    Warehousedt.Columns.Add("Quantity", typeof(string));
                    Warehousedt.Columns.Add("BatchID", typeof(string));
                    Warehousedt.Columns.Add("BatchNo", typeof(string));
                    Warehousedt.Columns.Add("SerialID", typeof(string));
                    Warehousedt.Columns.Add("SerialNo", typeof(string));
                    Warehousedt.Columns.Add("SalesUOMName", typeof(string));
                    Warehousedt.Columns.Add("SalesUOMCode", typeof(string));
                    Warehousedt.Columns.Add("SalesQuantity", typeof(string));
                    Warehousedt.Columns.Add("StkUOMName", typeof(string));
                    Warehousedt.Columns.Add("StkUOMCode", typeof(string));
                    Warehousedt.Columns.Add("StkQuantity", typeof(string));
                    Warehousedt.Columns.Add("ConversionMultiplier", typeof(string));
                    Warehousedt.Columns.Add("AvailableQty", typeof(string));
                    Warehousedt.Columns.Add("BalancrStk", typeof(string));
                    Warehousedt.Columns.Add("MfgDate", typeof(string));
                    Warehousedt.Columns.Add("ExpiryDate", typeof(string));
                    Warehousedt.Columns.Add("LoopID", typeof(string));
                    Warehousedt.Columns.Add("TotalQuantity", typeof(string));
                    Warehousedt.Columns.Add("Status", typeof(string));
                }

                if (Warehousedt != null && Warehousedt.Rows.Count > 0)
                {
                    DataView dvData = new DataView(Warehousedt);
                    dvData.RowFilter = "Product_SrlNo = '" + SerialID + "'";

                    GrdWarehouse.DataSource = dvData;
                    GrdWarehouse.DataBind();
                }
                else
                {
                    GrdWarehouse.DataSource = Warehousedt.DefaultView;
                    GrdWarehouse.DataBind();
                }
                changeGridOrder();
            }
            else if (strSplitCommand == "SaveDisplay")
            {
                int loopId = Convert.ToInt32(Session["SI_LoopWarehouse"]);

                string WarehouseID = Convert.ToString(e.Parameters.Split('~')[1]);
                string WarehouseName = Convert.ToString(e.Parameters.Split('~')[2]);
                string BatchID = Convert.ToString(e.Parameters.Split('~')[3]);
                string BatchName = Convert.ToString(e.Parameters.Split('~')[4]);
                string SerialID = Convert.ToString(e.Parameters.Split('~')[5]);
                string SerialName = Convert.ToString(e.Parameters.Split('~')[6]);
                string ProductID = Convert.ToString(hdfProductID.Value);
                string ProductSerialID = Convert.ToString(hdfProductSerialID.Value);
                string Qty = Convert.ToString(e.Parameters.Split('~')[7]);
                string editWarehouseID = Convert.ToString(e.Parameters.Split('~')[8]);

                string Sales_UOM_Name = "", Sales_UOM_Code = "", Stk_UOM_Name = "", Stk_UOM_Code = "", Conversion_Multiplier = "", Trans_Stock = "0", MfgDate = "", ExpiryDate = "";
                GetProductType(ref Type);

                DataTable Warehousedt = new DataTable();
                if (Session["SI_WarehouseData"] != null)
                {
                    Warehousedt = (DataTable)Session["SI_WarehouseData"];
                }
                else
                {
                    Warehousedt.Columns.Add("Product_SrlNo", typeof(string));
                    Warehousedt.Columns.Add("SrlNo", typeof(string));
                    Warehousedt.Columns.Add("WarehouseID", typeof(string));
                    Warehousedt.Columns.Add("WarehouseName", typeof(string));
                    Warehousedt.Columns.Add("Quantity", typeof(string));
                    Warehousedt.Columns.Add("BatchID", typeof(string));
                    Warehousedt.Columns.Add("BatchNo", typeof(string));
                    Warehousedt.Columns.Add("SerialID", typeof(string));
                    Warehousedt.Columns.Add("SerialNo", typeof(string));
                    Warehousedt.Columns.Add("SalesUOMName", typeof(string));
                    Warehousedt.Columns.Add("SalesUOMCode", typeof(string));
                    Warehousedt.Columns.Add("SalesQuantity", typeof(string));
                    Warehousedt.Columns.Add("StkUOMName", typeof(string));
                    Warehousedt.Columns.Add("StkUOMCode", typeof(string));
                    Warehousedt.Columns.Add("StkQuantity", typeof(string));
                    Warehousedt.Columns.Add("ConversionMultiplier", typeof(string));
                    Warehousedt.Columns.Add("AvailableQty", typeof(string));
                    Warehousedt.Columns.Add("BalancrStk", typeof(string));
                    Warehousedt.Columns.Add("MfgDate", typeof(string));
                    Warehousedt.Columns.Add("ExpiryDate", typeof(string));
                    Warehousedt.Columns.Add("LoopID", typeof(string));
                    Warehousedt.Columns.Add("TotalQuantity", typeof(string));
                    Warehousedt.Columns.Add("Status", typeof(string));
                }

                bool IsDelete = false;

                if (Type == "WBS")
                {
                    GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    string[] SerialIDList = SerialID.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string[] SerialNameList = SerialName.Split(new string[] { "||@||" }, StringSplitOptions.None);

                    if (editWarehouseID == "0")
                    {
                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];


                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, "D");
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", BatchID, "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, "D");
                            }
                        }
                    }
                    else
                    {
                        string strLoopID = "0", strSerial = "0";

                        DataRow[] result = Warehousedt.Select("SrlNo ='" + editWarehouseID + "'");
                        foreach (DataRow row in result)
                        {
                            strSerial = Convert.ToString(row["SerialID"]);
                            strLoopID = Convert.ToString(row["LoopID"]);
                        }

                        int count = 0;
                        DataRow[] dr = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        int value = (dr.Length + SerialIDList.Length - 1);

                        string[] temp_SerialIDList = new string[value];
                        string[] temp_SerialNameList = new string[value];

                        string[] check_SerialIDList = new string[value];
                        string[] check_SerialNameList = new string[value];

                        foreach (DataRow rows in dr)
                        {
                            if (strSerial != Convert.ToString(rows["SerialID"]))
                            {
                                temp_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                temp_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                check_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                check_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                count++;
                            }
                        }

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            temp_SerialIDList[count] = Convert.ToString(SerialIDList[i]);
                            temp_SerialNameList[count] = Convert.ToString(SerialNameList[i]);

                            count++;
                        }

                        DataRow[] delResult = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        foreach (DataRow delrow in delResult)
                        {
                            delrow.Delete();
                        }
                        Warehousedt.AcceptChanges();

                        SerialIDList = temp_SerialIDList;
                        SerialNameList = temp_SerialNameList;

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];
                            string repute = "D";
                            if (check_SerialIDList.Contains(strSrlID)) repute = "I";

                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, strLoopID, SerialIDList.Length, repute);
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", BatchID, "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", strLoopID, SerialIDList.Length, repute);
                            }
                        }
                    }
                }
                else if (Type == "W")
                {
                    GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);

                    decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                    string stkqtn = Convert.ToString(Math.Round((Convert.ToDecimal(Qty) * ConversionMultiplier), 2));
                    decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                    BatchID = "0";

                    if (editWarehouseID == "0")
                    {
                        string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                        var updaterows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND Product_SrlNo='" + ProductSerialID + "'");

                        if (updaterows.Length == 0)
                        {
                            Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                        }
                        else
                        {
                            foreach (var row in updaterows)
                            {
                                decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);
                                row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                            }
                        }
                    }
                    else
                    {
                        var rows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND Convert(TotalQuantity, 'System.Decimal')='" + Qty + "' AND SrlNo='" + editWarehouseID + "'");

                        if (rows.Length == 0)
                        {
                            string whID = "";
                            string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                            var updaterows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND Product_SrlNo='" + ProductSerialID + "'");
                            foreach (var row in updaterows)
                            {
                                whID = Convert.ToString(row["SrlNo"]);
                            }

                            if (updaterows.Length == 0)
                            {
                                IsDelete = true;
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");

                            }
                            else if (editWarehouseID == whID)
                            {
                                foreach (var row in updaterows)
                                {
                                    whID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = Qty;
                                    row["TotalQuantity"] = Qty;
                                    row["SalesQuantity"] = Qty + " " + Sales_UOM_Name;
                                }
                            }
                            else if (editWarehouseID != whID)
                            {
                                IsDelete = true;
                                foreach (var row in updaterows)
                                {
                                    ID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                                }
                            }
                        }
                    }
                }
                else if (Type == "WB")
                {
                    GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                    string stkqtn = Convert.ToString(Math.Round((Convert.ToDecimal(Qty) * ConversionMultiplier), 2));
                    decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);

                    if (editWarehouseID == "0")
                    {
                        string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                        var updaterows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND BatchID='" + BatchID + "' AND Product_SrlNo='" + ProductSerialID + "'");

                        if (updaterows.Length == 0)
                        {
                            Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                        }
                        else
                        {
                            foreach (var row in updaterows)
                            {
                                decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);
                                row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                            }
                        }
                    }
                    else
                    {
                        var rows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND BatchID='" + BatchID + "' AND Convert(TotalQuantity, 'System.Decimal')='" + Qty + "' AND SrlNo='" + editWarehouseID + "'");
                        if (rows.Length == 0)
                        {
                            string whID = "";
                            string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                            var updaterows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND BatchID='" + BatchID + "' AND Product_SrlNo='" + ProductSerialID + "'");
                            foreach (var row in updaterows)
                            {
                                whID = Convert.ToString(row["SrlNo"]);
                            }

                            if (updaterows.Length == 0)
                            {
                                IsDelete = true;
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                            }
                            else if (editWarehouseID == whID)
                            {
                                foreach (var row in updaterows)
                                {
                                    whID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = Qty;
                                    row["TotalQuantity"] = Qty;
                                    row["SalesQuantity"] = Qty + " " + Sales_UOM_Name;
                                }
                            }
                            else if (editWarehouseID != whID)
                            {
                                IsDelete = true;
                                foreach (var row in updaterows)
                                {
                                    ID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                                }
                            }
                        }
                    }
                }
                else if (Type == "B")
                {
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                    string stkqtn = Convert.ToString(Math.Round((Convert.ToDecimal(Qty) * ConversionMultiplier), 2));
                    decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                    WarehouseID = "0";


                    if (editWarehouseID == "0")
                    {
                        string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                        var updaterows = Warehousedt.Select("BatchID ='" + BatchID + "' AND Product_SrlNo='" + ProductSerialID + "'");

                        if (updaterows.Length == 0)
                        {
                            Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                        }
                        else
                        {
                            foreach (var row in updaterows)
                            {
                                decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);
                                row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                            }
                        }
                    }
                    else
                    {
                        var rows = Warehousedt.Select("BatchID='" + BatchID + "' AND Convert(TotalQuantity, 'System.Decimal')='" + Qty + "' AND SrlNo='" + editWarehouseID + "'");
                        if (rows.Length == 0)
                        {
                            string whID = "";
                            string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                            var updaterows = Warehousedt.Select("BatchID ='" + BatchID + "' AND Product_SrlNo='" + ProductSerialID + "'");
                            foreach (var row in updaterows)
                            {
                                whID = Convert.ToString(row["SrlNo"]);
                            }

                            if (updaterows.Length == 0)
                            {
                                IsDelete = true;
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                            }
                            else if (editWarehouseID == whID)
                            {
                                foreach (var row in updaterows)
                                {
                                    whID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = Qty;
                                    row["TotalQuantity"] = Qty;
                                    row["SalesQuantity"] = Qty + " " + Sales_UOM_Name;
                                }
                            }
                            else if (editWarehouseID != whID)
                            {
                                IsDelete = true;
                                foreach (var row in updaterows)
                                {
                                    ID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                                }
                            }
                        }
                    }
                }
                else if (Type == "S")
                {
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);

                    string[] SerialIDList = SerialID.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string[] SerialNameList = SerialName.Split(new string[] { "||@||" }, StringSplitOptions.None);

                    //Qty = Convert.ToString(SerialIDList.Length);
                    Qty = "1";
                    decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                    string stkqtn = Convert.ToString(Math.Round((Convert.ToDecimal(Qty) * ConversionMultiplier), 2));
                    decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                    WarehouseID = "0";
                    BatchID = "0";

                    if (editWarehouseID != "0")
                    {
                        DataRow[] delResult = Warehousedt.Select("SrlNo ='" + editWarehouseID + "'");
                        foreach (DataRow delrow in delResult)
                        {
                            delrow.Delete();
                        }
                        Warehousedt.AcceptChanges();

                        bool isfirstRow = false;
                        var updateDeleterows = Warehousedt.Select("Product_SrlNo='" + ProductSerialID + "'");
                        if (updateDeleterows.Length > 0)
                        {
                            foreach (var row in updateDeleterows)
                            {
                                decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                row["Quantity"] = (oldQuantity - Convert.ToDecimal(1));
                                row["TotalQuantity"] = (oldQuantity - Convert.ToDecimal(1));
                                if (Convert.ToString(row["SalesQuantity"]) != "")
                                {
                                    isfirstRow = true;
                                    row["SalesQuantity"] = (oldQuantity - Convert.ToDecimal(1)) + " " + Sales_UOM_Name;
                                }
                            }

                            if (isfirstRow == false)
                            {
                                foreach (var row in updateDeleterows)
                                {
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);
                                    row["SalesQuantity"] = oldQuantity + " " + Sales_UOM_Name;
                                }
                            }
                        }
                    }

                    for (int i = 0; i < SerialIDList.Length; i++)
                    {
                        string strSrlID = SerialIDList[i];
                        string strSrlName = SerialNameList[i];

                        if (editWarehouseID == "0")
                        {
                            var updaterows = Warehousedt.Select("Product_SrlNo='" + ProductSerialID + "'");
                            string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                            decimal oldQuantity = 0;
                            string whID = "1";

                            if (updaterows.Length > 0)
                            {
                                foreach (var row in updaterows)
                                {
                                    oldQuantity = Convert.ToDecimal(row["Quantity"]);
                                    whID = Convert.ToString(row["LoopID"]);

                                    row["Quantity"] = (oldQuantity + Convert.ToDecimal(1));
                                    row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(1));
                                    if (Convert.ToString(row["SalesQuantity"]) != "")
                                    {
                                        row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(1)) + " " + Sales_UOM_Name;
                                    }
                                }

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, (oldQuantity + Convert.ToDecimal(1)), BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, "", Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, whID, (oldQuantity + Convert.ToDecimal(1)), "D");
                            }
                            else
                            {
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, "1", BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, "1" + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, "1", "D");
                            }
                        }
                        else
                        {
                            var rows = Warehousedt.Select("SerialID ='" + strSrlID + "' AND SrlNo='" + editWarehouseID + "'");
                            if (rows.Length == 0)
                            {
                                //string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                //Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, "D");

                                var updaterows = Warehousedt.Select("Product_SrlNo='" + ProductSerialID + "'");
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                decimal oldQuantity = 0;
                                string whID = "1";

                                if (updaterows.Length > 0)
                                {
                                    foreach (var row in updaterows)
                                    {
                                        oldQuantity = Convert.ToDecimal(row["Quantity"]);
                                        whID = Convert.ToString(row["LoopID"]);

                                        row["Quantity"] = (oldQuantity + Convert.ToDecimal(1));
                                        row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(1));
                                        if (Convert.ToString(row["SalesQuantity"]) != "")
                                        {
                                            row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(1)) + " " + Sales_UOM_Name;
                                        }
                                    }

                                    Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, (oldQuantity + Convert.ToDecimal(1)), BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, "", Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, whID, (oldQuantity + Convert.ToDecimal(1)), "D");
                                }
                                else
                                {
                                    Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, "1", BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, "1" + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, "1", "D");
                                }
                            }
                        }
                    }
                }
                else if (Type == "WS")
                {
                    GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    //GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    string[] SerialIDList = SerialID.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string[] SerialNameList = SerialName.Split(new string[] { "||@||" }, StringSplitOptions.None);

                    if (editWarehouseID == "0")
                    {
                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];


                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, "0", BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, "D");
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", "0", "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, "D");
                            }
                        }
                    }
                    else
                    {
                        string strLoopID = "0", strSerial = "0";

                        DataRow[] result = Warehousedt.Select("SrlNo ='" + editWarehouseID + "'");
                        foreach (DataRow row in result)
                        {
                            strSerial = Convert.ToString(row["SerialID"]);
                            strLoopID = Convert.ToString(row["LoopID"]);
                        }

                        int count = 0;
                        DataRow[] dr = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        int value = (dr.Length + SerialIDList.Length - 1);

                        string[] temp_SerialIDList = new string[value];
                        string[] temp_SerialNameList = new string[value];

                        string[] check_SerialIDList = new string[value];
                        string[] check_SerialNameList = new string[value];

                        foreach (DataRow rows in dr)
                        {
                            if (strSerial != Convert.ToString(rows["SerialID"]))
                            {
                                temp_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                temp_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                check_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                check_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                count++;
                            }
                        }

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            temp_SerialIDList[count] = Convert.ToString(SerialIDList[i]);
                            temp_SerialNameList[count] = Convert.ToString(SerialNameList[i]);

                            count++;
                        }

                        DataRow[] delResult = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        foreach (DataRow delrow in delResult)
                        {
                            delrow.Delete();
                        }
                        Warehousedt.AcceptChanges();

                        SerialIDList = temp_SerialIDList;
                        SerialNameList = temp_SerialNameList;

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];
                            string repute = "D";
                            if (check_SerialIDList.Contains(strSrlID)) repute = "I";

                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, "0", BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, repute);
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", "0", "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, repute);
                            }
                        }
                    }
                }
                else if (Type == "BS")
                {
                    // GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    string[] SerialIDList = SerialID.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string[] SerialNameList = SerialName.Split(new string[] { "||@||" }, StringSplitOptions.None);

                    if (editWarehouseID == "0")
                    {
                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];
                            WarehouseID = "0";

                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, "D");
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", BatchID, "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, "D");
                            }
                        }
                    }
                    else
                    {
                        string strLoopID = "0", strSerial = "0";

                        DataRow[] result = Warehousedt.Select("SrlNo ='" + editWarehouseID + "'");
                        foreach (DataRow row in result)
                        {
                            strSerial = Convert.ToString(row["SerialID"]);
                            strLoopID = Convert.ToString(row["LoopID"]);
                        }

                        int count = 0;
                        DataRow[] dr = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        int value = (dr.Length + SerialIDList.Length - 1);

                        string[] temp_SerialIDList = new string[value];
                        string[] temp_SerialNameList = new string[value];

                        string[] check_SerialIDList = new string[value];
                        string[] check_SerialNameList = new string[value];

                        foreach (DataRow rows in dr)
                        {
                            if (strSerial != Convert.ToString(rows["SerialID"]))
                            {
                                temp_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                temp_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                check_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                check_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                count++;
                            }
                        }

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            temp_SerialIDList[count] = Convert.ToString(SerialIDList[i]);
                            temp_SerialNameList[count] = Convert.ToString(SerialNameList[i]);

                            count++;
                        }

                        DataRow[] delResult = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        foreach (DataRow delrow in delResult)
                        {
                            delrow.Delete();
                        }
                        Warehousedt.AcceptChanges();

                        SerialIDList = temp_SerialIDList;
                        SerialNameList = temp_SerialNameList;

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];
                            WarehouseID = "0";
                            string repute = "D";
                            if (check_SerialIDList.Contains(strSrlID)) repute = "I";

                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, repute);
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", BatchID, "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, repute);
                            }
                        }
                    }
                }

                //Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, "1", BatchID, BatchName, SerialID, SerialName);
                //string sortExpression = string.Format("{0} {1}", colName, direction);
                //dt.DefaultView.Sort = sortExpression;
                //Warehousedt.DefaultView.Sort = "SrlNo Asc";
                //Warehousedt = Warehousedt.DefaultView.ToTable(true);

                if (IsDelete == true)
                {
                    DataRow[] delResult = Warehousedt.Select("SrlNo ='" + editWarehouseID + "'");
                    foreach (DataRow delrow in delResult)
                    {
                        delrow.Delete();
                    }
                    Warehousedt.AcceptChanges();
                }

                Session["SI_WarehouseData"] = Warehousedt;
                changeGridOrder();

                GrdWarehouse.DataSource = Warehousedt.DefaultView;
                GrdWarehouse.DataBind();

                Session["SI_LoopWarehouse"] = loopId + 1;

                CmbWarehouse.SelectedIndex = -1;
                CmbBatch.SelectedIndex = -1;
            }
            else if (strSplitCommand == "Delete")
            {
                string strKey = e.Parameters.Split('~')[1];
                string strLoopID = "", strPreLoopID = "";

                DataTable Warehousedt = new DataTable();
                if (Session["SI_WarehouseData"] != null)
                {
                    Warehousedt = (DataTable)Session["SI_WarehouseData"];
                }

                DataRow[] result = Warehousedt.Select("SrlNo ='" + strKey + "'");
                foreach (DataRow row in result)
                {
                    strLoopID = row["LoopID"].ToString();
                }

                if (Warehousedt != null && Warehousedt.Rows.Count > 0)
                {
                    int count = 0;
                    bool IsFirst = false, IsAssign = false;
                    string WarehouseName = "", Quantity = "", BatchNo = "", SalesUOMName = "", SalesQuantity = "", StkUOMName = "", StkQuantity = "", ConversionMultiplier = "", AvailableQty = "", BalancrStk = "", MfgDate = "", ExpiryDate = "";


                    for (int i = 0; i < Warehousedt.Rows.Count; i++)
                    {
                        DataRow dr = Warehousedt.Rows[i];
                        string delSrlID = Convert.ToString(dr["SrlNo"]);
                        string delLoopID = Convert.ToString(dr["LoopID"]);

                        if (strPreLoopID != delLoopID)
                        {
                            count = 0;
                        }

                        if ((delLoopID == strLoopID) && (strKey == delSrlID) && count == 0)
                        {
                            IsFirst = true;

                            WarehouseName = Convert.ToString(dr["WarehouseName"]);
                            Quantity = Convert.ToString(dr["Quantity"]);
                            BatchNo = Convert.ToString(dr["BatchNo"]);
                            SalesUOMName = Convert.ToString(dr["SalesUOMName"]);
                            SalesQuantity = Convert.ToString(dr["SalesQuantity"]);
                            StkUOMName = Convert.ToString(dr["StkUOMName"]);
                            StkQuantity = Convert.ToString(dr["StkQuantity"]);
                            ConversionMultiplier = Convert.ToString(dr["ConversionMultiplier"]);
                            AvailableQty = Convert.ToString(dr["AvailableQty"]);
                            BalancrStk = Convert.ToString(dr["BalancrStk"]);
                            MfgDate = Convert.ToString(dr["MfgDate"]);
                            ExpiryDate = Convert.ToString(dr["ExpiryDate"]);

                            //dr.Delete();
                        }
                        else
                        {
                            if (delLoopID == strLoopID)
                            {
                                if (strKey == delSrlID)
                                {
                                    //dr.Delete();
                                }
                                else
                                {
                                    decimal S_Quantity = Convert.ToDecimal(dr["TotalQuantity"]);
                                    dr["Quantity"] = S_Quantity - 1;
                                    dr["TotalQuantity"] = S_Quantity - 1;

                                    if (IsFirst == true && IsAssign == false)
                                    {
                                        IsAssign = true;

                                        dr["WarehouseName"] = WarehouseName;
                                        dr["BatchNo"] = BatchNo;
                                        dr["SalesUOMName"] = SalesUOMName;
                                        dr["SalesQuantity"] = (S_Quantity - 1) + " " + SalesUOMName;//SalesQuantity;
                                        dr["StkUOMName"] = StkUOMName;
                                        dr["StkQuantity"] = StkQuantity;
                                        dr["ConversionMultiplier"] = ConversionMultiplier;
                                        dr["AvailableQty"] = AvailableQty;
                                        dr["BalancrStk"] = BalancrStk;
                                        dr["MfgDate"] = MfgDate;
                                        dr["ExpiryDate"] = ExpiryDate;
                                    }
                                    else
                                    {
                                        if (IsAssign == false)
                                        {
                                            IsAssign = true;
                                            SalesUOMName = Convert.ToString(dr["SalesUOMName"]);
                                            dr["SalesQuantity"] = (S_Quantity - 1) + " " + SalesUOMName;//SalesQuantity;
                                        }
                                    }
                                }
                            }
                        }

                        strPreLoopID = delLoopID;
                        count++;
                    }
                    Warehousedt.AcceptChanges();


                    for (int i = 0; i < Warehousedt.Rows.Count; i++)
                    {
                        DataRow dr = Warehousedt.Rows[i];
                        string delSrlID = Convert.ToString(dr["SrlNo"]);
                        if (strKey == delSrlID)
                        {
                            dr.Delete();
                        }
                    }
                    Warehousedt.AcceptChanges();
                }

                Session["SI_WarehouseData"] = Warehousedt;
                GrdWarehouse.DataSource = Warehousedt.DefaultView;
                GrdWarehouse.DataBind();
            }
            else if (strSplitCommand == "WarehouseDelete")
            {
                string ProductID = Convert.ToString(hdfProductSerialID.Value);
                DeleteUnsaveWarehouse(ProductID);
            }
            else if (strSplitCommand == "WarehouseFinal")
            {
                if (Session["SI_WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["SI_WarehouseData"];
                    string ProductID = Convert.ToString(hdfProductSerialID.Value);
                    string strPreLoopID = "";
                    decimal sum = 0;

                    for (int i = 0; i < Warehousedt.Rows.Count; i++)
                    {
                        DataRow dr = Warehousedt.Rows[i];
                        string delLoopID = Convert.ToString(dr["LoopID"]);
                        string Product_SrlNo = Convert.ToString(dr["Product_SrlNo"]);

                        if (ProductID == Product_SrlNo)
                        {
                            string strQuantity = (Convert.ToString(dr["SalesQuantity"]) != "") ? Convert.ToString(dr["SalesQuantity"]) : "0";
                            var weight = Decimal.Parse(Regex.Match(strQuantity, "[0-9]*\\.*[0-9]*").Value);
                            //string resultString = Regex.Match(strQuantity, @"[^0-9\.]+").Value;

                            sum = sum + Convert.ToDecimal(weight);
                        }
                    }

                    if (Convert.ToDecimal(sum) == Convert.ToDecimal(hdnProductQuantity.Value))
                    {
                        GrdWarehouse.JSProperties["cpIsSave"] = "Y";
                        for (int i = 0; i < Warehousedt.Rows.Count; i++)
                        {
                            DataRow dr = Warehousedt.Rows[i];
                            string Product_SrlNo = Convert.ToString(dr["Product_SrlNo"]);
                            if (ProductID == Product_SrlNo)
                            {
                                dr["Status"] = "I";
                            }
                        }
                        Warehousedt.AcceptChanges();
                    }
                    else
                    {
                        GrdWarehouse.JSProperties["cpIsSave"] = "N";
                    }

                    Session["SI_WarehouseData"] = Warehousedt;
                }
            }
        }
        protected void CallbackPanel_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string performpara = e.Parameter;
            if (performpara.Split('~')[0] == "EditWarehouse")
            {
                string SrlNo = performpara.Split('~')[1];
                string ProductType = Convert.ToString(hdfProductType.Value);

                if (Session["SI_WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["SI_WarehouseData"];

                    string strWarehouse = "", strBatchID = "", strSrlID = "", strQuantity = "0";
                    var rows = Warehousedt.Select(string.Format("SrlNo ='{0}'", SrlNo));
                    foreach (var dr in rows)
                    {
                        strWarehouse = (Convert.ToString(dr["WarehouseID"]) != "") ? Convert.ToString(dr["WarehouseID"]) : "0";
                        strBatchID = (Convert.ToString(dr["BatchID"]) != "") ? Convert.ToString(dr["BatchID"]) : "0";
                        strSrlID = (Convert.ToString(dr["SerialID"]) != "") ? Convert.ToString(dr["SerialID"]) : "0";
                        strQuantity = (Convert.ToString(dr["TotalQuantity"]) != "") ? Convert.ToString(dr["TotalQuantity"]) : "0";
                    }

                    //CmbWarehouse.DataSource = GetWarehouseData();
                    CmbBatch.DataSource = GetBatchData(strWarehouse);
                    CmbBatch.DataBind();

                    CallbackPanel.JSProperties["cpEdit"] = strWarehouse + "~" + strBatchID + "~" + strSrlID + "~" + strQuantity;
                }
            }
        }
        public void changeGridOrder()
        {
            string Type = "";
            GetProductType(ref Type);
            if (Type == "W")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = true;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = false;
                GrdWarehouse.Columns["MfgDate"].Visible = false;
                GrdWarehouse.Columns["ExpiryDate"].Visible = false;
                GrdWarehouse.Columns["SerialNo"].Visible = false;
            }
            else if (Type == "WB")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = true;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = true;
                GrdWarehouse.Columns["MfgDate"].Visible = true;
                GrdWarehouse.Columns["ExpiryDate"].Visible = true;
                GrdWarehouse.Columns["SerialNo"].Visible = false;
            }
            else if (Type == "WBS")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = true;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = true;
                GrdWarehouse.Columns["MfgDate"].Visible = true;
                GrdWarehouse.Columns["ExpiryDate"].Visible = true;
                GrdWarehouse.Columns["SerialNo"].Visible = true;
            }
            else if (Type == "B")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = false;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = true;
                GrdWarehouse.Columns["MfgDate"].Visible = true;
                GrdWarehouse.Columns["ExpiryDate"].Visible = true;
                GrdWarehouse.Columns["SerialNo"].Visible = false;
            }
            else if (Type == "S")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = false;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = false;
                GrdWarehouse.Columns["MfgDate"].Visible = false;
                GrdWarehouse.Columns["ExpiryDate"].Visible = false;
                GrdWarehouse.Columns["SerialNo"].Visible = true;
            }
            else if (Type == "WS")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = true;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = false;
                GrdWarehouse.Columns["MfgDate"].Visible = false;
                GrdWarehouse.Columns["ExpiryDate"].Visible = false;
                GrdWarehouse.Columns["SerialNo"].Visible = true;
            }
            else if (Type == "BS")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = false;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = true;
                GrdWarehouse.Columns["MfgDate"].Visible = true;
                GrdWarehouse.Columns["ExpiryDate"].Visible = true;
                GrdWarehouse.Columns["SerialNo"].Visible = true;
            }
        }
        public void GetTotalStock(ref string Trans_Stock, string WarehouseID)
        {
            string ProductID = Convert.ToString(hdfProductID.Value);

            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "GetBatchDetails");
            proc.AddVarcharPara("@ProductID", 100, Convert.ToString(ProductID));
            proc.AddVarcharPara("@WarehouseID", 100, Convert.ToString(WarehouseID));
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 500, Convert.ToString(Session["userbranchID"]));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            DataTable dt = proc.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                Trans_Stock = Convert.ToString(dt.Rows[0]["Trans_Stock"]);
            }
        }
        public void GetProductUOM(ref string Sales_UOM_Name, ref string Sales_UOM_Code, ref string Stk_UOM_Name, ref string Stk_UOM_Code, ref string Conversion_Multiplier, string ProductID)
        {
            DataTable Productdt = GetProductDetailsData(ProductID);
            if (Productdt != null && Productdt.Rows.Count > 0)
            {
                Sales_UOM_Name = Convert.ToString(Productdt.Rows[0]["Sales_UOM_Name"]);
                Sales_UOM_Code = Convert.ToString(Productdt.Rows[0]["Sales_UOM_Code"]);
                Stk_UOM_Name = Convert.ToString(Productdt.Rows[0]["Stk_UOM_Name"]);
                Stk_UOM_Code = Convert.ToString(Productdt.Rows[0]["Stk_UOM_Code"]);
                Conversion_Multiplier = Convert.ToString(Productdt.Rows[0]["Conversion_Multiplier"]);
            }
        }
        public void GetBatchDetails(ref string MfgDate, ref string ExpiryDate, string BatchID)
        {
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "GetBatchDetails");
            proc.AddVarcharPara("@BatchID", 100, Convert.ToString(BatchID));
            DataTable Batchdt = proc.GetTable();

            if (Batchdt != null && Batchdt.Rows.Count > 0)
            {
                MfgDate = Convert.ToString(Batchdt.Rows[0]["MfgDate"]);
                ExpiryDate = Convert.ToString(Batchdt.Rows[0]["ExpiryDate"]);
            }
        }
        public void GetProductType(ref string Type)
        {
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "GetSchemeType");
            proc.AddVarcharPara("@ProductID", 100, Convert.ToString(hdfProductID.Value));
            DataTable dt = proc.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                Type = Convert.ToString(dt.Rows[0]["Type"]);
            }
        }
        protected void GrdWarehouse_DataBinding(object sender, EventArgs e)
        {
            if (Session["SI_WarehouseData"] != null)
            {
                string Type = "";
                GetProductType(ref Type);
                string SerialID = Convert.ToString(hdfProductSerialID.Value);
                DataTable Warehousedt = (DataTable)Session["SI_WarehouseData"];

                if (Warehousedt != null && Warehousedt.Rows.Count > 0)
                {
                    DataView dvData = new DataView(Warehousedt);
                    dvData.RowFilter = "Product_SrlNo = '" + SerialID + "'";

                    GrdWarehouse.DataSource = dvData;
                }
                else
                {
                    GrdWarehouse.DataSource = Warehousedt.DefaultView;
                }
            }
        }
        [WebMethod]
        public static string getSchemeType(string Products_ID)
        {
            string Type = "";
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "GetSchemeType");
            proc.AddVarcharPara("@ProductID", 100, Convert.ToString(Products_ID));
            DataTable dt = proc.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                Type = Convert.ToString(dt.Rows[0]["Type"]);
            }

            return Convert.ToString(Type);
        }
        public void GetQuantityBaseOnProduct(string strProductSrlNo, ref decimal WarehouseQty)
        {
            decimal sum = 0;

            DataTable Warehousedt = new DataTable();
            if (Session["SI_WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["SI_WarehouseData"];
                for (int i = 0; i < Warehousedt.Rows.Count; i++)
                {
                    DataRow dr = Warehousedt.Rows[i];
                    string Product_SrlNo = Convert.ToString(dr["Product_SrlNo"]);

                    if (strProductSrlNo == Product_SrlNo)
                    {
                        string strQuantity = (Convert.ToString(dr["SalesQuantity"]) != "") ? Convert.ToString(dr["SalesQuantity"]) : "0";
                        var weight = Decimal.Parse(Regex.Match(strQuantity, "[0-9]*\\.*[0-9]*").Value);

                        sum = sum + Convert.ToDecimal(weight);
                    }
                }
            }

            WarehouseQty = sum;
        }
        public void DeleteWarehouse(string SrlNo)
        {
            DataTable Warehousedt = new DataTable();
            if (Session["SI_WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["SI_WarehouseData"];

                var rows = Warehousedt.Select(string.Format("Product_SrlNo ='{0}'", SrlNo));
                foreach (var row in rows)
                {
                    row.Delete();
                }
                Warehousedt.AcceptChanges();

                Session["SI_WarehouseData"] = Warehousedt;
            }
        }
        public void DeleteUnsaveWarehouse(string SrlNo)
        {
            DataTable Warehousedt = new DataTable();
            if (Session["SI_WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["SI_WarehouseData"];

                var rows = Warehousedt.Select("Product_SrlNo ='" + SrlNo + "' AND Status='D'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
                Warehousedt.AcceptChanges();

                Session["SI_WarehouseData"] = Warehousedt;
            }
        }
        public DataTable DeleteWarehouseBySrl(string strKey)
        {
            string strLoopID = "", strPreLoopID = "";

            DataTable Warehousedt = new DataTable();
            if (Session["SI_WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["SI_WarehouseData"];
            }

            DataRow[] result = Warehousedt.Select("SrlNo ='" + strKey + "'");
            foreach (DataRow row in result)
            {
                strLoopID = row["LoopID"].ToString();
            }

            if (Warehousedt != null && Warehousedt.Rows.Count > 0)
            {
                int count = 0;
                bool IsFirst = false, IsAssign = false;
                string WarehouseName = "", Quantity = "", BatchNo = "", SalesUOMName = "", SalesQuantity = "", StkUOMName = "", StkQuantity = "", ConversionMultiplier = "", AvailableQty = "", BalancrStk = "", MfgDate = "", ExpiryDate = "";


                for (int i = 0; i < Warehousedt.Rows.Count; i++)
                {
                    DataRow dr = Warehousedt.Rows[i];
                    string delSrlID = Convert.ToString(dr["SrlNo"]);
                    string delLoopID = Convert.ToString(dr["LoopID"]);

                    if (strPreLoopID != delLoopID)
                    {
                        count = 0;
                    }

                    if ((delLoopID == strLoopID) && (strKey == delSrlID) && count == 0)
                    {
                        IsFirst = true;

                        WarehouseName = Convert.ToString(dr["WarehouseName"]);
                        Quantity = Convert.ToString(dr["Quantity"]);
                        BatchNo = Convert.ToString(dr["BatchNo"]);
                        SalesUOMName = Convert.ToString(dr["SalesUOMName"]);
                        SalesQuantity = Convert.ToString(dr["SalesQuantity"]);
                        StkUOMName = Convert.ToString(dr["StkUOMName"]);
                        StkQuantity = Convert.ToString(dr["StkQuantity"]);
                        ConversionMultiplier = Convert.ToString(dr["ConversionMultiplier"]);
                        AvailableQty = Convert.ToString(dr["AvailableQty"]);
                        BalancrStk = Convert.ToString(dr["BalancrStk"]);
                        MfgDate = Convert.ToString(dr["MfgDate"]);
                        ExpiryDate = Convert.ToString(dr["ExpiryDate"]);

                        //dr.Delete();
                    }
                    else
                    {
                        if (delLoopID == strLoopID)
                        {
                            if (strKey == delSrlID)
                            {
                                //dr.Delete();
                            }
                            else
                            {
                                int S_Quantity = Convert.ToInt32(dr["TotalQuantity"]);
                                dr["Quantity"] = S_Quantity - 1;
                                dr["TotalQuantity"] = S_Quantity - 1;

                                if (IsFirst == true && IsAssign == false)
                                {
                                    IsAssign = true;

                                    dr["WarehouseName"] = WarehouseName;
                                    dr["BatchNo"] = BatchNo;
                                    dr["SalesUOMName"] = SalesUOMName;
                                    dr["SalesQuantity"] = (S_Quantity - 1) + " " + SalesUOMName;//SalesQuantity;
                                    dr["StkUOMName"] = StkUOMName;
                                    dr["StkQuantity"] = StkQuantity;
                                    dr["ConversionMultiplier"] = ConversionMultiplier;
                                    dr["AvailableQty"] = AvailableQty;
                                    dr["BalancrStk"] = BalancrStk;
                                    dr["MfgDate"] = MfgDate;
                                    dr["ExpiryDate"] = ExpiryDate;
                                }
                                else
                                {
                                    if (IsAssign == false)
                                    {
                                        IsAssign = true;
                                        SalesUOMName = Convert.ToString(dr["SalesUOMName"]);
                                        dr["SalesQuantity"] = (S_Quantity - 1) + " " + SalesUOMName;//SalesQuantity;
                                    }
                                }
                            }
                        }
                    }

                    strPreLoopID = delLoopID;
                    count++;
                }
                Warehousedt.AcceptChanges();


                for (int i = 0; i < Warehousedt.Rows.Count; i++)
                {
                    DataRow dr = Warehousedt.Rows[i];
                    string delSrlID = Convert.ToString(dr["SrlNo"]);
                    if (strKey == delSrlID)
                    {
                        dr.Delete();
                    }
                }
                Warehousedt.AcceptChanges();
            }

            return Warehousedt;
        }
        public void UpdateWarehouse(string oldSrlNo, string newSrlNo)
        {
            DataTable Warehousedt = new DataTable();
            if (Session["SI_WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["SI_WarehouseData"];

                for (int i = 0; i < Warehousedt.Rows.Count; i++)
                {
                    DataRow dr = Warehousedt.Rows[i];
                    string Product_SrlNo = Convert.ToString(dr["Product_SrlNo"]);
                    if (oldSrlNo == Product_SrlNo)
                    {
                        dr["Product_SrlNo"] = newSrlNo;
                    }
                }
                Warehousedt.AcceptChanges();

                Session["SI_WarehouseData"] = Warehousedt;
            }
        }
        protected void acpAvailableStock_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string performpara = e.Parameter;
            string strProductID = Convert.ToString(performpara.Split('~')[0]);
            string strBranch = Convert.ToString(ddl_Branch.SelectedValue);
            acpAvailableStock.JSProperties["cpstock"] = "0.00";

            try
            {
                DataTable dt2 = oDBEngine.GetDataTable("Select dbo.fn_CheckAvailableQuotation(" + strBranch + ",'" + Convert.ToString(Session["LastCompany"]) + "','" + Convert.ToString(Session["LastFinYear"]) + "'," + strProductID + ") as branchopenstock");

                if (dt2.Rows.Count > 0)
                {
                    acpAvailableStock.JSProperties["cpstock"] = Convert.ToString(Math.Round(Convert.ToDecimal(dt2.Rows[0]["branchopenstock"]), 2));
                }
                else
                {
                    acpAvailableStock.JSProperties["cpstock"] = "0.00";
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #endregion

        #region Unique Code Generated Section Start

        public string checkNMakeJVCode(string manual_str, int sel_schema_Id)
        {

            //oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);

            oDBEngine = new BusinessLogicLayer.DBEngine();

            DataTable dtSchema = new DataTable();
            DataTable dtC = new DataTable();
            string prefCompCode = string.Empty, sufxCompCode = string.Empty, startNo, paddedStr, sqlQuery = string.Empty;
            int EmpCode, prefLen, sufxLen, paddCounter;

            if (sel_schema_Id > 0)
            {
                dtSchema = oDBEngine.GetDataTable("tbl_master_idschema", "prefix, suffix, digit, startno, schema_type", "id=" + sel_schema_Id);
                int scheme_type = Convert.ToInt32(dtSchema.Rows[0]["schema_type"]);

                if (scheme_type != 0)
                {
                    startNo = dtSchema.Rows[0]["startno"].ToString();
                    paddCounter = Convert.ToInt32(dtSchema.Rows[0]["digit"]);
                    paddedStr = startNo.PadLeft(paddCounter, '0');
                    prefCompCode = dtSchema.Rows[0]["prefix"].ToString();
                    sufxCompCode = dtSchema.Rows[0]["suffix"].ToString();
                    prefLen = Convert.ToInt32(prefCompCode.Length);
                    sufxLen = Convert.ToInt32(sufxCompCode.Length);

                    sqlQuery = "SELECT max(tjv.Invoice_Number) FROM tbl_trans_SalesInvoice tjv WHERE dbo.RegexMatch('";
                    if (prefLen > 0)
                        sqlQuery += "^[" + prefCompCode + "]{" + prefLen + "}";
                    else if (scheme_type == 2)
                        sqlQuery += "^";
                    sqlQuery += "[0-9]{" + paddCounter + "}";
                    if (sufxLen > 0)
                        sqlQuery += "[" + sufxCompCode + "]{" + sufxLen + "}";
                    //sqlQuery += "?$', LTRIM(RTRIM(tjv.Invoice_Number))) = 1";
                    sqlQuery += "?$', LTRIM(RTRIM(tjv.Invoice_Number))) = 1 and Invoice_Number like '" + prefCompCode + "%'";
                    if (scheme_type == 2)
                        sqlQuery += " AND CONVERT(DATE, Invoice_Date) = CONVERT(DATE, GETDATE())";
                    dtC = oDBEngine.GetDataTable(sqlQuery);

                    if (dtC.Rows[0][0].ToString() == "")
                    {
                        sqlQuery = "SELECT max(tjv.Invoice_Number) FROM tbl_trans_SalesInvoice tjv WHERE dbo.RegexMatch('";
                        if (prefLen > 0)
                            sqlQuery += "^[" + prefCompCode + "]{" + prefLen + "}";
                        else if (scheme_type == 2)
                            sqlQuery += "^";
                        sqlQuery += "[0-9]{" + (paddCounter - 1) + "}";
                        if (sufxLen > 0)
                            sqlQuery += "[" + sufxCompCode + "]{" + sufxLen + "}";
                        //sqlQuery += "?$', LTRIM(RTRIM(tjv.Invoice_Number))) = 1";
                        sqlQuery += "?$', LTRIM(RTRIM(tjv.Invoice_Number))) = 1 and Invoice_Number like '" + prefCompCode + "%'";
                        if (scheme_type == 2)
                            sqlQuery += " AND CONVERT(DATE, Invoice_Date) = CONVERT(DATE, GETDATE())";
                        dtC = oDBEngine.GetDataTable(sqlQuery);
                    }

                    if (dtC.Rows.Count > 0 && dtC.Rows[0][0].ToString().Trim() != "")
                    {
                        string uccCode = dtC.Rows[0][0].ToString().Trim();
                        int UCCLen = uccCode.Length;
                        int decimalPartLen = UCCLen - (prefCompCode.Length + sufxCompCode.Length);
                        string uccCodeSubstring = uccCode.Substring(prefCompCode.Length, decimalPartLen);
                        EmpCode = Convert.ToInt32(uccCodeSubstring) + 1;
                        // out of range journal scheme
                        if (EmpCode.ToString().Length > paddCounter)
                        {
                            return "outrange";
                        }
                        else
                        {
                            paddedStr = EmpCode.ToString().PadLeft(paddCounter, '0');
                            UniqueQuotation = prefCompCode + paddedStr + sufxCompCode;
                            return "ok";
                        }
                    }
                    else
                    {
                        UniqueQuotation = startNo.PadLeft(paddCounter, '0');
                        UniqueQuotation = prefCompCode + paddedStr + sufxCompCode;
                        return "ok";
                    }
                }
                else
                {
                    sqlQuery = "SELECT Invoice_Number FROM tbl_trans_SalesInvoice WHERE Invoice_Number LIKE '" + manual_str.Trim() + "'";
                    dtC = oDBEngine.GetDataTable(sqlQuery);
                    // duplicate manual entry check
                    if (dtC.Rows.Count > 0 && dtC.Rows[0][0].ToString().Trim() != "")
                    {
                        return "duplicate";
                    }

                    UniqueQuotation = manual_str.Trim();
                    return "ok";
                }
            }
            else
            {
                return "noid";
            }
        }

        #endregion Unique Code Generated Section End

        #region Product Tax

        public void setValueForHeaderGST(ASPxComboBox aspxcmb, string taxId)
        {
            for (int i = 0; i < aspxcmb.Items.Count; i++)
            {
                if (Convert.ToString(aspxcmb.Items[i].Value).Split('~')[0] == taxId.Split('~')[0])
                {
                    aspxcmb.Items[i].Selected = true;
                    break;
                }
            }

        }
        protected DataTable GetTaxDataWithGST(DataTable existing)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
            proc.AddVarcharPara("@Action", 500, "TaxDetailsForGst");
            proc.AddVarcharPara("@InvoiceID", 500, Convert.ToString(Session["SI_InvoiceID"]));
            dt = proc.GetTable();
            if (dt.Rows.Count > 0)
            {
                DataRow gstRow = existing.NewRow();
                gstRow["Taxes_ID"] = 0;
                gstRow["Taxes_Name"] = dt.Rows[0]["TaxRatesSchemeName"];
                gstRow["Percentage"] = dt.Rows[0]["QuoteTax_Percentage"];
                gstRow["Amount"] = dt.Rows[0]["QuoteTax_Amount"];
                gstRow["AltTax_Code"] = dt.Rows[0]["Gst"];

                UpdateGstForCharges(Convert.ToString(dt.Rows[0]["Gst"]));
                txtGstCstVatCharge.Value = gstRow["Amount"];
                existing.Rows.Add(gstRow);
            }
            SetTotalCharges(existing);
            return existing;
        }
        public void SetTotalCharges(DataTable taxTableFinal)
        {
            decimal totalCharges = 0;
            foreach (DataRow dr in taxTableFinal.Rows)
            {
                if (Convert.ToString(dr["Taxes_ID"]) != "0")
                {
                    if (Convert.ToString(dr["Taxes_Name"]).Contains("(+)"))
                    {
                        totalCharges += Convert.ToDecimal(dr["Amount"]);
                    }
                    else
                    {
                        totalCharges -= Convert.ToDecimal(dr["Amount"]);
                    }
                }
                else
                {//Else part For Gst 
                    totalCharges += Convert.ToDecimal(dr["Amount"]);
                }
            }
            txtQuoteTaxTotalAmt.Value = totalCharges;

        }
        protected void UpdateGstForCharges(string data)
        {
            for (int i = 0; i < cmbGstCstVatcharge.Items.Count; i++)
            {
                if (Convert.ToString(cmbGstCstVatcharge.Items[i].Value).Split('~')[0] == data)
                {
                    cmbGstCstVatcharge.Items[i].Selected = true;
                    break;
                }
            }
        }
        public void GetStock(string strProductID)
        {
            string strBranch = Convert.ToString(ddl_Branch.SelectedValue);
            acpAvailableStock.JSProperties["cpstock"] = "0.00";

            try
            {
                DataTable dt2 = oDBEngine.GetDataTable("Select dbo.fn_CheckAvailableQuotation(" + strBranch + ",'" + Convert.ToString(Session["LastCompany"]) + "','" + Convert.ToString(Session["LastFinYear"]) + "'," + strProductID + ") as branchopenstock");

                if (dt2.Rows.Count > 0)
                {
                    taxUpdatePanel.JSProperties["cpstock"] = Convert.ToString(Math.Round(Convert.ToDecimal(dt2.Rows[0]["branchopenstock"]), 2));
                }
                else
                {
                    taxUpdatePanel.JSProperties["cpstock"] = "0.00";
                }
            }
            catch (Exception ex)
            {
            }
        }
        protected void taxUpdatePanel_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {

            string performpara = e.Parameter;
            if (performpara.Split('~')[0] == "DelProdbySl")
            {
                DataTable MainTaxDataTable = (DataTable)Session["SI_FinalTaxRecord"];

                DataRow[] deletedRow = MainTaxDataTable.Select("SlNo=" + performpara.Split('~')[1]);
                if (deletedRow.Length > 0)
                {
                    foreach (DataRow dr in deletedRow)
                    {
                        MainTaxDataTable.Rows.Remove(dr);
                    }

                }

                Session["SI_FinalTaxRecord"] = MainTaxDataTable;
                GetStock(Convert.ToString(performpara.Split('~')[2]));
                DeleteWarehouse(Convert.ToString(performpara.Split('~')[1]));
                DataTable taxDetails = (DataTable)Session["SI_TaxDetails"];
                if (taxDetails != null)
                {
                    foreach (DataRow dr in taxDetails.Rows)
                    {
                        dr["Amount"] = "0.00";
                    }
                    Session["SI_TaxDetails"] = taxDetails;
                }
            }
            else if (performpara.Split('~')[0] == "DeleteAllTax")
            {
                CreateDataTaxTable();

                DataTable taxDetails = (DataTable)Session["SI_TaxDetails"];

                if (taxDetails != null)
                {
                    foreach (DataRow dr in taxDetails.Rows)
                    {
                        dr["Amount"] = "0.00";
                    }
                    Session["SI_TaxDetails"] = taxDetails;
                }
            }
            else
            {
                DataTable MainTaxDataTable = (DataTable)Session["SI_FinalTaxRecord"];
                DataRow[] deletedRow = MainTaxDataTable.Select("SlNo=" + performpara.Split('~')[1]);
                if (deletedRow.Length > 0)
                {
                    foreach (DataRow dr in deletedRow)
                    {
                        MainTaxDataTable.Rows.Remove(dr);
                    }

                }
                Session["SI_FinalTaxRecord"] = MainTaxDataTable;


                DataTable taxDetails = (DataTable)Session["SI_TaxDetails"];
                if (taxDetails != null)
                {
                    foreach (DataRow dr in taxDetails.Rows)
                    {
                        dr["Amount"] = "0.00";
                    }
                    Session["SI_TaxDetails"] = taxDetails;
                }
            }
        }
        public double ReCalculateTaxAmount(string slno, double amount)
        {
            DataTable MainTaxDataTable = (DataTable)Session["SI_FinalTaxRecord"];
            double totalSum = 0.0;
            //Get The Existing datatable
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "PopulateAllTax");
            DataTable TaxDt = proc.GetTable();

            DataRow[] filterRow = MainTaxDataTable.Select("SlNo=" + slno);

            if (filterRow.Length > 0)
            {
                foreach (DataRow dr in filterRow)
                {
                    if (Convert.ToString(dr["TaxCode"]) != "0")
                    {
                        DataRow[] taxrow = TaxDt.Select("Taxes_ID=" + dr["TaxCode"]);
                        if (taxrow.Length > 0)
                        {
                            if (taxrow[0]["TaxCalculateMethods"] == "A")
                            {
                                dr["Amount"] = (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                                totalSum += (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                            }
                            else
                            {
                                dr["Amount"] = (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                                totalSum -= (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                            }
                        }
                    }
                    else
                    {
                        DataRow[] taxrow = TaxDt.Select("Taxes_ID=" + dr["AltTaxCode"]);
                        if (taxrow.Length > 0)
                        {
                            if (taxrow[0]["TaxCalculateMethods"] == "A")
                            {
                                dr["Amount"] = (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                                totalSum += (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                            }
                            else
                            {
                                dr["Amount"] = (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                                totalSum -= (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                            }
                        }
                    }
                }

            }
            Session["SI_FinalTaxRecord"] = MainTaxDataTable;

            return totalSum;

        }
        public void PopulateGSTCSTVATCombo(string quoteDate)
        {
            string LastCompany = "";
            if (Convert.ToString(Session["LastCompany"]) != null)
            {
                LastCompany = Convert.ToString(Session["LastCompany"]);
            }
            //DataTable dt = new DataTable();
            //dt = objCRMSalesDtlBL.PopulateGSTCSTVATCombo();
            //DataTable DT = oDBEngine.GetDataTable("select cast(td.TaxRates_ID as varchar(5))+'~'+ cast (td.TaxRates_Rate as varchar(25)) 'Taxes_ID',td.TaxRatesSchemeName 'Taxes_Name',th.Taxes_Name as 'TaxCodeName' from Master_Taxes th inner join Config_TaxRates td on th.Taxes_ID=td.TaxRates_TaxCode where (td.TaxRates_Country=0 or td.TaxRates_Country=(select add_country from tbl_master_address where add_cntId ='" + Convert.ToString(Session["LastCompany"]) + "' ))  and th.Taxes_ApplicableFor in ('B','S') and th.TaxTypeCode in('G','V','C')");

            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "LoadGSTCSTVATCombo");
            proc.AddVarcharPara("@S_QuoteAdd_CompanyID", 10, Convert.ToString(LastCompany));
            proc.AddVarcharPara("@S_quoteDate", 10, quoteDate);
            proc.AddIntegerPara("@branchId", Convert.ToInt32(Session["userbranchID"]));
            DataTable DT = proc.GetTable();
            cmbGstCstVat.DataSource = DT;
            cmbGstCstVat.TextField = "Taxes_Name";
            cmbGstCstVat.ValueField = "Taxes_ID";
            cmbGstCstVat.DataBind();
        }
        public void CreateDataTaxTable()
        {
            DataTable TaxRecord = new DataTable();

            TaxRecord.Columns.Add("SlNo", typeof(System.Int32));
            TaxRecord.Columns.Add("TaxCode", typeof(System.String));
            TaxRecord.Columns.Add("AltTaxCode", typeof(System.String));
            TaxRecord.Columns.Add("Percentage", typeof(System.Decimal));
            TaxRecord.Columns.Add("Amount", typeof(System.Decimal));
            Session["SI_FinalTaxRecord"] = TaxRecord;
        }

        //public IEnumerable GetTaxCode()
        //{
        //    List<taxCode> TaxList = new List<taxCode>();
        //    BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
        //    // DataTable DT = objEngine.GetDataTable("select Taxes_ID,Taxes_Name from dbo.Master_Taxes");
        //    DataTable DT = objEngine.GetDataTable("select cast(th.Taxes_ID as varchar(5))+'~'+ cast (td.TaxRates_Rate as varchar(25)) 'Taxes_ID',th.Taxes_Name 'Taxes_Name' from Master_Taxes th inner join Config_TaxRates td on th.Taxes_ID=td.TaxRates_TaxCode where (td.TaxRates_Country=0 or td.TaxRates_Country=(select add_country from tbl_master_address where add_cntId ='" + Convert.ToString(Session["LastCompany"]) + "' ))  and th.Taxes_ApplicableFor in ('B','S')");


        //    for (int i = 0; i < DT.Rows.Count; i++)
        //    {
        //        taxCode tax = new taxCode();
        //        tax.Taxes_ID = Convert.ToString(DT.Rows[i]["Taxes_ID"]);
        //        tax.Taxes_Name = Convert.ToString(DT.Rows[i]["Taxes_Name"]);
        //        TaxList.Add(tax);
        //    }

        //    return TaxList;
        //}

        public string GetTaxName(int id)
        {
            string taxName = "";
            string[] arr = oDBEngine.GetFieldValue1("Master_taxes", "Taxes_Name", "Taxes_ID=" + Convert.ToString(id), 1);
            if (arr[0] != "n")
            {
                taxName = arr[0];
            }
            return taxName;
        }
        public DataSet GetQuotationTaxData()
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "ProductTaxDetails");
            proc.AddVarcharPara("@QuotationID", 500, Convert.ToString(Session["SI_InvoiceID"]));
            ds = proc.GetDataSet();
            return ds;
        }
        public DataSet GetQuotationEditedTaxData()
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
            proc.AddVarcharPara("@Action", 500, "ProductEditedTaxDetails");
            proc.AddVarcharPara("@InvoiceID", 500, Convert.ToString(Session["SI_InvoiceID"]));
            ds = proc.GetDataSet();
            return ds;
        }
        public DataTable GetComponentEditedTaxData(string ComponentDetailsIDs, string strType)
        {
            DataTable ds = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
            proc.AddVarcharPara("@Action", 500, "ComponentProductTax");
            proc.AddVarcharPara("@SelectedComponentList", 500, ComponentDetailsIDs);
            proc.AddVarcharPara("@ComponentType", 500, strType);
            ds = proc.GetTable();
            return ds;
        }
        public DataTable GetComponentEditedAddressData(string ComponentDetailsIDs, string strType)
        {
            DataTable ds = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
            proc.AddVarcharPara("@Action", 500, "ComponentBillingAddress");
            proc.AddVarcharPara("@SelectedComponentList", 500, ComponentDetailsIDs);
            proc.AddVarcharPara("@ComponentType", 500, strType);
            ds = proc.GetTable();
            return ds;
        }
        public double GetTotalTaxAmount(List<TaxDetails> tax)
        {
            double sum = 0;
            foreach (TaxDetails td in tax)
            {
                if (td.Taxes_Name.Substring(td.Taxes_Name.Length - 3, 3) == "(+)")
                    sum += td.Amount;
                else
                    sum -= td.Amount;

            }
            return sum;
        }
        protected void cgridTax_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string retMsg = "";
            if (e.Parameters.Split('~')[0] == "SaveGST")
            {
                DataTable TaxRecord = (DataTable)Session["SI_FinalTaxRecord"];
                int slNo = Convert.ToInt32(HdSerialNo.Value);
                //For GST/CST/VAT
                if (cmbGstCstVat.Value != null)
                {

                    DataRow[] finalRow = TaxRecord.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode='0'");
                    if (finalRow.Length > 0)
                    {
                        finalRow[0]["Percentage"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[1];
                        finalRow[0]["Amount"] = txtGstCstVat.Text;
                        finalRow[0]["AltTaxCode"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[0];

                    }
                    else
                    {
                        DataRow newRowGST = TaxRecord.NewRow();
                        newRowGST["slNo"] = slNo;
                        newRowGST["Percentage"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[1];
                        newRowGST["TaxCode"] = "0";
                        newRowGST["AltTaxCode"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[0];
                        newRowGST["Amount"] = txtGstCstVat.Text;
                        TaxRecord.Rows.Add(newRowGST);
                    }
                }
                //End Here

                aspxGridTax.JSProperties["cpUpdated"] = "";

                Session["SI_FinalTaxRecord"] = TaxRecord;
            }
            else
            {
                #region fetch All data For Tax

                DataTable taxDetail = new DataTable();
                DataTable MainTaxDataTable = (DataTable)Session["SI_FinalTaxRecord"];
                DataTable databaseReturnTable = (DataTable)Session["SI_QuotationTaxDetails"];

                //if (Convert.ToInt32(e.Parameters.Split('~')[1]) == 1)
                //    taxDetail = oDBEngine.GetDataTable("select TaxRates_ID as Taxes_ID,TaxRatesSchemeName as Taxes_Name,TaxCalculateMethods,tm.Taxes_Name as taxCodeName,tm.Taxes_ApplicableOn as 'ApplicableOn',isnull((select TaxRatesSchemeName from Config_TaxRates where TaxRates_ID=tm.Taxes_OtherTax),'')  as dependOn from Master_taxes tm inner join Config_TaxRates ct on tm.Taxes_ID=ct.TaxRates_TaxCode where tm.TaxTypeCode not in('G','V','C') and tm.TaxItemlevel='Y'");
                //else if (Convert.ToInt32(e.Parameters.Split('~')[1]) == 2)
                //taxDetail = oDBEngine.GetDataTable("select TaxRates_ID as Taxes_ID,TaxRatesSchemeName as Taxes_Name,TaxCalculateMethods,tm.Taxes_Name as taxCodeName,tm.Taxes_ApplicableOn as 'ApplicableOn',isnull((select TaxRatesSchemeName from Config_TaxRates where TaxRates_ID=tm.Taxes_OtherTax),'')  as dependOn from Master_taxes tm inner join Config_TaxRates ct on tm.Taxes_ID=ct.TaxRates_TaxCode where tm.TaxTypeCode not in('G','V','C') and tm.TaxItemlevel='Y'");

                //ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
                //proc.AddVarcharPara("@Action", 500, "LoadOtherTaxDetails");
                //proc.AddVarcharPara("@ProductID", 10, Convert.ToString(setCurrentProdCode.Value));
                //proc.AddVarcharPara("@S_quoteDate", 10, dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                //taxDetail = proc.GetTable();

                ProcedureExecute proc = new ProcedureExecute("prc_TaxExceptionFind");
                proc.AddVarcharPara("@Action", 500, "SQ");
                proc.AddVarcharPara("@ProductID", 10, Convert.ToString(setCurrentProdCode.Value));
                proc.AddVarcharPara("@ENTITY_ID", 100, hdnCustomerId.Value);
                proc.AddVarcharPara("@Date", 10, dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                proc.AddVarcharPara("@Amount", 100, HdProdGrossAmt.Value);
                proc.AddVarcharPara("@Qty", 100, hdnQty.Value);
                taxDetail = proc.GetTable();

                //Get Company Gstin 09032017
                string CompInternalId = Convert.ToString(Session["LastCompany"]);
                string[] compGstin = oDBEngine.GetFieldValue1("tbl_master_company", "cmp_gstin", "cmp_internalid='" + CompInternalId + "'", 1);

                //Get BranchStateCode
                string BranchStateCode = "", BranchGSTIN = "";
                DataTable BranchTable = oDBEngine.GetDataTable("select StateCode,branch_GSTIN   from tbl_master_branch branch inner join tbl_master_state st on branch.branch_state=st.id where branch_id=" + Convert.ToString(ddl_Branch.SelectedValue));
                if (BranchTable != null)
                {
                    BranchStateCode = Convert.ToString(BranchTable.Rows[0][0]);
                    BranchGSTIN = Convert.ToString(BranchTable.Rows[0][1]);
                }


                string ShippingState = "";

                #region ##### Added By : Samrat Roy -- For BillingShippingUserControl ######
                string sstateCode = "";

                if (ddl_PosGst.Value.ToString() == "S")
                {
                    sstateCode = Sales_BillingShipping.GeteShippingStateCode();
                }
                else
                {
                    sstateCode = Sales_BillingShipping.GetBillingStateCode();
                }
                ShippingState = sstateCode;
                if (ShippingState.Trim() != "")
                {
                    ShippingState = ShippingState;
                }
                #region ##### Old Code -- BillingShipping ######
                ////if (chkBilling.Checked)
                ////{
                ////    if (CmbState.Value != null)
                ////    {
                ////        ShippingState = CmbState.Text;
                ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                ////    }
                ////}
                ////else
                ////{
                ////    if (CmbState1.Value != null)
                ////    {
                ////        ShippingState = CmbState1.Text;
                ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                ////    }
                ////}

                #endregion

                #endregion



                if (ShippingState.Trim() != "" && BranchStateCode != "")
                {


                    if (BranchStateCode == ShippingState)
                    {
                        //Check if the state is in union territories then only UTGST will apply
                        //   Chandigarh     Andaman and Nicobar Islands    DADRA & NAGAR HAVELI    DAMAN & DIU      Lakshadweep              PONDICHERRY
                        if (ShippingState == "4" || ShippingState == "26" || ShippingState == "25" || ShippingState == "35" || ShippingState == "31" || ShippingState == "34")
                        {
                            foreach (DataRow dr in taxDetail.Rows)
                            {
                                if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                                {
                                    dr.Delete();
                                }
                            }

                        }
                        else
                        {
                            foreach (DataRow dr in taxDetail.Rows)
                            {
                                if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                                {
                                    dr.Delete();
                                }
                            }
                        }
                        taxDetail.AcceptChanges();
                    }
                    else
                    {
                        foreach (DataRow dr in taxDetail.Rows)
                        {
                            if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                            {
                                dr.Delete();
                            }
                        }
                        taxDetail.AcceptChanges();

                    }

                }

                //If Company GSTIN is blank then Delete All CGST,UGST,IGST,CGST
                if (compGstin[0].Trim() == "" && BranchGSTIN == "")
                {
                    foreach (DataRow dr in taxDetail.Rows)
                    {
                        if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST")
                        {
                            dr.Delete();
                        }
                    }
                    taxDetail.AcceptChanges();
                }


                ////Check If any TaxScheme Set Against that Product Then update there rate 22-03-2017 and rate
                //string[] schemeIDViaProdID = oDBEngine.GetFieldValue1("master_sproducts", "isnull(sProduct_TaxSchemeSale,0)sProduct_TaxSchemeSale", "sProducts_ID='" + Convert.ToString(setCurrentProdCode.Value) + "'", 1);
                ////&& schemeIDViaProdID[0] != ""
                //if (schemeIDViaProdID.Length > 0)
                //{

                //    if (taxDetail.Select("Taxes_ID='" + schemeIDViaProdID[0] + "'").Length > 0)
                //    {
                //        foreach (DataRow dr in taxDetail.Rows)
                //        {
                //            if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST")
                //            {
                //                if (Convert.ToString(dr["Taxes_ID"]).Trim() != schemeIDViaProdID[0].Trim())
                //                    dr["TaxRates_Rate"] = 0;
                //            }
                //        }
                //    }
                //}



                int slNo = Convert.ToInt32(HdSerialNo.Value);

                //Get Gross Amount and Net Amount 
                decimal ProdGrossAmt = Convert.ToDecimal(HdProdGrossAmt.Value);
                decimal ProdNetAmt = Convert.ToDecimal(HdProdNetAmt.Value);

                List<TaxDetails> TaxDetailsDetails = new List<TaxDetails>();

                //Debjyoti 09032017
                decimal totalParcentage = 0;
                foreach (DataRow dr in taxDetail.Rows)
                {
                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                    {
                        totalParcentage += Convert.ToDecimal(dr["TaxRates_Rate"]);
                    }
                }

                if (e.Parameters.Split('~')[0] == "New")
                {
                    foreach (DataRow dr in taxDetail.Rows)
                    {
                        TaxDetails obj = new TaxDetails();
                        obj.Taxes_ID = Convert.ToInt32(dr["Taxes_ID"]);
                        obj.taxCodeName = Convert.ToString(dr["taxCodeName"]);
                        obj.TaxField = Convert.ToString(dr["TaxRates_Rate"]);
                        obj.Amount = 0.0;

                        #region set calculated on
                        //Check Tax Applicable on and set to calculated on
                        if (Convert.ToString(dr["ApplicableOn"]) == "G")
                        {
                            obj.calCulatedOn = ProdGrossAmt;
                        }
                        else if (Convert.ToString(dr["ApplicableOn"]) == "N")
                        {
                            obj.calCulatedOn = ProdNetAmt;
                        }
                        else
                        {
                            obj.calCulatedOn = 0;
                        }
                        //End Here
                        #endregion

                        //Debjyoti 09032017
                        if (Convert.ToString(ddl_AmountAre.Value) == "2")
                        {
                            if (Convert.ToString(ddl_VatGstCst.Value) == "0~0~X")
                            {
                                if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                                {
                                    decimal finalCalCulatedOn = 0;
                                    decimal backProcessRate = (1 + (totalParcentage / 100));
                                    finalCalCulatedOn = obj.calCulatedOn / backProcessRate;
                                    obj.calCulatedOn = finalCalCulatedOn;
                                }
                            }
                        }

                        if (Convert.ToString(dr["TaxCalculateMethods"]) == "A")
                        {
                            obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(+)";

                        }
                        else
                        {
                            obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(-)";
                        }

                        obj.Amount = Convert.ToDouble(obj.calCulatedOn * (Convert.ToDecimal(obj.TaxField) / 100));




                        DataRow[] filtr = MainTaxDataTable.Select("TaxCode ='" + obj.Taxes_ID + "' and slNo=" + Convert.ToString(slNo));
                        if (filtr.Length > 0)
                        {
                            obj.Amount = Convert.ToDouble(filtr[0]["Amount"]);
                            if (obj.Taxes_ID == 0)
                            {
                                //   obj.TaxField = GetTaxName(Convert.ToInt32(Convert.ToString(filtr[0]["AltTaxCode"])));
                                aspxGridTax.JSProperties["cpComboCode"] = Convert.ToString(filtr[0]["AltTaxCode"]);
                            }
                            else
                                obj.TaxField = Convert.ToString(filtr[0]["Percentage"]);
                        }

                        TaxDetailsDetails.Add(obj);
                    }
                }
                else
                {
                    string keyValue = e.Parameters.Split('~')[0];

                    DataTable TaxRecord = (DataTable)Session["SI_FinalTaxRecord"];

                    DataRow[] filtrRow = TaxRecord.Select("SlNo='" + Convert.ToString(slNo) + "'");
                    if (filtrRow.Length > 0)
                    {

                        foreach (DataRow dr in taxDetail.Rows)
                        {
                            TaxDetails obj = new TaxDetails();
                            obj.Taxes_ID = Convert.ToInt32(dr["Taxes_ID"]);
                            obj.taxCodeName = Convert.ToString(dr["taxCodeName"]);

                            if (Convert.ToString(dr["TaxCalculateMethods"]) == "A")
                                obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(+)";
                            else
                                obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(-)";
                            obj.TaxField = "";
                            obj.Amount = 0.0;

                            #region set calculated on
                            //Check Tax Applicable on and set to calculated on
                            if (Convert.ToString(dr["ApplicableOn"]) == "G")
                            {
                                obj.calCulatedOn = ProdGrossAmt;
                            }
                            else if (Convert.ToString(dr["ApplicableOn"]) == "N")
                            {
                                obj.calCulatedOn = ProdNetAmt;
                            }
                            else
                            {
                                obj.calCulatedOn = 0;
                            }
                            //End Here
                            #endregion

                            //Debjyoti 09032017
                            if (Convert.ToString(ddl_AmountAre.Value) == "2")
                            {
                                if (Convert.ToString(ddl_VatGstCst.Value) == "0~0~X")
                                {
                                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                                    {
                                        decimal finalCalCulatedOn = 0;
                                        decimal backProcessRate = (1 + (totalParcentage / 100));
                                        finalCalCulatedOn = obj.calCulatedOn / backProcessRate;
                                        obj.calCulatedOn = finalCalCulatedOn;
                                    }
                                }
                            }

                            DataRow[] filtronexsisting1 = TaxRecord.Select("TaxCode=" + obj.Taxes_ID + " and SlNo=" + Convert.ToString(slNo));
                            if (filtronexsisting1.Length > 0)
                            {
                                if (obj.Taxes_ID == 0)
                                {
                                    obj.TaxField = "0";
                                }
                                else
                                {
                                    obj.TaxField = Convert.ToString(filtronexsisting1[0]["Percentage"]);
                                }
                                obj.Amount = Convert.ToDouble(filtronexsisting1[0]["Amount"]);
                            }
                            else
                            {
                                #region checkingFordb


                                //DataRow[] filtr = databaseReturnTable.Select("ProductTax_ProductId ='" + keyValue + "' and ProductTax_QuoteId=" +Convert.ToString( Session["SI_InvoiceID"] )+ " and ProductTax_TaxTypeId=" + obj.Taxes_ID);
                                //if (filtr.Length > 0)
                                //{
                                //    obj.Amount = Convert.ToDouble(filtr[0]["ProductTax_Amount"]);
                                //    if (obj.Taxes_ID == 0)
                                //    {
                                //        //obj.TaxField = GetTaxName();
                                //        obj.TaxField = "0";
                                //    }
                                //    else
                                //    {
                                //        obj.TaxField = Convert.ToString(filtr[0]["ProductTax_Percentage"]);
                                //    }


                                //    DataRow[] filtronexsisting = TaxRecord.Select("TaxCode=" + obj.Taxes_ID + " and SlNo=" + Convert.ToString(slNo));
                                //    if (filtronexsisting.Length > 0)
                                //    {
                                //        filtronexsisting[0]["Amount"] = obj.Amount;
                                //        if (obj.Taxes_ID == 0)
                                //        {
                                //            filtronexsisting[0]["Percentage"] = 0;
                                //        }
                                //        else
                                //        {
                                //            filtronexsisting[0]["Percentage"] = obj.TaxField;
                                //        }

                                //    }
                                //    else
                                //    {

                                //        DataRow taxRecordNewRow = TaxRecord.NewRow();
                                //        taxRecordNewRow["SlNo"] = slNo;
                                //        taxRecordNewRow["TaxCode"] = obj.Taxes_ID;
                                //        taxRecordNewRow["AltTaxCode"] = "0";
                                //        taxRecordNewRow["Percentage"] = obj.TaxField;
                                //        taxRecordNewRow["Amount"] = obj.Amount;

                                //        TaxRecord.Rows.Add(taxRecordNewRow);
                                //    }

                                //}
                                //else
                                //{
                                //    DataRow[] filtronexsisting = TaxRecord.Select("TaxCode=" + obj.Taxes_ID + " and SlNo=" + Convert.ToString(slNo));
                                //    if (filtronexsisting.Length > 0)
                                //    {
                                //        DataRow taxRecordNewRow = TaxRecord.NewRow();
                                //        taxRecordNewRow["SlNo"] = slNo;
                                //        taxRecordNewRow["TaxCode"] = obj.Taxes_ID;
                                //        taxRecordNewRow["AltTaxCode"] = "0";
                                //        taxRecordNewRow["Percentage"] = 0.0;
                                //        taxRecordNewRow["Amount"] = "0";

                                //        TaxRecord.Rows.Add(taxRecordNewRow);
                                //    }
                                //}




                                #endregion


                                DataRow[] filtronexsisting = TaxRecord.Select("TaxCode=" + obj.Taxes_ID + " and SlNo=" + Convert.ToString(slNo));
                                if (filtronexsisting.Length > 0)
                                {
                                    DataRow taxRecordNewRow = TaxRecord.NewRow();
                                    taxRecordNewRow["SlNo"] = slNo;
                                    taxRecordNewRow["TaxCode"] = obj.Taxes_ID;
                                    taxRecordNewRow["AltTaxCode"] = "0";
                                    taxRecordNewRow["Percentage"] = 0.0;
                                    taxRecordNewRow["Amount"] = "0";

                                    TaxRecord.Rows.Add(taxRecordNewRow);
                                }

                            }
                            TaxDetailsDetails.Add(obj);

                            //      DataRow[] filtrIndex = databaseReturnTable.Select("ProductTax_ProductId ='" + keyValue + "' and ProductTax_QuoteId=" + Session["SI_InvoiceID"] + " and ProductTax_TaxTypeId=0");
                            DataRow[] filtrIndex = TaxRecord.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode=0");
                            if (filtrIndex.Length > 0)
                            {
                                aspxGridTax.JSProperties["cpComboCode"] = Convert.ToString(filtrIndex[0]["AltTaxCode"]);
                            }
                        }
                        Session["SI_FinalTaxRecord"] = TaxRecord;
                    }
                    else
                    {
                        foreach (DataRow dr in taxDetail.Rows)
                        {
                            TaxDetails obj = new TaxDetails();
                            obj.Taxes_ID = Convert.ToInt32(dr["Taxes_ID"]);
                            obj.taxCodeName = Convert.ToString(dr["taxCodeName"]);
                            obj.TaxField = Convert.ToString(dr["TaxRates_Rate"]);
                            obj.Amount = 0.0;

                            #region set calculated on
                            //Check Tax Applicable on and set to calculated on
                            if (Convert.ToString(dr["ApplicableOn"]) == "G")
                            {
                                obj.calCulatedOn = ProdGrossAmt;
                            }
                            else if (Convert.ToString(dr["ApplicableOn"]) == "N")
                            {
                                obj.calCulatedOn = ProdNetAmt;
                            }
                            else
                            {
                                obj.calCulatedOn = 0;
                            }
                            //End Here
                            #endregion

                            //Debjyoti 09032017
                            if (Convert.ToString(ddl_AmountAre.Value) == "2")
                            {
                                if (Convert.ToString(ddl_VatGstCst.Value) == "0~0~X")
                                {
                                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                                    {
                                        decimal finalCalCulatedOn = 0;
                                        decimal backProcessRate = (1 + (totalParcentage / 100));
                                        finalCalCulatedOn = obj.calCulatedOn / backProcessRate;
                                        obj.calCulatedOn = finalCalCulatedOn;
                                    }
                                }
                            }

                            if (Convert.ToString(dr["TaxCalculateMethods"]) == "A")
                            {
                                obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(+)";

                            }
                            else
                            {
                                obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(-)";
                            }

                            obj.Amount = Convert.ToDouble(obj.calCulatedOn * (Convert.ToDecimal(obj.TaxField) / 100));




                            DataRow[] filtr = MainTaxDataTable.Select("TaxCode ='" + obj.Taxes_ID + "' and slNo=" + Convert.ToString(slNo));
                            if (filtr.Length > 0)
                            {
                                obj.Amount = Convert.ToDouble(filtr[0]["Amount"]);
                                if (obj.Taxes_ID == 0)
                                {
                                    //   obj.TaxField = GetTaxName(Convert.ToInt32(Convert.ToString(filtr[0]["AltTaxCode"])));
                                    aspxGridTax.JSProperties["cpComboCode"] = Convert.ToString(filtr[0]["AltTaxCode"]);
                                }
                                else
                                    obj.TaxField = Convert.ToString(filtr[0]["Percentage"]);
                            }

                            TaxDetailsDetails.Add(obj);
                        }
                    }
                }

                //New Changes 170217
                //GstCode should fetch everytime
                DataRow[] finalFiltrIndex = MainTaxDataTable.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode=0");
                if (finalFiltrIndex.Length > 0)
                {
                    aspxGridTax.JSProperties["cpComboCode"] = Convert.ToString(finalFiltrIndex[0]["AltTaxCode"]);
                }

                for (var i = 0; i < TaxDetailsDetails.Count; i++)
                {
                    decimal _Amount = Convert.ToDecimal(TaxDetailsDetails[i].Amount);
                    decimal _calCulatedOn = Convert.ToDecimal(TaxDetailsDetails[i].calCulatedOn);

                    TaxDetailsDetails[i].Amount = RoundUp(Convert.ToDouble(_Amount), 2); //GetRoundOfValue(_Amount);
                    TaxDetailsDetails[i].calCulatedOn = Convert.ToDecimal(GetRoundOfValue(_calCulatedOn));
                }

                aspxGridTax.JSProperties["cpJsonData"] = createJsonForDetails(taxDetail);

                retMsg = Convert.ToString(GetTotalTaxAmount(TaxDetailsDetails));
                aspxGridTax.JSProperties["cpUpdated"] = "ok~" + retMsg;

                TaxDetailsDetails = setCalculatedOn(TaxDetailsDetails, taxDetail);
                aspxGridTax.DataSource = TaxDetailsDetails;
                aspxGridTax.DataBind();

                #endregion
            }
        }
        public string createJsonForDetails(DataTable lstTaxDetails)
        {
            List<TaxSetailsJson> jsonList = new List<TaxSetailsJson>();
            TaxSetailsJson jsonObj;
            int visIndex = 0;
            foreach (DataRow taxObj in lstTaxDetails.Rows)
            {
                jsonObj = new TaxSetailsJson();

                jsonObj.SchemeName = Convert.ToString(taxObj["Taxes_Name"]);
                jsonObj.vissibleIndex = visIndex;
                jsonObj.applicableOn = Convert.ToString(taxObj["ApplicableOn"]);
                if (jsonObj.applicableOn == "G" || jsonObj.applicableOn == "N")
                {
                    jsonObj.applicableBy = "None";
                }
                else
                {
                    jsonObj.applicableBy = Convert.ToString(taxObj["dependOn"]);
                }
                visIndex++;
                jsonList.Add(jsonObj);
            }

            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return oSerializer.Serialize(jsonList);
        }
        public List<TaxDetails> setCalculatedOn(List<TaxDetails> gridSource, DataTable taxDt)
        {
            foreach (TaxDetails taxObj in gridSource)
            {
                DataRow[] dependOn = taxDt.Select("dependOn='" + taxObj.Taxes_Name.Replace("(+)", "").Replace("(-)", "") + "'");
                if (dependOn.Length > 0)
                {
                    foreach (DataRow dr in dependOn)
                    {
                        //  List<TaxDetails> dependObj = gridSource.Where(r => r.Taxes_Name.Replace("(+)", "").Replace("(-)", "") == Convert.ToString(dependOn[0]["Taxes_Name"])).ToList();
                        foreach (var setCalObj in gridSource.Where(r => r.Taxes_Name.Replace("(+)", "").Replace("(-)", "") == Convert.ToString(dependOn[0]["Taxes_Name"])))
                        {
                            setCalObj.calCulatedOn = Convert.ToDecimal(taxObj.Amount);
                        }
                    }

                }

            }
            return gridSource;
        }
        public double GetRoundOfValue(decimal Value)
        {
            double RoundOfValue = 0;

            if (Value > 0)
            {
                decimal _Value = Convert.ToDecimal(Value);
                _Value = Math.Round(Value, 2);

                RoundOfValue = Convert.ToDouble(_Value);
            }

            return RoundOfValue;
        }
        public double RoundDown(double i, double decimalPlaces)
        {
            double power = Math.Pow(10, decimalPlaces);
            return Math.Floor(i * power) / power;
        }
        public double RoundUp(double i, double decimalPlaces)
        {
            double Amount = 0;

            string input_decimal_number = Convert.ToString(i);
            string decimal_places = "";

            var regex = new System.Text.RegularExpressions.Regex("(?<=[\\.])[0-9]+");
            if (regex.IsMatch(input_decimal_number)) decimal_places = regex.Match(input_decimal_number).Value;

            if (decimal_places.Length > 2)
            {
                string last_decimal_places = decimal_places.Substring(decimal_places.Length - 1);
                if (Convert.ToInt32(last_decimal_places) >= 5)
                {
                    double power = Math.Pow(10, decimalPlaces);
                    Amount = Math.Ceiling(i * power) / power;
                }
                else
                {
                    Amount = GetRoundOfValue(Convert.ToDecimal(i));
                }
            }
            else
            {
                Amount = GetRoundOfValue(Convert.ToDecimal(i));
            }

            return Amount;
        }

        public class TaxSetailsJson
        {
            public string SchemeName { get; set; }
            public int vissibleIndex { get; set; }
            public string applicableOn { get; set; }
            public string applicableBy { get; set; }
        }
        public class TaxDetails
        {
            public int Taxes_ID { get; set; }
            public string Taxes_Name { get; set; }

            public double Amount { get; set; }
            public string TaxField { get; set; }

            public string taxCodeName { get; set; }

            public decimal calCulatedOn { get; set; }

        }
        class taxCode
        {
            public string Taxes_ID { get; set; }
            public string Taxes_Name { get; set; }
        }
        protected void aspxGridTax_CellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
        {

            if (e.Column.FieldName == "Taxes_Name")
            {
                e.Editor.ReadOnly = true;
            }
            if (e.Column.FieldName == "taxCodeName")
            {
                e.Editor.ReadOnly = true;
            }
            if (e.Column.FieldName == "calCulatedOn")
            {
                e.Editor.ReadOnly = true;
            }
            //else if (e.Column.FieldName == "Amount")
            //{
            //    e.Editor.ReadOnly = true;
            //}
            else
            {
                e.Editor.ReadOnly = false;
            }
        }
        protected void aspxGridTax_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {

        }
        protected void taxgrid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {

            int slNo = Convert.ToInt32(HdSerialNo.Value);
            DataTable TaxRecord = (DataTable)Session["SI_FinalTaxRecord"];
            foreach (var args in e.UpdateValues)
            {

                string TaxCodeDes = Convert.ToString(args.NewValues["Taxes_Name"]);
                decimal Percentage = 0;

                Percentage = Convert.ToDecimal(args.NewValues["TaxField"]);

                decimal Amount = Convert.ToDecimal(args.NewValues["Amount"]);
                string TaxCode = "0";
                if (!Convert.ToString(args.Keys[0]).Contains('~'))
                {
                    TaxCode = Convert.ToString(args.Keys[0]);
                }



                DataRow[] finalRow = TaxRecord.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode='" + TaxCode + "'");
                if (finalRow.Length > 0)
                {
                    finalRow[0]["Percentage"] = Percentage;
                    // finalRow[0]["TaxCode"] = args.NewValues["TaxField"]; 
                    finalRow[0]["Amount"] = Amount;

                    finalRow[0]["TaxCode"] = args.Keys[0];
                    finalRow[0]["AltTaxCode"] = "0";

                }
                else
                {
                    DataRow newRow = TaxRecord.NewRow();
                    newRow["slNo"] = slNo;
                    newRow["Percentage"] = Percentage;
                    newRow["TaxCode"] = TaxCode;
                    newRow["AltTaxCode"] = "0";
                    newRow["Amount"] = Amount;
                    TaxRecord.Rows.Add(newRow);
                }


            }

            //For GST/CST/VAT
            if (cmbGstCstVat.Value != null)
            {

                DataRow[] finalRow = TaxRecord.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode='0'");
                if (finalRow.Length > 0)
                {
                    finalRow[0]["Percentage"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[1];
                    finalRow[0]["Amount"] = txtGstCstVat.Text;
                    finalRow[0]["AltTaxCode"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[0];

                }
                else
                {
                    DataRow newRowGST = TaxRecord.NewRow();
                    newRowGST["slNo"] = slNo;
                    newRowGST["Percentage"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[1];
                    newRowGST["TaxCode"] = "0";
                    newRowGST["AltTaxCode"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[0];
                    newRowGST["Amount"] = txtGstCstVat.Text;
                    TaxRecord.Rows.Add(newRowGST);
                }
            }
            //End Here


            Session["SI_FinalTaxRecord"] = TaxRecord;


            #region oldpart


            //DataTable taxdtByProductCode = new DataTable();
            //if (Session["ProdTax" + "_" + Convert.ToString(HdSerialNo.Value)] == null)
            //{

            //    taxdtByProductCode.TableName = "ProdTax"  + "_" + Convert.ToString(HdSerialNo.Value);


            //    taxdtByProductCode.Columns.Add("TaxCode", typeof(System.String));
            //    taxdtByProductCode.Columns.Add("TaxCodeDescription", typeof(System.String));
            //    taxdtByProductCode.Columns.Add("Percentage", typeof(System.Decimal));
            //    taxdtByProductCode.Columns.Add("Amount", typeof(System.Decimal));
            //    DataRow dr;
            //    foreach (var args in e.UpdateValues)
            //    {
            //        dr = taxdtByProductCode.NewRow();
            //        string TaxCodeDes = Convert.ToString(args.NewValues["Taxes_Name"]);
            //        decimal Percentage = 0;
            //        if (TaxCodeDes == "GST/CST/VAT")
            //        {
            //            Percentage = Convert.ToDecimal(Convert.ToString(args.NewValues["TaxField"]).Split('~')[1]);
            //        }
            //        else
            //        {
            //            Percentage = Convert.ToDecimal(args.NewValues["TaxField"]);
            //        }
            //        decimal Amount = Convert.ToDecimal(args.NewValues["Amount"]);

            //        dr["TaxCodeDescription"] = TaxCodeDes;
            //        if (Convert.ToString(args.Keys[0]) == "0")
            //        {
            //            dr["TaxCode"] = "0~" + Convert.ToString(args.NewValues["TaxField"]).Split('~')[0];
            //        }
            //        else
            //        {
            //            dr["TaxCode"] = args.Keys[0];
            //        }
            //        dr["Percentage"] = Percentage;
            //        dr["Amount"] = Amount;

            //        taxdtByProductCode.Rows.Add(dr);
            //    }
            //}
            //else
            //{
            //    taxdtByProductCode = (DataTable)Session["ProdTax"  +"_"+ Convert.ToString(HdSerialNo.Value)];

            //    foreach (var args in e.UpdateValues)
            //    {
            //        DataRow[] filtr ;

            //        if (Convert.ToString(args.Keys[0]) == "0")
            //        {
            //            filtr = taxdtByProductCode.Select("TaxCode like '%0~%'"); ;
            //        }
            //        else
            //        {
            //            filtr = taxdtByProductCode.Select("TaxCode='" + args.Keys[0]+"'");
            //        }

            //        if (filtr.Length > 0)
            //        {
            //            string TaxCodeDes = Convert.ToString(args.NewValues["Taxes_Name"]);
            //        filtr[0]["TaxCodeDescription"] = TaxCodeDes;
            //        if (Convert.ToString(args.Keys[0]) == "0")
            //        {
            //            filtr[0]["TaxCode"] = "0~" + Convert.ToString(args.NewValues["TaxField"]).Split('~')[0];
            //        }
            //        else
            //        {
            //            filtr[0]["TaxCode"] = args.Keys[0];
            //        }

            //        decimal Percentage = 0;
            //        if (TaxCodeDes == "GST/CST/VAT")
            //        {
            //            Percentage = Convert.ToDecimal(Convert.ToString(args.NewValues["TaxField"]).Split('~')[1]);
            //        }
            //        else
            //        {
            //            Percentage = Convert.ToDecimal(args.NewValues["TaxField"]);
            //        }
            //        decimal Amount = Convert.ToDecimal(args.NewValues["Amount"]);
            //        filtr[0]["Percentage"] = Percentage;
            //        filtr[0]["Amount"] = Amount;

            //        }
            //    }


            //}

            #endregion
            //  Session[taxdtByProductCode.TableName] = taxdtByProductCode;

        }
        protected void cmbGstCstVat_Callback(object sender, CallbackEventArgsBase e)
        {
            DateTime quoteDate = Convert.ToDateTime(dt_PLQuote.Date.ToString("yyyy-MM-dd"));

            PopulateGSTCSTVATCombo(quoteDate.ToString("yyyy-MM-dd"));
            CreateDataTaxTable();
            //DataTable taxTableItemLvl = (DataTable)Session["SI_FinalTaxRecord"];
            //foreach (DataRow dr in taxTableItemLvl.Rows)
            //    dr.Delete();
            //taxTableItemLvl.AcceptChanges();
            //Session["SI_FinalTaxRecord"] = taxTableItemLvl;
        }
        protected void cmbGstCstVatcharge_Callback(object sender, CallbackEventArgsBase e)
        {
            Session["SI_TaxDetails"] = null;
            DateTime quoteDate = Convert.ToDateTime(dt_PLQuote.Date.ToString("yyyy-MM-dd"));
            PopulateChargeGSTCSTVATCombo(quoteDate.ToString("yyyy-MM-dd"));
        }
        public DataTable getAllTaxDetails(DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            DataTable FinalTable = new DataTable();
            FinalTable.Columns.Add("SlNo", typeof(System.Int32));
            FinalTable.Columns.Add("TaxCode", typeof(System.String));
            FinalTable.Columns.Add("AltTaxCode", typeof(System.String));
            FinalTable.Columns.Add("Percentage", typeof(System.Decimal));
            FinalTable.Columns.Add("Amount", typeof(System.Decimal));

            //for insert
            foreach (var args in e.InsertValues)
            {
                string Slno = Convert.ToString(args.NewValues["SrlNo"]);
                DataRow existsRow;
                if (Session["ProdTax_" + Slno] != null)
                {
                    DataTable sessiontable = (DataTable)Session["ProdTax_" + Slno];
                    foreach (DataRow dr in sessiontable.Rows)
                    {
                        existsRow = FinalTable.NewRow();

                        existsRow["SlNo"] = Slno;
                        if (Convert.ToString(dr["taxCode"]).Contains("~"))
                        {
                            existsRow["TaxCode"] = "0";
                            existsRow["AltTaxCode"] = Convert.ToString(dr["taxCode"]).Split('~')[1];
                        }
                        else
                        {
                            existsRow["TaxCode"] = Convert.ToString(dr["taxCode"]);
                            existsRow["AltTaxCode"] = "0";
                        }

                        existsRow["Percentage"] = Convert.ToString(dr["Percentage"]);
                        existsRow["Amount"] = Convert.ToString(dr["Amount"]);

                        FinalTable.Rows.Add(existsRow);
                    }
                    Session.Remove("ProdTax_" + Slno);
                }
            }

            return FinalTable;
        }
        protected void taxgrid_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void taxgrid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void taxgrid_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            e.Cancel = true;
        }
        public void DeleteTaxDetails(string SrlNo)
        {
            DataTable TaxDetailTable = new DataTable();
            if (Session["SI_FinalTaxRecord"] != null)
            {
                TaxDetailTable = (DataTable)Session["SI_FinalTaxRecord"];

                var rows = TaxDetailTable.Select("SlNo ='" + SrlNo + "'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
                TaxDetailTable.AcceptChanges();

                Session["SI_FinalTaxRecord"] = TaxDetailTable;
            }
        }
        public void UpdateTaxDetails(string oldSrlNo, string newSrlNo)
        {
            DataTable TaxDetailTable = new DataTable();
            if (Session["SI_FinalTaxRecord"] != null)
            {
                TaxDetailTable = (DataTable)Session["SI_FinalTaxRecord"];

                for (int i = 0; i < TaxDetailTable.Rows.Count; i++)
                {
                    DataRow dr = TaxDetailTable.Rows[i];
                    string Product_SrlNo = Convert.ToString(dr["SlNo"]);
                    if (oldSrlNo == Product_SrlNo)
                    {
                        dr["SlNo"] = newSrlNo;
                    }
                }
                TaxDetailTable.AcceptChanges();

                Session["SI_FinalTaxRecord"] = TaxDetailTable;
            }
        }


        public string createJsonForChargesTax(DataTable lstTaxDetails)
        {
            List<TaxSetailsJson> jsonList = new List<TaxSetailsJson>();
            TaxSetailsJson jsonObj;
            int visIndex = 0;
            foreach (DataRow taxObj in lstTaxDetails.Rows)
            {
                jsonObj = new TaxSetailsJson();

                jsonObj.SchemeName = Convert.ToString(taxObj["Taxes_Name"]);
                jsonObj.vissibleIndex = visIndex;
                jsonObj.applicableOn = "G";// Convert.ToString(taxObj["ApplicableOn"]);
                if (jsonObj.applicableOn == "G" || jsonObj.applicableOn == "N")
                {
                    jsonObj.applicableBy = "None";
                }
                else
                {
                    jsonObj.applicableBy = Convert.ToString(taxObj["dependOn"]);
                }
                visIndex++;
                jsonList.Add(jsonObj);
            }

            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return oSerializer.Serialize(jsonList);
        }
        public List<Taxes> setChargeCalculatedOn(List<Taxes> gridSource, DataTable taxDt)
        {
            foreach (Taxes taxObj in gridSource)
            {
                DataRow[] dependOn = taxDt.Select("dependOn='" + taxObj.TaxName.Replace("(+)", "").Replace("(-)", "").Trim() + "'");
                if (dependOn.Length > 0)
                {
                    foreach (DataRow dr in dependOn)
                    {
                        //  List<TaxDetails> dependObj = gridSource.Where(r => r.Taxes_Name.Replace("(+)", "").Replace("(-)", "") == Convert.ToString(dependOn[0]["Taxes_Name"])).ToList();
                        foreach (var setCalObj in gridSource.Where(r => r.TaxName.Replace("(+)", "").Replace("(-)", "").Trim() == Convert.ToString(dependOn[0]["Taxes_Name"]).Replace("(+)", "").Replace("(-)", "").Trim()))
                        {
                            setCalObj.calCulatedOn = Convert.ToDecimal(taxObj.Amount);
                        }
                    }

                }

            }
            return gridSource;
        }

        public void PopulateChargeGSTCSTVATCombo(string quoteDate)
        {
            string LastCompany = "";
            if (Convert.ToString(Session["LastCompany"]) != null)
            {
                LastCompany = Convert.ToString(Session["LastCompany"]);
            }
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "LoadChargeGSTCSTVATCombo");
            proc.AddVarcharPara("@S_QuoteAdd_CompanyID", 10, Convert.ToString(LastCompany));
            proc.AddVarcharPara("@S_quoteDate", 10, quoteDate);
            proc.AddIntegerPara("@branchId", Convert.ToInt32(Session["userbranchID"]));
            DataTable DT = proc.GetTable();
            cmbGstCstVatcharge.DataSource = DT;
            cmbGstCstVatcharge.TextField = "Taxes_Name";
            cmbGstCstVatcharge.ValueField = "Taxes_ID";
            cmbGstCstVatcharge.DataBind();
        }

        #endregion

        #region Header Portion Detail of the Page

        protected void ddlCashBank_Callback(object sender, CallbackEventArgsBase e)
        {
            BindCashBank();

            string strBranch = Convert.ToString(ddl_Branch.SelectedValue);
            string sqlQuery = " Select * from tbl_master_ApprovalConfiguration where Active=1 AND Entries_Id=4 AND BranchId='" + strBranch + "'";
            DataTable dtC = oDBEngine.GetDataTable(sqlQuery);
            if (dtC != null && dtC.Rows.Count > 0)
            {
                ddlCashBank.SelectedIndex = 0;
                ddlCashBank.ClientEnabled = false;
            }
            else
            {
                ddlCashBank.ClientEnabled = true;
            }

            ddlCashBank.ClientEnabled = false;
        }
        public void BindCashBank()
        {
            string CompanyId = Convert.ToString(Session["LastCompany"]);
            string userbranch = Convert.ToString(ddl_Branch.SelectedValue);

            //CustomerVendorReceiptPaymentBL objCustomerVendorReceiptPaymentBL = new CustomerVendorReceiptPaymentBL();
            //DataTable dtCashBank = objCustomerVendorReceiptPaymentBL.GetCustomerCashBank(userbranch, CompanyId);
            //if (dtCashBank.Rows.Count > 0)
            //{
            //    ddlCashBank.TextField = "IntegrateMainAccount";
            //    ddlCashBank.ValueField = "MainAccount_ReferenceID";
            //    ddlCashBank.DataSource = dtCashBank;
            //    ddlCashBank.DataBind();
            //}
            //else
            //{
            //    ddlCashBank.TextField = "IntegrateMainAccount";
            //    ddlCashBank.ValueField = "MainAccount_ReferenceID";
            //    ddlCashBank.DataSource = dtCashBank;
            //    ddlCashBank.DataBind();
            //}
        }
        [WebMethod]
        public static bool CheckUniqueCode(string QuoteNo)
        {
            bool flag = false;
            BusinessLogicLayer.GenericMethod oGenericMethod = new BusinessLogicLayer.GenericMethod();
            try
            {
                MShortNameCheckingBL objShortNameChecking = new MShortNameCheckingBL();
                flag = objShortNameChecking.CheckUnique(QuoteNo, "0", "Quotation");
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
            return flag;
        }
        [WebMethod]
        public static string GetCurrentConvertedRate(string CurrencyId)
        {

            string[] ActCurrency = new string[] { };

            string CompID = "";
            if (HttpContext.Current.Session["LastCompany"] != null)
            {
                CompID = Convert.ToString(HttpContext.Current.Session["LastCompany"]);


            }
            string currentRate = "";
            if (HttpContext.Current.Session["LocalCurrency"] != null)
            {
                string currency = Convert.ToString(HttpContext.Current.Session["LocalCurrency"]);
                ActCurrency = currency.Split('~');
                int BaseCurrencyId = Convert.ToInt32(ActCurrency[0]);
                int ConvertedCurrencyId = Convert.ToInt32(CurrencyId);
                SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
                DataTable dt = objSlaesActivitiesBL.GetCurrentConvertedRate(BaseCurrencyId, ConvertedCurrencyId, CompID);
                if (dt != null && dt.Rows.Count > 0)
                {
                    currentRate = Convert.ToString(dt.Rows[0]["SalesRate"]);
                    return currentRate;
                }
            }
            return null;

        }
        protected void cmbContactPerson_Callback(object sender, CallbackEventArgsBase e)
        {
            Session["SI_QuotationAddressDtl"] = null;
            Session["SI_BillingAddressLookup"] = null;
            Session["SI_ShippingAddressLookup"] = null;
            string WhichCall = e.Parameter.Split('~')[0];
            if (WhichCall == "BindContactPerson")
            {
                string InternalId = Convert.ToString(Convert.ToString(e.Parameter.Split('~')[1]));
                PopulateContactPersonOfCustomer(InternalId);

                DataTable dtDeuDate = objProjectInvoiceBL.GetCustomerDetails_InvoiceRelated_Days(InternalId);
                foreach (DataRow dr in dtDeuDate.Rows)
                {
                    string strDueDate = Convert.ToString(dr["DueDate"]);
                    cmbContactPerson.JSProperties["cpDueDate"] = strDueDate;
                    //dt_SaleInvoiceDue.Date = Convert.ToDateTime(strDeuDate);
                }

                DataTable dtTotalDues = objProjectInvoiceBL.GetCustomerTotalDues(InternalId);
                if (dtTotalDues != null && dtTotalDues.Rows.Count > 0)
                {
                    string totalDues = Convert.ToString(dtTotalDues.Rows[0]["NetOutstanding"]);
                    cmbContactPerson.JSProperties["cpTotalDue"] = totalDues;
                }
                else
                {
                    cmbContactPerson.JSProperties["cpTotalDue"] = "0.00";
                }
            }
        }
        protected void PopulateContactPersonOfCustomer(string InternalId)
        {
            string ContactPerson = "";
            DataTable dtContactPerson = new DataTable();
            SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
            dtContactPerson = objSlaesActivitiesBL.PopulateContactPersonOfCustomer(InternalId);
            if (dtContactPerson != null && dtContactPerson.Rows.Count > 0)
            {
                cmbContactPerson.TextField = "contactperson";
                cmbContactPerson.ValueField = "add_id";
                cmbContactPerson.DataSource = dtContactPerson;
                cmbContactPerson.DataBind();
                foreach (DataRow dr in dtContactPerson.Rows)
                {
                    if (Convert.ToString(dr["Isdefault"]) == "True")
                    {
                        ContactPerson = Convert.ToString(dr["add_id"]);
                        break;
                    }
                }
                cmbContactPerson.SelectedItem = cmbContactPerson.Items.FindByValue(ContactPerson);
            }
        }
        protected void ddl_VatGstCst_Callback(object sender, CallbackEventArgsBase e)
        {
            string type = e.Parameter.Split('~')[0];
            PopulateGSTCSTVAT(type);
        }
        protected void PopulateGSTCSTVAT(string type)
        {
            DataTable dtGSTCSTVAT = new DataTable();
            SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
            if (type == "2")
            {
                dtGSTCSTVAT = objSlaesActivitiesBL.PopulateGSTCSTVAT(dt_PLQuote.Date.ToString("yyyy-MM-dd"));

                #region Delete Igst,Cgst,Sgst respectively

                string CompInternalId = Convert.ToString(Session["LastCompany"]);
                string BranchId = Convert.ToString(Session["userbranchID"]);
                string[] compGstin = oDBEngine.GetFieldValue1("tbl_master_company", "cmp_gstin", "cmp_internalid='" + CompInternalId + "'", 1);
                string[] branchGstin = oDBEngine.GetFieldValue1("tbl_master_branch", "isnull(branch_GSTIN,'')branch_GSTIN", "branch_id='" + BranchId + "'", 1);

                String GSTIN = "";

                if (branchGstin[0].Trim() != "")
                {
                    GSTIN = branchGstin[0].Trim();
                }
                else
                {
                    if (compGstin.Length > 0)
                    {
                        GSTIN = compGstin[0].Trim();
                    }
                }

                string ShippingState = "";

                #region ##### Added By : Samrat Roy -- For BillingShippingUserControl ######
                string sstateCode;
                if (ddl_PosGst.Value.ToString() == "S")
                {
                    sstateCode = Sales_BillingShipping.GetShippingStateId();
                }
                else
                {
                    sstateCode = Sales_BillingShipping.GetBillingStateId();
                }
                ShippingState = sstateCode;
                if (ShippingState.Trim() != "")
                {
                    ShippingState = ShippingState;
                }
                #region ##### Old Code -- BillingShipping ######
                ////if (chkBilling.Checked)
                ////{
                ////    if (CmbState.Value != null)
                ////    {
                ////        ShippingState = CmbState.Text;
                ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                ////    }
                ////}
                ////else
                ////{
                ////    if (CmbState1.Value != null)
                ////    {
                ////        ShippingState = CmbState1.Text;
                ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                ////    }
                ////}

                #endregion

                #endregion


                if (ShippingState.Trim() != "" && GSTIN.Trim() != "")
                {


                    if (GSTIN.Substring(0, 2) == ShippingState)
                    {
                        //Check if the state is in union territories then only UTGST will apply
                        //   Chandigarh     Andaman and Nicobar Islands    DADRA & NAGAR HAVELI    DAMAN & DIU      Lakshadweep              PONDICHERRY
                        if (ShippingState == "4" || ShippingState == "26" || ShippingState == "25" || ShippingState == "35" || ShippingState == "31" || ShippingState == "34")
                        {
                            foreach (DataRow dr in dtGSTCSTVAT.Rows)
                            {
                                if (Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "I" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "S")
                                {
                                    dr.Delete();
                                }
                            }

                        }
                        else
                        {
                            foreach (DataRow dr in dtGSTCSTVAT.Rows)
                            {
                                if (Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "I" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "U")
                                {
                                    dr.Delete();
                                }
                            }
                        }
                        dtGSTCSTVAT.AcceptChanges();
                    }
                    else
                    {
                        foreach (DataRow dr in dtGSTCSTVAT.Rows)
                        {
                            if (Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "C" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "S" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "U")
                            {
                                dr.Delete();
                            }
                        }
                        dtGSTCSTVAT.AcceptChanges();

                    }


                }

                //If Company GSTIN is blank then Delete All CGST,UGST,IGST,CGST
                if (GSTIN.Trim() == "")
                {
                    foreach (DataRow dr in dtGSTCSTVAT.Rows)
                    {
                        if (Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "C" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "S" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "U" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "I")
                        {
                            dr.Delete();
                        }
                    }
                    dtGSTCSTVAT.AcceptChanges();
                }


                #endregion



                if (dtGSTCSTVAT != null && dtGSTCSTVAT.Rows.Count > 0)
                {
                    ddl_VatGstCst.TextField = "Taxes_Name";
                    ddl_VatGstCst.ValueField = "Taxes_ID";
                    ddl_VatGstCst.DataSource = dtGSTCSTVAT;
                    ddl_VatGstCst.DataBind();
                    ddl_VatGstCst.SelectedIndex = 0;
                }
            }
            else
            {
                ddl_VatGstCst.DataSource = null;
                ddl_VatGstCst.DataBind();
            }
        }

        #region PrePopulated Data If Page is not Post Back Section Start
        public void SetFinYearCurrentDate()
        {
            dt_PLQuote.EditFormatString = objConverter.GetDateFormat("Date");
            string fDate = null;

            //DateTime dt = DateTime.ParseExact("3/31/2016", "MM/dd/yyy", CultureInfo.InvariantCulture);
            string[] FinYEnd = Convert.ToString(Session["FinYearEnd"]).Split(' ');
            string FinYearEnd = FinYEnd[0];

            DateTime date3 = DateTime.ParseExact(FinYearEnd, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            ForJournalDate = Convert.ToString(date3);

            //ForJournalDate =Session["FinYearEnd"].ToString();
            int month = oDBEngine.GetDate().Month;
            int date = oDBEngine.GetDate().Day;
            int Year = oDBEngine.GetDate().Year;

            if (date3 < oDBEngine.GetDate().Date)
            {
                fDate = Convert.ToString(Convert.ToDateTime(ForJournalDate).Month) + "/" + Convert.ToString(Convert.ToDateTime(ForJournalDate).Day) + "/" + Convert.ToString(Convert.ToDateTime(ForJournalDate).Year);
            }
            else
            {
                fDate = Convert.ToString(Convert.ToDateTime(oDBEngine.GetDate().Date).Month) + "/" + Convert.ToString(Convert.ToDateTime(oDBEngine.GetDate().Date).Day) + "/" + Convert.ToString(Convert.ToDateTime(oDBEngine.GetDate().Date).Year);
            }

            dt_PLQuote.Value = DateTime.ParseExact(fDate, @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
        public void GetFinacialYearBasedQouteDate()
        {
            String finyear = "";
            string setdate = null;
            if (Session["LastFinYear"] != null)
            {
                finyear = Convert.ToString(Session["LastFinYear"]).Trim();
                DataTable dtFinYear = objSlaesActivitiesBL.GetFinacialYearBasedQouteDate(finyear);
                if (dtFinYear != null && dtFinYear.Rows.Count > 0)
                {
                    Session["FinYearStartDate"] = Convert.ToString(dtFinYear.Rows[0]["finYearStartDate"]);
                    Session["FinYearEndDate"] = Convert.ToString(dtFinYear.Rows[0]["finYearEndDate"]);
                    if (Session["FinYearStartDate"] != null)
                    {
                        dt_PLQuote.MinDate = Convert.ToDateTime(Convert.ToString(Session["FinYearStartDate"]));
                    }
                    if (Session["FinYearEndDate"] != null)
                    {
                        dt_PLQuote.MaxDate = Convert.ToDateTime(Convert.ToString(Session["FinYearEndDate"]));
                    }
                    if (oDBEngine.GetDate().Date >= Convert.ToDateTime(Convert.ToString(Session["FinYearStartDate"])) && oDBEngine.GetDate().Date <= Convert.ToDateTime(Convert.ToString(Session["FinYearEndDate"])))
                    {

                    }
                    else if (oDBEngine.GetDate().Date >= Convert.ToDateTime(Convert.ToString(Session["FinYearStartDate"])) && oDBEngine.GetDate().Date >= Convert.ToDateTime(Convert.ToString(Session["FinYearEndDate"])))
                    {
                        setdate = Convert.ToString(Convert.ToDateTime(Session["FinYearEndDate"]).Month) + "/" + Convert.ToString(Convert.ToDateTime(Session["FinYearEndDate"]).Day) + "/" + Convert.ToString(Convert.ToDateTime(Session["FinYearEndDate"]).Year);
                        dt_PLQuote.Value = DateTime.ParseExact(setdate, @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        //dt_PLQuote.Value = DateTime.ParseExact(Convert.ToString(Session["FinYearStartDate"]), @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else if (oDBEngine.GetDate().Date <= Convert.ToDateTime(Convert.ToString(Session["FinYearStartDate"])) && oDBEngine.GetDate().Date <= Convert.ToDateTime(Convert.ToString(Session["FinYearEndDate"])))
                    {
                        setdate = Convert.ToString(Convert.ToDateTime(Session["FinYearStartDate"]).Month) + "/" + Convert.ToString(Convert.ToDateTime(Session["FinYearStartDate"]).Day) + "/" + Convert.ToString(Convert.ToDateTime(Session["FinYearStartDate"]).Year);
                        dt_PLQuote.Value = DateTime.ParseExact(setdate, @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        //dt_PLQuote.Value = DateTime.ParseExact(Convert.ToString(Session["FinYearEndDate"]), @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
            //dt_PLQuote.Value = Convert.ToDateTime(oDBEngine.GetDate().ToString());
        }
        public void GetAllDropDownDetailForSalesQuotation(string userbranch)
        {
            #region Schema Drop Down Start
            DataSet dst = new DataSet();
            string strCompanyID = Convert.ToString(Session["LastCompany"]);
            string strBranchID = Convert.ToString(Session["userbranchID"]);
            string FinYear = Convert.ToString(Session["LastFinYear"]);
            string userbranchHierarchy = Convert.ToString(Session["userbranchHierarchy"]);

            dst = objProjectInvoiceBL.GetAllDropDownDetailForProjectInvoice(userbranch, strCompanyID, strBranchID);

            SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
            DataTable Schemadt = objSlaesActivitiesBL.GetNumberingSchema(strCompanyID, userbranchHierarchy, FinYear, "113", "Y");
            if (Schemadt != null && Schemadt.Rows.Count > 0)
            {
                ddl_numberingScheme.DataTextField = "SchemaName";
                ddl_numberingScheme.DataValueField = "Id";
                ddl_numberingScheme.DataSource = Schemadt;
                ddl_numberingScheme.DataBind();
            }
            #endregion Schema Drop Down Start

            #region Branch Drop Down Start
            if (dst.Tables[1] != null && dst.Tables[1].Rows.Count > 0)
            {
                ddl_Branch.DataTextField = "branch_description";
                ddl_Branch.DataValueField = "branch_id";
                ddl_Branch.DataSource = dst.Tables[1];
                ddl_Branch.DataBind();
                //ddl_Branch.Items.Insert(0, new ListItem(FinalWarehouse"Select", "0"));
            }
            if (Session["userbranchID"] != null)
            {
                if (ddl_Branch.Items.Count > 0)
                {
                    int branchindex = 0;
                    int cnt = 0;
                    foreach (ListItem li in ddl_Branch.Items)
                    {
                        if (li.Value == Convert.ToString(Session["userbranchID"]))
                        {
                            cnt = 1;
                            break;
                        }
                        else
                        {
                            branchindex += 1;
                        }
                    }
                    if (cnt == 1)
                    {
                        ddl_Branch.SelectedIndex = branchindex;
                    }
                    else
                    {
                        ddl_Branch.SelectedIndex = cnt;
                    }
                }
            }

            #endregion Branch Drop Down End

            #region Saleman DropDown Start
            //if (dst.Tables[2] != null && dst.Tables[2].Rows.Count > 0)
            //{
            //    ddl_SalesAgent.DataTextField = "Name";
            //    ddl_SalesAgent.DataValueField = "cnt_internalId";
            //    ddl_SalesAgent.DataSource = dst.Tables[2];
            //    ddl_SalesAgent.DataBind();
            //}
            //ddl_SalesAgent.Items.Insert(0, new ListItem("Select", "0"));
            #endregion Saleman DropDown End

            #region Currency Drop Down Start

            if (dst.Tables[3] != null && dst.Tables[3].Rows.Count > 0)
            {
                ddl_Currency.DataTextField = "Currency_Name";
                ddl_Currency.DataValueField = "Currency_ID";
                ddl_Currency.DataSource = dst.Tables[3];
                ddl_Currency.DataBind();
            }
            int currencyindex = 1;
            int no = 0;
            if (Session["LocalCurrency"] != null)
            {
                if (ddl_Currency.Items.Count > 0)
                {
                    string[] ActCurrency = new string[] { };
                    string currency = Convert.ToString(HttpContext.Current.Session["LocalCurrency"]);
                    ActCurrency = currency.Split('~');
                    foreach (ListItem li in ddl_Currency.Items)
                    {
                        if (li.Value == Convert.ToString(ActCurrency[0]))
                        {
                            //ddl_Currency.Items.Remove(li);
                            no = 1;
                            break;
                        }
                        else
                        {
                            currencyindex += 1;
                        }
                    }
                }
                ddl_Currency.Items.Insert(0, new ListItem("Select", "0"));
                if (no == 1)
                {
                    ddl_Currency.SelectedIndex = currencyindex;
                    txt_Rate.ClientEnabled = false;
                }
                else
                {
                    ddl_Currency.SelectedIndex = no;
                    txt_Rate.ClientEnabled = true;
                }
            }

            #endregion Currency Drop Down End

            #region TaxGroupType DropDown Start
            if (dst.Tables[4] != null && dst.Tables[4].Rows.Count > 0)
            {
                ddl_AmountAre.TextField = "taxGrp_Description";
                ddl_AmountAre.ValueField = "taxGrp_Id";
                ddl_AmountAre.DataSource = dst.Tables[4];
                ddl_AmountAre.DataBind();
            }
            #endregion TaxGroupType DropDown Start

            #region Cash/Bank DropDown Start
            //if (dst.Tables[5] != null && dst.Tables[5].Rows.Count > 0)
            //{
            //    ddlCashBank.TextField = "IntegrateMainAccount";
            //    ddlCashBank.ValueField = "MainAccount_ReferenceID";
            //    ddlCashBank.DataSource = dst.Tables[5];
            //    ddlCashBank.DataBind();
            //}
            BindCashBank();
            #endregion Cash/Bank DropDown Start
        }

        #endregion PrePopulated Data If Page is not Post Back Section End

        #region PrePopulated Data in Page Load Due to use Searching Functionality Section Start
        //public void PopulateCustomerDetail()
        //{
        //    if (Session["SI_CustomerDetail"] == null)
        //    {
        //        DataTable dtCustomer = new DataTable();
        //        dtCustomer = objProjectInvoiceBL.PopulateCustomerDetail();

        //        if (dtCustomer != null && dtCustomer.Rows.Count > 0)
        //        {
        //            lookup_Customer.DataSource = dtCustomer;
        //            lookup_Customer.DataBind();
        //            Session["SI_CustomerDetail"] = dtCustomer;
        //        }
        //    }
        //    else
        //    {
        //        lookup_Customer.DataSource = (DataTable)Session["SI_CustomerDetail"];
        //        lookup_Customer.DataBind();
        //    }

        //}
        #endregion PrePopulated Data in Page Load Due to use Searching Functionality Section End

        #endregion

        #region Trash Code Start

        //protected void aspxGridTax_CellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
        //{

        //    if (e.Column.FieldName == "TaxName")
        //    {
        //        e.Editor.ReadOnly = true;
        //    }
        //    else if (e.Column.FieldName == "Amount")
        //    {
        //        e.Editor.ReadOnly = true;
        //    }
        //    else
        //    {
        //        e.Editor.ReadOnly = false;
        //    }
        //}

        //protected void Popup_SalesQuote_WindowCallback(object source, PopupWindowCallbackArgs e)
        //{
        //    Popup_SalesQuote.JSProperties["cpshow"] = "";
        //    Popup_SalesQuote.JSProperties["cpshowShip"] = "";
        //    DataTable dtaddbill = oDBEngine.GetDataTable("select add_addressType,add_address1,add_address2,add_address3,add_landMark,add_country,add_state,add_city,add_pin,add_area from tbl_master_address where add_cntId='LDP0000002' and add_addressType='Billing'");
        //    if (dtaddbill.Rows.Count > 0 && dtaddbill != null)
        //    {

        //        //for (int m = 0; m < dtaddbill.Rows.Count; m++)
        //        //{
        //        string add_addressType = Convert.ToString(dtaddbill.Rows[0]["add_addressType"]);
        //        string add_address1 = Convert.ToString(dtaddbill.Rows[0]["add_address1"]);
        //        string add_address2 = Convert.ToString(dtaddbill.Rows[0]["add_address2"]);
        //        string add_address3 = Convert.ToString(dtaddbill.Rows[0]["add_address3"]);
        //        string add_landMark = Convert.ToString(dtaddbill.Rows[0]["add_landMark"]);
        //        string add_country = Convert.ToString(dtaddbill.Rows[0]["add_country"]);
        //        string add_state = Convert.ToString(dtaddbill.Rows[0]["add_state"]);
        //        string add_city = Convert.ToString(dtaddbill.Rows[0]["add_city"]);
        //        string add_pin = Convert.ToString(dtaddbill.Rows[0]["add_pin"]);
        //        string add_area = Convert.ToString(dtaddbill.Rows[0]["add_area"]);

        //        //}

        //        Popup_SalesQuote.JSProperties["cpshow"] = add_addressType + "~"
        //                                           + add_address1 + "~"
        //                                           + add_address2 + "~"
        //                                           + add_address3 + "~"
        //                                           + add_landMark + "~"
        //                                           + add_country + "~"
        //                                           + add_state + "~"
        //                                           + add_city + "~"
        //                                           + add_pin + "~"
        //                                           + add_area + "~";

        //    }

        //    DataTable dtaship = oDBEngine.GetDataTable("select add_addressType,add_address1,add_address2,add_address3,add_landMark,add_country,add_state,add_city,add_pin,add_area from tbl_master_address where add_cntId='LDP0000002' and add_addressType='Shipping'");
        //    if (dtaship.Rows.Count > 0 && dtaship != null)
        //    {
        //        string add_saddressType = Convert.ToString(dtaship.Rows[0]["add_addressType"]);
        //        string add_saddress1 = Convert.ToString(dtaship.Rows[0]["add_address1"]);
        //        string add_saddress2 = Convert.ToString(dtaship.Rows[0]["add_address2"]);
        //        string add_saddress3 = Convert.ToString(dtaship.Rows[0]["add_address3"]);
        //        string add_slandMark = Convert.ToString(dtaship.Rows[0]["add_landMark"]);
        //        string add_scountry = Convert.ToString(dtaship.Rows[0]["add_country"]);
        //        string add_sstate = Convert.ToString(dtaship.Rows[0]["add_state"]);
        //        string add_scity = Convert.ToString(dtaship.Rows[0]["add_city"]);
        //        string add_spin = Convert.ToString(dtaship.Rows[0]["add_pin"]);
        //        string add_sarea = Convert.ToString(dtaship.Rows[0]["add_area"]);

        //        Popup_SalesQuote.JSProperties["cpshowShip"] = add_saddressType + "~"
        //                                          + add_saddress1 + "~"
        //                                          + add_saddress2 + "~"
        //                                          + add_saddress3 + "~"
        //                                          + add_slandMark + "~"
        //                                          + add_scountry + "~"
        //                                          + add_sstate + "~"
        //                                          + add_scity + "~"
        //                                          + add_spin + "~"
        //                                          + add_sarea + "~";

        //    }





        //}

        //public void GetEditablePermission()
        //{
        //    if (Request.QueryString["Permission"] != null)
        //    {
        //        if (Convert.ToString(Request.QueryString["Permission"]) == "1")
        //        {
        //            //pnl_quotation.Enabled = false;
        //            btn_SaveRecords.Visible = false;
        //            ASPxButton1.Visible = false;
        //        }
        //        else if (Convert.ToString(Request.QueryString["Permission"]) == "2")
        //        {
        //            //pnl_quotation.Enabled = true;
        //            btn_SaveRecords.Visible = true;
        //            ASPxButton1.Visible = true;
        //        }
        //        else if (Convert.ToString(Request.QueryString["Permission"]) == "3")
        //        {
        //            //pnl_quotation.Enabled = false;
        //            btn_SaveRecords.Visible = false;
        //            ASPxButton1.Visible = false;
        //        }
        //    }
        //}

        #endregion Trash Code End

        #region Component Tagging

        protected void ComponentQuotation_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string status = string.Empty;
            string Customer = string.Empty;
            string OrderDate = string.Empty;
            string ComponentType = string.Empty;
            string Action = string.Empty;
            string BranchID = string.Empty;
            string inventory = string.Empty;
            string inventoryType = string.Empty;
            string FinYear = Convert.ToString(Session["LastFinYear"]);

            if (e.Parameter.Split('~')[0] == "BindComponentGrid")
            {
                BranchID = Convert.ToString(ddl_Branch.SelectedValue);
                if (e.Parameter.Split('~')[1] != null) Customer = e.Parameter.Split('~')[1];
                if (e.Parameter.Split('~')[2] != null) OrderDate = e.Parameter.Split('~')[2];
                if (e.Parameter.Split('~')[4] != null) ComponentType = e.Parameter.Split('~')[4];
                if (e.Parameter.Split('~')[5] != null) inventory = e.Parameter.Split('~')[5];
                if (e.Parameter.Split('~')[6] != null) inventoryType = e.Parameter.Split('~')[6];

                if (ComponentType == "QO")
                {
                    Action = "GetQuotation";
                    lbl_InvoiceNO.Text = "PI/Quotation Date";
                }
                else if (ComponentType == "SO")
                {
                    Action = "GetOrder";
                    lbl_InvoiceNO.Text = "Project Order Date";
                }
                else if (ComponentType == "SC")
                {
                    Action = "GetChallan";
                    lbl_InvoiceNO.Text = "Project Challan Date";
                }

                if (e.Parameter.Split('~')[3] == "DateCheck")
                {
                    lookup_quotation.GridView.Selection.UnselectAll();
                }


                string strInvoiceID = Convert.ToString(Session["SI_InvoiceID"]);
                DataTable ComponentTable = objProjectInvoiceBL.GetComponent(Customer, OrderDate, ComponentType, FinYear, BranchID, Action, strInvoiceID, inventory, inventoryType);
                lookup_quotation.GridView.Selection.CancelSelection();
                lookup_quotation.DataSource = ComponentTable;
                lookup_quotation.DataBind();

                Session["SI_ComponentData"] = ComponentTable;
            }

            else if (e.Parameter.Split('~')[0] == "RebindGridQuote")//Subhabrata for binding quotation
            {
                QuotationIds = OldSelectedKeyvalue.Value.TrimStart(',');
                if (!String.IsNullOrEmpty(QuotationIds))
                {
                    string[] eachQuo = QuotationIds.Split(',');
                    if (eachQuo.Length > 1)//More tha one quotation
                    {
                        ComponentQuotationPanel.JSProperties["cpRebindGridQuote"] = "Multiple Select Quotation Dates";
                        lookup_quotation.GridView.Selection.UnselectAll();
                        foreach (string val in eachQuo)
                        {
                            lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                        }
                    }
                    else if (eachQuo.Length == 1)//Single Quotation
                    {
                        lookup_quotation.GridView.Selection.UnselectAll();
                        foreach (string val in eachQuo)
                        {
                            lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                        }
                        ComponentQuotationPanel.JSProperties["cpRebindGridQuote"] = Convert.ToString(lookup_quotation.GridView.GetSelectedFieldValues("ComponentDate")[0]);
                    }
                    else//No Quotation selected
                    {
                        lookup_quotation.GridView.Selection.UnselectAll();
                    }
                }



            }
            else if (e.Parameter.Split('~')[0] == "BindComponentGridOnSelection")//Subhabrata for binding quotation
            {
                if (grid_Products.GetSelectedFieldValues("ComponentDetailsID").Count != 0)
                {
                    for (int i = 0; i < grid_Products.GetSelectedFieldValues("ComponentDetailsID").Count; i++)
                    {
                        QuotationIds += "," + grid_Products.GetSelectedFieldValues("ComponentID")[i];
                    }
                    QuotationIds = QuotationIds.TrimStart(',');
                    lookup_quotation.GridView.Selection.UnselectAll();
                    if (!String.IsNullOrEmpty(QuotationIds))
                    {
                        string[] eachQuo = QuotationIds.Split(',');
                        if (eachQuo.Length > 1)//More tha one quotation
                        {
                            txt_InvoiceDate.Text = "Multiple Select Quotation Dates";

                            foreach (string val in eachQuo)
                            {
                                lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                            }
                        }
                        else if (eachQuo.Length == 1)//Single Quotation
                        {
                            foreach (string val in eachQuo)
                            {
                                lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                            }
                        }
                        else//No Quotation selected
                        {
                            lookup_quotation.GridView.Selection.UnselectAll();
                        }
                    }
                }
                else if (grid_Products.GetSelectedFieldValues("ComponentDetailsID").Count == 0)
                {
                    lookup_quotation.GridView.Selection.UnselectAll();
                }

                string strType = Convert.ToString(rdl_SaleInvoice.SelectedValue);
                DataTable dt = objProjectInvoiceBL.GetNecessaryData(QuotationIds, strType);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string Reference = Convert.ToString(dt.Rows[0]["Reference"]);
                    string Currency_Id = Convert.ToString(dt.Rows[0]["Currency_Id"]);
                    string SalesmanId = Convert.ToString(dt.Rows[0]["SalesmanId"]);
                    string SalesmanName = Convert.ToString(dt.Rows[0]["SalesmanName"]);
                    string ExpiryDate = Convert.ToString(dt.Rows[0]["ExpiryDate"]);
                    string CurrencyRate = Convert.ToString(dt.Rows[0]["CurrencyRate"]);
                    string Type = Convert.ToString(dt.Rows[0]["ComponentType"]);
                    string CreditDays = Convert.ToString(dt.Rows[0]["CreditDays"]);
                    string DueDate = Convert.ToString(dt.Rows[0]["DueDate"]);

                    ComponentQuotationPanel.JSProperties["cpDetails"] = Reference + "~" + Currency_Id + "~" + SalesmanId + "~" + ExpiryDate + "~" + CurrencyRate + "~" + Type + "~" + CreditDays + "~" + DueDate + "~" + SalesmanName;
                }
            }
            else if (e.Parameter.Split('~')[0] == "DateCheckOnChanged")//Subhabrata for binding quotation
            {
                if (grid_Products.GetSelectedFieldValues("Quotation_No").Count != 0)
                {
                    DateTime SalesOrderDate = Convert.ToDateTime(e.Parameter.Split('~')[2]);
                    if (lookup_quotation.GridView.GetSelectedFieldValues("Date").Count() != 0)
                    {
                        DateTime QuotationDate = Convert.ToDateTime(lookup_quotation.GridView.GetSelectedFieldValues("Date")[0]);
                        if (SalesOrderDate < QuotationDate)
                        {
                            lookup_quotation.GridView.Selection.UnselectAll();
                        }
                    }
                }
            }
        }
        protected void lookup_quotation_DataBinding(object sender, EventArgs e)
        {
            if (Session["SI_ComponentData"] != null)
            {
                lookup_quotation.DataSource = (DataTable)Session["SI_ComponentData"];
            }
        }
        protected void ComponentDatePanel_Callback(object sender, CallbackEventArgsBase e)
        {
            string strSplitCommand = e.Parameter.Split('~')[0];
            if (strSplitCommand == "BindComponentDate")
            {
                string ComponentNo = Convert.ToString(e.Parameter.Split('~')[1]);
                string type = Convert.ToString(e.Parameter.Split('~')[2]);

                DataTable dtDetails = GetComponentDate(ComponentNo, type);
                if (dtDetails != null && dtDetails.Rows.Count > 0)
                {
                    string Date = Convert.ToString(dtDetails.Rows[0]["ComponentDate"]);
                    if (!string.IsNullOrEmpty(Date))
                    {
                        txt_InvoiceDate.Text = Convert.ToString(Date);
                    }
                }
            }
        }
        protected void grid_Products_DataBinding(Object sender, EventArgs e)
        {

            if (Session["PSI_ProductDetails"] != null)
            {
                DataTable Quotationdt = (DataTable)Session["PSI_ProductDetails"];
                //DataView dvData = new DataView(Quotationdt);
                //grid_Products.DataSource = GetProductsInfo(Quotationdt);
                grid_Products.DataSource = Quotationdt;
                //grid_Products.DataBind();
            }
        }
        protected void cgridProducts_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string strSplitCommand = e.Parameters.Split('~')[0];
            if (strSplitCommand == "BindProductsDetails")
            {
                string Quote_Nos = Convert.ToString(e.Parameters.Split('~')[1]);

                String QuoComponent = "";
                List<object> QuoList = lookup_quotation.GridView.GetSelectedFieldValues("ComponentID");
                foreach (object Quo in QuoList)
                {
                    QuoComponent += "," + Quo;
                }
                QuoComponent = QuoComponent.TrimStart(',');

                if (Quote_Nos != "$")
                {
                    string strAction = "";
                    string strType = Convert.ToString(rdl_SaleInvoice.SelectedValue);

                    if (strType == "QO")
                    {
                        strAction = "GetQuotationProducts";
                        grid_Products.Columns["ComponentNumber"].Caption = "Quotation No";
                    }
                    else if (strType == "SO")
                    {
                        strAction = "GetOrderProducts";
                        grid_Products.Columns["ComponentNumber"].Caption = "Order No";
                    }
                    else if (strType == "SC")
                    {
                        strAction = "GetChallanProducts";
                        grid_Products.Columns["ComponentNumber"].Caption = "Challan No";
                    }

                    string strInvoiceID = Convert.ToString(Session["SI_InvoiceID"]);
                    DataTable dtDetails = objProjectInvoiceBL.GetComponentProductList(strAction, QuoComponent, strInvoiceID);
                    Session["PSI_ProductDetails"] = dtDetails;
                    grid_Products.DataSource = dtDetails;
                    grid_Products.DataBind();
                }
                else
                {
                    grid_Products.DataSource = null;
                    grid_Products.DataBind();
                }

            }
            if (strSplitCommand == "SelectAndDeSelectProducts")
            {
                ASPxGridView gv = sender as ASPxGridView;
                string command = e.Parameters.ToString();
                string State = Convert.ToString(e.Parameters.Split('~')[1]);
                if (State == "SelectAll")
                {
                    for (int i = 0; i < gv.VisibleRowCount; i++)
                    {
                        gv.Selection.SelectRow(i);
                    }
                }
                if (State == "UnSelectAll")
                {
                    for (int i = 0; i < gv.VisibleRowCount; i++)
                    {
                        gv.Selection.UnselectRow(i);
                    }
                }
                if (State == "Revart")
                {
                    for (int i = 0; i < gv.VisibleRowCount; i++)
                    {
                        if (gv.Selection.IsRowSelected(i))
                            gv.Selection.UnselectRow(i);
                        else
                            gv.Selection.SelectRow(i);
                    }
                }
            }
        }
        public DataTable GetComponentDate(string Component_ID, string Type)
        {
            ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
            proc.AddVarcharPara("@SelectedComponentList", 100, Component_ID);
            proc.AddVarcharPara("@ComponentType", 100, Type);
            proc.AddVarcharPara("@Action", 100, "GetComponentDateAddEdit");

            return proc.GetTable();
        }

        #endregion

        #region Invoice Mail
        public int SendMail(string Output)
        {
            int stat = 0;

            Employee_BL objemployeebal = new Employee_BL();
            DataTable dt2 = new DataTable();
            dt2 = objemployeebal.GetSystemsettingmail("Show Email in SI");
            if (Convert.ToString(dt2.Rows[0]["Variable_Value"]) == "Yes")
            {
                ExceptionLogging mailobj = new ExceptionLogging();
                EmailSenderHelperEL emailSenderSettings = new EmailSenderHelperEL();
                DataTable dt_EmailConfig = new DataTable();
                DataTable dt_EmailConfigpurchase = new DataTable();

                DataTable dt_Emailbodysubject = new DataTable();
                SalesOrderEmailTags fetchModel = new SalesOrderEmailTags();
                string Subject = "";
                string Body = "";
                string emailTo = "";
                int MailStatus = 0;
                var customerid = Convert.ToString(hdnCustomerId.Value);
                dt_EmailConfig = objemployeebal.Getemailids(customerid);
                string FilePath = string.Empty;
                string path = System.Web.HttpContext.Current.Server.MapPath("~");
                string path1 = string.Empty;
                string DesignPath = "";
                string fullpath = Server.MapPath("~");

                if (ConfigurationManager.AppSettings["IsDevelopedZone"] != null)
                {
                    FilePath = Server.MapPath("~/Reports/Reports/RepxReportDesign/ProjectInvoice/DocDesign/PDFFiles/" + "ProjectInvoice-Original-" + Output + ".pdf");
                }
                else
                {
                    FilePath = Server.MapPath("~/Reports/RepxReportDesign/ProjectInvoice/DocDesign/PDFFiles/" + "ProjectInvoice-Original-" + Output + ".pdf");
                }
                FilePath = FilePath.Replace("ERP.UI\\", "");

                string FileName = FilePath;
                if (dt_EmailConfig.Rows.Count > 0)
                {
                    emailTo = Convert.ToString(dt_EmailConfig.Rows[0]["eml_email"]);
                    dt_Emailbodysubject = objemployeebal.Getemailtemplates("17");

                    if (dt_Emailbodysubject.Rows.Count > 0)
                    {
                        Body = Convert.ToString(dt_Emailbodysubject.Rows[0]["body"]);
                        Subject = Convert.ToString(dt_Emailbodysubject.Rows[0]["subjct"]);

                        dt_EmailConfigpurchase = objemployeebal.Getemailtagsforpurchase(Output, "ProjectInvoiceEmailTags");

                        if (dt_EmailConfigpurchase.Rows.Count > 0)
                        {
                            fetchModel = DbHelpers.ToModel<SalesOrderEmailTags>(dt_EmailConfigpurchase);
                            Body = Employee_BL.GetFormattedString<SalesOrderEmailTags>(fetchModel, Body);
                            Subject = Employee_BL.GetFormattedString<SalesOrderEmailTags>(fetchModel, Subject);
                        }

                        emailSenderSettings = mailobj.GetEmailSettingsforAllreport(emailTo, "", "", FilePath, Body, Subject);

                        if (emailSenderSettings.IsSuccess)
                        {
                            string Message = "";
                            EmailSenderEL obj2 = new EmailSenderEL();
                            stat = SendEmailUL.sendMailInHtmlFormat(emailSenderSettings.ModelCast<EmailSenderEL>(), out Message);
                        }
                    }
                }
            }
            return stat;
        }
        #endregion

        protected void EntityServerModeDataSource_Selecting(object sender, DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs e)
        {
            e.KeyExpression = "cnt_internalid";

            // string connectionString = ConfigurationManager.ConnectionStrings["crmConnectionString"].ConnectionString;

            string connectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);

            ERPDataClassesDataContext dc = new ERPDataClassesDataContext(connectionString);
            var q = from d in dc.v_CustomerLists
                    orderby d.cnt_internalid descending
                    select d;
            e.QueryableSource = q;
        }

        [WebMethod]
        public static object SaveDocumentAddress(string OrderId, string TagDocType)
        {
            List<DocumentDetails> Detail = new List<DocumentDetails>();
            if (HttpContext.Current.Session["userid"] != null)
            {
                ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
                proc.AddVarcharPara("@Action", 500, "GetDocumentAddress");
                proc.AddVarcharPara("@TagDocType", 500, TagDocType);
                proc.AddVarcharPara("@OrderId", 100, OrderId);
                DataTable address = proc.GetTable();



                Detail = (from DataRow dr in address.Rows
                          select new DocumentDetails()

                          {
                              Type = Convert.ToString(dr["Type"]),
                              Address1 = Convert.ToString(dr["Address1"]),
                              Address2 = Convert.ToString(dr["Address2"]),
                              Address3 = Convert.ToString(dr["Address3"]),
                              CountryId = Convert.ToInt32(dr["CountryId"]),
                              CountryName = Convert.ToString(dr["CountryName"]),
                              StateId = Convert.ToInt32(dr["StateId"]),
                              StateName = Convert.ToString(dr["StateName"]),
                              StateCode = Convert.ToString(dr["StateCode"]),
                              CityId = Convert.ToInt32(dr["CityId"]),
                              CityName = Convert.ToString(dr["CityName"]),
                              PinId = Convert.ToInt32(dr["PinId"]),
                              PinCode = Convert.ToString(dr["PinCode"]),
                              AreaId = Convert.ToInt32(dr["AreaId"]),
                              AreaName = Convert.ToString(dr["AreaName"]),
                              ShipToPartyId = Convert.ToString(dr["ShipToPartyId"]),
                              ShipToPartyName = Convert.ToString(dr["ShipToPartyName"]),
                              Distance = Convert.ToDecimal(dr["Distance"]),
                              GSTIN = Convert.ToString(dr["GSTIN"]),
                              Landmark = Convert.ToString(dr["Landmark"]),
                              PosForGst = Convert.ToString(dr["PosForGst"])

                          }).ToList();
                return Detail;

            }
            return null;

        }


        [WebMethod]
        public static object SetProjectCode(string OrderId, string TagDocType)
        {
            List<DocumentDetails> Detail = new List<DocumentDetails>();
            if (HttpContext.Current.Session["userid"] != null)
            {
                ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
                proc.AddVarcharPara("@Action", 500, "SalesOrdertaggingProjectdata");
                proc.AddVarcharPara("@OrderId", 100, OrderId);
                proc.AddVarcharPara("@TagDocType", 500, TagDocType);
                DataTable address = proc.GetTable();



                Detail = (from DataRow dr in address.Rows
                          select new DocumentDetails()

                          {
                              ProjectId = Convert.ToInt64(dr["ProjectId"]),
                              ProjectCode = Convert.ToString(dr["ProjectCode"])
                          }).ToList();
                return Detail;

            }
            return null;

        }


        #region Set session For Packing Quantity
        [WebMethod]
        public static string SetSessionPacking(string list)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            List<ProductQuantity> finalResult = jsSerializer.Deserialize<List<ProductQuantity>>(list);
            HttpContext.Current.Session["SessionPackingDetails"] = finalResult;
            return null;

        }
        #endregion


        protected void callback_InlineRemarks_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string strSplitCommand = e.Parameter.Split('~')[0];
            string SrlNo = e.Parameter.Split('~')[1];
            string RemarksVal = e.Parameter.Split('~')[2];
            Remarks = new DataTable();


            callback_InlineRemarks.JSProperties["cpDisplayFocus"] = "";

            if (strSplitCommand == "RemarksFinal")
            {
                if (Session["InlineRemarks"] != null)
                {
                    Remarks = (DataTable)Session["InlineRemarks"];
                }
                else
                {
                    if (Remarks == null || Remarks.Rows.Count == 0)
                    {
                        Remarks.Columns.Add("SrlNo", typeof(string));
                        Remarks.Columns.Add("ProjectAdditionRemarks", typeof(string));

                    }


                }

                DataRow[] deletedRow = Remarks.Select("SrlNo=" + SrlNo);

                if (deletedRow.Length > 0)
                {
                    foreach (DataRow dr in deletedRow)
                    {
                        Remarks.Rows.Remove(dr);
                    }
                    Remarks.AcceptChanges();
                }


                Remarks.Rows.Add(SrlNo, RemarksVal);

                Session["InlineRemarks"] = Remarks;
            }


            //else if(strSplitCommand=="BindRemarks")
            //{

            //    DataTable dt = GetOrderData().Tables[1];

            //   //txtInlineRemarks.Text=


            //}

            else if (strSplitCommand == "DisplayRemarks")
            {
                DataTable Remarksdt = (DataTable)Session["InlineRemarks"];
                if (Remarksdt != null)
                {
                    DataView dvData = new DataView(Remarksdt);
                    dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                    if (dvData.Count > 0)
                        txtInlineRemarks.Text = dvData[0]["ProjectAdditionRemarks"].ToString();
                    else
                        txtInlineRemarks.Text = "";
                }

                callback_InlineRemarks.JSProperties["cpDisplayFocus"] = "DisplayRemarksFocus";
            }
            //else if (strSplitCommand=="RemarksDelete")
            //{
            //    DeleteUnsaveaddRemarks(SrlNo, RemarksVal);

            //}


        }

        //Tanmoy Hierarchy
        public void bindHierarchy()
        {
            DataTable hierarchydt = oDBEngine.GetDataTable("select 0 as ID ,'Select' as H_Name union select ID,H_Name from V_HIERARCHY");
            if (hierarchydt != null && hierarchydt.Rows.Count > 0)
            {
                ddlHierarchy.DataTextField = "H_Name";
                ddlHierarchy.DataValueField = "ID";
                ddlHierarchy.DataSource = hierarchydt;
                ddlHierarchy.DataBind();
            }
        }

        [WebMethod]
        public static object DocWiseSimilarProjectCheck(string quote_Id, string Doctype)
        {
            string returnValue = "0";
            if (HttpContext.Current.Session["userid"] != null)
            {
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();

                //quote_Id = quote_Id.Replace("'", "''");

                DataTable dtMainAccount = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("prc_CRMProjectInvoice_Details");
                proc.AddVarcharPara("@Action", 100, "GetSimilarProjectCheck");
                proc.AddVarcharPara("@TagDocType", 100, Doctype);
                proc.AddVarcharPara("@SelectedComponentList", 2000, quote_Id);
                proc.AddVarcharPara("@ReturnValue", 50, "0", QueryParameterDirection.Output);
                proc.RunActionQuery();
                returnValue = Convert.ToString(proc.GetParaValue("@ReturnValue"));

            }
            return returnValue;

        }


        [WebMethod]
        public static String getHierarchyID(string ProjID)
        {
            BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
            string Hierarchy_ID = "";
            DataTable dt2 = oDBEngine.GetDataTable("Select Hierarchy_ID from V_ProjectList where Proj_Code='" + ProjID + "'");
            if (dt2.Rows.Count > 0)
            {
                Hierarchy_ID = Convert.ToString(dt2.Rows[0]["Hierarchy_ID"]);
                return Hierarchy_ID;
            }
            else
            {
                Hierarchy_ID = "0";
                return Hierarchy_ID;
            }
        }
        //Tanmoy Hierarchy End

        protected void GridTCSdocs_DataBinding(object sender, EventArgs e)
        {
            if (Session["TcsGrid"] != null)
                GridTCSdocs.DataSource = (DataTable)Session["TcsGrid"];
        }

        protected void GridTCSdocs_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_TCSDetails");
            proc.AddVarcharPara("@CustomerID", 500, hdnCustomerId.Value);
            proc.AddVarcharPara("@Action", 500, "ShowTDSList");
            proc.AddVarcharPara("@branch_id", 500, ddl_Branch.SelectedValue);
            proc.AddVarcharPara("@trans_date", 500, dt_PLQuote.Text);
            proc.AddVarcharPara("@invoice_id", 500, hdnEditID.Value);
            proc.AddVarcharPara("@module_name", 500, "SI");
            proc.AddVarcharPara("@AddOrEdit", 500, hdAddOrEdit.Value);
            dt = proc.GetTable();

            Session["TcsGrid"] = dt;
            GridTCSdocs.DataBind();


        }
    }
}