﻿using System;
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
using System.Globalization;

namespace ERP.OMS.Management.Activities
{
    public partial class PurchaseReturn : ERP.OMS.ViewState_class.VSPage// System.Web.UI.Page
    {
        BusinessLogicLayer.Converter objConverter = new BusinessLogicLayer.Converter();


       // BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
        BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();


        clsDropDownList oclsDropDownList = new clsDropDownList();
        BusinessLogicLayer.Contact oContactGeneralBL = new BusinessLogicLayer.Contact();
        public EntityLayer.CommonELS.UserRightsForPage rights = new UserRightsForPage();
        static string ForJournalDate = null;
        SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
        PurchaseReturnBL objPurchaseReturnBL = new PurchaseReturnBL();
        CRMSalesDtlBL objCRMSalesDtlBL = new CRMSalesDtlBL();
        ERPDocPendingApprovalBL objERPDocPendingApproval = new ERPDocPendingApprovalBL();
        string UniqueQuotation = string.Empty;
        PurchaseOrderBL objPurchaseOrderBL = new PurchaseOrderBL();
        public string pageAccess = "";
        string userbranch = "";
        BusinessLogicLayer.RemarkCategoryBL reCat = new BusinessLogicLayer.RemarkCategoryBL();
        #region Code Checked and Modified
        protected void Page_PreInit(object sender, EventArgs e) // lead add
        {
            if (Request.QueryString.AllKeys.Contains("status"))
            {
                this.Page.MasterPageFile = "~/OMS/MasterPage/PopUp.Master";
                //divcross.Visible = false;
            }
            else
            {
                this.Page.MasterPageFile = "~/OMS/MasterPage/ERP.Master";
                //divcross.Visible = true;
            }
            if (!IsPostBack)
            {
                string sPath = Convert.ToString(HttpContext.Current.Request.Url);


                oDBEngine.Call_CheckPageaccessebility(sPath);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    ((GridViewDataComboBoxColumn)grid.Columns["ProductID"]).PropertiesComboBox.DataSource = GetProduct();
            //}
        }

        #endregion Code Checked and Modified  By Sam on 17022016

        public void IsExistsDocumentInERPDocApproveStatus(string strQuotationId)
        {
            string editable = "";
            string status = "";
            DataTable dt = new DataTable();
            int quoteid = Convert.ToInt32(strQuotationId);
            dt = objERPDocPendingApproval.IsExistsDocumentInERPDocApproveStatus(quoteid, 1);
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
                    ASPxButton2.Visible = false;
                }
                else
                {
                    lbl_quotestatusmsg.Visible = false;
                    btn_SaveRecords.Visible = true;
                    ASPxButton2.Visible = true;
                }
            }
        }
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
            DataTable udfCount = oDBEngine.GetDataTable("select 1 from tbl_master_remarksCategory rc where cat_applicablefor='PR'  and ( exists (select * from tbl_master_udfGroup where id=rc.cat_group_id and grp_isVisible=1) or rc.cat_group_id=0)");
            return udfCount.Rows.Count;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/management/Activities/PurchaseReturn.aspx");
            if (HttpContext.Current.Session["userid"] == null)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "SighOff", "<script>SignOff();</script>");
            }
            LastCompany.Value = Convert.ToString(Session["LastCompany"]);
            LastFinancialYear.Value = Convert.ToString(Session["LastFinYear"]);
            if (!IsPostBack)
            {
                CommonBL cbl = new CommonBL();
                string BackDatedEntryPurchaseGRN = cbl.GetSystemSettingsResult("BackDatedEntryPurchaseGRN");
                if (!String.IsNullOrEmpty(BackDatedEntryPurchaseGRN))
                {
                    if (BackDatedEntryPurchaseGRN.ToUpper().Trim() == "YES")
                    {
                        HdnBackDatedEntryPurchaseGRN.Value = "1";

                    }
                    else if (BackDatedEntryPurchaseGRN.ToUpper().Trim() == "NO")
                    {
                        HdnBackDatedEntryPurchaseGRN.Value = "0";

                    }
                }


                dsCustomer.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                CountrySelect.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                StateSelect.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                SelectCity.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                SelectArea.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                SelectPin.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                sqltaxDataSource.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                ProductDataSource.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);




                this.Session["LastCompanyPR"] = Session["LastCompany"];
                this.Session["LastFinYearPR"] = Session["LastFinYear"];
                GetProductData();
                //txt_PLQuoteNo.Enabled = false;
                ddl_numberingScheme.Focus();
                #region Sandip Section For Checking User Level for Allow Edit to logging User or Not
                GetEditablePermission();
                // PopulateCustomerDetail();
                SetFinYearCurrentDate();
                GetFinacialYearBasedQouteDate();
                if (Session["userbranchHierarchy"] != null)
                {
                    userbranch = Convert.ToString(Session["userbranchHierarchy"]);
                }
                GetAllDropDownDetailForSalesQuotation(userbranch);

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
                //Session["QuotationAddressDtl"] = null;
                Session["PR_BillingAddressLookup"] = null;
                Session["PR_ShippingAddressLookup"] = null;
                #endregion Session Null Initialization End


                //Purpose : Binding Batch Edit Grid
                //Name : Sudip 
                // Dated : 21-01-2017

                Session["PR_ReturnID"] = "";
                Session["PR_CustomerDetail"] = null;
                Session["PR_QuotationDetails"] = null;
                Session["PR_WarehouseData"] = null;
                Session["PR_QuotationTaxDetails"] = null;
                Session["LoopPRWarehouse"] = 1;
                Session["PR_TaxDetails"] = null;
                Session["PR_ActionType"] = "";

                Session["TaggingPurchaseInvoce"] = "";

                Session["PRwarehousedetailstemp"] = null;
                Session["PRwarehousedetailstempUpdate"] = null;
                Session["PRwarehousedetailstempDelete"] = null;
                Session["PurchaseReturnAddressDtl"] = null;
                PopulateGSTCSTVATCombo(DateTime.Now.ToString("yyyy-MM-dd"));
                PopulateChargeGSTCSTVATCombo(DateTime.Now.ToString("yyyy-MM-dd"));
                string strQuotationId = "";
                string strPurchaseReturnId = "";
                hdnCustomerId.Value = "";
                hdnPageStatus.Value = "first";

                //LoadGSTCSTVATCombo();
                //  Session["PRQuotationAddressDtl"] = null;
                Session["PR_QuotationAddressDtl"] = null;
                //string strLocalCurrency = Convert.ToString(Session["LocalCurrency"]);
                //if(strLocalCurrency!="")
                //{
                //    string[] currencyList = strLocalCurrency.Split(new string[] { "~" }, StringSplitOptions.None);
                //    string currency = currencyList[1];
                //    grid.Columns["TotalAmount"].Caption = "Total Amount in " + currency;
                //}

                if (Request.QueryString["key"] != null)
                {
                    if (Convert.ToString(Request.QueryString["key"]) != "ADD")
                    {
                        lblHeadTitle.Text = "Modify Purchase Return";
                        hdnPageStatus.Value = "update";
                        divScheme.Style.Add("display", "none");
                        strPurchaseReturnId = Convert.ToString(Request.QueryString["key"]);
                        Session["PR_KeyVal_InternalID"] = "SRQUOTE" + strPurchaseReturnId;


                        Keyval_internalId.Value = "PurchaseReturn" + strPurchaseReturnId;
                        #region Subhra Section
                        Session["PR_key_QutId"] = strPurchaseReturnId;
                        //if (Request.QueryString["status"] == null)
                        //{
                        //    IsExistsDocumentInERPDocApproveStatus(strPurchaseReturnId);
                        //}

                        #endregion End Section

                        #region Product, Quotation Tax, Warehouse

                        Session["PR_ReturnID"] = strPurchaseReturnId;
                        Session["PR_ActionType"] = "Edit";
                        SetPurchaseReturnDetails(strPurchaseReturnId);
                        //  DateTime quoteDate = Convert.ToDateTime(dt_PLQuote.Date.ToString("dd/MM/yyyy"));
                        Session["PR_TaxDetails"] = GetTaxDataWithGST(GetTaxData(dt_PLQuote.Date.ToString("yyyy-MM-dd")));

                        //  Session["PR_WarehouseData"] = GetQuotationWarehouseData();


                        DataTable Productdt = new DataTable();
                        if (Session["TaggingPurchaseInvoce"] == "1")
                        {

                            Productdt = objPurchaseReturnBL.GetPurchaseReturnTaggingProductData(strPurchaseReturnId, Convert.ToString(Session["LastFinYear"]), Convert.ToString(Session["LastCompany"])).Tables[0];
                        }
                        else
                        {
                            Productdt = objPurchaseReturnBL.GetPurchaseReturnProductData(strPurchaseReturnId, Convert.ToString(Session["LastFinYear"]), Convert.ToString(Session["LastCompany"])).Tables[0];
                        }

                        //DataTable Productdt = objPurchaseReturnBL.GetPurchaseReturnProductData(strPurchaseReturnId, Convert.ToString(Session["LastFinYear"]), Convert.ToString(Session["LastCompany"])).Tables[0];
                        Session["PR_QuotationDetails"] = Productdt;
                        grid.DataSource = GetQuotation(Productdt);
                        grid.DataBind();

                        Session["PR_QuotationAddressDtl"] = objPurchaseReturnBL.GetPurchaseReturnBillingAddress(strPurchaseReturnId); ;

                        #endregion

                        #region Debjyoti Get Tax Details in Edit Mode


                        if (GetQuotationTaxData().Tables[0] != null)
                            Session["PR_QuotationTaxDetails"] = GetQuotationTaxData().Tables[0];

                        if (GetQuotationEditedTaxData().Tables[0] != null)
                        {
                            DataTable quotetable = GetQuotationEditedTaxData().Tables[0];
                            if (quotetable == null)
                            {
                                CreateDataTaxTable();
                            }
                            else
                            {
                                Session["PR_FinalTaxRecord"] = quotetable;
                            }
                        }
                        #endregion Debjyoti Get Tax Details in Edit Mode

                        #region Samrat Roy -- Hide Save Button in Edit Mode
                        if (Request.QueryString["req"] != null && Request.QueryString["req"] == "V")
                        {
                            lblHeadTitle.Text = "View Purchase Return";
                            lbl_quotestatusmsg.Text = "*** View Mode Only";
                            btn_SaveRecords.Visible = false;
                            ASPxButton1.Visible = false;
                            lbl_quotestatusmsg.Visible = true;
                        }
                        #endregion
                    }
                    else
                    {
                        #region To Show By Default Cursor after SAVE AND NEW
                        if (Session["PR_SaveMode"] != null)  // it has been removed from coding side of Quotation list 
                        {
                            if (Session["PR_schemavalue"] != null)  // it has been removed from coding side of Quotation list 
                            {
                                ddl_numberingScheme.SelectedValue = Convert.ToString(Session["PR_schemavalue"]); // it has been removed from coding side of Quotation list 
                            }
                            if (Convert.ToString(Session["PR_SaveMode"]) == "A")
                            {
                                dt_PLQuote.Focus();
                                txt_PLQuoteNo.ClientEnabled = false;
                                txt_PLQuoteNo.Text = "Auto";
                            }
                            else if (Convert.ToString(Session["PR_SaveMode"]) == "M")
                            {
                                txt_PLQuoteNo.ClientEnabled = true;
                                txt_PLQuoteNo.Text = "";
                                txt_PLQuoteNo.Focus();

                            }
                        }
                        else
                        {
                            ddl_numberingScheme.Focus();
                        }
                        #endregion To Show By Default Cursor after SAVE AND NEW



                        Keyval_internalId.Value = "Add";
                        Session["PR_ActionType"] = "Add";
                        //ASPxButton2.Enabled = false;
                        Session["PR_TaxDetails"] = null;
                        CreateDataTaxTable();
                        lblHeadTitle.Text = "Add Purchase Return";
                        ddl_AmountAre.Value = "1";
                        ddl_VatGstCst.SelectedIndex = 0;
                        ddl_VatGstCst.ClientEnabled = false;
                        hdnPageStatus.Value = "Add";

                    }
                }

                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "GridCallBack()", true);
                GetPurchaseReturnSchemaLength();
            }
            else
            {
                // PopulateCustomerDetail();
            }

        }
        [WebMethod]
        public static String GetAvaiableStockCheckStockOut(string ProductID, string FinYear, string Company, string Branch)
        {
            //BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
            BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();


            DataTable dt = oDBEngine.GetDataTable("Select dbo.fn_CheckAvailableQuotation(" + Branch + ",'" + Convert.ToString(Company) + "','" + Convert.ToString(FinYear) + "'," + ProductID + ") as branchopenstock");
            string SalesRate = "Y";

            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (Convert.ToString(Math.Round(Convert.ToDecimal(dt.Rows[0]["branchopenstock"]), 2)) != "0.00")
                    {
                        SalesRate = "N";
                    }
                    else
                    {
                        SalesRate = "Y";
                    }
                }
                else
                {
                    SalesRate = "Y";
                }
            }
            catch
            {

            }

            //if (dt.Rows.Count <= 0)
            //{
            //    SalesRate = "N";
            //}
            //else
            //{
            //    SalesRate = "Y";
            //}

            return SalesRate;
        }

        public void GetPurchaseReturnSchemaLength()
        {
            DataTable Dt = new DataTable();
            Dt = objPurchaseReturnBL.GetSchemaLengthPurchaseReturn();
            if (Dt != null && Dt.Rows.Count > 0)
            {
                hdnSchemaLength.Value = Convert.ToString(Dt.Rows[0]["length"]);

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

        #region Product Details, Warehouse


        public void SetQuotationDetails()
        {
            DataTable QuotationEditdt = GetQuotationEditData();
            if (QuotationEditdt != null && QuotationEditdt.Rows.Count > 0)
            {
                string Branch_Id = Convert.ToString(QuotationEditdt.Rows[0]["Quote_BranchId"]);
                string Quote_Number = Convert.ToString(QuotationEditdt.Rows[0]["Quote_Number"]);
                string Quote_Date = Convert.ToString(QuotationEditdt.Rows[0]["Quote_Date"]);
                string Quote_Expiry = Convert.ToString(QuotationEditdt.Rows[0]["Quote_Expiry"]);
                string Customer_Id = Convert.ToString(QuotationEditdt.Rows[0]["Customer_Id"]);
                string Contact_Person_Id = Convert.ToString(QuotationEditdt.Rows[0]["Contact_Person_Id"]);
                string Quote_Reference = Convert.ToString(QuotationEditdt.Rows[0]["Quote_Reference"]);
                string Currency_Id = Convert.ToString(QuotationEditdt.Rows[0]["Currency_Id"]);
                string Currency_Conversion_Rate = Convert.ToString(QuotationEditdt.Rows[0]["Currency_Conversion_Rate"]);
                string Tax_Option = Convert.ToString(QuotationEditdt.Rows[0]["Tax_Option"]);
                string Tax_Code = Convert.ToString(QuotationEditdt.Rows[0]["Tax_Code"]);
                string Quote_SalesmanId = Convert.ToString(QuotationEditdt.Rows[0]["Quote_SalesmanId"]);
                string IsUsed = Convert.ToString(QuotationEditdt.Rows[0]["IsUsed"]);

                txt_PLQuoteNo.Text = Quote_Number;
                dt_PLQuote.Date = Convert.ToDateTime(Quote_Date);
                PopulateGSTCSTVATCombo(Convert.ToDateTime(Quote_Date).ToString("yyyy-MM-dd"));
                PopulateChargeGSTCSTVATCombo(Convert.ToDateTime(Quote_Date).ToString("yyyy-MM-dd"));
                dt_PlQuoteExpiry.Date = Convert.ToDateTime(Quote_Expiry);
                lookup_Customer.GridView.Selection.SelectRowByKey(Customer_Id);
                hdnCustomerId.Value = Customer_Id;
                TabPage page = ASPxPageControl1.TabPages.FindByName("[B]illing/Shipping");
                page.ClientEnabled = true;
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Jc", "<script  language='javascript'>SettingTabStatus();</script>", true);
                PopulateContactPersonOfCustomer(Customer_Id);
                cmbContactPerson.SelectedItem = cmbContactPerson.Items.FindByValue(Contact_Person_Id);
                txt_Refference.Text = Quote_Reference;
                ddl_Branch.SelectedValue = Branch_Id;
                ddl_SalesAgent.SelectedValue = Quote_SalesmanId;
                ddl_Currency.SelectedValue = Currency_Id;
                txt_Rate.Value = Currency_Conversion_Rate;
                txt_Rate.Text = Currency_Conversion_Rate;
                if (Tax_Option != "0") ddl_AmountAre.Value = Tax_Option;
                if (Tax_Code != "0")
                {
                    //ddl_VatGstCst.Value = Tax_Code;
                    PopulateGSTCSTVAT("2");
                    setValueForHeaderGST(ddl_VatGstCst, Tax_Code);
                }
                ddl_AmountAre.ClientEnabled = false;
                ddl_VatGstCst.ClientEnabled = false;

                if (IsUsed == "Y")
                {
                    dt_PLQuote.ClientEnabled = false;
                    lookup_Customer.ClientEnabled = false;
                }
                else
                {
                    dt_PLQuote.ClientEnabled = true;
                    lookup_Customer.ClientEnabled = true;
                }
            }
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
        public class Quotation
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
            public string ProductDisID { get; set; }
            public string Product { get; set; }
        }
        #endregion

        #region Product Details



        #region Code Checked and Modified  By Sam on 17022016

        public IEnumerable GetProduct()
        {
            List<Product> ProductList = new List<Product>();
           // BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
            BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine();

            DataTable DT = GetProductData().Tables[0];
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
        //public DataTable GetProductData()
        //{
        //    DataTable ds = new DataTable();
        //    ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
        //    proc.AddVarcharPara("@Action", 500, "ProductDetails");
        //    ds = proc.GetTable();
        //    return ds;
        //}


        public DataSet GetProductData()
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_Purchasechallan_Details");
            proc.AddVarcharPara("@Action", 500, "ProductDetails");
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchid", 500, Convert.ToString(Session["userbranchID"]));
            proc.AddVarcharPara("@campany_Id", 500, Convert.ToString(Session["LastCompany"]));
            ds = proc.GetDataSet();
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

        #endregion Code Checked and Modified  By Sam on 17022016


        //public DataSet GetProductData()
        //{
        //    DataSet ds = new DataSet();
        //    ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
        //    proc.AddVarcharPara("@Action", 500, "ProductDetails");
        //    ds = proc.GetDataSet();
        //    return ds;
        //}

        public DataTable GetQuotationEditData()
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "QuotationEditDetails");
            proc.AddVarcharPara("@QuotationID", 500, Convert.ToString(Session["QuotationID"]));
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 500, Convert.ToString(Session["userbranchID"]));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
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
            proc.AddVarcharPara("@QuotationID", 500, Convert.ToString(Session["QuotationID"]));
            proc.AddVarcharPara("@ProductID", 500, Convert.ToString(hdfProductID.Value));
            proc.AddVarcharPara("@WarehouseID", 500, WarehouseID);
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 500, Convert.ToString(Session["userbranchID"]));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            dt = proc.GetTable();
            return dt;
        }
        public DataTable GetSerialata(string WarehouseID, string BatchID)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "GetSerialByProductIDWarehouseBatch");
            proc.AddVarcharPara("@QuotationID", 500, Convert.ToString(Session["QuotationID"]));
            proc.AddVarcharPara("@ProductID", 500, Convert.ToString(hdfProductID.Value));
            proc.AddVarcharPara("@BatchID", 500, BatchID);
            proc.AddVarcharPara("@WarehouseID", 500, WarehouseID);
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 500, Convert.ToString(Session["userbranchID"]));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            dt = proc.GetTable();
            return dt;
        }
        public IEnumerable GetQuotation()
        {
            List<Quotation> QuotationList = new List<Quotation>();
            DataTable Quotationdt = GetQuotationData().Tables[0];

            for (int i = 0; i < Quotationdt.Rows.Count; i++)
            {
                Quotation Quotations = new Quotation();

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
                QuotationList.Add(Quotations);
            }

            return QuotationList;
        }
        public IEnumerable GetQuotation(DataTable Quotationdt)
        {
            List<Quotation> QuotationList = new List<Quotation>();

            if (Quotationdt != null && Quotationdt.Rows.Count > 0)
            {
                for (int i = 0; i < Quotationdt.Rows.Count; i++)
                {
                    Quotation Quotations = new Quotation();

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


                    Quotations.ProductDisID = Convert.ToString(Quotationdt.Rows[i]["ProductDisID"]);
                    Quotations.Product = Convert.ToString(Quotationdt.Rows[i]["Product"]);
                    QuotationList.Add(Quotations);
                }
            }

            return QuotationList;
        }
        public DataSet GetQuotationData()
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "QuotationDetails");
            proc.AddVarcharPara("@QuotationID", 500, Convert.ToString(Session["PR_ReturnID"]));
            ds = proc.GetDataSet();
            return ds;
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
            //else if (e.Column.FieldName == "SalePrice")
            //{
            //    e.Editor.Enabled = true;
            //}
            else if (e.Column.FieldName == "Amount")
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

            string validate = "";
            grid.JSProperties["cpQuotationNo"] = "";
            grid.JSProperties["cpSaveSuccessOrFail"] = "";

            if (Session["PR_QuotationDetails"] != null)
            {
                Quotationdt = (DataTable)Session["PR_QuotationDetails"];
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
                Quotationdt.Columns.Add("ProductDisID", typeof(string));
                Quotationdt.Columns.Add("Product", typeof(string));
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


                    string ComponentNumber = (Convert.ToString(args.NewValues["ComponentNumber"]) != "") ? Convert.ToString(args.NewValues["ComponentNumber"]) : "0";
                    string TotalQty = (Convert.ToString(args.NewValues["TotalQty"]) != "") ? Convert.ToString(args.NewValues["TotalQty"]) : "0";


                    string BalanceQty = (Convert.ToString(args.NewValues["BalanceQty"]) != "" && Convert.ToString(args.NewValues["BalanceQty"]) != "NaN") ? Convert.ToString(args.NewValues["BalanceQty"]) : "0";

                    string IsComponentProduct = (Convert.ToString(args.NewValues["IsComponentProduct"]) != "") ? Convert.ToString(args.NewValues["IsComponentProduct"]) : "0";

                    string ProductDisID = (Convert.ToString(args.NewValues["ProductDisID"]) != "") ? Convert.ToString(args.NewValues["ProductDisID"]) : "0";
                    string Product = (Convert.ToString(args.NewValues["Product"]) != "") ? Convert.ToString(args.NewValues["Product"]) : "0";
                    Quotationdt.Rows.Add(SrlNo, "0", ProductDetails, Description, Quantity, UOM, Warehouse, StockQuantity, StockUOM, SalePrice, Discount, Amount, TaxAmount, TotalAmount, "I", ProductName, ComponentID, ComponentNumber, TotalQty, BalanceQty, IsComponentProduct, ProductDisID, Product);
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
                        string ComponentNumber = (Convert.ToString(args.NewValues["ComponentNumber"]) != "") ? Convert.ToString(args.NewValues["ComponentNumber"]) : "0";
                        string TotalQty = (Convert.ToString(args.NewValues["TotalQty"]) != "") ? Convert.ToString(args.NewValues["TotalQty"]) : "0";
                        string BalanceQty = (Convert.ToString(args.NewValues["BalanceQty"]) != "" && Convert.ToString(args.NewValues["BalanceQty"]) != "NAN") ? Convert.ToString(args.NewValues["BalanceQty"]) : "0";
                        string IsComponentProduct = (Convert.ToString(args.NewValues["IsComponentProduct"]) != "") ? Convert.ToString(args.NewValues["IsComponentProduct"]) : "N";
                        string ProductDisID = (Convert.ToString(args.NewValues["ProductDisID"]) != "") ? Convert.ToString(args.NewValues["ProductDisID"]) : "0";

                        string[] ProductDList = ProductDisID.Split(new string[] { "||@||" }, StringSplitOptions.None);
                        ProductDisID = Convert.ToString(ProductDList[0]);


                        string Product = (Convert.ToString(args.NewValues["Product"]) != "") ? Convert.ToString(args.NewValues["Product"]) : "N";
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
                                drr["ComponentNumber"] = ComponentNumber;
                                drr["TotalQty"] = TotalQty;
                                drr["BalanceQty"] = BalanceQty;
                                drr["IsComponentProduct"] = IsComponentProduct;
                                drr["ProductDisID"] = ProductDisID;
                                drr["Product"] = Product;

                                break;
                            }
                        }

                        if (Isexists == false)
                        {
                            Quotationdt.Rows.Add(SrlNo, QuotationID, ProductDetails, Description, Quantity, UOM, Warehouse, StockQuantity, StockUOM, SalePrice, Discount, Amount, TaxAmount, TotalAmount, "U", ProductName, ComponentID, ComponentNumber, TotalQty, BalanceQty, IsComponentProduct, ProductDisID, Product);
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

                //DeleteWarehouse(SrlNo);
                DeleteTaxDetails(SrlNo);

                if (QuotationID.Contains("~") != true)
                {
                    Quotationdt.Rows.Add("0", QuotationID, "0", "", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "D", "", "0", "0", "0", "0", "0", "0", "0");
                }
            }

            ///////////////////////

            string strDeleteSrlNo = Convert.ToString(hdnDeleteSrlNo.Value);
            if (strDeleteSrlNo != "" && strDeleteSrlNo != "0")
            {
                // DeleteWarehouse(strDeleteSrlNo);
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

                // UpdateWarehouse(oldSrlNo, newSrlNo);
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

            Session["PR_QuotationDetails"] = Quotationdt;

            //////////////////////


            if (IsDeleteFrom != "D")
            {
                string InvoiceComponentDate = string.Empty, InvoiceComponent = "";

                string ActionType = Convert.ToString(Session["PR_ActionType"]);
                string MainInvoiceID = Convert.ToString(Session["PR_ReturnID"]);


                string comtype = "PGRN";
               // string comtype = Convert.ToString(rdl_SaleInvoice.SelectedValue);
                string strSchemeType = Convert.ToString(ddl_numberingScheme.SelectedValue);
                string strInvoiceNo = Convert.ToString(txt_PLQuoteNo.Text);
                string strInvoiceDate = Convert.ToString(dt_PLQuote.Date);
                string strBranch = Convert.ToString(ddl_Branch.SelectedValue);

                string strCustomer = Convert.ToString(hdfLookupCustomer.Value);
                string strContactName = Convert.ToString(cmbContactPerson.Value);
                string Reference = Convert.ToString(txt_Refference.Text);
                string strAgents = Convert.ToString(ddl_SalesAgent.SelectedValue);


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
                string strCashBank = Convert.ToString(ddlCashBank.Value);
                string strDueDate = Convert.ToString(dt_SaleInvoiceDue.Date);

                string strCurrency = Convert.ToString(ddl_Currency.SelectedValue);
                string strRate = Convert.ToString(txt_Rate.Value);
                string strTaxType = Convert.ToString(ddl_AmountAre.Value);
                string strTaxCode = Convert.ToString(ddl_VatGstCst.Value).Split('~')[0];

                string CompID = Convert.ToString(HttpContext.Current.Session["LastCompany"]);
                string currency = Convert.ToString(HttpContext.Current.Session["LocalCurrency"]);
                string[] ActCurrency = currency.Split('~');
                int BaseCurrencyId = Convert.ToInt32(ActCurrency[0]);
                int ConvertedCurrencyId = Convert.ToInt32(strCurrency);
                ///  int ConvertedCurrencyId = BaseCurrencyId;

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
                if (Session["PR_FinalTaxRecord"] != null)
                {
                    TaxDetailTable = (DataTable)Session["PR_FinalTaxRecord"];
                }

                // DataTable of Warehouse

                DataTable tempWarehousedt = new DataTable();
                if (Session["PR_WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["PR_WarehouseData"];
                    tempWarehousedt = Warehousedt.DefaultView.ToTable();
                    // tempWarehousedt = Warehousedt.DefaultView.ToTable(false, "LoopID", "WarehouseID", "TotalQuantity", "BatchID", "SerialID");
                }
                else
                {

                    tempWarehousedt.Columns.Add("SrlNo", typeof(string));
                    tempWarehousedt.Columns.Add("WarehouseID", typeof(string));
                    tempWarehousedt.Columns.Add("TotalQuantity", typeof(string));
                    tempWarehousedt.Columns.Add("BatchID", typeof(string));
                    tempWarehousedt.Columns.Add("SerialID", typeof(string));
                }

                // End

                // DataTable Of Quotation Tax Details 

                DataTable TaxDetailsdt = new DataTable();
                if (Session["PR_TaxDetails"] != null)
                {
                    TaxDetailsdt = (DataTable)Session["PR_TaxDetails"];
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
                tempBillAddress = BillingShippingControl.SaveBillingShippingControlData();

                #region #### Old_Process ####
                ////// DataTable Of Billing Address

                ////DataTable tempBillAddress = new DataTable();
                ////if (Session["PR_QuotationAddressDtl"] != null)
                ////{
                ////    tempBillAddress = (DataTable)Session["PR_QuotationAddressDtl"];
                ////}
                ////else
                ////{
                ////    tempBillAddress = StoreQuotationAddressDetail();
                ////}

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
                    validate = checkNMakeJVCode(strInvoiceNo, Convert.ToInt32(SchemeList[0]));
                }

                foreach (DataRow dr in tempQuotation.Rows)
                {
                    string strSrlNo = Convert.ToString(dr["SrlNo"]);
                    decimal strProductQuantity = Convert.ToDecimal(dr["Quantity"]);

                    decimal strWarehouseQuantity = 0;
                 //GetQuantityBaseOnProduct(strSrlNo, ref strWarehouseQuantity);


                 //if (Convert.ToString(Request.QueryString["key"]) != "ADD")
                 //{
                 //    if (Session["PR_WarehouseData"] != null)
                 //    {
                 //        if (strProductQuantity != strWarehouseQuantity)
                 //        {
                 //            validate = "checkWarehouse";
                 //            grid.JSProperties["cpProductSrlIDCheck"] = strSrlNo;
                 //            break;
                 //        }
                 //    }
                 //}
                 //else
                 //{

                 //    if (strProductQuantity != strWarehouseQuantity)
                 //    {
                 //        validate = "checkWarehouse";
                 //        grid.JSProperties["cpProductSrlIDCheck"] = strSrlNo;
                 //        break;
                 //    }

                 //}

                    //if (strWarehouseQuantity != 0)
                    //{
                    //    if (strProductQuantity != strWarehouseQuantity)
                    //    {
                    //        validate = "checkWarehouse";
                    //        grid.JSProperties["cpProductSrlIDCheck"] = strSrlNo;
                    //        break;
                    //    }
                    //}
                }

                var duplicateRecords = tempQuotation.AsEnumerable()
               .GroupBy(r => r["ProductID"]) //coloumn name which has the duplicate values
               .Where(gr => gr.Count() > 1)
               .Select(g => g.Key);

                foreach (var d in duplicateRecords)
                {
                    validate = "duplicateProduct";
                }

                foreach (DataRow dr in tempQuotation.Rows)
                {
                    decimal ProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                    string Status = Convert.ToString(dr["Status"]);

                    if (Status != "D")
                    {
                        if (ProductQuantity == 0)
                        {
                            validate = "nullQuantity";
                            break;
                        }
                    }
                }

                decimal ProductAmount = 0;
                foreach (DataRow dr in tempQuotation.Rows)
                {
                    ProductAmount = ProductAmount + Convert.ToDecimal(dr["Amount"]);
                }

                if (ProductAmount == 0)
                {
                    validate = "nullAmount";
                }

                //// ############# Added By : Samrat Roy -- 02/05/2017 -- To check Transporter Mandatory Control 
                //BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);

                BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine();

                DataTable DT = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='Transporter_PRMandatory' AND IsActive=1");
                if (DT != null && DT.Rows.Count > 0)
                {
                    string IsMandatory = Convert.ToString(DT.Rows[0]["Variable_Value"]).Trim();
                    
                    if (Convert.ToString(Session["TransporterVisibilty"]).Trim() == "Yes")
                    {
                        if (IsMandatory == "Yes")
                        {
                            if (VehicleDetailsControl.GetControlValue("cmbTransporter") == "")
                            {
                                validate = "transporteMandatory";
                            }
                        }
                    }
                }

                //// ############# Added By : Samrat Roy -- 02/05/2017 -- To check Transporter Mandatory Control 

                if (validate == "outrange" || validate == "duplicate" || validate == "checkWarehouse" || validate == "duplicateProduct" || validate == "nullAmount" || validate == "nullQuantity" || validate == "transporteMandatory")
                {
                    grid.JSProperties["cpSaveSuccessOrFail"] = validate;
                }
                else
                {
                    grid.JSProperties["cpQuotationNo"] = UniqueQuotation;

                    #region To Show By Default Cursor after SAVE AND NEW
                    if (Convert.ToString(Session["PR_ActionType"]) == "Add") // session has been removed from quotation list page working good
                    {
                        string[] schemaid = new string[] { };
                        string schemavalue = ddl_numberingScheme.SelectedValue;
                        Session["PR_schemavalue"] = schemavalue;        // session has been removed from quotation list page working good
                        schemaid = ddl_numberingScheme.SelectedValue.Split('~');

                        string schematype = schemaid[1];
                        if (hdnRefreshType.Value == "N")
                        {
                            if (schematype == "1")
                            {
                                Session["PR_SaveMode"] = "A";
                            }
                            else
                            {
                                Session["PR_SaveMode"] = "M";
                            }
                        }
                    }

                    #endregion

                    #region Subhabrata Section Start

                    int strIsComplete = 0, strQuoteID = 0;

                    ModifyPurchaseReturn(MainInvoiceID, strSchemeType, UniqueQuotation, strInvoiceDate, strCustomer, strContactName,
                                    Reference, strBranch, strAgents, strCurrency, strRate, strTaxType, strTaxCode,
                                    InvoiceComponent, InvoiceComponentDate, strCashBank, strDueDate,
                                    tempQuotation, TaxDetailTable, tempWarehousedt, tempTaxDetailsdt, tempBillAddress,
                                    approveStatus, ActionType,comtype, ref strIsComplete, ref strQuoteID);
                    //####### Coded By Samrat Roy For Custom Control Data Process #########

                    if (!string.IsNullOrEmpty(hfControlData.Value))
                    {
                        CommonBL objCommonBL = new CommonBL();
                        objCommonBL.InsertTransporterControlDetails(Convert.ToInt32(strQuoteID), "PR", hfControlData.Value, Convert.ToString(HttpContext.Current.Session["userid"]));
                    }

                    //Udf Add mode
                    DataTable udfTable = (DataTable)Session["UdfDataOnAdd"];
                    if (udfTable != null)
                        Session["UdfDataOnAdd"] = reCat.insertRemarksCategoryAddMode("PR", "PurchaseReturn" + Convert.ToString(strQuoteID), udfTable, Convert.ToString(Session["userid"]));


                    if (strIsComplete == 1)
                    {
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



                        Employee_BL objemployeebal = new Employee_BL();
                        int MailStatus = 0;
                        string AssignedTo_Email = string.Empty;
                        string ReceiverEmail = string.Empty;
                        decimal Level1_User = Convert.ToDecimal(Session["userid"]);
                        decimal Level2User = Convert.ToDecimal(Session["userid"]);
                        decimal Level3User = Convert.ToDecimal(Session["userid"]);
                        bool L1 = false;
                        bool L2 = false;
                        bool L3 = false;
                        string ActivityName = string.Empty;

                        DataTable dtbl_AssignedTo = new DataTable();
                        DataTable dtbl_AssignedBy = new DataTable();
                        DataTable dtEmail_To = new DataTable();
                        DataTable dt_EmailConfig = new DataTable();
                        DataTable dt_ActivityName = new DataTable();
                        DataTable Dt_LevelUser = new DataTable();

                        dt_EmailConfig = objemployeebal.GetEmailAccountConfigDetails("", 1); // Checked fetch datatatable of email setup with action 1 param
                        Dt_LevelUser = objemployeebal.GetAllLevelUsers("1", 12);

                        ActivityName = UniqueQuotation;

                        if (Dt_LevelUser != null && string.IsNullOrEmpty(approveStatus))
                        {
                            L1 = true;
                            dtEmail_To = objemployeebal.GetEmailLevelUsersWise(1, 11, 1);
                        }
                        else if (Dt_LevelUser != null && Dt_LevelUser.Rows[0].Field<decimal>("Level1_user_id") == Level1_User && approveStatus == "2")
                        {
                            L2 = true;
                            dtEmail_To = objemployeebal.GetEmailLevelUsersWise(1, 11, 2);
                        }
                        else if (Dt_LevelUser != null && Dt_LevelUser.Rows[0].Field<decimal>("Level2_user_id") == Level2User && approveStatus == "2")
                        {
                            L3 = true;
                            dtEmail_To = objemployeebal.GetEmailLevelUsersWise(1, 11, 3);
                        }

                        if (dtEmail_To != null && dtEmail_To.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtEmail_To.Rows[0].Field<string>("Email_Id")))
                            {
                                ReceiverEmail = Convert.ToString(dtEmail_To.Rows[0].Field<string>("Email_Id"));
                            }
                            else
                            {
                                ReceiverEmail = "";
                            }
                        }



                        ListDictionary replacements = new ListDictionary();
                        if (dtEmail_To.Rows.Count > 0)
                        {
                            replacements.Add("<%Approver%>", Convert.ToString(dtEmail_To.Rows[0].Field<string>("Approver")));
                        }
                        else
                        {
                            replacements.Add("<%Approver%>", "");
                        }
                        replacements.Add("<%QuotationNo%>", UniqueQuotation);
                        //ExceptionLogging.SendExceptionMail(ex, Convert.ToInt32(lineNumber));

                        if (!string.IsNullOrEmpty(ReceiverEmail))
                        {
                            //ExceptionLogging.SendEmailToAssigneeByUser(ReceiverEmail, "", replacements, "~/OMS/EmailTemplates/EmailSendToAssigneeByUserID.html", dt_EmailConfig, ActivityName,4);
                            MailStatus = ExceptionLogging.SendEmailToAssigneeByUser(ReceiverEmail, "", replacements, dt_EmailConfig, ActivityName, 12);
                        }
                    }
                    else if (strIsComplete == 2)
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "quantityTagged";
                    }
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


        public void GetQuantityBaseOnProduct(string strProductSrlNo, ref decimal WarehouseQty)
        {
            decimal sum = 0;

            DataTable Warehousedt = new DataTable();
            if (Session["PR_WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["PR_WarehouseData"];
                for (int i = 0; i < Warehousedt.Rows.Count; i++)
                {
                    DataRow dr = Warehousedt.Rows[i];
                    string Product_SrlNo = Convert.ToString(dr["SrlNo"]);

                    if (strProductSrlNo == Product_SrlNo)
                    {
                        string strQuantity = (Convert.ToString(dr["Quantity"]) != "") ? Convert.ToString(dr["Quantity"]) : "0";
                        var weight = Decimal.Parse(Regex.Match(strQuantity, "[0-9]*\\.*[0-9]*").Value);

                        sum = sum + Convert.ToDecimal(weight);
                    }
                }
            }

            WarehouseQty = sum;
        }
        protected void grid_DataBinding(object sender, EventArgs e)
        {


            if (Session["PR_QuotationDetails"] != null)
            {
                DataTable Quotationdt = (DataTable)Session["PR_QuotationDetails"];

                if (Quotationdt.Rows.Count > 0)
                {
                    DataView dvData = new DataView(Quotationdt);
                    dvData.RowFilter = "Status <> 'D'";
                    grid.DataSource = GetQuotation(dvData.ToTable());
                }
                else
                {
                    grid.DataSource = null;
                    // grid.DataBind();
                }

            }
            else
            {
                grid.DataSource = null;
                // grid.DataBind();
            }
        }
        protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string strSplitCommand = e.Parameters.Split('~')[0];
            if (strSplitCommand == "Display")
            {
                string IsDeleteFrom = Convert.ToString(hdfIsDelete.Value);
                if (IsDeleteFrom == "D")
                {
                    DataTable Quotationdt = (DataTable)Session["PR_QuotationDetails"];
                    grid.DataSource = GetQuotation(Quotationdt);
                    grid.DataBind();
                }
            }
            else if (strSplitCommand == "GridBlank")
            {
                Session["PR_QuotationDetails"] = null;
                Session["PR_QuotationAddressDtl"] = null;
                Session["PR_WarehouseData"] = null;
                Session["PRwarehousedetailstemp"] = null;
                Session["PR_QuotationTaxDetails"] = null;
                grid.DataSource = null;
                grid.DataBind();

                grid.JSProperties["cpGridBlank"] = "1";
            }
            else if (strSplitCommand == "RemoveDisplay")
            {
                DataTable purchaseReturndt = new DataTable();
                if (Session["PR_QuotationDetails"] != null)
                {
                    purchaseReturndt = (DataTable)Session["PR_QuotationDetails"];
                }

                DataRow[] dr = purchaseReturndt.Select();
                foreach (DataRow row in dr)
                {
                    purchaseReturndt.Rows.Remove(row);
                }

                purchaseReturndt.AcceptChanges();
                Session["PR_QuotationDetails"] = purchaseReturndt;
                grid.DataBind();

                grid.JSProperties["cpRemoveProductInvoice"] = "valid";
                //salesReturndt.AcceptChanges();
                //Session["SR_QuotationDetails"] = salesReturndt;
                //grid.DataSource = salesReturndt;
                //grid.DataBind();


            }
            else if (strSplitCommand == "CurrencyChangeDisplay")
            {
                if (Session["PR_QuotationDetails"] != null)
                {
                    DataTable Quotationdt = (DataTable)Session["PR_QuotationDetails"];

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

                            //if (Convert.ToDecimal(dr["SalePrice"]) == 0)
                            //{
                            //    strSalePrice = ProductDetailsList[6];
                            //}
                            //else
                            //{
                            //    strSalePrice = Convert.ToDecimal(dr["SalePrice"]);
                            //}
                            //strSalePrice = ProductDetailsList[6];

                            decimal SalePrice = Convert.ToDecimal(strSalePrice) / strRate;
                            string Amount = Convert.ToString(Math.Round((Convert.ToDecimal(QuantityValue) * Convert.ToDecimal(strFactor) * SalePrice), 2));
                            string amountAfterDiscount = Convert.ToString(Math.Round((Convert.ToDecimal(Amount) - ((Convert.ToDecimal(Discount) * Convert.ToDecimal(Amount)) / 100)), 2));


                            dr["SalePrice"] = Convert.ToString(Math.Round(SalePrice, 2));
                            dr["Amount"] = amountAfterDiscount;
                            //dr["TaxAmount"] = amountAfterDiscount;
                            //dr["TotalAmount"] = TotalAmount;
                            dr["TaxAmount"] = ReCalculateTaxAmount(Convert.ToString(dr["SrlNo"]), Convert.ToDouble(amountAfterDiscount));
                            dr["TotalAmount"] = Convert.ToDecimal(dr["Amount"]) + Convert.ToDecimal(dr["TaxAmount"]);
                        }
                    }

                    Session["PR_QuotationDetails"] = Quotationdt;

                    DataView dvData = new DataView(Quotationdt);
                    dvData.RowFilter = "Status <> 'D'";

                    grid.DataSource = GetQuotation(dvData.ToTable());
                    grid.DataBind();
                }
            }
            else if (strSplitCommand == "DateChangeDisplay")
            {
                if (Session["PR_QuotationDetails"] != null)
                {
                    DataTable Quotationdt = (DataTable)Session["PR_QuotationDetails"];

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

                    Session["PR_QuotationDetails"] = Quotationdt;

                    DataView dvData = new DataView(Quotationdt);
                    dvData.RowFilter = "Status <> 'D'";

                    grid.DataSource = GetQuotation(dvData.ToTable());
                    grid.DataBind();
                }
            }
            else if (strSplitCommand == "BindGridOnQuotation")
            {
                ASPxGridView gv = sender as ASPxGridView;
                string command = e.Parameters.ToString();
                string State = Convert.ToString(e.Parameters.Split('~')[1]);
                String QuoComponent1 = "";
                string Product_id1 = "";
                string InvoiceDetails_Id = "";
                for (int i = 0; i < grid_Products.GetSelectedFieldValues("Quotation_No").Count; i++)
                {

                    QuoComponent1 += "," + Convert.ToString(grid_Products.GetSelectedFieldValues("Quotation_No")[i]);
                    Product_id1 += "," + Convert.ToString(grid_Products.GetSelectedFieldValues("gvColProduct")[i]);
                    InvoiceDetails_Id += "," + Convert.ToString(grid_Products.GetSelectedFieldValues("ComponentDetailsID")[i]);
                }
                QuoComponent1 = QuoComponent1.TrimStart(',');
                Product_id1 = Product_id1.TrimStart(',');
                string Quote_Nos = Convert.ToString(e.Parameters.Split('~')[1]);
                string companyId = Convert.ToString(HttpContext.Current.Session["LastCompany"]);

                string fin_year = Convert.ToString(HttpContext.Current.Session["LastFinYear"]);
                if (Quote_Nos != "$")
                {

                    DataTable dt_QuotationDetails = new DataTable();
                    string IdKey = Convert.ToString(Request.QueryString["key"]);
                    if (!string.IsNullOrEmpty(IdKey))
                    {
                        if (IdKey != "ADD")
                        {
                            dt_QuotationDetails = objPurchaseReturnBL.GetIndentDetailsForPOGridBind(QuoComponent1, IdKey, Product_id1, companyId, fin_year).Tables[0];
                        }
                        else
                        {
                            dt_QuotationDetails = objPurchaseReturnBL.GetIndentDetailsForPOGridBind(QuoComponent1, IdKey, Product_id1, companyId, fin_year).Tables[0];
                        }

                    }
                    else
                    {
                        dt_QuotationDetails = objPurchaseReturnBL.GetIndentDetailsForPOGridBind(QuoComponent1, IdKey, Product_id1, companyId, fin_year).Tables[0];
                    }
                    // Session["OrderDetails"] = null;

                    Session["PR_QuotationDetails"] = null;
                    grid.DataSource = GetInvoiceInfo(dt_QuotationDetails, IdKey);
                    grid.DataBind();
                }
                else
                {
                    grid.DataSource = null;
                    grid.DataBind();
                }

                //Session["PR_QuotationAddressDtl"] = GetComponentEditedAddressData(InvoiceDetails_Id, "PI");
                //if (Session["PR_QuotationAddressDtl"] != null)
                //{
                //    DataTable dt = (DataTable)Session["PR_QuotationAddressDtl"];
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        if (dt.Rows.Count == 2)
                //        {
                //            if (Convert.ToString(dt.Rows[0]["ReturnAdd_addressType"]) == "Shipping")
                //            {
                //                string countryid = Convert.ToString(dt.Rows[0]["ReturnAdd_countryId"]);
                //                CmbCountry1.Value = countryid;
                //                FillStateCombo(CmbState1, Convert.ToInt32(countryid));
                //                string stateid = Convert.ToString(dt.Rows[0]["ReturnAdd_stateId"]);
                //                CmbState1.Value = stateid;
                //            }
                //            else if (Convert.ToString(dt.Rows[1]["ReturnAdd_addressType"]) == "Shipping")
                //            {
                //                string countryid = Convert.ToString(dt.Rows[0]["ReturnAdd_countryId"]);
                //                CmbCountry1.Value = countryid;
                //                FillStateCombo(CmbState1, Convert.ToInt32(countryid));
                //                string stateid = Convert.ToString(dt.Rows[0]["ReturnAdd_stateId"]);
                //                CmbState1.Value = stateid;
                //            }
                //        }
                //        else if (dt.Rows.Count == 1)
                //        {
                //            if (Convert.ToString(dt.Rows[0]["ReturnAdd_addressType"]) == "Shipping")
                //            {
                //                string countryid = Convert.ToString(dt.Rows[0]["ReturnAdd_countryId"]);
                //                CmbCountry1.Value = countryid;
                //                FillStateCombo(CmbState1, Convert.ToInt32(countryid));
                //                string stateid = Convert.ToString(dt.Rows[0]["ReturnAdd_stateId"]);
                //                CmbState1.Value = stateid;
                //            }

                //        }
                //    }
                //}
            }

        }
        public void ModifyPurchaseReturn(string QuotationID, string strSchemeType, string strQuoteNo, string strQuoteDate, string strCustomer, string strContactName,
                                    string Reference, string strBranch, string strAgents, string strCurrency, string strRate, string strTaxType, string strTaxCode,
                                    string strInvoiceComponent, string strInvoiceComponentDate, string strCashBank, string strDueDate,
                                    DataTable Productdt, DataTable TaxDetailTable, DataTable Warehousedt, DataTable SalesReturnTaxdt, DataTable BillAddressdt, string approveStatus, string ActionType, string ComType,
                                    ref int strIsComplete, ref int strInvoiceID)
        {
            try
            {
                DataSet dsInst = new DataSet();


                //SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["DBConnectionDefault"]);

                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));




                SqlCommand cmd = new SqlCommand("prc_CRMPurchaseReturn", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", ActionType);
                cmd.Parameters.AddWithValue("@approveStatus", approveStatus);

                cmd.Parameters.AddWithValue("@PurchaseReturnID", QuotationID);
                cmd.Parameters.AddWithValue("@SalesReturnNo", strQuoteNo);
                cmd.Parameters.AddWithValue("@SalesReturnDate", Convert.ToDateTime(strQuoteDate));
                cmd.Parameters.AddWithValue("@BranchID", strBranch);

                cmd.Parameters.AddWithValue("@CustomerID", strCustomer);
                cmd.Parameters.AddWithValue("@ContactPerson", strContactName);
                cmd.Parameters.AddWithValue("@Reference", Reference);
                cmd.Parameters.AddWithValue("@Agents", strAgents);

                cmd.Parameters.AddWithValue("@InvoiceComponent", strInvoiceComponent);
                if (String.IsNullOrEmpty(strInvoiceComponent))
                { cmd.Parameters.AddWithValue("@ComponentType", ""); }
                else {
                    if (ComType=="PI")
                    { cmd.Parameters.AddWithValue("@ComponentType", "PI"); }
                    else { cmd.Parameters.AddWithValue("@ComponentType", "PGRN"); }
                   
                
                }


                if (String.IsNullOrEmpty(strInvoiceComponentDate))
                { cmd.Parameters.AddWithValue("@InvoiceComponentDate", strInvoiceComponentDate); }
                else
                {
                    DateTime dt = DateTime.ParseExact(strInvoiceComponentDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    cmd.Parameters.AddWithValue("@InvoiceComponentDate", Convert.ToDateTime(dt).ToString("yyyy-MM-dd"));

                }

                cmd.Parameters.AddWithValue("@CashBank", strCashBank);
                cmd.Parameters.AddWithValue("@DueDate", Convert.ToDateTime(strDueDate));

                cmd.Parameters.AddWithValue("@Currency", strCurrency);
                cmd.Parameters.AddWithValue("@Rate", strRate);
                cmd.Parameters.AddWithValue("@TaxType", strTaxType);
                cmd.Parameters.AddWithValue("@TaxCode", strTaxCode);

                cmd.Parameters.AddWithValue("@CompanyID", Convert.ToString(Session["LastCompany"]));
                cmd.Parameters.AddWithValue("@FinYear", Convert.ToString(Session["LastFinYear"]));
                cmd.Parameters.AddWithValue("@CreatedBy", Convert.ToString(Session["userid"]));

                cmd.Parameters.AddWithValue("@ProductDetails", Productdt);
                cmd.Parameters.AddWithValue("@TaxDetail", TaxDetailTable);

                if (Session["PRwarehousedetailstemp"] != null)
                {
                    DataTable temtable = new DataTable();
                    DataTable Warehousedtssss = (DataTable)Session["PRwarehousedetailstemp"];

                    temtable = Warehousedtssss.DefaultView.ToTable(false, "SrlNo", "BatchWarehouseID", "BatchWarehousedetailsID", "BatchID", "SerialID", "WarehouseID", "WarehouseName", "BatchNo", "SerialNo", "MFGDate", "ExpiryDate", "Quantitysum", "productid", "Inventrytype", "StockID", "isnew");
                    cmd.Parameters.AddWithValue("@udt_StockOpeningwarehousentrie", temtable);
                }
                //  cmd.Parameters.AddWithValue("@WarehouseDetail", Warehousedt);
                cmd.Parameters.AddWithValue("@SalesReturnTax", SalesReturnTaxdt);
                cmd.Parameters.AddWithValue("@BillAddress", BillAddressdt);

                cmd.Parameters.Add("@ReturnValue", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@ReturnPurchaseReturnID", SqlDbType.VarChar, 50);

                cmd.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
                cmd.Parameters["@ReturnPurchaseReturnID"].Direction = ParameterDirection.Output;

                cmd.CommandTimeout = 0;
                SqlDataAdapter Adap = new SqlDataAdapter();
                Adap.SelectCommand = cmd;
                Adap.Fill(dsInst);

                strIsComplete = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value.ToString());
                strInvoiceID = Convert.ToInt32(cmd.Parameters["@ReturnPurchaseReturnID"].Value.ToString());

                cmd.Dispose();
                con.Dispose();

                #region warehouse Update and delete

                updatewarehouse();
                deleteALL();

                #endregion
            }
            catch (Exception ex)
            {
            }
        }


        protected void acpContactPersonPhone_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                string WhichCall = e.Parameter.Split('~')[0];
                if (WhichCall == "ContactPersonPhone")
                {
                    string InternalId = Convert.ToString(Convert.ToString(e.Parameter.Split('~')[1]));

                    DataTable dtContactPersonPhone = new DataTable();
                    dtContactPersonPhone = objPurchaseOrderBL.PopulateContactPersonPhone(InternalId);
                    if (dtContactPersonPhone != null && dtContactPersonPhone.Rows.Count > 0)
                    {
                        pageheaderContent.Attributes.Add("style", "display:block");
                        divContactPhone.Attributes.Add("style", "display:block");

                        foreach (DataRow dr in dtContactPersonPhone.Rows)
                        {
                            string phone = Convert.ToString(dr["phf_phoneNumber"]);
                            if (!string.IsNullOrEmpty(phone))
                            {
                                // lblContactPhone.Text = phone;
                                acpContactPersonPhone.JSProperties["cpPhone"] = phone;
                                break;
                            }
                        }

                    }

                }
            }
            catch { }
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

        #endregion

        #region Quotation Tax Details

        public IEnumerable GetTaxes()
        {
            List<Taxes> TaxList = new List<Taxes>();


          //  BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
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
            ProcedureExecute proc = new ProcedureExecute("prc_PurchaseReturn_Details");
            proc.AddVarcharPara("@Action", 500, "TaxDetails");
            proc.AddVarcharPara("@PurchaseReturnID", 500, Convert.ToString(Session["PR_ReturnID"]));
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 500, Convert.ToString(Session["userbranchID"]));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
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
                if (Session["PR_TaxDetails"] == null)
                {
                    Session["PR_TaxDetails"] = GetTaxData(dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                }

                if (Session["PR_TaxDetails"] != null)
                {
                    TaxDetailsdt = (DataTable)Session["PR_TaxDetails"];

                    #region Delete Igst,Cgst,Sgst respectively
                    //Get Company Gstin 09032017
                    string CompInternalId = Convert.ToString(Session["LastCompany"]);
                    string[] compGstin = oDBEngine.GetFieldValue1("tbl_master_company", "cmp_gstin", "cmp_internalid='" + CompInternalId + "'", 1);
                    
                    string ShippingState = "";

                    #region ##### Added By : Samrat Roy -- For BillingShippingUserControl ######
                    string sstateCode = BillingShippingControl.GetShippingStateCode(Request.QueryString["key"]);
                    ShippingState = sstateCode;
                    if (ShippingState.Trim() != "")
                    {
                        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
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

                    if (ShippingState.Trim() != "" && compGstin[0].Trim() != "")
                    {
                        if (compGstin.Length > 0)
                        {
                            if (compGstin[0].Substring(0, 2) == ShippingState)
                            {
                                //Check if the state is in union territories then only UTGST will apply
                                //   Chandigarh     Andaman and Nicobar Islands    DADRA & NAGAR HAVELI    DAMAN & DIU    Lakshadweep              PONDICHERRY
                                if (ShippingState == "4" || ShippingState == "35" || ShippingState == "26" || ShippingState == "25" ||  ShippingState == "31" || ShippingState == "34")
                                {
                                    foreach (DataRow dr in TaxDetailsdt.Rows)
                                    {
                                        if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
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
                    }

                    //If Company GSTIN is blank then Delete All CGST,UGST,IGST,CGST
                    if (compGstin[0].Trim() == "")
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
                if (Session["PR_TaxDetails"] != null)
                {
                    TaxDetailsdt = (DataTable)Session["PR_TaxDetails"];
                }
                else
                {
                    TaxDetailsdt.Columns.Add("Taxes_ID", typeof(string));
                    TaxDetailsdt.Columns.Add("Taxes_Name", typeof(string));
                    TaxDetailsdt.Columns.Add("Percentage", typeof(string));
                    TaxDetailsdt.Columns.Add("Amount", typeof(string));
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

                    Session["PR_TaxDetails"] = TaxDetailsdt;
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
            if (Session["PR_TaxDetails"] != null)
            {
                TaxDetailsdt = (DataTable)Session["PR_TaxDetails"];
            }
            else
            {
                TaxDetailsdt.Columns.Add("Taxes_ID", typeof(string));
                TaxDetailsdt.Columns.Add("Taxes_Name", typeof(string));
                TaxDetailsdt.Columns.Add("Percentage", typeof(string));
                TaxDetailsdt.Columns.Add("Amount", typeof(string));
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

            Session["PR_TaxDetails"] = TaxDetailsdt;

            gridTax.DataSource = GetTaxes(TaxDetailsdt);
            gridTax.DataBind();
        }
        protected void gridTax_DataBinding(object sender, EventArgs e)
        {
            if (Session["PR_TaxDetails"] != null)
            {
                DataTable TaxDetailsdt = (DataTable)Session["PR_TaxDetails"];

                //gridTax.DataSource = GetTaxes();
                var taxlist = (List<Taxes>)GetTaxes(TaxDetailsdt);
                var taxChargeDataSource = setChargeCalculatedOn(taxlist, TaxDetailsdt);
                gridTax.DataSource = taxChargeDataSource;
            }
        }


        #endregion


        #region Subhabrata-Products
        protected void Productgrid_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void Productgrid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void Productgrid_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void aspxGridProduct_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {

        }
        #endregion
        #region warehousepopup
        protected void GrdWarehousePC_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string strSplitCommand = e.Parameters.Split('~')[0];


            if (hdniswarehouse.Value == "true")
            {
                GrdWarehousePC.Columns["viewWarehouseName"].Visible = true;
                GrdWarehousePC.Columns["Quantity"].Visible = true;
                GrdWarehousePC.Columns["viewQuantity"].Visible = false;

            }
            else
            {
                GrdWarehousePC.Columns["viewWarehouseName"].Visible = false;
                GrdWarehousePC.Columns["Quantity"].Visible = false;
                GrdWarehousePC.Columns["viewQuantity"].Visible = true;
            }
            if (hdnisbatch.Value == "true")
            {
                GrdWarehousePC.Columns["viewBatchNo"].Visible = true;
                GrdWarehousePC.Columns["viewMFGDate"].Visible = true;
                GrdWarehousePC.Columns["viewExpiryDate"].Visible = true;
                GrdWarehousePC.Columns["Quantity"].Visible = false;
                GrdWarehousePC.Columns["viewQuantity"].Visible = true;

            }
            else
            {
                GrdWarehousePC.Columns["viewBatchNo"].Visible = false;
                GrdWarehousePC.Columns["viewMFGDate"].Visible = false;
                GrdWarehousePC.Columns["viewExpiryDate"].Visible = false;
                GrdWarehousePC.Columns["Quantity"].Visible = true;
                GrdWarehousePC.Columns["viewQuantity"].Visible = false;
            }
            if (hdnisserial.Value == "true")
            {
                GrdWarehousePC.Columns["viewSerialNo"].Visible = true;
                GrdWarehousePC.Columns["Quantity"].Visible = false;
                GrdWarehousePC.Columns["viewQuantity"].Visible = true;
            }
            else
            {
                GrdWarehousePC.Columns["viewSerialNo"].Visible = false;
            }


            #region Savenew
            if (strSplitCommand == "Display")
            {
                string WarehouseName = Convert.ToString(e.Parameters.Split('~')[2]);

                string WarehouseID = Convert.ToString(e.Parameters.Split('~')[1]).Trim();

                string BatchName = Convert.ToString(e.Parameters.Split('~')[3]);

                string SerialName = Convert.ToString(e.Parameters.Split('~')[4]);
                string ProductID = Convert.ToString(hdfProductIDPC.Value);
                string stockid = Convert.ToString(hdfstockidPC.Value);
                decimal openingstock = Convert.ToDecimal(txtqnty.Text);
                string branchid = Convert.ToString(hdbranchIDPC.Value);
                string qnty = Convert.ToString(e.Parameters.Split('~')[5]);

                decimal totalopeining = Convert.ToDecimal(hdfopeningstockPC.Value);
                decimal oldtotalopeining = Convert.ToDecimal(oldopeningqntity.Value);

                if (qnty == "0.0000" && openingstock <= 0)
                {
                    qnty = batchqnty.Text;
                    openingstock = Convert.ToDecimal(qnty);
                }

                if (!string.IsNullOrEmpty(BatchName))
                {
                    BatchName = BatchName.Trim();
                }
                if (!string.IsNullOrEmpty(SerialName))
                {
                    SerialName = SerialName.Trim();
                }
                if (WarehouseID == "null")
                {
                    WarehouseID = "0";
                }

                if (Convert.ToDecimal(openingstock) > totalopeining)
                {
                    GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("You have entered Quantity greater than Opening Quantity. Cannot Proceed.");

                }
                else
                {
                    DataTable Warehousedt = new DataTable();
                    DataTable Warehousedtnew = new DataTable();
                    if (Session["PR_WarehouseData"] != null)
                    {
                        Warehousedt = (DataTable)Session["PR_WarehouseData"];
                    }
                    else
                    {
                        //  Warehousedt.Columns.Add("Product_SrlNo", typeof(string));
                        Warehousedt.Columns.Add("SrlNo", typeof(Int32));
                        Warehousedt.Columns.Add("WarehouseID", typeof(string));
                        Warehousedt.Columns.Add("WarehouseName", typeof(string));

                        Warehousedt.Columns.Add("BatchNo", typeof(string));
                        Warehousedt.Columns.Add("SerialNo", typeof(string));

                        Warehousedt.Columns.Add("MFGDate", typeof(DateTime));
                        Warehousedt.Columns.Add("ExpiryDate", typeof(DateTime));
                        Warehousedt.Columns.Add("Quantity", typeof(decimal));

                        Warehousedt.Columns.Add("BatchWarehouseID", typeof(string));
                        Warehousedt.Columns.Add("BatchWarehousedetailsID", typeof(string));
                        Warehousedt.Columns.Add("BatchID", typeof(string));
                        Warehousedt.Columns.Add("SerialID", typeof(string));


                        Warehousedt.Columns.Add("viewWarehouseName", typeof(string));

                        Warehousedt.Columns.Add("viewBatchNo", typeof(string));
                        Warehousedt.Columns.Add("viewQuantity", typeof(string));
                        Warehousedt.Columns.Add("viewSerialNo", typeof(string));



                        Warehousedt.Columns.Add("viewMFGDate", typeof(DateTime));
                        Warehousedt.Columns.Add("viewExpiryDate", typeof(DateTime));

                        Warehousedt.Columns.Add("Quantitysum", typeof(decimal));
                        Warehousedt.Columns.Add("isnew", typeof(string));
                        Warehousedt.Columns.Add("productid", typeof(string));
                        Warehousedt.Columns.Add("Inventrytype", typeof(string));
                        Warehousedt.Columns.Add("StockID", typeof(string));
                        Warehousedt.Columns.Add("pcslno", typeof(Int32));

                    }



                    DateTime dtmgh = txtmkgdate.Date;
                    DateTime dtexp = txtexpirdate.Date;

                    string isedited = hdnisedited.Value;
                    decimal inputqnty = 0;
                    int noofserial = 0;
                    int oldrowcount = Convert.ToInt32(hdnoldrowcount.Value);
                    int newrowcount = 0;
                    decimal hdntotalqntyPCs = Convert.ToDecimal(hdntotalqntyPC.Value);


                    if (Session["PR_WarehouseData"] != null)
                    {
                        DataTable Warehousedts = (DataTable)Session["PR_WarehouseData"];

                        newrowcount = Warehousedts.Select("isnew = 'new'").Count<DataRow>();

                        if (newrowcount != null && hdnisserial.Value == "true")
                        {

                            var inputqntys = Warehousedts.Select("WarehouseName= '" + WarehouseName + "' AND BatchNo = '" + BatchName + "' AND isnew = 'new'").Count<DataRow>();
                            if (inputqntys != null && !string.IsNullOrEmpty(Convert.ToString(inputqntys)))
                            {
                                noofserial = Convert.ToInt32(inputqntys + 1);

                            }
                            else
                            {
                                noofserial = 0;
                            }
                        }
                    }

                    if (hidencountforserial.Value != "2")
                    {
                        if (Warehousedt.Rows.Count > 0)
                        {
                            var inputqntys = Warehousedt.Compute("sum(Quantitysum)", "isnew = 'new'");
                            if (inputqntys != null && !string.IsNullOrEmpty(Convert.ToString(inputqntys)))
                            {
                                inputqnty = Convert.ToDecimal(inputqntys);

                            }
                            else
                            {
                                inputqnty = 0;
                            }

                        }
                        //commentout below line for it should not add.
                        if (hdnisserial.Value == "false" && hdniswarehouse.Value == "true" && hdnisbatch.Value == "false")
                        {
                            inputqnty = inputqnty + Convert.ToDecimal(openingstock);
                        }
                        if (hdnisserial.Value == "false" && hdniswarehouse.Value == "true" && hdnisbatch.Value == "true")
                        {
                            inputqnty = inputqnty + Convert.ToDecimal(openingstock);
                        }
                        if (hdnisserial.Value == "false" && hdniswarehouse.Value == "false" && hdnisbatch.Value == "true")
                        {
                            inputqnty = inputqnty + Convert.ToDecimal(openingstock);
                        }


                    }

                    //checking quantity only for warehouse is true.
                    if (inputqnty == 0 && hdnisserial.Value == "false")
                    {
                        inputqnty = Convert.ToDecimal(openingstock);
                    }

                    if (hidencountforserial.Value == "1" && hdnisserial.Value == "true" && noofserial != openingstock && hdnisbatch.Value == "true" && hdniswarehouse.Value == "true")
                    {
                        GrdWarehousePC.JSProperties["cpupdatemssgserialsetdisblebatch"] = Convert.ToString("disable");
                    }
                    if (hidencountforserial.Value == "2" && hdnisserial.Value == "true" && noofserial == openingstock && hdnisbatch.Value == "true" && hdniswarehouse.Value == "true")
                    {
                        GrdWarehousePC.JSProperties["cpupdatemssgserialsetenablebatch"] = Convert.ToString("enable");
                    }
                    if (hidencountforserial.Value == "1" && hdnisserial.Value == "true" && noofserial != openingstock && hdnisbatch.Value == "false" && hdniswarehouse.Value == "true")
                    {
                        GrdWarehousePC.JSProperties["cpupdatemssgserialsetdisblebatch"] = Convert.ToString("disable");
                    }
                    if (hidencountforserial.Value == "2" && hdnisserial.Value == "true" && noofserial == openingstock && hdnisbatch.Value == "false" && hdniswarehouse.Value == "true")
                    {
                        GrdWarehousePC.JSProperties["cpupdatemssgserialsetenablebatch"] = Convert.ToString("enable");
                    }

                    ///batch and serial only
                    if (hidencountforserial.Value == "1" && hdnisserial.Value == "true" && noofserial != openingstock && hdnisbatch.Value == "true" && hdniswarehouse.Value == "false")
                    {
                        GrdWarehousePC.JSProperties["cpupdatemssgserialsetdisblebatch"] = Convert.ToString("disable");
                    }

                    if (hidencountforserial.Value == "2" && hdnisserial.Value == "true" && noofserial == openingstock && hdnisbatch.Value == "true" && hdniswarehouse.Value == "false")
                    {
                        GrdWarehousePC.JSProperties["cpupdatemssgserialsetenablebatch"] = Convert.ToString("enable");
                    }

                    ///end batch and serial only



                    //

                    //if (hdnisserial.Value == "true" && ((Convert.ToDecimal(Convert.ToString((oldrowcount - newrowcount)).Replace('-', '0')) == hdntotalqntyPCs) || (Convert.ToDecimal(Convert.ToString((oldrowcount - newrowcount)).Replace('-', '0')) == (hdntotalqntyPCs * Convert.ToDecimal(hdnbatchchanged.Value)))) && ((hdniswarehouse.Value == "true" && hdnisbatch.Value == "true") && (hdnoldwarehousname.Value == WarehouseName && hdnoldbatchno.Value == BatchName)) || (hdniswarehouse.Value == "true" && hdnisbatch.Value == "false") && (hdnoldwarehousname.Value == WarehouseName) || (hdniswarehouse.Value == "false" && hdnisbatch.Value == "true") && (hdnoldbatchno.Value == BatchName))
                    //{
                    //    GrdWarehousePC.JSProperties["cpupdatemssgserial"] = Convert.ToString("Please make sure quantity and no of Serial are equal or not.");
                    //}
                    if (hdnisserial.Value == "true" && noofserial > openingstock)
                    {
                        GrdWarehousePC.JSProperties["cpupdatemssgserial"] = Convert.ToString("Please make sure quantity and no of Serial are equal or not.");
                    }
                    else
                    {

                        if (inputqnty <= totalopeining || isedited == "true")
                        {

                            if (hdniswarehouse.Value == "true" && hdnisbatch.Value == "true" && hidencountforserial.Value == "2" && hdnisserial.Value == "true" && hdnoldwarehousname.Value == WarehouseName && hdnoldbatchno.Value == BatchName)
                            {
                                if (Convert.ToString(dtmgh) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtexp) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtmgh) != "01-01-0001 12:00:00 AM" && Convert.ToString(dtexp) != "01-01-0001 12:00:00 AM")
                                {

                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, dtmgh, dtexp, openingstock, "0", "0", "0", "0", "", "", "", SerialName, null, null, 0, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                                    GrdWarehousePC.JSProperties["cpinsertmssgserial"] = Convert.ToString("Inserted.");

                                }
                                else
                                {
                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, null, null, openingstock, "0", "0", "0", "0", "", "", "", SerialName, null, null, 0, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));

                                    GrdWarehousePC.JSProperties["cpinsertmssgserial"] = Convert.ToString("Inserted.");

                                }
                            }
                            else if (hdniswarehouse.Value == "false" && hdnisbatch.Value == "false" && hdnisserial.Value == "true")
                            {

                                if (Convert.ToString(dtmgh) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtexp) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtmgh) != "01-01-0001 12:00:00 AM" && Convert.ToString(dtexp) != "01-01-0001 12:00:00 AM")
                                {

                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, dtmgh, dtexp, openingstock, "0", "0", "0", "0", WarehouseName, BatchName, openingstock, SerialName, dtmgh, dtexp, openingstock, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                                    GrdWarehousePC.JSProperties["cpinsertmssgserial"] = Convert.ToString("Inserted.");
                                }
                                else
                                {
                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, null, null, openingstock, "0", "0", "0", "0", WarehouseName, BatchName, openingstock, SerialName, null, null, openingstock, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                                    GrdWarehousePC.JSProperties["cpinsertmssgserial"] = Convert.ToString("Inserted.");

                                }

                            }
                            else if (hdniswarehouse.Value == "true" && hidencountforserial.Value == "2" && hdnisbatch.Value == "false" && hdnisserial.Value == "true")
                            {

                                if (Convert.ToString(dtmgh) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtexp) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtmgh) != "01-01-0001 12:00:00 AM" && Convert.ToString(dtexp) != "01-01-0001 12:00:00 AM")
                                {

                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, dtmgh, dtexp, openingstock, "0", "0", "0", "0", "", "", "", SerialName, null, null, 0, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                                    GrdWarehousePC.JSProperties["cpinsertmssgserial"] = Convert.ToString("Inserted.");

                                }
                                else
                                {
                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, null, null, openingstock, "0", "0", "0", "0", "", "", "", SerialName, null, null, 0, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));

                                    GrdWarehousePC.JSProperties["cpinsertmssgserial"] = Convert.ToString("Inserted.");

                                }
                            }
                            //batch only with serial

                            else if (hdniswarehouse.Value == "false" && hidencountforserial.Value == "2" && hdnisbatch.Value == "true" && hdnisserial.Value == "true")
                            {

                                if (Convert.ToString(dtmgh) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtexp) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtmgh) != "01-01-0001 12:00:00 AM" && Convert.ToString(dtexp) != "01-01-0001 12:00:00 AM")
                                {

                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, dtmgh, dtexp, openingstock, "0", "0", "0", "0", "", "", "", SerialName, null, null, 0, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                                    GrdWarehousePC.JSProperties["cpinsertmssgserial"] = Convert.ToString("Inserted.");

                                }
                                else
                                {
                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, null, null, openingstock, "0", "0", "0", "0", "", "", "", SerialName, null, null, 0, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));

                                    GrdWarehousePC.JSProperties["cpinsertmssgserial"] = Convert.ToString("Inserted.");

                                }
                            }
                            //batch only.
                            else
                            {

                                if (Convert.ToString(dtmgh) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtexp) != "1/1/0001 12:00:00 AM" && Convert.ToString(dtmgh) != "01-01-0001 12:00:00 AM" && Convert.ToString(dtexp) != "01-01-0001 12:00:00 AM")
                                {

                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, dtmgh, dtexp, openingstock, "0", "0", "0", "0", WarehouseName, BatchName, openingstock, SerialName, dtmgh, dtexp, openingstock, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));

                                }
                                else
                                {
                                    Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, null, null, openingstock, "0", "0", "0", "0", WarehouseName, BatchName, openingstock, SerialName, null, null, openingstock, "new", Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));


                                }
                                if (hdnisserial.Value == "true")
                                {
                                    GrdWarehousePC.JSProperties["cpinsertmssg"] = Convert.ToString("Inserted.");
                                }
                                if (hdnisserial.Value == "false" && hdniswarehouse.Value == "true" && hdnisbatch.Value == "true")
                                {
                                    GrdWarehousePC.JSProperties["cpbatchinsertmssg"] = Convert.ToString("Inserted.");
                                }

                            }


                            Session["PR_WarehouseData"] = Warehousedt;


                            GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                            GrdWarehousePC.DataBind();
                        }
                        else
                        {
                            GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("You have entered Quantity greater than Opening Quantity. Cannot Proceed.");
                        }
                    }
                }
                //
            }
            #endregion
            #region SaveAll
            if (strSplitCommand == "Saveall")
            {
                if (Session["PR_WarehouseData"] == null && hdnisolddeleted.Value == "false")
                {
                    if (!string.IsNullOrEmpty(hdnvalue.Value) && !string.IsNullOrEmpty(hdnrate.Value))
                    {

                    }
                    else
                    {
                        GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Nothing to Saved .");
                    }

                }
                else if (Session["PR_WarehouseData"] == null && hdnisolddeleted.Value == "true")
                {

                    deleteALL();
                    GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Saved Successfully.");
                }
                else
                {
                    string ProductID = Convert.ToString(hdfProductIDPC.Value);
                    string stockid = Convert.ToString(hdfstockidPC.Value);
                    string openingstock = Convert.ToString(hdfopeningstockPC.Value);
                    string branchid = Convert.ToString(hdbranchIDPC.Value);
                    string isolddeletd = hdnisolddeleted.Value;

                    int oldrowcount = Convert.ToInt32(hdnoldrowcount.Value);
                    int newrowcount = 0;
                    int updaterows = 0;
                    int deleted = 0;
                    decimal hdntotalqntyPCs = Convert.ToDecimal(hdntotalqntyPC.Value);

                    DataTable Warehousedts = (DataTable)Session["PR_WarehouseData"];

                    newrowcount = Warehousedts.Select("isnew = 'new'").Count<DataRow>();
                    updaterows = Warehousedts.Select("isnew = 'Updated'").Count<DataRow>();


                    if (newrowcount != 0 || updaterows != 0 || Session["PRWarehouseDataDelete"] != null)
                    {
                        decimal inputqnty = 0;
                        decimal totalopeining = Convert.ToDecimal(hdfopeningstockPC.Value);
                        decimal oldtotalopeining = Convert.ToDecimal(oldopeningqntity.Value);

                        var inputqntys = Warehousedts.Compute("sum(Quantitysum)", "isnew = 'new'");
                        var updateqnty = Warehousedts.Compute("sum(Quantitysum)", "isnew = 'Updated'");
                        var oldeqnty = Warehousedts.Compute("sum(Quantitysum)", "isnew = 'old'");
                        decimal deletd = Convert.ToDecimal(hdndeleteqnity.Value);

                        if (inputqntys != null && !string.IsNullOrEmpty(Convert.ToString(inputqntys)))
                        {
                            inputqnty = Convert.ToDecimal(inputqntys);

                        }
                        if (updateqnty != null && !string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                        {
                            inputqnty = inputqnty + Convert.ToDecimal(updateqnty);


                        }
                        if (oldeqnty != null && !string.IsNullOrEmpty(Convert.ToString(oldeqnty)))
                        {
                            inputqnty = inputqnty + Convert.ToDecimal(oldeqnty);


                        }
                        //commnet for allowing reduce from main page
                        //if ((oldeqnty != null && !string.IsNullOrEmpty(Convert.ToString(oldeqnty))) || (updateqnty != null && !string.IsNullOrEmpty(Convert.ToString(updateqnty))))
                        //{

                        //    //totalopeining = totalopeining + oldtotalopeining; //commnet for allowing reduce from main page
                        //    //if (hdnisreduing.Value == "true")
                        //    //{
                        //    //    totalopeining = oldtotalopeining;
                        //    //}
                        //    //else
                        //    //{
                        //    //    totalopeining = totalopeining + oldtotalopeining;
                        //    //}
                        //    totalopeining = oldtotalopeining;
                        //}
                        //if (deletd > 0 & isolddeletd == "true")
                        //{
                        //    totalopeining = Convert.ToDecimal(oldopeningqntity.Value) - deletd;
                        //}

                        if (inputqnty == totalopeining)
                        {
                            //if (hdnisserial.Value == "true" && ((Convert.ToDecimal(Convert.ToString((oldrowcount - newrowcount)).Replace('-', '0')) == hdntotalqntyPCs) || (Convert.ToDecimal(Convert.ToString((oldrowcount - newrowcount)).Replace('-', '0')) == (hdntotalqntyPCs * Convert.ToDecimal(hdnbatchchanged.Value)))))
                            if (hdnisserial.Value == "true" && hdnisbatch.Value == "true" && hdniswarehouse.Value == "true")
                            {

                                if (ProductID != "0" && branchid != "0")
                                {
                                    int output = Insertupdatedata(ProductID, stockid, branchid);
                                    if (output > 0)
                                    {
                                        DataTable Warehousedt = new DataTable();
                                        Warehousedt = GetRecord(stockid);
                                        if (Warehousedt.Rows.Count > 0)
                                        {
                                            Session["PR_WarehouseData"] = Warehousedt;

                                            GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                                            GrdWarehousePC.DataBind();
                                        }
                                        else
                                        {

                                            Session["PR_WarehouseData"] = null;

                                            GrdWarehousePC.DataSource = null;
                                            GrdWarehousePC.DataBind();

                                        }

                                        GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Saved Successfully.");

                                    }

                                }
                                else
                                {
                                    GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Please try again null parameters.");
                                }
                            }
                            else if (hdnisserial.Value != "true" && hdnisbatch.Value != "true" && hdniswarehouse.Value == "true")
                            {
                                if (ProductID != "0" && branchid != "0")
                                {
                                    int output = Insertupdatedata(ProductID, stockid, branchid);
                                    if (output > 0)
                                    {
                                        DataTable Warehousedt = new DataTable();
                                        Warehousedt = GetRecord(stockid);
                                        if (Warehousedt.Rows.Count > 0)
                                        {
                                            Session["PR_WarehouseData"] = Warehousedt;

                                            GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                                            GrdWarehousePC.DataBind();
                                        }
                                        else
                                        {

                                            Session["PR_WarehouseData"] = null;

                                            GrdWarehousePC.DataSource = null;
                                            GrdWarehousePC.DataBind();

                                        }

                                        GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Saved Successfully.");

                                    }

                                }
                                else
                                {
                                    GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Please try again null parameters.");
                                }
                            }
                            else if (hdnisserial.Value != "true" && hdnisbatch.Value == "true")
                            {
                                if (ProductID != "0" && branchid != "0")
                                {
                                    int output = Insertupdatedata(ProductID, stockid, branchid);
                                    if (output > 0)
                                    {
                                        DataTable Warehousedt = new DataTable();
                                        Warehousedt = GetRecord(stockid);
                                        if (Warehousedt.Rows.Count > 0)
                                        {
                                            Session["PR_WarehouseData"] = Warehousedt;

                                            GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                                            GrdWarehousePC.DataBind();
                                        }
                                        else
                                        {

                                            Session["PR_WarehouseData"] = null;

                                            GrdWarehousePC.DataSource = null;
                                            GrdWarehousePC.DataBind();

                                        }

                                        GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Saved Successfully.");

                                    }

                                }
                                else
                                {
                                    GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Please try again null parameters.");
                                }

                            }
                            else if (hdnisserial.Value == "true" && hdnisbatch.Value != "true" && hdniswarehouse.Value != "true")
                            {
                                if (ProductID != "0" && branchid != "0")
                                {
                                    int output = Insertupdatedata(ProductID, stockid, branchid);
                                    if (output > 0)
                                    {
                                        DataTable Warehousedt = new DataTable();
                                        Warehousedt = GetRecord(stockid);
                                        if (Warehousedt.Rows.Count > 0)
                                        {
                                            Session["PR_WarehouseData"] = Warehousedt;

                                            GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                                            GrdWarehousePC.DataBind();
                                        }
                                        else
                                        {

                                            Session["PR_WarehouseData"] = null;

                                            GrdWarehousePC.DataSource = null;
                                            GrdWarehousePC.DataBind();

                                        }

                                        GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Saved Successfully.");

                                    }

                                }
                                else
                                {
                                    GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Please try again null parameters.");
                                }
                            }
                            else if (hdnisserial.Value == "true" && hdnisbatch.Value != "true" && hdniswarehouse.Value == "true")
                            {

                                if (ProductID != "0" && branchid != "0")
                                {
                                    int output = Insertupdatedata(ProductID, stockid, branchid);
                                    if (output > 0)
                                    {
                                        DataTable Warehousedt = new DataTable();
                                        Warehousedt = GetRecord(stockid);
                                        if (Warehousedt.Rows.Count > 0)
                                        {
                                            Session["PR_WarehouseData"] = Warehousedt;

                                            GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                                            GrdWarehousePC.DataBind();
                                        }
                                        else
                                        {

                                            Session["PR_WarehouseData"] = null;

                                            GrdWarehousePC.DataSource = null;
                                            GrdWarehousePC.DataBind();

                                        }

                                        GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Saved Successfully.");

                                    }

                                }
                                else
                                {
                                    GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Please try again null parameters.");
                                }
                            }
                            else if (hdnisserial.Value == "true" && hdnisbatch.Value == "true" && hdniswarehouse.Value != "true")
                            {

                                if (ProductID != "0" && branchid != "0")
                                {
                                    int output = Insertupdatedata(ProductID, stockid, branchid);
                                    if (output > 0)
                                    {
                                        DataTable Warehousedt = new DataTable();
                                        Warehousedt = GetRecord(stockid);
                                        if (Warehousedt.Rows.Count > 0)
                                        {
                                            Session["PR_WarehouseData"] = Warehousedt;

                                            GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                                            GrdWarehousePC.DataBind();
                                        }
                                        else
                                        {

                                            Session["PR_WarehouseData"] = null;

                                            GrdWarehousePC.DataSource = null;
                                            GrdWarehousePC.DataBind();

                                        }

                                        GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Saved Successfully.");

                                    }

                                }
                                else
                                {
                                    GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Please try again null parameters.");
                                }
                            }
                            else
                            {
                                GrdWarehousePC.JSProperties["cpupdatemssgserial"] = Convert.ToString("Please make sure quantity and no of Serial are equal or not.");

                            }

                        }
                        else
                        {
                            GrdWarehousePC.JSProperties["cpupdatemssgserial"] = Convert.ToString("You have entered Quantity greater than Opening Quantity. Cannot Proceed.");
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(hdnvalue.Value) && !string.IsNullOrEmpty(hdnrate.Value))
                        {
                            int olfdatterows = Warehousedts.Select("isnew = 'old'").Count<DataRow>();
                            if (olfdatterows != 0)
                            {
                                var oldeqnty = Warehousedts.Compute("sum(Quantitysum)", "isnew = 'old'");
                                if (Convert.ToDecimal(hdfopeningstockPC.Value) < Convert.ToDecimal(oldeqnty))
                                {
                                    GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Opening quantity and enterd quantity are mismatch.");
                                }
                                else
                                {

                                }

                            }
                            else
                            {

                            }

                        }
                        else
                        {
                            GrdWarehousePC.JSProperties["cpupdatemssg"] = Convert.ToString("Nothing to Saved .");
                        }

                    }
                }

            }

            #endregion
            #region CheckDataExist
            if (strSplitCommand == "checkdataexist")
            {
                Session["PR_WarehouseData"] = null;

                GrdWarehousePC.DataSource = null;

                string strProductname = Convert.ToString(e.Parameters.Split('~')[4]);
                string name = strProductname;
                name = name.Replace("squot", "'");
                name = name.Replace("coma", ",");
                name = name.Replace("slash", "/");

                Session["PRWarehouseUpdatedData"] = null;
                Session["PRWarehouseDataDelete"] = null;

                GrdWarehousePC.JSProperties["cpproductname"] = Convert.ToString(name);


                string strProductID = Convert.ToString(e.Parameters.Split('~')[1]);

                string stockids = Convert.ToString(e.Parameters.Split('~')[2]);

                string branchid = Convert.ToString(e.Parameters.Split('~')[3]);

                DataTable Warehousedt = new DataTable();
                Warehousedt = GetRecord(stockids);
                if (Warehousedt.Rows.Count > 0)
                {
                    Session["PR_WarehouseData"] = Warehousedt;

                    GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                    GrdWarehousePC.DataBind();
                }
                else
                {

                    Session["PR_WarehouseData"] = null;

                    GrdWarehousePC.DataSource = null;
                    GrdWarehousePC.DataBind();

                }


            }
            //if (strSplitCommand == "UpdatedataReffresh")
            //{
            //    if (Session["PCWarehouseData"] != null)
            //    {

            //        GrdWarehousePC.DataSource = Session["PCWarehouseData"];
            //        GrdWarehousePC.DataBind();
            //    }
            //}
            #endregion
            #region updatedata
            if (strSplitCommand == "Updatedata")
            {
                string WarehouseID = Convert.ToString(e.Parameters.Split('~')[1]);

                string WarehouseName = Convert.ToString(e.Parameters.Split('~')[2]);

                string BatchName = Convert.ToString(e.Parameters.Split('~')[3]);

                string SerialName = Convert.ToString(e.Parameters.Split('~')[4]);

                string slno = Convert.ToString(e.Parameters.Split('~')[5]);
                string qntity = Convert.ToString(e.Parameters.Split('~')[6]);
                string isviewqntitynull = hdnisviewqntityhas.Value;
                string isserialenable = hdnisserial.Value;
                DateTime expdate = txtexpirdate.Date;
                DateTime mkgdate = txtmkgdate.Date;
                DataTable Warehousedt = new DataTable();

                decimal openingstock = Convert.ToDecimal(txtqnty.Text);
                if (qntity == "0.0000" && openingstock <= 0)
                {
                    qntity = batchqnty.Text;
                    openingstock = Convert.ToDecimal(qntity);
                }

                if (WarehouseID == "null")
                {
                    WarehouseID = "0";
                    WarehouseName = "";
                }

                if (Session["PR_WarehouseData"] != null)
                {
                    Warehousedt = (DataTable)Session["PR_WarehouseData"];
                    DataSet dataSet1 = new DataSet();
                    DataTable dt1 = new DataTable();
                    dt1 = Warehousedt.Copy();
                    dataSet1.Tables.Add(dt1);

                    if (dataSet1.Tables[0].Rows.Count > 0)
                    {
                        DataRow[] customerRow = dataSet1.Tables[0].Select("SrlNo ='" + slno + "'");
                        if (isserialenable == "false" && isviewqntitynull != "false")
                        {
                            customerRow[0]["WarehouseID"] = WarehouseID;
                            customerRow[0]["WarehouseName"] = WarehouseName;
                            customerRow[0]["BatchNo"] = BatchName;
                            customerRow[0]["SerialNo"] = SerialName;
                            customerRow[0]["Quantity"] = openingstock;

                            if (Convert.ToString(expdate) != "1/1/0001 12:00:00 AM" && Convert.ToString(mkgdate) != "1/1/0001 12:00:00 AM" && Convert.ToString(expdate) != "01-01-0001 12:00:00 AM" && Convert.ToString(mkgdate) != "01-01-0001 12:00:00 AM")
                            {
                                customerRow[0]["viewMFGDate"] = mkgdate;
                                customerRow[0]["viewExpiryDate"] = expdate;
                            }

                            customerRow[0]["viewWarehouseName"] = WarehouseName;
                            customerRow[0]["viewBatchNo"] = BatchName;
                            customerRow[0]["viewSerialNo"] = SerialName;
                            customerRow[0]["viewQuantity"] = qntity;
                            customerRow[0]["Quantitysum"] = openingstock;
                            customerRow[0]["isnew"] = "Updated";
                        }
                        if (isserialenable == "true" && isviewqntitynull == "false")
                        {
                            customerRow[0]["WarehouseID"] = WarehouseID;
                            customerRow[0]["WarehouseName"] = WarehouseName;
                            customerRow[0]["BatchNo"] = BatchName;
                            customerRow[0]["SerialNo"] = SerialName;
                            customerRow[0]["Quantity"] = openingstock;

                            if (Convert.ToString(expdate) != "1/1/0001 12:00:00 AM" && Convert.ToString(mkgdate) != "1/1/0001 12:00:00 AM" && Convert.ToString(expdate) != "01-01-0001 12:00:00 AM" && Convert.ToString(mkgdate) != "01-01-0001 12:00:00 AM")
                            {
                                customerRow[0]["viewMFGDate"] = mkgdate;
                                customerRow[0]["viewExpiryDate"] = expdate;
                            }

                            customerRow[0]["viewWarehouseName"] = WarehouseName;
                            customerRow[0]["viewBatchNo"] = BatchName;
                            customerRow[0]["viewSerialNo"] = SerialName;
                            customerRow[0]["viewQuantity"] = openingstock;
                            customerRow[0]["Quantitysum"] = openingstock;
                            customerRow[0]["isnew"] = "Updated";
                        }
                        if (isserialenable == "true" && isviewqntitynull == "true")
                        {
                            customerRow[0]["WarehouseID"] = WarehouseID;
                            customerRow[0]["WarehouseName"] = WarehouseName;
                            customerRow[0]["BatchNo"] = BatchName;
                            customerRow[0]["SerialNo"] = SerialName;
                            customerRow[0]["Quantity"] = openingstock;



                            customerRow[0]["viewSerialNo"] = SerialName;

                            customerRow[0]["Quantitysum"] = 0;
                            customerRow[0]["isnew"] = "Updated";
                        }
                        if (isserialenable == "false" && isviewqntitynull == "false")
                        {
                            customerRow[0]["WarehouseID"] = WarehouseID;
                            customerRow[0]["WarehouseName"] = WarehouseName;
                            customerRow[0]["BatchNo"] = BatchName;
                            customerRow[0]["SerialNo"] = SerialName;
                            customerRow[0]["Quantity"] = openingstock;

                            customerRow[0]["SerialNo"] = SerialName;
                            customerRow[0]["Quantity"] = openingstock;
                            if (Convert.ToString(expdate) != "1/1/0001 12:00:00 AM" && Convert.ToString(mkgdate) != "1/1/0001 12:00:00 AM" && Convert.ToString(expdate) != "01-01-0001 12:00:00 AM" && Convert.ToString(mkgdate) != "01-01-0001 12:00:00 AM")
                            {
                                customerRow[0]["viewMFGDate"] = mkgdate;
                                customerRow[0]["viewExpiryDate"] = expdate;
                            }
                            customerRow[0]["viewWarehouseName"] = WarehouseName;
                            customerRow[0]["viewBatchNo"] = BatchName;
                            customerRow[0]["viewSerialNo"] = SerialName;
                            customerRow[0]["viewQuantity"] = openingstock;
                            customerRow[0]["Quantitysum"] = openingstock;
                            customerRow[0]["isnew"] = "Updated";
                        }

                        GrdWarehousePC.JSProperties["cpupdateexistingdata"] = Convert.ToString("Saved.");
                    }
                    Warehousedt = null;
                    Warehousedt = dataSet1.Tables[0];
                    Session["PR_WarehouseData"] = Warehousedt;

                    Session["PRWarehouseUpdatedData"] = Warehousedt;

                    GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                    GrdWarehousePC.DataBind();

                }
            }

            #endregion
            #region newrowupdate
            if (strSplitCommand == "NewUpdatedata")
            {
                string WarehouseID = Convert.ToString(e.Parameters.Split('~')[1]);

                string WarehouseName = Convert.ToString(e.Parameters.Split('~')[2]);

                string BatchName = Convert.ToString(e.Parameters.Split('~')[3]);

                string SerialName = Convert.ToString(e.Parameters.Split('~')[4]);

                string slno = Convert.ToString(e.Parameters.Split('~')[5]);
                string qntity = Convert.ToString(e.Parameters.Split('~')[6]);
                string isviewqntitynull = hdnisviewqntityhas.Value;
                string isserialenable = hdnisserial.Value;
                DataTable Warehousedt = new DataTable();
                decimal openingstock = Convert.ToDecimal(txtqnty.Text);
                if (qntity == "0.0000" && openingstock <= 0)
                {
                    qntity = batchqnty.Text;
                    openingstock = Convert.ToDecimal(qntity);
                }

                if (Session["PR_WarehouseData"] != null)
                {
                    Warehousedt = (DataTable)Session["PR_WarehouseData"];
                    DataSet dataSet1 = new DataSet();
                    DataTable dt1 = new DataTable();
                    dt1 = Warehousedt.Copy();
                    dataSet1.Tables.Add(dt1);

                    if (dataSet1.Tables[0].Rows.Count > 0)
                    {
                        DataRow[] customerRow = dataSet1.Tables[0].Select("SrlNo ='" + slno + "'");
                        if (isserialenable == "false" && isviewqntitynull != "false")
                        {
                            customerRow[0]["WarehouseID"] = WarehouseID;
                            customerRow[0]["WarehouseName"] = WarehouseName;
                            customerRow[0]["BatchNo"] = BatchName;
                            customerRow[0]["SerialNo"] = SerialName;
                            customerRow[0]["Quantity"] = openingstock;

                            customerRow[0]["viewWarehouseName"] = WarehouseName;
                            customerRow[0]["viewBatchNo"] = BatchName;
                            customerRow[0]["viewSerialNo"] = SerialName;
                            customerRow[0]["viewQuantity"] = openingstock;
                            customerRow[0]["Quantitysum"] = openingstock;
                            customerRow[0]["isnew"] = "new";
                        }
                        if (isserialenable == "true" && isviewqntitynull == "false")
                        {
                            customerRow[0]["WarehouseID"] = WarehouseID;
                            customerRow[0]["WarehouseName"] = WarehouseName;
                            customerRow[0]["BatchNo"] = BatchName;
                            customerRow[0]["SerialNo"] = SerialName;
                            customerRow[0]["Quantity"] = openingstock;

                            customerRow[0]["viewWarehouseName"] = WarehouseName;
                            customerRow[0]["viewBatchNo"] = BatchName;
                            customerRow[0]["viewSerialNo"] = SerialName;
                            customerRow[0]["viewQuantity"] = openingstock;
                            customerRow[0]["Quantitysum"] = openingstock;
                            customerRow[0]["isnew"] = "new";
                        }
                        if (isserialenable == "true" && isviewqntitynull == "true")
                        {
                            customerRow[0]["WarehouseID"] = WarehouseID;
                            customerRow[0]["WarehouseName"] = WarehouseName;
                            customerRow[0]["BatchNo"] = BatchName;
                            customerRow[0]["SerialNo"] = SerialName;
                            customerRow[0]["Quantity"] = openingstock;



                            customerRow[0]["viewSerialNo"] = SerialName;

                            customerRow[0]["Quantitysum"] = 0;
                            customerRow[0]["isnew"] = "new";
                        }
                        if (isserialenable == "false" && isviewqntitynull == "false")
                        {
                            customerRow[0]["WarehouseID"] = WarehouseID;
                            customerRow[0]["WarehouseName"] = WarehouseName;
                            customerRow[0]["BatchNo"] = BatchName;
                            customerRow[0]["SerialNo"] = SerialName;
                            customerRow[0]["Quantity"] = openingstock;

                            customerRow[0]["viewWarehouseName"] = WarehouseName;
                            customerRow[0]["viewBatchNo"] = BatchName;
                            customerRow[0]["viewSerialNo"] = SerialName;
                            customerRow[0]["viewQuantity"] = openingstock;
                            customerRow[0]["Quantitysum"] = openingstock;
                            customerRow[0]["isnew"] = "new";
                        }

                        GrdWarehousePC.JSProperties["cpupdatenewdata"] = Convert.ToString("Saved.");
                    }
                    Warehousedt = null;
                    Warehousedt = dataSet1.Tables[0];
                    Session["PR_WarehouseData"] = Warehousedt;

                    GrdWarehousePC.DataSource = Warehousedt.DefaultView;
                    GrdWarehousePC.DataBind();

                }


            }
            #endregion
            #region Delete
            if (strSplitCommand == "Delete")
            {
                string slno = Convert.ToString(e.Parameters.Split('~')[1]);

                string viewQuantity = Convert.ToString(e.Parameters.Split('~')[2]);

                string Quantity = Convert.ToString(e.Parameters.Split('~')[3]);
                string warehouseid = Convert.ToString(e.Parameters.Split('~')[4]);
                string batch = Convert.ToString(e.Parameters.Split('~')[5]);
                DataTable Warehousedt = new DataTable();

                // string isviewqntitynull = hdnisviewqntityhas.Value;
                string isserialenable = hdnisserial.Value;
                string isbatch = hdnisbatch.Value;
                string iswarehouse = hdniswarehouse.Value;
                string isolddeletd = hdnisolddeleted.Value;
                if (Session["PR_WarehouseData"] != null)
                {
                    Warehousedt = (DataTable)Session["PR_WarehouseData"];
                    DataSet dataSet1 = new DataSet();

                    DataTable dt1 = new DataTable();
                    dt1 = Warehousedt.Copy();



                    dataSet1.Tables.Add(dt1);

                    if (dataSet1.Tables[0].Rows.Count > 0)
                    {

                        if (isserialenable == "true" && (viewQuantity == "" || viewQuantity == "0") && isbatch == "true" && iswarehouse == "true")
                        {
                            DataRow[] customerRows = dataSet1.Tables[0].Select("WarehouseID= '" + warehouseid + "' AND BatchNo='" + batch + "'");

                            for (int i = 0; i <= customerRows.Count() - 1; i++)
                            {
                                if (Convert.ToString(customerRows[i]["SrlNo"]) == slno && string.IsNullOrEmpty(Convert.ToString(customerRows[i]["viewQuantity"])))
                                {
                                    customerRows[i]["isnew"] = "DeleteSL";
                                }
                                else if (Convert.ToString(customerRows[i]["SrlNo"]) == slno && !string.IsNullOrEmpty(Convert.ToString(customerRows[i]["viewQuantity"])))
                                {

                                    customerRows[i]["isnew"] = "DeleteWHBTSL";

                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(customerRows[i]["viewQuantity"])))
                                {
                                    customerRows[i]["viewQuantity"] = Convert.ToDecimal(customerRows[i]["Quantitysum"]) - 1;
                                    customerRows[i]["Quantity"] = Convert.ToDecimal(customerRows[i]["Quantity"]) - 1;
                                    customerRows[i]["Quantitysum"] = Convert.ToDecimal(customerRows[i]["Quantitysum"]) - 1;

                                    if (Convert.ToString(customerRows[i]["isnew"]) != "new")
                                    {
                                        customerRows[i]["isnew"] = "Updated";
                                    }
                                    else
                                    {
                                        customerRows[i]["isnew"] = "new";
                                    }

                                }
                                //else
                                //{
                                //    customerRows[i]["Quantity"] = 0;
                                //    customerRows[i]["Quantitysum"] = 0;
                                //}
                            }


                            GrdWarehousePC.JSProperties["cpdeletedata"] = Convert.ToString(1);

                        }
                        else if (isserialenable == "true" && (viewQuantity == "" || viewQuantity == "0") && isbatch != "true" && iswarehouse == "true")
                        {
                            DataRow[] customerRows = dataSet1.Tables[0].Select("WarehouseID= '" + warehouseid + "'");

                            for (int i = 0; i <= customerRows.Count() - 1; i++)
                            {
                                if (Convert.ToString(customerRows[i]["SrlNo"]) == slno && string.IsNullOrEmpty(Convert.ToString(customerRows[i]["viewQuantity"])))
                                {
                                    customerRows[i]["isnew"] = "DeleteSL";
                                }
                                else if (Convert.ToString(customerRows[i]["SrlNo"]) == slno && !string.IsNullOrEmpty(Convert.ToString(customerRows[i]["viewQuantity"])))
                                {

                                    customerRows[i]["isnew"] = "DeleteWHSL";


                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(customerRows[i]["viewQuantity"])))
                                {
                                    customerRows[i]["viewQuantity"] = Convert.ToDecimal(customerRows[i]["Quantitysum"]) - 1;
                                    customerRows[i]["Quantity"] = Convert.ToDecimal(customerRows[i]["Quantity"]) - 1;
                                    customerRows[i]["Quantitysum"] = Convert.ToDecimal(customerRows[i]["Quantitysum"]) - 1;

                                    if (Convert.ToString(customerRows[i]["isnew"]) != "new")
                                    {
                                        customerRows[i]["isnew"] = "Updated";
                                    }
                                    else
                                    {
                                        customerRows[i]["isnew"] = "new";
                                    }

                                }
                                else
                                {
                                    customerRows[i]["Quantity"] = 0;
                                    customerRows[i]["Quantitysum"] = 0;
                                }
                            }


                            GrdWarehousePC.JSProperties["cpdeletedata"] = Convert.ToString(1);

                        }

                        else if (isserialenable == "true" && (viewQuantity == "" || viewQuantity == "0") && isbatch == "true" && iswarehouse != "true")
                        {
                            DataRow[] customerRows = dataSet1.Tables[0].Select("BatchNo= '" + batch + "'");

                            for (int i = 0; i <= customerRows.Count() - 1; i++)
                            {
                                if (Convert.ToString(customerRows[i]["SrlNo"]) == slno && string.IsNullOrEmpty(Convert.ToString(customerRows[i]["viewQuantity"])))
                                {
                                    customerRows[i]["isnew"] = "DeleteSL";
                                }
                                else if (Convert.ToString(customerRows[i]["SrlNo"]) == slno && !string.IsNullOrEmpty(Convert.ToString(customerRows[i]["viewQuantity"])))
                                {

                                    customerRows[i]["isnew"] = "DeleteWHBTSL";

                                    //customerRows[i]["isnew"] = "DeleteWHBTSL";
                                }
                                if (!string.IsNullOrEmpty(Convert.ToString(customerRows[i]["viewQuantity"])))
                                {
                                    customerRows[i]["viewQuantity"] = Convert.ToDecimal(customerRows[i]["Quantitysum"]) - 1;
                                    customerRows[i]["Quantity"] = Convert.ToDecimal(customerRows[i]["Quantity"]) - 1;
                                    customerRows[i]["Quantitysum"] = Convert.ToDecimal(customerRows[i]["Quantitysum"]) - 1;

                                    if (Convert.ToString(customerRows[i]["isnew"]) != "new")
                                    {
                                        customerRows[i]["isnew"] = "Updated";
                                    }
                                    else
                                    {
                                        customerRows[i]["isnew"] = "new";
                                    }

                                }
                                else
                                {

                                    customerRows[i]["Quantity"] = 0;
                                    customerRows[i]["Quantitysum"] = 0;

                                }

                            }

                            GrdWarehousePC.JSProperties["cpdeletedata"] = Convert.ToString(1);


                        }
                        else
                        {

                            DataRow[] customerRow = dataSet1.Tables[0].Select("SrlNo ='" + slno + "'");
                            if (isserialenable != "true" && isbatch == "true" && iswarehouse == "true")
                            {
                                customerRow[0]["isnew"] = "DeleteWHBT";
                                GrdWarehousePC.JSProperties["cpdeletedata"] = Convert.ToString(viewQuantity);
                            }
                            if (isserialenable != "true" && isbatch != "true" && iswarehouse == "true")
                            {
                                customerRow[0]["isnew"] = "DeleteWH";
                                GrdWarehousePC.JSProperties["cpdeletedata"] = Convert.ToString(Quantity);
                            }
                            if (isserialenable != "true" && isbatch == "true" && iswarehouse != "true")
                            {
                                customerRow[0]["isnew"] = "DeleteWHBT";
                                GrdWarehousePC.JSProperties["cpdeletedata"] = Convert.ToString(viewQuantity);
                            }
                            if (isserialenable == "true" && isbatch == "true" && iswarehouse == "true")
                            {
                                customerRow[0]["isnew"] = "DeleteWHBTSL";
                                GrdWarehousePC.JSProperties["cpdeletedata"] = Convert.ToString(Quantity);
                            }
                            if (isserialenable == "true" && isbatch != "true" && iswarehouse == "true")
                            {
                                customerRow[0]["isnew"] = "DeleteWHBTSL";
                                GrdWarehousePC.JSProperties["cpdeletedata"] = Convert.ToString(1);
                            }
                            if (isserialenable == "true" && isbatch == "true" && iswarehouse != "true")
                            {
                                customerRow[0]["isnew"] = "DeleteWHBTSL";
                                GrdWarehousePC.JSProperties["cpdeletedata"] = Convert.ToString(1);
                            }
                        }

                    }

                    Warehousedt = null;
                    DataTable setdeletedate = new DataTable();
                    DataTable getdeletedate = dataSet1.Tables[0];

                    DataRow[] drs = getdeletedate.Select("isnew = 'DeleteWHBT' OR isnew = 'DeleteWH' OR isnew = 'DeleteWHBT' OR isnew = 'DeleteBTSL' OR isnew = 'DeleteSL' OR isnew = 'DeleteWHSL' OR isnew = 'DeleteWHBTSL'");
                    if (drs.Count() != 0)
                    {
                        if (Session["PRWarehouseDataDelete"] != null)
                        {
                            setdeletedate = drs.CopyToDataTable();
                            DataTable dtmp = (DataTable)Session["PRWarehouseDataDelete"];
                            dtmp.Merge(setdeletedate);
                            Session["PRWarehouseDataDelete"] = dtmp;
                        }
                        else
                        {
                            setdeletedate = drs.CopyToDataTable();
                            Session["PRWarehouseDataDelete"] = setdeletedate;
                        }


                    }


                    DataTable Warehousedts = dataSet1.Tables[0];
                    DataRow[] dr = Warehousedts.Select("isnew = 'new' OR isnew = 'Updated' OR isnew = 'old'");
                    if (dr.Count() > 0)
                    {

                        Warehousedt = dr.CopyToDataTable();
                        Warehousedt.DefaultView.Sort = "SrlNo asc";
                        Warehousedt = Warehousedt.DefaultView.ToTable(true);

                        Session["PR_WarehouseData"] = Warehousedt;

                        GrdWarehousePC.DataSource = Session["PR_WarehouseData"];
                        GrdWarehousePC.DataBind();
                    }
                    else
                    {
                        Session["PR_WarehouseData"] = null;

                        //Warehousedt.Columns.Add("SrlNo", typeof(string));
                        //Warehousedt.Columns.Add("WarehouseID", typeof(string));
                        //Warehousedt.Columns.Add("WarehouseName", typeof(string));

                        //Warehousedt.Columns.Add("BatchNo", typeof(string));
                        //Warehousedt.Columns.Add("SerialNo", typeof(string));

                        //Warehousedt.Columns.Add("MFGDate", typeof(DateTime));
                        //Warehousedt.Columns.Add("ExpiryDate", typeof(DateTime));
                        //Warehousedt.Columns.Add("Quantity", typeof(decimal));

                        //Warehousedt.Columns.Add("BatchWarehouseID", typeof(string));
                        //Warehousedt.Columns.Add("BatchWarehousedetailsID", typeof(string));
                        //Warehousedt.Columns.Add("BatchID", typeof(string));
                        //Warehousedt.Columns.Add("SerialID", typeof(string));


                        //Warehousedt.Columns.Add("viewWarehouseName", typeof(string));

                        //Warehousedt.Columns.Add("viewBatchNo", typeof(string));
                        //Warehousedt.Columns.Add("viewQuantity", typeof(string));
                        //Warehousedt.Columns.Add("viewSerialNo", typeof(string));



                        //Warehousedt.Columns.Add("viewMFGDate", typeof(DateTime));
                        //Warehousedt.Columns.Add("viewExpiryDate", typeof(DateTime));

                        //Warehousedt.Columns.Add("Quantitysum", typeof(decimal));
                        //Warehousedt.Columns.Add("isnew", typeof(string));

                        GrdWarehousePC.DataSource = Warehousedt;
                        GrdWarehousePC.DataBind();



                    }


                }
            #endregion
            }


        }



        public DataTable GetRecord(string stockids)
        {
            Session["PR_WarehouseData"] = null;
            DataTable Warehousedt = new DataTable();

            if (Session["PR_WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["PR_WarehouseData"];
            }
            else
            {
                //Warehousedt.Columns.Add("Product_SrlNo", typeof(string));
                Warehousedt.Columns.Add("SrlNo", typeof(Int32));
                Warehousedt.Columns.Add("WarehouseID", typeof(string));
                Warehousedt.Columns.Add("WarehouseName", typeof(string));

                Warehousedt.Columns.Add("BatchNo", typeof(string));
                Warehousedt.Columns.Add("SerialNo", typeof(string));

                Warehousedt.Columns.Add("MFGDate", typeof(DateTime));
                Warehousedt.Columns.Add("ExpiryDate", typeof(DateTime));
                Warehousedt.Columns.Add("Quantity", typeof(decimal));

                Warehousedt.Columns.Add("BatchWarehouseID", typeof(string));
                Warehousedt.Columns.Add("BatchWarehousedetailsID", typeof(string));
                Warehousedt.Columns.Add("BatchID", typeof(string));
                Warehousedt.Columns.Add("SerialID", typeof(string));


                Warehousedt.Columns.Add("viewWarehouseName", typeof(string));

                Warehousedt.Columns.Add("viewBatchNo", typeof(string));
                Warehousedt.Columns.Add("viewQuantity", typeof(string));
                Warehousedt.Columns.Add("viewSerialNo", typeof(string));



                Warehousedt.Columns.Add("viewMFGDate", typeof(DateTime));
                Warehousedt.Columns.Add("viewExpiryDate", typeof(DateTime));

                Warehousedt.Columns.Add("Quantitysum", typeof(decimal));
                Warehousedt.Columns.Add("isnew", typeof(string));

                Warehousedt.Columns.Add("productid", typeof(string));
                Warehousedt.Columns.Add("Inventrytype", typeof(string));
                Warehousedt.Columns.Add("StockID", typeof(string));
                Warehousedt.Columns.Add("pcslno", typeof(Int32));
            }


            DataTable dts = GetStockWarehouseData(stockids);

            string oldwarehousename = string.Empty;
            string oldbatchname = string.Empty;
            string oldquantity = string.Empty;
            string isoldornew = string.Empty;
            string BatchWarehouseID = string.Empty;

            if (dts.Rows.Count > 0)
            {
                for (int i = 0; i <= dts.Rows.Count - 1; i++)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dts.Rows[i]["BatchWarehouseID"])) && Convert.ToString(dts.Rows[i]["BatchWarehouseID"]) != "0" && Convert.ToString(Session["PR_ActionType"]) == "Edit")
                    {
                        isoldornew = "old";
                    }
                    else
                    {
                        isoldornew = "new";
                    }

                    if (hdnisserial.Value == "true" && i != 0)
                    {
                        oldwarehousename = Convert.ToString(dts.Rows[i - 1]["warehouseID"]);
                        oldbatchname = Convert.ToString(dts.Rows[i - 1]["batchNO"]);
                        oldquantity = Convert.ToString(dts.Rows[i - 1]["Quantitysum"]);
                        BatchWarehouseID = Convert.ToString(dts.Rows[i - 1]["BatchWarehouseID"]);

                        if (oldwarehousename == Convert.ToString(dts.Rows[i]["warehouseID"]) && oldbatchname == Convert.ToString(dts.Rows[i]["batchNO"]) && (oldquantity == Convert.ToString(dts.Rows[i]["Quantitysum"]) || Convert.ToString(dts.Rows[i]["Quantitysum"]) == "0") && BatchWarehouseID == Convert.ToString(dts.Rows[i]["BatchWarehouseID"]))
                        {

                            if (Convert.ToString(dts.Rows[i]["MfgDate"]) == "1/1/1900 12:00:00 AM" || Convert.ToString(dts.Rows[i]["MfgDate"]) == "01-01-0001 12:00:00 AM")
                            {
                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), "", "", "", Convert.ToString(dts.Rows[i]["serialno"]), null, null, 0, isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                            else if (string.IsNullOrEmpty(Convert.ToString(dts.Rows[i]["MfgDate"])))
                            {
                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), "", "", "", Convert.ToString(dts.Rows[i]["serialno"]), null, null, 0, isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                            else
                            {

                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), Convert.ToDateTime(dts.Rows[i]["MfgDate"]), Convert.ToDateTime(dts.Rows[i]["ExpiryDate"]), Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), "", "", "", Convert.ToString(dts.Rows[i]["serialno"]), null, null, 0, isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                        }
                        //else if (oldwarehousename == Convert.ToString(dts.Rows[i]["warehouseID"]) && oldbatchname == Convert.ToString(dts.Rows[i]["batchNO"]))
                        //{
                        //    if (Convert.ToString(dts.Rows[i]["MfgDate"]) == "1/1/1900 12:00:00 AM")
                        //    {
                        //        Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), "", "", "", Convert.ToString(dts.Rows[i]["serialno"]), null, null, 0, "old");
                        //    }
                        //    else if (string.IsNullOrEmpty(Convert.ToString(dts.Rows[i]["MfgDate"])))
                        //    {
                        //        Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), "", "", "", Convert.ToString(dts.Rows[i]["serialno"]), null, null, 0, "old");
                        //    }
                        //    else
                        //    {

                        //        Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), Convert.ToDateTime(dts.Rows[i]["MfgDate"]), Convert.ToDateTime(dts.Rows[i]["ExpiryDate"]), Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), "", "", "", Convert.ToString(dts.Rows[i]["serialno"]), null, null, 0, "old");
                        //    }
                        //}
                        else
                        {
                            //string viewqunatity = (Convert.ToString(dts.Rows[i]["viewQuantity"])!=string.Empty)? Convert.ToString(dts.Rows[i]["viewQuantity"]):"0";
                            string viewqunatity = Convert.ToString(dts.Rows[i]["viewQuantity"]);
                            string sumquantiy = (Convert.ToString(dts.Rows[i]["Quantitysum"]) != string.Empty) ? Convert.ToString(dts.Rows[i]["Quantitysum"]) : "0";
                            //if (Convert.ToDateTime(dts.Rows[i]["MfgDate"]) != default(DateTime))
                            if (Convert.ToString(dts.Rows[i]["MfgDate"]) == "1/1/1900 12:00:00 AM" || Convert.ToString(dts.Rows[i]["MfgDate"]) == "01-01-0001 12:00:00 AM")
                            {
                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(sumquantiy), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), viewqunatity, Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(sumquantiy), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                            else if (string.IsNullOrEmpty(Convert.ToString(dts.Rows[i]["MfgDate"])))
                            {
                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(sumquantiy), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), viewqunatity, Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(sumquantiy), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                            else
                            {

                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), Convert.ToDateTime(dts.Rows[i]["MfgDate"]), Convert.ToDateTime(dts.Rows[i]["ExpiryDate"]), Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), viewqunatity, Convert.ToString(dts.Rows[i]["serialno"]), Convert.ToDateTime(dts.Rows[i]["MfgDate"]), Convert.ToDateTime(dts.Rows[i]["ExpiryDate"]), Convert.ToString(sumquantiy), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }

                        }

                    }
                    else if (hdnisserial.Value == "false" && hdniswarehouse.Value == "true" && hdnisbatch.Value == "true")
                    {
                        //string oldquantity=string.Empty;
                        if (i != 0)
                        {
                            oldwarehousename = Convert.ToString(dts.Rows[i - 1]["warehouseID"]);
                            oldquantity = Convert.ToString(dts.Rows[i - 1]["viewQuantity"]);
                        }



                        if (oldwarehousename == Convert.ToString(dts.Rows[i]["warehouseID"]))
                        {
                            //if (Convert.ToDateTime(dts.Rows[i]["MfgDate"]) != default(DateTime))
                            if (Convert.ToString(dts.Rows[i]["MfgDate"]) == "1/1/1900 12:00:00 AM" || Convert.ToString(dts.Rows[i]["MfgDate"]) == "01-01-0001 12:00:00 AM")
                            {
                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), "", Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(dts.Rows[i]["viewQuantity"]), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                            else if (string.IsNullOrEmpty(Convert.ToString(dts.Rows[i]["MfgDate"])))
                            {
                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), "", Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(dts.Rows[i]["viewQuantity"]), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                            else
                            {

                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), Convert.ToDateTime(dts.Rows[i]["MfgDate"]), Convert.ToDateTime(dts.Rows[i]["ExpiryDate"]), Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), "", Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(dts.Rows[i]["viewQuantity"]), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                        }
                        else
                        {
                            //  if (Convert.ToDateTime(dts.Rows[i]["MfgDate"]) != default(DateTime))
                            if (Convert.ToString(dts.Rows[i]["MfgDate"]) == "1/1/1900 12:00:00 AM" || Convert.ToString(dts.Rows[i]["MfgDate"]) == "01-01-0001 12:00:00 AM")
                            {
                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(dts.Rows[i]["viewQuantity"]), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                            else if (string.IsNullOrEmpty(Convert.ToString(dts.Rows[i]["MfgDate"])))
                            {
                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(dts.Rows[i]["viewQuantity"]), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                            else
                            {

                                Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), Convert.ToDateTime(dts.Rows[i]["MfgDate"]), Convert.ToDateTime(dts.Rows[i]["ExpiryDate"]), Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), Convert.ToString(dts.Rows[i]["serialno"]), Convert.ToDateTime(dts.Rows[i]["MfgDate"]), Convert.ToDateTime(dts.Rows[i]["ExpiryDate"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                            }
                        }

                    }
                    else
                    {
                        if (Convert.ToString(dts.Rows[i]["MfgDate"]) == "1/1/1900 12:00:00 AM" || Convert.ToString(dts.Rows[i]["MfgDate"]) == "01-01-0001 12:00:00 AM")
                        {
                            Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(dts.Rows[i]["viewQuantity"]), isoldornew, Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                        }
                        else if (string.IsNullOrEmpty(Convert.ToString(dts.Rows[i]["MfgDate"])))
                        {
                            Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["serialno"]), null, null, Convert.ToString(dts.Rows[i]["viewQuantity"]), isoldornew, Convert.ToString(hdfProductIDPC.Value), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                        }
                        else
                        {

                            Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, Convert.ToString(dts.Rows[i]["warehouseID"]).Trim(), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["serialno"]), Convert.ToDateTime(dts.Rows[i]["MfgDate"]), Convert.ToDateTime(dts.Rows[i]["ExpiryDate"]), Convert.ToDecimal(dts.Rows[i]["Quantitysum"]), Convert.ToString(dts.Rows[i]["BatchWarehouseID"]), Convert.ToString(dts.Rows[i]["BatchWarehousedetailsID"]), Convert.ToString(dts.Rows[i]["BatchID"]), Convert.ToString(dts.Rows[i]["SerialID"]), Convert.ToString(dts.Rows[i]["WarehouseName"]), Convert.ToString(dts.Rows[i]["batchNO"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), Convert.ToString(dts.Rows[i]["serialno"]), Convert.ToDateTime(dts.Rows[i]["MfgDate"]), Convert.ToDateTime(dts.Rows[i]["ExpiryDate"]), Convert.ToString(dts.Rows[i]["viewQuantity"]), isoldornew, Convert.ToString(dts.Rows[i]["productid"]), getinventry(), Convert.ToString(hdfstockidPC.Value), Convert.ToInt32(hdnpcslno.Value));
                        }

                    }


                    //Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, BatchName, SerialName, "", "", openingstock, "isnew");

                }
            }

            hdnoldrowcount.Value = Convert.ToString(Warehousedt.Rows.Count);

            return Warehousedt;
        }

        private int Insertupdatedata(string ProductID, string stockid, string branchid)
        {
            DataSet dsEmail = new DataSet();

            string IsserialActive = "false";

            int newrowcount = 0;

            #region Insert

            DataTable temtable = new DataTable();
            DataTable temtable2 = new DataTable();
            DataTable temtable23 = new DataTable();
            if (Session["PR_WarehouseData"] != null)
            {
                DataTable Warehousedt = (DataTable)Session["PR_WarehouseData"];

                temtable = Warehousedt.DefaultView.ToTable(false, "SrlNo", "BatchWarehouseID", "BatchWarehousedetailsID", "BatchID", "SerialID", "WarehouseID", "WarehouseName", "BatchNo", "SerialNo", "MFGDate", "ExpiryDate", "Quantitysum", "isnew", "viewQuantity", "productid", "Inventrytype", "StockID", "pcslno");

                if (Session["PRwarehousedetailstemp"] != null)
                {
                    temtable23 = (DataTable)Session["PRwarehousedetailstemp"];
                    DataRow[] rows;
                    rows = temtable23.Select("pcslno = '" + hdnpcslno.Value + "'");  //'UserName' is ColumnName
                    foreach (DataRow row in rows)
                        temtable23.Rows.Remove(row);

                    temtable23.Merge(temtable);
                    Session["PRwarehousedetailstemp"] = temtable23;
                }
                else
                {
                    Session["PRwarehousedetailstemp"] = temtable;
                }

            }


            #endregion
            #region Update
            if (Session["PR_WarehouseData"] != null)
            {
                DataTable dt = new DataTable();
                DataTable Warehousedtups = (DataTable)Session["PR_WarehouseData"];
                DataRow[] dr = Warehousedtups.Select("isnew = 'Updated'");
                if (dr.Count() != 0)
                {
                    dt = dr.CopyToDataTable();
                    if (Session["PRwarehousedetailstempUpdate"] != null)
                    {
                        temtable23 = (DataTable)Session["PRwarehousedetailstempUpdate"];
                        DataRow[] rows;
                        rows = temtable23.Select("pcslno = '" + hdnpcslno.Value + "'");  //'UserName' is ColumnName
                        foreach (DataRow row in rows)
                            temtable23.Rows.Remove(row);

                        temtable23.Merge(dt);
                        Session["PRwarehousedetailstempUpdate"] = temtable23;
                    }
                    else
                    {
                        Session["PRwarehousedetailstempUpdate"] = dt;
                    }


                }
            }

            #endregion
            #region delete

            if (Session["PRWarehouseDataDelete"] != null)
            {
                DataTable dt = new DataTable();
                DataTable Warehousedtups = (DataTable)Session["PRWarehouseDataDelete"];
                DataRow[] dr = Warehousedtups.Select("isnew = 'DeleteWHBT' OR isnew = 'DeleteWH' OR isnew = 'DeleteWHBT' OR isnew = 'DeleteBTSL' OR isnew = 'DeleteSL' OR isnew = 'DeleteWHSL' OR isnew = 'DeleteWHBTSL'");
                if (dr.Count() != 0)
                {
                    dt = dr.CopyToDataTable();
                    if (Session["PRwarehousedetailstempDelete"] != null)
                    {
                        temtable23 = (DataTable)Session["PRwarehousedetailstempDelete"];
                        DataRow[] rows;
                        rows = temtable23.Select("pcslno = '" + hdnpcslno.Value + "'");  //'UserName' is ColumnName
                        foreach (DataRow row in rows)
                            temtable23.Rows.Remove(row);

                        temtable23.Merge(dt);
                        Session["PRwarehousedetailstempDelete"] = temtable23;
                    }
                    else
                    {
                        Session["PRwarehousedetailstempDelete"] = dt;
                    }


                }

                //deleteALL(stockid);

            }
            #endregion

            return 1;


        }
        protected void GrdWarehousePC_DataBinding(object sender, EventArgs e)
        {
            if (Session["PR_WarehouseData"] != null)
            {
                DataTable Warehousedt = (DataTable)Session["PR_WarehouseData"];

                GrdWarehousePC.DataSource = Warehousedt;
            }
        }

        protected void CmbWarehouse_Callback(object sender, CallbackEventArgsBase e)
        {
            string WhichCall = e.Parameter.Split('~')[0];

            if (WhichCall == "BindWarehouse")
            {
                DataTable dt = GetWarehouseData();
                GetAvailableStock();

                CmbWarehouse.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CmbWarehouse.Items.Add(Convert.ToString(dt.Rows[i]["WarehouseName"]), Convert.ToString(dt.Rows[i]["WarehouseID"]));
                }
                if (hdndefaultID != null || hdndefaultID.Value != "")
                    CmbWarehouse.Value = hdndefaultID.Value;
            }

        }

        public DataTable GetWarehouseData()
        {
            string strBranch = Convert.ToString(ddl_Branch.SelectedValue);

            DataTable dt = new DataTable();
            dt = oDBEngine.GetDataTable("select  bui_id as WarehouseID,bui_Name as WarehouseName from tbl_master_building Where IsNull(bui_BranchId,0) in ('0','" + strBranch + "') order by bui_Name");
            return dt;
        }

        public void GetAvailableStock()
        {
            //DataTable dt2 = oDBEngine.GetDataTable("select ISnull(ISNULL(tblwarehous.StockBranchWarehouse_StockIn,0)- isnull(tblwarehous.StockBranchWarehouse_StockOut,0),0) as branchopenstock from Trans_StockBranchWarehouse tblwarehous where tblwarehous.StockBranchWarehouse_StockId=" + hdfstockidPC.Value + " and tblwarehous.StockBranchWarehouse_CompanyId='" + Convert.ToString(Session["LastCompany"]) + "' and tblwarehous.StockBranchWarehouse_FinYear='" + Convert.ToString(Session["LastFinYear"]) + "' and tblwarehous.StockBranchWarehouse_BranchId=" + hdbranchIDPC.Value + "");
            try
            {
                DataTable dt2 = oDBEngine.GetDataTable("Select dbo.fn_CheckAvailableStockOpening(" + hdbranchIDPC.Value + ",'" + Convert.ToString(Session["LastCompany"]) + "','" + Convert.ToString(Session["LastFinYear"]) + "'," + hdfstockidPC.Value + ") as branchopenstock");

                if (dt2.Rows.Count > 0)
                {
                    CmbWarehouse.JSProperties["cpstock"] = Convert.ToString(dt2.Rows[0]["branchopenstock"]);
                }
                else
                {

                    CmbWarehouse.JSProperties["cpstock"] = "0.0000";
                }
            }
            catch (Exception ex)
            {

            }
        }
        public DataTable GetStockWarehouseData(string stockid)
        {
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            if (Convert.ToString(Session["PR_ActionType"]) == "Edit")
            {
                DataSet dsInst = new DataSet();


                //SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["DBConnectionDefault"]);
                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));


                SqlCommand cmd = new SqlCommand("prc_StockGetPCwarehousentry", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StockID", Convert.ToInt32(stockid));
                cmd.Parameters.AddWithValue("@ProductID", Convert.ToInt32(hdfProductIDPC.Value));
                cmd.Parameters.AddWithValue("@branchID", Convert.ToInt32(hdbranchIDPC.Value));
                cmd.Parameters.AddWithValue("@compnay", Convert.ToString(Session["LastCompany"]));
                cmd.Parameters.AddWithValue("@Finyear", Convert.ToString(Session["LastFinYear"]));
                cmd.Parameters.AddWithValue("@PCNumber", Convert.ToString(Session["PR_ReturnID"]));
                cmd.Parameters.AddWithValue("@ISfromPO", "MainPurchaseReturnWarehouse");
                cmd.CommandTimeout = 0;
                SqlDataAdapter Adap = new SqlDataAdapter();
                Adap.SelectCommand = cmd;
                Adap.Fill(dsInst);
                cmd.Dispose();
                con.Dispose();

                if (dsInst.Tables != null)
                {


                    if (Session["PRwarehousedetailstemp"] != null)
                    {
                        DataTable Warehousedtss = (DataTable)Session["PRwarehousedetailstemp"];

                        DataRow[] dr = Warehousedtss.Select("productid = '" + Convert.ToString(hdfProductIDPC.Value) + "' AND pcslno = '" + hdnpcslno.Value + "'");
                        if (dr.Count() > 0)
                        {

                            Warehousedtss = dr.CopyToDataTable();
                            Warehousedtss.DefaultView.Sort = "SrlNo asc";
                            Warehousedtss = Warehousedtss.DefaultView.ToTable(true);

                            Session["PRWarehouseDatabyslno"] = Warehousedtss;

                            dt2 = (DataTable)Session["PRWarehouseDatabyslno"];
                            DataTable dtmp = dsInst.Tables[0];

                            if (Session["PRwarehousedetailstempUpdate"] != null)
                            {
                                DataRow[] drs = Warehousedtss.Select("productid = '" + Convert.ToString(hdfProductIDPC.Value) + "' AND pcslno = '" + hdnpcslno.Value + "' AND isnew='Updated'");
                                if (drs.Count() <= 0)
                                {
                                    dt2.Merge(dtmp);
                                }
                            }

                            dt = dt2;

                        }
                        else
                        {
                            dt = dsInst.Tables[0];
                        }

                    }
                    else
                    {
                        dt = dsInst.Tables[0];
                    }

                }
            }


            else if (Convert.ToString(Session["PR_ActionType"]) != "Edit")
            {
                DataSet dsInst = new DataSet();
               // SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["DBConnectionDefault"]);

                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));

                SqlCommand cmd = new SqlCommand("prc_StockGetPCwarehousentry", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StockID", Convert.ToInt32(stockid));
                cmd.Parameters.AddWithValue("@ProductID", Convert.ToInt32(hdfProductIDPC.Value));
                cmd.Parameters.AddWithValue("@branchID", Convert.ToInt32(hdbranchIDPC.Value));
                cmd.Parameters.AddWithValue("@compnay", Convert.ToString(Session["LastCompany"]));
                cmd.Parameters.AddWithValue("@Finyear", Convert.ToString(Session["LastFinYear"]));
                cmd.Parameters.AddWithValue("@PCNumber", Convert.ToString(hdnInnumber.Value));
                cmd.Parameters.AddWithValue("@ISfromPO", "MainPurchaseInvoiceReturnWarehouse");
                cmd.CommandTimeout = 0;
                SqlDataAdapter Adap = new SqlDataAdapter();
                Adap.SelectCommand = cmd;
                Adap.Fill(dsInst);
                cmd.Dispose();
                con.Dispose();

                if (dsInst.Tables != null)
                {

                    if (Session["PRwarehousedetailstemp"] != null)
                    {
                        DataTable Warehousedtss = (DataTable)Session["PRwarehousedetailstemp"];

                        DataRow[] dr = Warehousedtss.Select("productid = '" + Convert.ToString(hdfProductIDPC.Value) + "' AND pcslno = '" + hdnpcslno.Value + "'");
                        if (dr.Count() > 0)
                        {

                            Warehousedtss = dr.CopyToDataTable();
                            Warehousedtss.DefaultView.Sort = "SrlNo asc";
                            Warehousedtss = Warehousedtss.DefaultView.ToTable(true);

                            Session["PRWarehouseDatabyslno"] = Warehousedtss;

                            dt2 = (DataTable)Session["PRWarehouseDatabyslno"];
                            DataTable dtmp = dsInst.Tables[0];

                            if (Session["PRwarehousedetailstempUpdate"] != null)
                            {
                                DataRow[] drs = Warehousedtss.Select("productid = '" + Convert.ToString(hdfProductIDPC.Value) + "' AND pcslno = '" + hdnpcslno.Value + "' AND isnew='Updated'");
                                if (drs.Count() <= 0)
                                {
                                    dt2.Merge(dtmp);
                                }
                            }

                            dt = dt2;

                        }
                        else
                        {
                            dt = dsInst.Tables[0];
                        }

                    }
                    else
                    {
                        dt = dsInst.Tables[0];
                    }

                }
            }
            else if (Session["PRwarehousedetailstemp"] != null)
            {
                DataTable Warehousedtss = (DataTable)Session["PRwarehousedetailstemp"];

                DataRow[] dr = Warehousedtss.Select("productid = '" + Convert.ToString(hdfProductIDPC.Value) + "' AND pcslno = '" + hdnpcslno.Value + "'");
                if (dr.Count() > 0)
                {

                    Warehousedtss = dr.CopyToDataTable();
                    Warehousedtss.DefaultView.Sort = "SrlNo asc";
                    Warehousedtss = Warehousedtss.DefaultView.ToTable(true);

                    Session["PRWarehouseDatabyslno"] = Warehousedtss;

                    dt = (DataTable)Session["PRWarehouseDatabyslno"];

                }

            }

            return dt;
        }

        private string getinventry()
        {

            string inventryType = string.Empty;
            if (hdniswarehouse.Value == "true")
            {
                inventryType = "WH";

            }
            if (hdnisbatch.Value == "true")
            {

                inventryType += "BT";
            }
            if (hdnisserial.Value == "true")
            {
                inventryType += "SL";

            }
            return inventryType;
        }

        public string SetVisibilityStock(object container)
        {
            string vs = string.Empty;
            GridViewDataItemTemplateContainer c = container as GridViewDataItemTemplateContainer;
            if (!string.IsNullOrEmpty(Convert.ToString(c.KeyValue)))
            {
                //if (isexist)
                //{
                vs = "display:block";
                //}
                //else
                //{
                //    vs = "display:none";
                //}

            }
            else
            {
                vs = "display:block";
            }

            return vs;


        }

        public void deleteALL()
        {

            #region delete

            if (Session["PRwarehousedetailstempDelete"] != null)
            {
                string stockid = string.Empty;
                DataTable Warehousedtups = (DataTable)Session["PRwarehousedetailstempDelete"];
                for (int i = 0; i <= Warehousedtups.Rows.Count - 1; i++)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(Warehousedtups.Rows[i]["BatchWarehouseID"])) && Convert.ToString(Warehousedtups.Rows[i]["BatchWarehouseID"]) != "0")
                    {
                        stockid = Convert.ToString(Warehousedtups.Rows[i]["StockID"]);

                        if (Convert.ToString(Warehousedtups.Rows[i]["isnew"]) == "DeleteSL")
                        {
                            string sqls = "delete Trans_StockSerialMapping where Stock_MapId=" + Convert.ToString(Warehousedtups.Rows[i]["SerialID"]) + " " +
                                               " update Trans_Stock set Stock_In=isnull(Stock_In,0)-1," +
                                       " Stock_ModifiedDate=GETDATE() WHERE Stock_ID=" + stockid + "	";

                            oDBEngine.GetDataTable(sqls);
                        }
                        else if (Convert.ToString(Warehousedtups.Rows[i]["isnew"]) == "DeleteWHBTSL")
                        {
                            var updateqnty = Warehousedtups.Compute("sum(Quantitysum)", "isnew = 'DeleteWHBTSL' AND BatchWarehouseID='" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehouseID"]) + "'");

                            string sqls = "delete from Trans_StockBranchWarehouseDetails where StockBranchWarehouseDetail_Id=" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehousedetailsID"]) + " " +
                                          " delete from Trans_StockBranchWarehouse where StockBranchWarehouse_Id=" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehouseID"]) + " " +
                                          " delete from Trans_StockBranchBatch where StockBranchWarehouseDetail_Id=" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehousedetailsID"]) + " " +
                                          " delete from Trans_StockSerialMapping where Stock_BranchBatchId=" + Convert.ToString(Warehousedtups.Rows[i]["BatchID"]) + " update Trans_Stock set Stock_In=isnull(Stock_In,0)-" + updateqnty + "," +
                                       " Stock_ModifiedDate=GETDATE() WHERE Stock_ID=" + stockid + "	";
                            if (!string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                            {
                                oDBEngine.GetDataTable(sqls);
                            }

                        }
                        else if (Convert.ToString(Warehousedtups.Rows[i]["isnew"]) == "DeleteWHSL")
                        {

                            var updateqnty = Warehousedtups.Compute("sum(Quantitysum)", "isnew = 'DeleteWHSL' AND BatchWarehouseID='" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehouseID"]) + "'");

                            string sqls = "delete from Trans_StockBranchWarehouseDetails where StockBranchWarehouseDetail_Id=" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehousedetailsID"]) + " " +
                                          " delete from Trans_StockBranchWarehouse where StockBranchWarehouse_Id=" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehouseID"]) + " " +
                                          " delete from Trans_StockBranchBatch where StockBranchWarehouseDetail_Id=" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehousedetailsID"]) + " " +
                                          " delete from Trans_StockSerialMapping where Stock_BranchBatchId=" + Convert.ToString(Warehousedtups.Rows[i]["BatchID"]) + " update Trans_Stock set Stock_In=isnull(Stock_In,0)-" + updateqnty + "," +
                                       " Stock_ModifiedDate=GETDATE() WHERE Stock_ID=" + stockid + "	";
                            if (!string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                            {
                                oDBEngine.GetDataTable(sqls);
                            }


                        }
                        else if (Convert.ToString(Warehousedtups.Rows[i]["isnew"]) == "DeleteWHBT")
                        {
                            string sqls = string.Empty;
                            var updateqnty = Warehousedtups.Compute("sum(Quantitysum)", "isnew = 'DeleteWHBT' AND BatchWarehouseID='" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehouseID"]) + "'");

                            if (string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                            { updateqnty = 0.0; }


                            if (stockid != "0")
                            {
                                DataSet dsEmail = new DataSet();

                                //String conn = ConfigurationSettings.AppSettings["DBConnectionDefault"];

                                String conn = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);


                                SqlConnection con = new SqlConnection(conn);
                                SqlCommand cmd3 = new SqlCommand("prc_StockPchallanwarehousbatchDelete", con);
                                cmd3.CommandType = CommandType.StoredProcedure;

                                cmd3.Parameters.AddWithValue("@compnay", Convert.ToString(Session["LastCompany"]));
                                cmd3.Parameters.AddWithValue("@Finyear", Convert.ToString(Session["LastFinYear"]));
                                cmd3.Parameters.AddWithValue("@StockID", Convert.ToInt32(stockid));

                                cmd3.Parameters.AddWithValue("@BatchWarehouseID", Convert.ToInt32(Warehousedtups.Rows[i]["BatchWarehouseID"]));
                                cmd3.Parameters.AddWithValue("@BatchWarehousedetailsID", Convert.ToInt32(Warehousedtups.Rows[i]["BatchWarehousedetailsID"]));
                                cmd3.Parameters.AddWithValue("@BatchID", Convert.ToInt32(Warehousedtups.Rows[i]["BatchID"]));


                                //cmd3.Parameters.AddWithValue("@branchID", Convert.ToInt32(hdnselectedbranch.Value));
                                cmd3.Parameters.AddWithValue("@modifiedBy", Convert.ToInt32(HttpContext.Current.Session["userid"]));
                                cmd3.Parameters.AddWithValue("@rate", Convert.ToDecimal(hdnrate.Value));
                                cmd3.Parameters.AddWithValue("@updateqnty", Convert.ToDecimal(updateqnty));

                                SqlParameter output = new SqlParameter("@ReturnValue", SqlDbType.Int);
                                output.Direction = ParameterDirection.Output;
                                cmd3.Parameters.Add(output);
                                cmd3.CommandTimeout = 0;
                                SqlDataAdapter Adap = new SqlDataAdapter();
                                Adap.SelectCommand = cmd3;
                                Adap.Fill(dsEmail);
                                dsEmail.Clear();
                                cmd3.Dispose();
                                con.Dispose();
                                GC.Collect();
                            }
                        }
                        else if (Convert.ToString(Warehousedtups.Rows[i]["isnew"]) == "DeleteWH")
                        {

                            var updateqnty = Warehousedtups.Compute("sum(Quantity)", "isnew = 'DeleteWH' AND BatchWarehouseID='" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehouseID"]) + "'");

                            string sqls = "delete from Trans_StockBranchWarehouseDetails where StockBranchWarehouseDetail_Id=" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehousedetailsID"]) + " " +
                                          " delete from Trans_StockBranchWarehouse where StockBranchWarehouse_Id=" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehouseID"]) + " " +
                                          " delete from Trans_StockBranchBatch where StockBranchWarehouseDetail_Id=" + Convert.ToString(Warehousedtups.Rows[i]["BatchWarehousedetailsID"]) + "  update Trans_Stock set Stock_In=isnull(Stock_In,0)-" + updateqnty + "," +
                                       " Stock_ModifiedDate=GETDATE() WHERE Stock_ID=" + stockid + "	";
                            if (!string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                            {
                                oDBEngine.GetDataTable(sqls);
                            }
                        }
                    }


                }


            }
            #endregion

        }


        private int Insertwaredataalldata(string ProductID, string stockid, string branchid)
        {
            DataSet dsEmail = new DataSet();
            string inventryType = string.Empty;
            string IsserialActive = "false";

            int newrowcount = 0;

            #region Insert
           // String conn = ConfigurationSettings.AppSettings["DBConnectionDefault"];

            String conn = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);


            SqlConnection con = new SqlConnection(conn);
            SqlCommand cmd3 = new SqlCommand("prc_StockPChallanwarehousentry", con);
            cmd3.CommandType = CommandType.StoredProcedure;

            cmd3.Parameters.AddWithValue("@compnay", Convert.ToString(Session["LastCompany"]));
            cmd3.Parameters.AddWithValue("@Finyear", Convert.ToString(Session["LastFinYear"]));
            cmd3.Parameters.AddWithValue("@StockID", Convert.ToInt32(stockid));
            cmd3.Parameters.AddWithValue("@ProductID", Convert.ToInt32(ProductID));
            cmd3.Parameters.AddWithValue("@branchID", Convert.ToInt32(branchid));
            cmd3.Parameters.AddWithValue("@createdBy", Convert.ToInt32(HttpContext.Current.Session["userid"]));
            cmd3.Parameters.AddWithValue("@PCNumber", Convert.ToInt32(HttpContext.Current.Session["userid"]));
            cmd3.Parameters.AddWithValue("@totalqntity", Convert.ToString(hdntotalqntyPC.Value));


            if (hdniswarehouse.Value == "true")
            {
                inventryType = "WH";

            }
            if (hdnisbatch.Value == "true")
            {

                inventryType += "BT";
            }
            if (hdnisserial.Value == "true")
            {
                inventryType += "SL";
                IsserialActive = "true";
            }

            cmd3.Parameters.AddWithValue("@IsSerialActive", IsserialActive);


            cmd3.Parameters.AddWithValue("@Inventrytype", inventryType);

            DataTable temtable = new DataTable();
            DataTable temtable2 = new DataTable();
            DataTable temtable23 = new DataTable();
            if (Session["PR_WarehouseData"] != null)
            {
                DataTable Warehousedt = (DataTable)Session["PR_WarehouseData"];

                temtable = Warehousedt.DefaultView.ToTable(false, "SrlNo", "BatchWarehouseID", "BatchWarehousedetailsID", "BatchID", "SerialID", "WarehouseID", "WarehouseName", "BatchNo", "SerialNo", "MFGDate", "ExpiryDate", "Quantitysum", "productid", "Inventrytype", "StockID", "isnew");
                cmd3.Parameters.AddWithValue("@udt_StockOpeningwarehousentried", temtable);
            }
            if (Session["PRWarehouseDataDelete"] != null)
            {
                DataTable Warehousedts = (DataTable)Session["PRWarehouseDataDelete"];

                temtable2 = Warehousedts.DefaultView.ToTable(false, "SrlNo", "BatchWarehouseID", "BatchWarehousedetailsID", "BatchID", "SerialID", "WarehouseID", "WarehouseName", "BatchNo", "SerialNo", "MFGDate", "ExpiryDate", "Quantity", "isnew");
                cmd3.Parameters.AddWithValue("@udt_StockOpeningwarehousentrieDelete", null);
            }

            #endregion

            return 1;


        }
        #endregion

        #endregion

        #region Unique Code Generated Section Start

        public string checkNMakeJVCode(string manual_str, int sel_schema_Id)
        {

           // oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
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

                    sqlQuery = "SELECT max(tjv.Return_Number) FROM tbl_trans_purchasereturn tjv WHERE dbo.RegexMatch('";
                    if (prefLen > 0)
                        sqlQuery += "^[" + prefCompCode + "]{" + prefLen + "}";
                    else if (scheme_type == 2)
                        sqlQuery += "^";
                    sqlQuery += "[0-9]{" + paddCounter + "}";
                    if (sufxLen > 0)
                        sqlQuery += "[" + sufxCompCode + "]{" + sufxLen + "}";
                    sqlQuery += "?$', LTRIM(RTRIM(tjv.Return_Number))) = 1";
                    if (scheme_type == 2)
                        sqlQuery += " AND CONVERT(DATE, Return_Date) = CONVERT(DATE, GETDATE())";
                    dtC = oDBEngine.GetDataTable(sqlQuery);

                    if (dtC.Rows[0][0].ToString() == "")
                    {
                        sqlQuery = "SELECT max(tjv.Return_Number) FROM tbl_trans_purchasereturn tjv WHERE dbo.RegexMatch('";
                        if (prefLen > 0)
                            sqlQuery += "^[" + prefCompCode + "]{" + prefLen + "}";
                        else if (scheme_type == 2)
                            sqlQuery += "^";
                        sqlQuery += "[0-9]{" + (paddCounter - 1) + "}";
                        if (sufxLen > 0)
                            sqlQuery += "[" + sufxCompCode + "]{" + sufxLen + "}";
                        sqlQuery += "?$', LTRIM(RTRIM(tjv.Return_Number))) = 1";
                        if (scheme_type == 2)
                            sqlQuery += " AND CONVERT(DATE, Return_Date) = CONVERT(DATE, GETDATE())";
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
                    sqlQuery = "SELECT Return_Number FROM tbl_trans_purchasereturn WHERE Return_Number LIKE '" + manual_str.Trim() + "'";
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

        #region Debu

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
            ProcedureExecute proc = new ProcedureExecute("prc_PurchaseReturn_Details");
            proc.AddVarcharPara("@Action", 500, "TaxDetailsForGst");
            proc.AddVarcharPara("@PurchaseReturnID", 500, Convert.ToString(Session["PR_ReturnID"]));
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
                DataTable MainTaxDataTable = (DataTable)Session["PR_FinalTaxRecord"];

                DataRow[] deletedRow = MainTaxDataTable.Select("SlNo=" + performpara.Split('~')[1]);
                if (deletedRow.Length > 0)
                {
                    foreach (DataRow dr in deletedRow)
                    {
                        MainTaxDataTable.Rows.Remove(dr);
                    }

                }

                Session["PR_FinalTaxRecord"] = MainTaxDataTable;
                GetStock(Convert.ToString(performpara.Split('~')[1]));
                // DeleteWarehouse(Convert.ToString(performpara.Split('~')[1]));
                DataTable taxDetails = (DataTable)Session["PR_TaxDetails"];
                if (taxDetails != null)
                {
                    foreach (DataRow dr in taxDetails.Rows)
                    {
                        dr["Amount"] = "0.00";
                    }
                    Session["PR_TaxDetails"] = taxDetails;
                }
            }
            else if (performpara.Split('~')[0] == "DeleteAllTax")
            {
                CreateDataTaxTable();

                DataTable taxDetails = (DataTable)Session["PR_TaxDetails"];

                if (taxDetails != null)
                {
                    foreach (DataRow dr in taxDetails.Rows)
                    {
                        dr["Amount"] = "0.00";
                    }
                    Session["PR_TaxDetails"] = taxDetails;
                }
            }
            else
            {
                DataTable MainTaxDataTable = (DataTable)Session["PR_FinalTaxRecord"];

                DataRow[] deletedRow = MainTaxDataTable.Select("SlNo=" + performpara.Split('~')[1]);
                if (deletedRow.Length > 0)
                {
                    foreach (DataRow dr in deletedRow)
                    {
                        MainTaxDataTable.Rows.Remove(dr);
                    }

                }

                Session["PR_FinalTaxRecord"] = MainTaxDataTable;

                DataTable taxDetails = (DataTable)Session["PR_TaxDetails"];

                if (taxDetails != null)
                {
                    foreach (DataRow dr in taxDetails.Rows)
                    {
                        dr["Amount"] = "0.00";
                    }
                    Session["PR_TaxDetails"] = taxDetails;
                }
            }
        }
        public double ReCalculateTaxAmount(string slno, double amount)
        {
            DataTable MainTaxDataTable = (DataTable)Session["PR_FinalTaxRecord"];
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
            Session["PR_FinalTaxRecord"] = MainTaxDataTable;

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
            Session["PR_FinalTaxRecord"] = TaxRecord;
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
            ProcedureExecute proc = new ProcedureExecute("prc_PurchaseReturn_Details");
            proc.AddVarcharPara("@Action", 500, "ProductTaxDetails");
            proc.AddVarcharPara("@PurchaseReturnID", 500, Convert.ToString(Session["PR_ReturnID"]));
            ds = proc.GetDataSet();
            return ds;
        }
        public DataSet GetQuotationEditedTaxData()
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_PurchaseReturn_Details");
            proc.AddVarcharPara("@Action", 500, "ProductEditedTaxDetails");
            proc.AddVarcharPara("@PurchaseReturnID", 500, Convert.ToString(Session["PR_ReturnID"]));
            ds = proc.GetDataSet();
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
                DataTable TaxRecord = (DataTable)Session["PR_FinalTaxRecord"];
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

                Session["PR_FinalTaxRecord"] = TaxRecord;
            }
            else
            {
                #region fetch All data For Tax

                DataTable taxDetail = new DataTable();
                DataTable MainTaxDataTable = (DataTable)Session["PR_FinalTaxRecord"];
                DataTable databaseReturnTable = (DataTable)Session["PR_QuotationTaxDetails"];

                //if (Convert.ToInt32(e.Parameters.Split('~')[1]) == 1)
                //    taxDetail = oDBEngine.GetDataTable("select TaxRates_ID as Taxes_ID,TaxRatesSchemeName as Taxes_Name,TaxCalculateMethods,tm.Taxes_Name as taxCodeName,tm.Taxes_ApplicableOn as 'ApplicableOn',isnull((select TaxRatesSchemeName from Config_TaxRates where TaxRates_ID=tm.Taxes_OtherTax),'')  as dependOn from Master_taxes tm inner join Config_TaxRates ct on tm.Taxes_ID=ct.TaxRates_TaxCode where tm.TaxTypeCode not in('G','V','C') and tm.TaxItemlevel='Y'");
                //else if (Convert.ToInt32(e.Parameters.Split('~')[1]) == 2)
                //taxDetail = oDBEngine.GetDataTable("select TaxRates_ID as Taxes_ID,TaxRatesSchemeName as Taxes_Name,TaxCalculateMethods,tm.Taxes_Name as taxCodeName,tm.Taxes_ApplicableOn as 'ApplicableOn',isnull((select TaxRatesSchemeName from Config_TaxRates where TaxRates_ID=tm.Taxes_OtherTax),'')  as dependOn from Master_taxes tm inner join Config_TaxRates ct on tm.Taxes_ID=ct.TaxRates_TaxCode where tm.TaxTypeCode not in('G','V','C') and tm.TaxItemlevel='Y'");

                ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
                proc.AddVarcharPara("@Action", 500, "LoadOtherTaxDetails");
                proc.AddVarcharPara("@S_quoteDate", 10, dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                taxDetail = proc.GetTable();

                //Get Company Gstin 09032017
                string CompInternalId = Convert.ToString(Session["LastCompany"]);
                string[] compGstin = oDBEngine.GetFieldValue1("tbl_master_company", "cmp_gstin", "cmp_internalid='" + CompInternalId + "'", 1);

                string ShippingState = "";

                #region ##### Added By : Samrat Roy -- For BillingShippingUserControl ######
                string sstateCode = BillingShippingControl.GetShippingStateCode(Request.QueryString["key"]);
                ShippingState = sstateCode;
                if (ShippingState.Trim() != "")
                {
                    ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
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

               


                if (ShippingState.Trim() != "" && compGstin[0].Trim() != "")
                {

                    if (compGstin.Length > 0)
                    {
                        if (compGstin[0].Substring(0, 2) == ShippingState)
                        {
                            //Check if the state is in union territories then only UTGST will apply
                            //   Chandigarh     Andaman and Nicobar Islands    DADRA & NAGAR HAVELI    DAMAN & DIU      Lakshadweep              PONDICHERRY
                            if (ShippingState == "4" || ShippingState == "35" || ShippingState == "26" || ShippingState == "25" || ShippingState == "31" || ShippingState == "34")
                            {
                                foreach (DataRow dr in taxDetail.Rows)
                                {
                                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
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
                }

                //If Company GSTIN is blank then Delete All CGST,UGST,IGST,CGST
                if (compGstin[0].Trim() == "")
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

                //Check If any TaxScheme Set Against that Product Then update there rate 22-03-2017 and rate
                string[] schemeIDViaProdID = oDBEngine.GetFieldValue1("master_sproducts", "isnull(sProduct_TaxSchemeSale,0)sProduct_TaxSchemeSale", "sProducts_ID='" + Convert.ToString(setCurrentProdCode.Value) + "'", 1);
                //&& schemeIDViaProdID[0] != ""
                if (schemeIDViaProdID.Length > 0)
                {

                    if (taxDetail.Select("Taxes_ID='" + schemeIDViaProdID[0] + "'").Length > 0)
                    {
                        foreach (DataRow dr in taxDetail.Rows)
                        {
                            if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST")
                            {
                                if (Convert.ToString(dr["Taxes_ID"]).Trim() != schemeIDViaProdID[0].Trim())
                                    dr["TaxRates_Rate"] = 0;
                            }
                        }
                    }
                }

                int slNo = Convert.ToInt32(HdSerialNo.Value);

                //Get Gross Amount and Net Amount 
                decimal ProdGrossAmt = Convert.ToDecimal(HdProdGrossAmt.Value);
                decimal ProdNetAmt = Convert.ToDecimal(HdProdNetAmt.Value);

                List<TaxDetails> TaxDetailsDetails = new List<TaxDetails>();

                //Debjyoti 09032017
                decimal totalParcentage = 0;
                foreach (DataRow dr in taxDetail.Rows)
                {
                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
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
                                    obj.calCulatedOn = Math.Round(finalCalCulatedOn);
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

                    DataTable TaxRecord = (DataTable)Session["PR_FinalTaxRecord"];


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
                                    obj.calCulatedOn = Math.Round(finalCalCulatedOn);
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


                            //DataRow[] filtr = databaseReturnTable.Select("ProductTax_ProductId ='" + keyValue + "' and ProductTax_QuoteId=" +Convert.ToString( Session["QuotationID"] )+ " and ProductTax_TaxTypeId=" + obj.Taxes_ID);
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

                        //      DataRow[] filtrIndex = databaseReturnTable.Select("ProductTax_ProductId ='" + keyValue + "' and ProductTax_QuoteId=" + Session["QuotationID"] + " and ProductTax_TaxTypeId=0");
                        DataRow[] filtrIndex = TaxRecord.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode=0");
                        if (filtrIndex.Length > 0)
                        {
                            aspxGridTax.JSProperties["cpComboCode"] = Convert.ToString(filtrIndex[0]["AltTaxCode"]);
                        }
                    }
                    Session["PR_FinalTaxRecord"] = TaxRecord;

                }
                //New Changes 170217
                //GstCode should fetch everytime
                DataRow[] finalFiltrIndex = MainTaxDataTable.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode=0");
                if (finalFiltrIndex.Length > 0)
                {
                    aspxGridTax.JSProperties["cpComboCode"] = Convert.ToString(finalFiltrIndex[0]["AltTaxCode"]);
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
            DataTable TaxRecord = (DataTable)Session["PR_FinalTaxRecord"];
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


            Session["PR_FinalTaxRecord"] = TaxRecord;


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
            //DataTable taxTableItemLvl = (DataTable)Session["FinalTaxRecord"];
            //foreach (DataRow dr in taxTableItemLvl.Rows)
            //    dr.Delete();
            //taxTableItemLvl.AcceptChanges();
            //Session["FinalTaxRecord"] = taxTableItemLvl;
        }

        protected void cmbGstCstVatcharge_Callback(object sender, CallbackEventArgsBase e)
        {
            Session["PR_TaxDetails"] = null;
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
            if (Session["PR_FinalTaxRecord"] != null)
            {
                TaxDetailTable = (DataTable)Session["PR_FinalTaxRecord"];

                var rows = TaxDetailTable.Select("SlNo ='" + SrlNo + "'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
                TaxDetailTable.AcceptChanges();

                Session["PR_FinalTaxRecord"] = TaxDetailTable;
            }
        }
        public void UpdateTaxDetails(string oldSrlNo, string newSrlNo)
        {
            DataTable TaxDetailTable = new DataTable();
            if (Session["PR_FinalTaxRecord"] != null)
            {
                TaxDetailTable = (DataTable)Session["PR_FinalTaxRecord"];

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

                Session["PR_FinalTaxRecord"] = TaxDetailTable;
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
            DataTable DT = proc.GetTable();
            cmbGstCstVatcharge.DataSource = DT;
            cmbGstCstVatcharge.TextField = "Taxes_Name";
            cmbGstCstVatcharge.ValueField = "Taxes_ID";
            cmbGstCstVatcharge.DataBind();
        }

        #endregion

       
        #region Sam Section Start

        #region PrePopulated Data If Page is not Post Back Section Start
        public void SetFinYearCurrentDate()
        {
            try
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
            catch (Exception ex)
            {

            }
            finally
            {
            }
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
            string userbranchHierarchy = Convert.ToString(Session["userbranchHierarchy"]);
            string strYear = Convert.ToString(Session["LastFinYear"]);

            dst = objPurchaseReturnBL.GetAllDropDownDetailForSalesInvoice(userbranchHierarchy, strCompanyID, strBranchID, strYear);

            //if (dst.Tables[0] != null && dst.Tables[0].Rows.Count > 0)
            //{
            //    ddl_numberingScheme.DataTextField = "SchemaName";
            //    ddl_numberingScheme.DataValueField = "Id";
            //    ddl_numberingScheme.DataSource = dst.Tables[0];
            //    ddl_numberingScheme.DataBind();
            //}

            SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
            DataTable Schemadt = objSlaesActivitiesBL.GetNumberingSchema(strCompanyID, userbranchHierarchy, strYear, "20", "Y");
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
                //ddl_Branch.Items.Insert(0, new ListItem("Select", "0"));
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
            if (dst.Tables[2] != null && dst.Tables[2].Rows.Count > 0)
            {
                ddl_SalesAgent.DataTextField = "Name";
                ddl_SalesAgent.DataValueField = "cnt_Id";
                ddl_SalesAgent.DataSource = dst.Tables[2];
                ddl_SalesAgent.DataBind();
            }
            ddl_SalesAgent.Items.Insert(0, new ListItem("Select", "0"));
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
                }
                else
                {
                    ddl_Currency.SelectedIndex = no;
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
            if (dst.Tables[5] != null && dst.Tables[5].Rows.Count > 0)
            {
                ddlCashBank.TextField = "IntegrateMainAccount";
                ddlCashBank.ValueField = "MainAccount_ReferenceID";
                ddlCashBank.DataSource = dst.Tables[5];
                ddlCashBank.DataBind();
            }
            #endregion Cash/Bank DropDown Start
        }

        #endregion PrePopulated Data If Page is not Post Back Section End

        #region PrePopulated Data in Page Load Due to use Searching Functionality Section Start
        public void PopulateCustomerDetail()
        {
            if (Session["PR_CustomerDetail"] == null)
            {
                DataTable dtCustomer = new DataTable();
                dtCustomer = objPurchaseReturnBL.PopulateCustomerDetail();

                if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                {
                    lookup_Customer.DataSource = dtCustomer;
                    lookup_Customer.DataBind();
                    Session["PR_CustomerDetail"] = dtCustomer;
                }
            }
            else
            {
                lookup_Customer.DataSource = (DataTable)Session["PR_CustomerDetail"];
                lookup_Customer.DataBind();
            }

        }
        #endregion PrePopulated Data in Page Load Due to use Searching Functionality Section End

        #region Header Portion Detail of the Page By Sam

        #region Check Billing and Shipping Address

        [WebMethod]
        public static int CheckCustomerBillingShippingAddress(string Customerid)
        {
            int addressStatus = 0;
            try
            {
                CRMSalesDtlBL objCRMSalesDtlBL = new CRMSalesDtlBL();
                addressStatus = objCRMSalesDtlBL.CheckCustomerBillingShippingAddress(Customerid);
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
            return addressStatus;
        }

        #endregion Check Billing and Shipping Address

        [WebMethod]
        public static bool CheckUniqueCode(string ReturnNo)
        {
            bool flag = false;
            BusinessLogicLayer.GenericMethod oGenericMethod = new BusinessLogicLayer.GenericMethod();
            try
            {
                MShortNameCheckingBL objShortNameChecking = new MShortNameCheckingBL();
                flag = objShortNameChecking.CheckUnique(ReturnNo, "0", "PurchaseReturn");
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
            Session["PR_QuotationAddressDtl"] = null;
            Session["PR_BillingAddressLookup"] = null;
            Session["PR_ShippingAddressLookup"] = null;
            string WhichCall = e.Parameter.Split('~')[0];
            if (WhichCall == "BindContactPerson")
            {
                string InternalId = Convert.ToString(Convert.ToString(e.Parameter.Split('~')[1]));

                populateReferredBy(InternalId);
                PopulateContactPersonOfCustomer(InternalId);

                DataTable dtDeuDate = objPurchaseReturnBL.GetCustomerDetails_InvoiceRelated(InternalId);
                foreach (DataRow dr in dtDeuDate.Rows)
                {
                    string strDueDate = Convert.ToString(dr["DueDate"]);
                    cmbContactPerson.JSProperties["cpDueDate"] = strDueDate;
                    //dt_SaleInvoiceDue.Date = Convert.ToDateTime(strDeuDate);
                }

                DataTable GSTNTable = objPurchaseReturnBL.GetVendorGSTIN(InternalId);
                string strGSTN = Convert.ToString(GSTNTable.Rows[0]["CNT_GSTIN"]).Trim();
                if (strGSTN != "")
                {
                    cmbContactPerson.JSProperties["cpGSTN"] = "Yes";
                }
                else
                {
                    cmbContactPerson.JSProperties["cpGSTN"] = "No";
                }
            }
        }

        protected void populateReferredBy(string InternalId)
        {

            DataTable dtReferredBy = objPurchaseReturnBL.GetReferredByVendor(InternalId);
            if (dtReferredBy != null && dtReferredBy.Rows.Count > 0)
            {
                //   txt_Refference.Text = Convert.ToString(dtReferredBy.Rows[0]["cnt_referedBy"]);

                cmbContactPerson.JSProperties["cpReferredBy"] = Convert.ToString(dtReferredBy.Rows[0]["cnt_referedBy"]);
            }
        }

        protected void PopulateContactPersonOfCustomer(string InternalId)
        {
            //string ContactPerson = "";
            DataTable dtContactPerson = new DataTable();
            dtContactPerson = objPurchaseOrderBL.PopulateContactPersonOfCustomer(InternalId);
            if (dtContactPerson != null && dtContactPerson.Rows.Count > 0)
            {
                cmbContactPerson.TextField = "cp_name";
                cmbContactPerson.ValueField = "cp_contactId";
                cmbContactPerson.DataSource = dtContactPerson;
                cmbContactPerson.DataBind();



                //foreach (DataRow dr in dtContactPerson.Rows)
                //{
                //    if (Convert.ToString(dr["Isdefault"]) == "True")
                //    {
                //        ContactPerson = Convert.ToString(dr["add_id"]);
                //        break;
                //    }
                //}
                //cmbContactPerson.SelectedItem = cmbContactPerson.Items.FindByValue(ContactPerson);

            }
        }
        //protected void PopulateContactPersonOfCustomer(string InternalId)
        //{
        //    string ContactPerson = "";
        //    DataTable dtContactPerson = new DataTable();
        //    SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
        //    dtContactPerson = objSlaesActivitiesBL.PopulateContactPersonOfCustomer(InternalId);
        //    if (dtContactPerson != null && dtContactPerson.Rows.Count > 0)
        //    {
        //        cmbContactPerson.TextField = "contactperson";
        //        cmbContactPerson.ValueField = "add_id";
        //        cmbContactPerson.DataSource = dtContactPerson;
        //        cmbContactPerson.DataBind();
        //        foreach (DataRow dr in dtContactPerson.Rows)
        //        {
        //            if (Convert.ToString(dr["Isdefault"]) == "True")
        //            {
        //                ContactPerson = Convert.ToString(dr["add_id"]);
        //                break;
        //            }
        //        }
        //        cmbContactPerson.SelectedItem = cmbContactPerson.Items.FindByValue(ContactPerson);
        //    }
        //}
        protected void ddl_VatGstCst_Callback(object sender, CallbackEventArgsBase e)
        {
            string type = e.Parameter.Split('~')[0];
            if (type == "Tax-code")
            {
                string Tax_Code = e.Parameter.Split('~')[1];

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
            }
            else { PopulateGSTCSTVAT(type); }
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
                string[] compGstin = oDBEngine.GetFieldValue1("tbl_master_company", "cmp_gstin", "cmp_internalid='" + CompInternalId + "'", 1);

                string ShippingState = "";

                #region ##### Added By : Samrat Roy -- For BillingShippingUserControl ######
                string sstateCode = BillingShippingControl.GetShippingStateCode(Request.QueryString["key"]);
                ShippingState = sstateCode;
                if (ShippingState.Trim() != "")
                {
                    ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
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

                



                if (ShippingState.Trim() != "" && compGstin[0].Trim() != "")
                {

                    if (compGstin.Length > 0)
                    {
                        if (compGstin[0].Substring(0, 2) == ShippingState)
                        {
                            //Check if the state is in union territories then only UTGST will apply
                            //   Chandigarh     Andaman and Nicobar Islands    DADRA & NAGAR HAVELI    DAMAN & DIU     Lakshadweep              PONDICHERRY
                            if (ShippingState == "4" || ShippingState == "35" || ShippingState == "26" || ShippingState == "25" || ShippingState == "31" || ShippingState == "34")
                            {
                                foreach (DataRow dr in dtGSTCSTVAT.Rows)
                                {
                                    if (Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "I" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "C" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "S")
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
                }

                //If Company GSTIN is blank then Delete All CGST,UGST,IGST,CGST
                if (compGstin[0].Trim() == "")
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

        #endregion Header Portion Detail of the Page By Sam

        public DataTable GetBillingAddress()
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRM_Details");
            proc.AddVarcharPara("@Action", 500, "QuotationBillingAddress");
            proc.AddVarcharPara("@QuotationID", 500, Convert.ToString(Session["PR_ReturnID"]));
            dt = proc.GetTable();
            return dt;
        }

        #endregion Sam Section Start

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

        protected void CmbProduct_Init(object sender, EventArgs e)
        {
            ASPxComboBox cityCombo = sender as ASPxComboBox;
            cityCombo.DataSource = GetProduct();
        }
        #region  Available Stock
        protected void acpAvailableStock_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string performpara = e.Parameter;
            string strProductID = Convert.ToString(performpara.Split('~')[0]);
            // string strBranch = Convert.ToString(ddl_Branch.SelectedValue);
            string strBranch = Convert.ToString(performpara.Split('~')[1]);
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
        #endregion Available Stock

        [WebMethod]
        public static string getProductType(string Products_ID)
        {
            string Type = "";
            string query = @"Select
                           (Case When Isnull(Is_active_warehouse,'0')='0' AND Isnull(Is_active_Batch,'0')='0' AND Isnull(Is_active_serialno,'0')='0' Then ''
                           When Isnull(Is_active_warehouse,'0')='1' AND Isnull(Is_active_Batch,'0')='0' AND Isnull(Is_active_serialno,'0')='0' Then 'W'
                           When Isnull(Is_active_warehouse,'0')='0' AND Isnull(Is_active_Batch,'0')='1' AND Isnull(Is_active_serialno,'0')='0' Then 'B'
                           When Isnull(Is_active_warehouse,'0')='0' AND Isnull(Is_active_Batch,'0')='0' AND Isnull(Is_active_serialno,'0')='1' Then 'S'
                           When Isnull(Is_active_warehouse,'0')='1' AND Isnull(Is_active_Batch,'0')='1' AND Isnull(Is_active_serialno,'0')='0' Then 'WB'
                           When Isnull(Is_active_warehouse,'0')='1' AND Isnull(Is_active_Batch,'0')='0' AND Isnull(Is_active_serialno,'0')='1' Then 'WS'
                           When Isnull(Is_active_warehouse,'0')='1' AND Isnull(Is_active_Batch,'0')='1' AND Isnull(Is_active_serialno,'0')='1' Then 'WBS'
                           When Isnull(Is_active_warehouse,'0')='0' AND Isnull(Is_active_Batch,'0')='1' AND Isnull(Is_active_serialno,'0')='1' Then 'BS'
                           END) as Type
                           from Master_sProducts
                           where sProducts_ID='" + Products_ID + "'";

           // BusinessLogicLayer.DBEngine oDbEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
            BusinessLogicLayer.DBEngine oDbEngine = new BusinessLogicLayer.DBEngine();


            DataTable dt = oDbEngine.GetDataTable(query);
            if (dt != null && dt.Rows.Count > 0)
            {
                Type = Convert.ToString(dt.Rows[0]["Type"]);
            }

            return Convert.ToString(Type);
        }



        public DataTable GetComponentEditedAddressData(string ComponentDetailsIDs, string strType)
        {
            DataTable ds = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_PurchaseReturn_Details");
            proc.AddVarcharPara("@Action", 500, "ComponentBillingAddress");
            proc.AddVarcharPara("@SelectedComponentList", 500, ComponentDetailsIDs);
            proc.AddVarcharPara("@ComponentType", 500, strType);
            ds = proc.GetTable();
            return ds;
        }

        public void SetPurchaseReturnDetails(string strPurchaseReturnId)
        {
            DataTable PurchaseReturnEditdt = objPurchaseReturnBL.GetPurchaseReturnEditData(strPurchaseReturnId);
            if (PurchaseReturnEditdt != null && PurchaseReturnEditdt.Rows.Count > 0)
            {
                string Branch_Id = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Return_BranchId"]);
                string Quote_Number = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Return_Number"]);
                string Quote_Date = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Return_Date"]);
                string Customer_Id = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Customer_Id"]);
                string Contact_Person_Id = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Contact_Person_Id"]);
                string Quote_Reference = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Return_Reference"]);
                string Currency_Id = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Currency_Id"]);
                string Currency_Conversion_Rate = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Currency_Conversion_Rate"]);
                string Tax_Option = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Tax_Option"]);
                string Tax_Code = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Tax_Code"]);
                string Quote_SalesmanId = Convert.ToString(PurchaseReturnEditdt.Rows[0]["Return_SalesmanId"]);
                string IsUsed = Convert.ToString(PurchaseReturnEditdt.Rows[0]["IsUsed"]);

                string CashBank_Code = Convert.ToString(PurchaseReturnEditdt.Rows[0]["CashBank_Code"]);
                string InvoiceCreatedFromDoc = Convert.ToString(PurchaseReturnEditdt.Rows[0]["ReturnCreatedFromDoc"]);
                string InvoiceCreatedFromDoc_Ids = Convert.ToString(PurchaseReturnEditdt.Rows[0]["ReturnCreatedFromDoc_Ids"]);
                string InvoiceCreatedFromDocDate = Convert.ToString(PurchaseReturnEditdt.Rows[0]["ReturnCreatedFromDocDate"]);
                string DueDate = Convert.ToString(PurchaseReturnEditdt.Rows[0]["DueDate"]);

                ddlCashBank.Value = CashBank_Code;
                //if (InvoiceCreatedFromDoc != "") rdl_SaleInvoice.SelectedValue = InvoiceCreatedFromDoc;
                txt_InvoiceDate.Text = InvoiceCreatedFromDocDate;
                dt_SaleInvoiceDue.Date = Convert.ToDateTime(DueDate);


                //if (InvoiceCreatedFromDoc != "") rdl_SaleInvoice.SelectedValue = InvoiceCreatedFromDoc;

                if (Contact_Person_Id != "0")
                {
                    cmbContactPerson.Value = Contact_Person_Id;

                    DataTable dtContactPersonPhone = new DataTable();
                    dtContactPersonPhone = objPurchaseOrderBL.PopulateContactPersonPhone(Contact_Person_Id);
                    if (dtContactPersonPhone != null && dtContactPersonPhone.Rows.Count > 0)
                    {
                        pageheaderContent.Attributes.Add("style", "display:block");
                        divContactPhone.Attributes.Add("style", "display:block");

                        foreach (DataRow dr in dtContactPersonPhone.Rows)
                        {
                            string phone = Convert.ToString(dr["phf_phoneNumber"]);
                            if (!string.IsNullOrEmpty(phone))
                            {
                                lblContactPhone.Text = phone;
                                break;
                            }
                        }

                    }

                }
                DataTable OutStandingTable = objPurchaseReturnBL.GetCustomerOutStanding(Customer_Id);


                if (OutStandingTable.Rows.Count > 0)
                {
                    pageheaderContent.Attributes.Add("style", "display:block");
                    divOutstanding.Attributes.Add("style", "display:block");

                    var convertDecimal = Convert.ToDecimal(Convert.ToString(OutStandingTable.Rows[0]["NetOutstanding"]));


                    if (convertDecimal > 0)
                    {
                        lblOutstanding.Text = Convert.ToString(convertDecimal) + "(Cr)";
                    }
                    else
                    {

                        lblOutstanding.Text = Convert.ToString(convertDecimal * -1).ToString() + "(Db)";
                    }


                    // cmbContactPerson.JSProperties["cpOutstanding"] = Convert.ToString(OutStandingTable.Rows[0]["NetOutstanding"]);
                }


                //gstn
                DataTable GSTNTable = objPurchaseReturnBL.GetVendorGSTIN(Customer_Id);
                if (GSTNTable != null && GSTNTable.Rows.Count > 0)
                {
                    pageheaderContent.Attributes.Add("style", "display:block");
                    divGSTN.Attributes.Add("style", "display:block");

                    var ghstnval = Convert.ToString(Convert.ToString(GSTNTable.Rows[0]["CNT_GSTIN"]));

                    if (ghstnval != "")
                    { lblGSTIN.Text = "Yes"; }
                    else { lblGSTIN.Text = "No"; }





                    // cmbContactPerson.JSProperties["cpOutstanding"] = Convert.ToString(OutStandingTable.Rows[0]["NetOutstanding"]);
                }
                //gstn
                if (!String.IsNullOrEmpty(InvoiceCreatedFromDoc_Ids))
                {
                    string[] eachQuo = InvoiceCreatedFromDoc_Ids.Split(',');
                    if (eachQuo.Length > 1)//More tha one quotation
                    {
                        BindLookUp(Customer_Id, Convert.ToDateTime(Quote_Date).ToString("yyyy-MM-dd"), Branch_Id);
                        foreach (string val in eachQuo)
                        {
                            lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                        }
                        Session["TaggingPurchaseInvoce"] = "1";
                    }
                    else if (eachQuo.Length == 1)//Single Quotation
                    {
                        BindLookUp(Customer_Id, Convert.ToDateTime(Quote_Date).ToString("yyyy-MM-dd"), Branch_Id);
                        foreach (string val in eachQuo)
                        {
                            lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                        }
                        Session["TaggingPurchaseInvoce"] = "1";
                    }
                    else//No Quotation selected
                    {
                        BindLookUp(Customer_Id, Convert.ToDateTime(Quote_Date).ToString("yyyy-MM-dd"), Branch_Id);
                        Session["TaggingPurchaseInvoce"] = "0";
                    }
                }


                txt_PLQuoteNo.Text = Quote_Number;
                dt_PLQuote.Date = Convert.ToDateTime(Quote_Date);
                PopulateGSTCSTVATCombo(Convert.ToDateTime(Quote_Date).ToString("yyyy-MM-dd"));
                PopulateChargeGSTCSTVATCombo(Convert.ToDateTime(Quote_Date).ToString("yyyy-MM-dd"));
                lookup_Customer.GridView.Selection.SelectRowByKey(Customer_Id);
                hdnCustomerId.Value = Customer_Id;
                TabPage page = ASPxPageControl1.TabPages.FindByName("Billing/Shipping");
                page.ClientEnabled = true;

                PopulateContactPersonOfCustomer(Customer_Id);
                cmbContactPerson.SelectedItem = cmbContactPerson.Items.FindByValue(Contact_Person_Id);
                txt_Refference.Text = Quote_Reference;
                ddl_Branch.SelectedValue = Branch_Id;
                ddl_SalesAgent.SelectedValue = Quote_SalesmanId;
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
                    lookup_Customer.ClientEnabled = false;
                }
                else
                {
                    dt_PLQuote.ClientEnabled = true;
                    lookup_Customer.ClientEnabled = true;
                }
            }
        }

        private int updatewarehouse()
        {
            if (Session["PRwarehousedetailstempUpdate"] != null)
            {
                DataTable dt = new DataTable();
                DataTable Warehousedtups = (DataTable)Session["PRwarehousedetailstempUpdate"];
                DataRow[] dr = Warehousedtups.Select("isnew = 'Updated'");
                if (dr.Count() != 0)
                {
                    dt = dr.CopyToDataTable();
                    //if (Session["PcwarehousedetailstempUpdate"] != null)
                    //{
                    //    temtable23 = (DataTable)Session["PcwarehousedetailstempUpdate"];
                    //    DataRow[] rows;
                    //    rows = temtable23.Select("pcslno = '" + hdnpcslno.Value + "'");  //'UserName' is ColumnName
                    //    foreach (DataRow row in rows)
                    //        temtable23.Rows.Remove(row);

                    //    temtable23.Merge(dt);
                    //    Session["PcwarehousedetailstempUpdate"] = temtable23;
                    //}
                    //else
                    //{
                    //    Session["PcwarehousedetailstempUpdate"] = dt;
                    //}

                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        string iswarehouse = string.Empty;
                        string isbatch = string.Empty;
                        string isserial = string.Empty;



                        #region WHBTSL
                        if (Convert.ToString(dt.Rows[i]["Inventrytype"]) == "WHBTSL")
                        {
                            DateTime? mfgex;
                            DateTime? exdate;
                            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewMFGDate"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewExpiryDate"])))
                            {
                                if ((Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "01-01-0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "01-01-0001 12:00:00 AM"))
                                {
                                    mfgex = Convert.ToDateTime(dt.Rows[i]["viewMFGDate"]);
                                    exdate = Convert.ToDateTime(dt.Rows[i]["viewExpiryDate"]);
                                }
                                else
                                {
                                    mfgex = null;
                                    exdate = null;
                                }

                            }
                            else
                            {
                                mfgex = null;
                                exdate = null;
                            }

                            if (Convert.ToString(dt.Rows[i]["viewQuantity"]) != "0" && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewQuantity"])))
                            {
                                string sql = string.Empty;
                                if (mfgex == null && exdate == null)
                                {
                                    if (hdnisolddeleted.Value == "true")
                                    {

                                        var updateqnty = dt.Compute("sum(Quantitysum)", "isnew = 'Updated' AND WarehouseID='" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + "' AND BatchNo='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'");
                                        if (string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                                        {
                                            updateqnty = Convert.ToDecimal(dt.Rows[i]["Quantitysum"]);
                                        }

                                        sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(updateqnty) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                  "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                             "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(updateqnty) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                             "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                             "ModifiedDate=GETDATE()" +
                                                             "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                             "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                           "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +


                                                           "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                           "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                      "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                      "  update Trans_StockSerialMapping" +
                                            " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                            " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";
                                    }
                                    else
                                    {
                                        sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                    "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                               "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                               "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                               "ModifiedDate=GETDATE() " +
                                                               "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                               "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                             "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +


                                                             "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                             "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                        "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                        "  update Trans_StockSerialMapping" +
                                              " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                              " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";
                                    }
                                }
                                else
                                {
                                    if (hdnisolddeleted.Value == "true")
                                    {

                                        var updateqnty = dt.Compute("sum(Quantitysum)", "isnew = 'Updated' AND WarehouseID='" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + "' AND BatchNo='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'");
                                        if (string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                                        {
                                            updateqnty = Convert.ToDecimal(dt.Rows[i]["Quantitysum"]);
                                        }
                                        sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(updateqnty) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                      "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                                 "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(updateqnty) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                                 "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                                 "ModifiedDate=GETDATE()" +
                                                                 "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                                 "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                               "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +

                                                               "StockBranchBatch_MfgDate='" + mfgex + "'," +
                                                               "StockBranchBatch_ExpiryDate='" + exdate + "'," +
                                                               "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                               "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                          "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                          "  update Trans_StockSerialMapping" +
                                                " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                                " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";
                                    }
                                    else
                                    {
                                        sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                 "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                            "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                            "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                            "ModifiedDate=GETDATE()" +
                                                            "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                            "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                          "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +

                                                          "StockBranchBatch_MfgDate='" + mfgex + "'," +
                                                          "StockBranchBatch_ExpiryDate='" + exdate + "'," +
                                                          "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                          "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                     "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                     "  update Trans_StockSerialMapping" +
                                           " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                           " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";
                                    }


                                }


                                oDBEngine.GetDataTable(sql);

                            }
                            else
                            {
                                //var updateqnty = dt.Compute("sum(Quantitysum)", "isnew = 'Updated' AND WarehouseID='" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + "' AND BatchNo='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'");


                                string sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                              "  update Trans_StockBranchWarehouseDetails set " +
                                                         "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                         "ModifiedDate=GETDATE() " +
                                                         "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +

                                                  "  update Trans_StockSerialMapping" +
                                        " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                        " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";

                                oDBEngine.GetDataTable(sql);
                            }
                        }
                        #endregion
                        #region WHBT
                        if (Convert.ToString(dt.Rows[i]["Inventrytype"]) == "WHBT")
                        {
                            DateTime? mfgex;
                            DateTime? exdate;

                            var updateqnty = dt.Compute("sum(Quantitysum)", "isnew = 'Updated' AND BatchWarehouseID='" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "'");
                            var OLDSAMEwarehouse = Warehousedtups.Compute("sum(Quantitysum)", "isnew = 'old' AND BatchWarehouseID='" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "'");
                            if (!string.IsNullOrEmpty(Convert.ToString(OLDSAMEwarehouse)) && !string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                            {

                                updateqnty = Convert.ToDecimal(updateqnty) + Convert.ToDecimal(OLDSAMEwarehouse);
                            }

                            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewMFGDate"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewExpiryDate"])))
                            {
                                if ((Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "01-01-0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "01-01-0001 12:00:00 AM"))
                                {
                                    mfgex = Convert.ToDateTime(dt.Rows[i]["viewMFGDate"]);
                                    exdate = Convert.ToDateTime(dt.Rows[i]["viewExpiryDate"]);
                                }
                                else
                                {
                                    mfgex = null;
                                    exdate = null;
                                }

                            }
                            else
                            {
                                mfgex = null;
                                exdate = null;
                            }

                            if (Convert.ToString(dt.Rows[i]["viewQuantity"]) != "0" && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewQuantity"])) && !string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                            {
                                string sql = string.Empty;
                                if (mfgex == null && exdate == null)
                                {
                                    sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(updateqnty) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                              "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                         "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(updateqnty) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                         "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                         "ModifiedDate=GETDATE() " +
                                                         "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                         "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                       "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +


                                                       "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                       "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                  "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]);


                                }
                                else
                                {
                                    sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(updateqnty) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                  "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                             "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(updateqnty) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                             "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                             "ModifiedDate=GETDATE() " +
                                                             "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                             "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                           "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +

                                                           "StockBranchBatch_MfgDate='" + mfgex + "'," +
                                                           "StockBranchBatch_ExpiryDate='" + exdate + "'," +
                                                           "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                           "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                      "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]);
                                }


                                oDBEngine.GetDataTable(sql);

                            }


                        }
                        #endregion
                        #region WHSL
                        if (Convert.ToString(dt.Rows[i]["Inventrytype"]) == "WHSL")
                        {
                            DateTime? mfgex;
                            DateTime? exdate;
                            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewMFGDate"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewExpiryDate"])))
                            {
                                if ((Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "01-01-0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "01-01-0001 12:00:00 AM"))
                                {
                                    mfgex = Convert.ToDateTime(dt.Rows[i]["viewMFGDate"]);
                                    exdate = Convert.ToDateTime(dt.Rows[i]["viewExpiryDate"]);
                                }
                                else
                                {
                                    mfgex = null;
                                    exdate = null;
                                }

                            }
                            else
                            {
                                mfgex = null;
                                exdate = null;
                            }

                            if (Convert.ToString(dt.Rows[i]["viewQuantity"]) != "0" && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewQuantity"])))
                            {
                                string sql = string.Empty;
                                if (mfgex == null && exdate == null)
                                {
                                    sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                              "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                         "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                         "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                         "ModifiedDate=GETDATE()" +
                                                         "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                         "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                       "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +


                                                       "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                       "StockBranchBatch_ModifiedDate=GETDATE()" +
                                                  "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                  "  update Trans_StockSerialMapping" +
                                        " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                        " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";

                                }
                                else
                                {
                                    sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                  "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                             "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                             "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                             "ModifiedDate=GETDATE()" +
                                                             "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                             "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                           "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +

                                                           "StockBranchBatch_MfgDate='" + mfgex + "'," +
                                                           "StockBranchBatch_ExpiryDate='" + exdate + "'," +
                                                           "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                           "StockBranchBatch_ModifiedDate=GETDATE()" +
                                                      "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                      "  update Trans_StockSerialMapping" +
                                            " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                            " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";
                                }


                                oDBEngine.GetDataTable(sql);

                            }
                            else
                            {
                                //var updateqnty = dt.Compute("sum(Quantitysum)", "isnew = 'Updated' AND WarehouseID='" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + "' AND BatchNo='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'");


                                string sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                              "  update Trans_StockBranchWarehouseDetails set " +
                                                         "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                         "ModifiedDate=GETDATE()" +
                                                         "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +

                                                  "  update Trans_StockSerialMapping" +
                                        " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                        " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";

                                oDBEngine.GetDataTable(sql);
                            }
                        }
                        #endregion

                        #region WH
                        if (Convert.ToString(dt.Rows[i]["Inventrytype"]) == "WH")
                        {
                            if (Convert.ToString(dt.Rows[i]["viewQuantity"]) != "0" && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Quantity"])))
                            {

                                string sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantity"]) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                            "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                       " Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(dt.Rows[i]["Quantity"]) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                       " ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                       " ModifiedDate=GETDATE() " +
                                                       " where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                       "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                     "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantity"]) + "," +


                                                     "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                     "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                " where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]);




                                oDBEngine.GetDataTable(sql);

                            }


                        }
                        #endregion

                        #region BTSL
                        if (Convert.ToString(dt.Rows[i]["Inventrytype"]) == "BTSL")
                        {
                            DateTime? mfgex;
                            DateTime? exdate;
                            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewMFGDate"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewExpiryDate"])))
                            {
                                if ((Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "01-01-0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "01-01-0001 12:00:00 AM"))
                                {
                                    mfgex = Convert.ToDateTime(dt.Rows[i]["viewMFGDate"]);
                                    exdate = Convert.ToDateTime(dt.Rows[i]["viewExpiryDate"]);
                                }
                                else
                                {
                                    mfgex = null;
                                    exdate = null;
                                }

                            }
                            else
                            {
                                mfgex = null;
                                exdate = null;
                            }

                            if (Convert.ToString(dt.Rows[i]["viewQuantity"]) != "0" && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewQuantity"])))
                            {
                                string sql = string.Empty;
                                if (mfgex == null && exdate == null)
                                {
                                    if (hdnisolddeleted.Value == "true")
                                    {

                                        var updateqnty = dt.Compute("sum(Quantitysum)", "isnew = 'Updated'  AND BatchNo='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'");
                                        if (string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                                        {
                                            updateqnty = Convert.ToDecimal(dt.Rows[i]["Quantitysum"]);
                                        }

                                        sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(updateqnty) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                  "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                             "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(updateqnty) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                             "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                             "ModifiedDate=GETDATE()" +
                                                             "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                             "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                           "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +


                                                           "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                           "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                      "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                      "  update Trans_StockSerialMapping" +
                                            " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                            " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";
                                    }
                                    else
                                    {
                                        sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                    "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                               "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                               "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                               "ModifiedDate=GETDATE() " +
                                                               "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                               "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                             "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +


                                                             "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                             "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                        "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                        "  update Trans_StockSerialMapping" +
                                              " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                              " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";
                                    }
                                }
                                else
                                {
                                    if (hdnisolddeleted.Value == "true")
                                    {

                                        var updateqnty = dt.Compute("sum(Quantitysum)", "isnew = 'Updated' AND BatchNo='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'");
                                        if (string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                                        {
                                            updateqnty = Convert.ToDecimal(dt.Rows[i]["Quantitysum"]);
                                        }
                                        sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(updateqnty) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                      "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                                 "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(updateqnty) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                                 "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                                 "ModifiedDate=GETDATE()" +
                                                                 "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                                 "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                               "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +

                                                               "StockBranchBatch_MfgDate='" + mfgex + "'," +
                                                               "StockBranchBatch_ExpiryDate='" + exdate + "'," +
                                                               "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                               "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                          "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                          "  update Trans_StockSerialMapping" +
                                                " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                                " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";
                                    }
                                    else
                                    {
                                        sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                 "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                            "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                            "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                            "ModifiedDate=GETDATE()" +
                                                            "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                            "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                          "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +

                                                          "StockBranchBatch_MfgDate='" + mfgex + "'," +
                                                          "StockBranchBatch_ExpiryDate='" + exdate + "'," +
                                                          "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                          "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                     "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]) + "	  " +
                                                     "  update Trans_StockSerialMapping" +
                                           " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                           " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";
                                    }


                                }


                                oDBEngine.GetDataTable(sql);

                            }
                            else
                            {
                                //var updateqnty = dt.Compute("sum(Quantitysum)", "isnew = 'Updated' AND WarehouseID='" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + "' AND BatchNo='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'");


                                string sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                              "  update Trans_StockBranchWarehouseDetails set " +
                                                         "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                         "ModifiedDate=GETDATE() " +
                                                         "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +

                                                  "  update Trans_StockSerialMapping" +
                                        " Set Stock_BranchSerialNumber='" + Convert.ToString(dt.Rows[i]["SerialNo"]) + "'" +
                                        " where Stock_MapId=" + Convert.ToString(dt.Rows[i]["SerialID"]) + "" + "";

                                oDBEngine.GetDataTable(sql);
                            }
                        }
                        #endregion

                        #region BT
                        if (Convert.ToString(dt.Rows[i]["Inventrytype"]) == "BTSL")
                        {
                            DateTime? mfgex;
                            DateTime? exdate;

                            var updateqnty = dt.Compute("sum(Quantitysum)", "isnew = 'Updated' AND BatchWarehouseID='" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "'");
                            var OLDSAMEwarehouse = Warehousedtups.Compute("sum(Quantitysum)", "isnew = 'old' AND BatchWarehouseID='" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "'");
                            if (!string.IsNullOrEmpty(Convert.ToString(OLDSAMEwarehouse)) && !string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                            {

                                updateqnty = Convert.ToDecimal(updateqnty) + Convert.ToDecimal(OLDSAMEwarehouse);
                            }

                            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewMFGDate"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewExpiryDate"])))
                            {
                                if ((Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "1/1/0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewMFGDate"]) != "01-01-0001 12:00:00 AM" && Convert.ToString(dt.Rows[i]["viewExpiryDate"]) != "01-01-0001 12:00:00 AM"))
                                {
                                    mfgex = Convert.ToDateTime(dt.Rows[i]["viewMFGDate"]);
                                    exdate = Convert.ToDateTime(dt.Rows[i]["viewExpiryDate"]);
                                }
                                else
                                {
                                    mfgex = null;
                                    exdate = null;
                                }

                            }
                            else
                            {
                                mfgex = null;
                                exdate = null;
                            }

                            if (Convert.ToString(dt.Rows[i]["viewQuantity"]) != "0" && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["viewQuantity"])) && !string.IsNullOrEmpty(Convert.ToString(updateqnty)))
                            {
                                string sql = string.Empty;
                                if (mfgex == null && exdate == null)
                                {
                                    sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(updateqnty) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                              "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                         "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(updateqnty) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                         "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                         "ModifiedDate=GETDATE() " +
                                                         "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                         "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                       "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +


                                                       "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                       "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                  "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]);


                                }
                                else
                                {
                                    sql = "update Trans_StockBranchWarehouse set StockBranchWarehouse_StockIn=" + Convert.ToDecimal(updateqnty) + ",StockBranchWarehouse_WarehouseId=" + Convert.ToString(dt.Rows[i]["WarehouseID"]) + ",ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + ",ModifiedDate=getdate() where StockBranchWarehouse_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehouseID"]) + "" +
                                                  "  update Trans_StockBranchWarehouseDetails set Stock_OpeningRate=" + Convert.ToDecimal(hdnrate.Value) + "," +
                                                             "Stock_OpeningValue=ISNULL(isnull(" + Convert.ToDecimal(updateqnty) + ",0)*isnull(" + Convert.ToDecimal(hdnrate.Value) + ",0),0)," +
                                                             "ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                             "ModifiedDate=GETDATE() " +
                                                             "where StockBranchWarehouseDetail_Id=" + Convert.ToString(dt.Rows[i]["BatchWarehousedetailsID"]) + "" +
                                                             "   update Trans_StockBranchBatch set   StockBranchBatch_BatchNumber='" + Convert.ToString(dt.Rows[i]["BatchNo"]) + "'," +
                                                           "StockBranchBatch_StockIn=" + Convert.ToDecimal(dt.Rows[i]["Quantitysum"]) + "," +

                                                           "StockBranchBatch_MfgDate='" + mfgex + "'," +
                                                           "StockBranchBatch_ExpiryDate='" + exdate + "'," +
                                                           "StockBranchBatch_ModifiedBy=" + Convert.ToInt32(HttpContext.Current.Session["userid"]) + "," +
                                                           "StockBranchBatch_ModifiedDate=GETDATE() " +
                                                      "where StockBranchBatch_Id=" + Convert.ToString(dt.Rows[i]["BatchID"]);
                                }


                                oDBEngine.GetDataTable(sql);

                            }


                        }
                        #endregion

                    }
                }
            }

            return 0;
        }





        protected void ComponentQuotation_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string Reference = string.Empty;
            string Currency_Id = string.Empty;
            string SalesmanId = string.Empty;
            string ExpiryDate = string.Empty;
            string CurrencyRate = string.Empty;
            string Contact_person_id = string.Empty;
            string Tax_Option = string.Empty;
            string Tax_Code = string.Empty;
            string ComponentType = string.Empty;
            string InvoiceIds = string.Empty;
            string Customer = string.Empty;
            string PurchaseReturnDate = string.Empty;
          
            string Action = string.Empty;
            string branchId = Convert.ToString(ddl_Branch.SelectedValue);
            if (e.Parameter.Split('~')[0] == "BindComponentGrid")
            {
                if (e.Parameter.Split('~')[1] != null) Customer = e.Parameter.Split('~')[1];
                if (e.Parameter.Split('~')[2] != null) PurchaseReturnDate = e.Parameter.Split('~')[2];

                string strPurchaseReturnID = Convert.ToString(Session["PR_ReturnID"]);
                if (e.Parameter.Split('~')[3] == "DateCheck")
                {
                    lookup_quotation.GridView.Selection.UnselectAll();


                    ComponentQuotationPanel.JSProperties["cpDetails"] = Reference + "~" + Currency_Id + "~" + SalesmanId + "~" + ExpiryDate + "~" + CurrencyRate + "~" + Contact_person_id + "~" + Tax_Option + "~" + Tax_Code; 
                }

                if (e.Parameter.Split('~')[4] != null) ComponentType = e.Parameter.Split('~')[4];
                DataTable ComponentTable = objPurchaseReturnBL.GetComponent(Customer, PurchaseReturnDate, strPurchaseReturnID, branchId, ComponentType);
                lookup_quotation.GridView.Selection.CancelSelection();
                lookup_quotation.DataSource = ComponentTable;
                lookup_quotation.DataBind();

                Session["PR_ComponentData"] = ComponentTable;



            }
            else if (e.Parameter.Split('~')[0] == "RemoveComponentGridOnSelection")//Subhabrata for binding quotation
            {

                ComponentQuotationPanel.JSProperties["cpDetails"] = Reference + "~" + Currency_Id + "~" + SalesmanId + "~" + ExpiryDate + "~" + CurrencyRate + "~" + Contact_person_id + "~" + Tax_Option + "~" + Tax_Code; 
            }
            else if (e.Parameter.Split('~')[0] == "BindComponentGridOnSelection")//Subhabrata for binding quotation
            {
                if (grid_Products.GetSelectedFieldValues("ComponentDetailsID").Count != 0)
                {
                    for (int i = 0; i < grid_Products.GetSelectedFieldValues("ComponentDetailsID").Count; i++)
                    {
                        InvoiceIds += "," + grid_Products.GetSelectedFieldValues("ComponentID")[i];
                    }
                    InvoiceIds = InvoiceIds.TrimStart(',');
                    lookup_quotation.GridView.Selection.UnselectAll();
                    if (!String.IsNullOrEmpty(InvoiceIds))
                    {
                        string[] eachQuo = InvoiceIds.Split(',');
                        if (eachQuo.Length > 1)//More tha one quotation
                        {
                            txt_InvoiceDate.Text = "Multiple Select Purchase Invoice Dates";

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

                string strType = "PI";

              
                DataTable dt = objPurchaseReturnBL.GetNecessaryData(InvoiceIds, strType);
                if (dt != null && dt.Rows.Count > 0)
                {
                    Reference = Convert.ToString(dt.Rows[0]["Reference"]);
                    Currency_Id = Convert.ToString(dt.Rows[0]["Currency_Id"]);
                    SalesmanId = Convert.ToString(dt.Rows[0]["SalesmanId"]);
                    ExpiryDate = Convert.ToString(dt.Rows[0]["ExpiryDate"]);
                    CurrencyRate = Convert.ToString(dt.Rows[0]["CurrencyRate"]);
                    Contact_person_id = Convert.ToString(dt.Rows[0]["Contact_Person_Id"]);
                    Tax_Option = Convert.ToString(dt.Rows[0]["Tax_Option"]);
                    Tax_Code = Convert.ToString(dt.Rows[0]["Tax_Code"]);
                }
                ComponentQuotationPanel.JSProperties["cpDetails"] = Reference + "~" + Currency_Id + "~" + SalesmanId + "~" + ExpiryDate + "~" + CurrencyRate + "~" + Contact_person_id + "~" + Tax_Option + "~" + Tax_Code ; 
            }
            else if (e.Parameter.Split('~')[0] == "DateCheckOnChanged")//Subhabrata for binding quotation
            {

                if (grid_Products.GetSelectedFieldValues("Quotation_No").Count != 0)
                {

                   // DateTime SalesOrderDate = Convert.ToDateTime(e.Parameter.Split('~')[2]);
                    if (lookup_quotation.GridView.GetSelectedFieldValues("ComponentDate").Count() != 0)
                    {
                        //DateTime QuotationDate = DateTime.Parse(Convert.ToString(lookup_quotation.GridView.GetSelectedFieldValues("Quote_Date")[0]));
                        //if (SalesOrderDate < QuotationDate)
                        //{
                        lookup_quotation.GridView.Selection.UnselectAll();
                        //}
                    }


                    //Quote_Date

                }
            }

            else if (e.Parameter.Split('~')[0] == "BranchCheckOnChanged")
            {

                if (grid_Products.GetSelectedFieldValues("Quotation_No").Count != 0)
                {
                    if (lookup_quotation.GridView.GetSelectedFieldValues("ComponentDate").Count() != 0)
                    {
                        lookup_quotation.GridView.Selection.UnselectAll();

                    }

                }

            }
        }


        protected void lookup_quotation_DataBinding(object sender, EventArgs e)
        {
            if (Session["PR_ComponentData"] != null)
            {
                lookup_quotation.DataSource = (DataTable)Session["PR_ComponentData"];
            }
        }


        protected void ComponentDatePanel_Callback(object sender, CallbackEventArgsBase e)
        {
            string strSplitCommand = e.Parameter.Split('~')[0];
            if (strSplitCommand == "BindComponentDate")
            {
                string Invoice_No = Convert.ToString(e.Parameter.Split('~')[1]);

                DataTable dt_QuotationDetails = objPurchaseReturnBL.GetPurchaseInvoiceDate(Invoice_No);
                if (dt_QuotationDetails != null && dt_QuotationDetails.Rows.Count > 0)
                {
                    string quotationdate = Convert.ToString(dt_QuotationDetails.Rows[0]["Invoice_Date"]);
                    if (!string.IsNullOrEmpty(quotationdate))
                    {
                        txt_InvoiceDate.Text = Convert.ToString(quotationdate);
                    }
                }
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
                    DataTable dt_QuotationDetails = new DataTable();
                    string IdKey = Convert.ToString(Request.QueryString["key"]);
                    if (!string.IsNullOrEmpty(IdKey))
                    {
                        if (IdKey != "ADD")
                        {
                            dt_QuotationDetails = objPurchaseReturnBL.GetPurchaseInvoiceDetailsOnly(QuoComponent, "Edit");
                            // dt_QuotationDetails = objPurchaseChallanBL.GetInvoiceDetailsOnly(QuoComponent, IdKey, "");
                        }
                        else
                        {
                            dt_QuotationDetails = objPurchaseReturnBL.GetPurchaseInvoiceDetailsOnly(QuoComponent, "Add");
                        }

                    }
                    else
                    {
                        dt_QuotationDetails = objPurchaseReturnBL.GetPurchaseInvoiceDetailsOnly(QuoComponent, "Add");
                    }
                    // Session["OrderDetails"] = null;

                    grid_Products.DataSource = GetProductsInfo(dt_QuotationDetails);
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


        public IEnumerable GetProductsInfo(DataTable SalesInvoicedt1)
        {
            List<SalesOrder> OrderList = new List<SalesOrder>();
            for (int i = 0; i < SalesInvoicedt1.Rows.Count; i++)
            {
                SalesOrder Orders = new SalesOrder();

                Orders.SrlNo = Convert.ToString(i + 1);
                // Orders.Key_UniqueId = Convert.ToString(i + 1);
                Orders.ComponentDetailsID = Convert.ToString(SalesInvoicedt1.Rows[i]["ComponentDetailsID"]);
                if (!string.IsNullOrEmpty(Convert.ToString(SalesInvoicedt1.Rows[i]["Quotation_No"])))
                { Orders.Quotation_No = Convert.ToInt64(SalesInvoicedt1.Rows[i]["Quotation_No"]); }
                else
                { Orders.Quotation_No = 0; }
                Orders.gvColProduct = Convert.ToString(SalesInvoicedt1.Rows[i]["QuoteDetails_ProductId"]);
                Orders.gvColDiscription = Convert.ToString(SalesInvoicedt1.Rows[i]["Description"]);
                Orders.gvColQuantity = Convert.ToString(SalesInvoicedt1.Rows[i]["QuoteDetails_Quantity"]);
                Orders.Quotation_Num = Convert.ToString(SalesInvoicedt1.Rows[i]["Quotation"]);
                Orders.Product_Shortname = Convert.ToString(SalesInvoicedt1.Rows[i]["Product_Name"]);
                Orders.ComponentID = Convert.ToString(SalesInvoicedt1.Rows[i]["ComponentID"]);
                OrderList.Add(Orders);
            }

            return OrderList;
        }


        public class SalesOrder
        {
            public string SrlNo { get; set; }
            public string OrderDetails_Id { get; set; }
            public string gvColProduct { get; set; }
            public string gvColDiscription { get; set; }
            public string gvColQuantity { get; set; }
            public string gvColUOM { get; set; }
            public string Warehouse { get; set; }
            public string gvColStockQty { get; set; }
            public string gvColStockUOM { get; set; }
            public string gvColStockPurchasePrice { get; set; }
            public string gvColDiscount { get; set; }
            public string gvColAmount { get; set; }
            public string gvColTaxAmount { get; set; }
            public string gvColTotalAmountINR { get; set; }
            public Int64 Quotation_No { get; set; }
            public string Quotation_Num { get; set; }
            public string ComponentDetailsID { get; set; }
            public string Product_Shortname { get; set; }
            public string ProductName { get; set; }
            public string ComponentID { get; set; }
            public string ComponentNumber { get; set; }
            public string TotalQty { get; set; }
            public string BalanceQty { get; set; }
            public string IsComponentProduct { get; set; }
            public string ProductDisID { get; set; }
            public string Product { get; set; }
        }



        public class SR
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
            public string gvColStockPurchasePrice { get; set; }
            public string Discount { get; set; }
            public string Amount { get; set; }
            public string TaxAmount { get; set; }
            public string TotalAmount { get; set; }

            public string gvColTotalAmountINR { get; set; }
            public Int64 Quotation_No { get; set; }
            public string Quotation_Num { get; set; }
            public string ComponentDetailsID { get; set; }
            public string Product_Shortname { get; set; }
            public string ProductName { get; set; }
            public string ComponentID { get; set; }
            public string ComponentNumber { get; set; }
            public string TotalQty { get; set; }
            public string BalanceQty { get; set; }
            public string IsComponentProduct { get; set; }
            public string ProductDisID { get; set; }
            public string Product { get; set; }
        }

        public bool ImplementTaxonTagging(int id, int slno)
        {
            Boolean inUse = false;
            DataTable taxTable = (DataTable)Session["PR_FinalTaxRecord"];
            ProcedureExecute proc = new ProcedureExecute("prc_taxReturnTable");
            proc.AddVarcharPara("@Module", 20, "PurchaseReturn");
            proc.AddIntegerPara("@id", id);
            proc.AddIntegerPara("@slno", slno);
            proc.AddBooleanPara("@inUse", true, QueryParameterDirection.Output);

            DataTable returnedTable = proc.GetTable();
            inUse = Convert.ToBoolean(proc.GetParaValue("@inUse"));

            if (returnedTable != null)
                taxTable.Merge(returnedTable);


            Session["PR_FinalTaxRecord"] = taxTable;

            return inUse;
        }


        //public IEnumerable GetInvoiceInfo(DataTable SalesInvoicedt1, string Order_Id)
        //{
        //    List<SR> OrderList = new List<SR>();
        //    DataColumnCollection dtC = SalesInvoicedt1.Columns;
        //    CreateDataTaxTable();
        //    string commaSeparatedString = String.Join(",", SalesInvoicedt1.AsEnumerable().Select(x => x.Field<Int64>("QuotationID").ToString()).ToArray());
        //    DataTable tempWarehouse = GetInvoiceWarehouse(commaSeparatedString);



        //    for (int i = 0; i < SalesInvoicedt1.Rows.Count; i++)
        //    {
        //        SR Orders = new SR();

        //        Orders.SrlNo = Convert.ToString(i + 1);
        //        Orders.QuotationID = Convert.ToString(i + 1);
        //        Orders.ProductID = Convert.ToString(SalesInvoicedt1.Rows[i]["ProductID"]);
        //        Orders.Description = Convert.ToString(SalesInvoicedt1.Rows[i]["Description"]);
        //        Orders.Quantity = Convert.ToString(SalesInvoicedt1.Rows[i]["Quantity"]);
        //        Orders.UOM = Convert.ToString(SalesInvoicedt1.Rows[i]["StockUOM"]);
        //        Orders.Warehouse = "";
        //        Orders.StockQuantity = Convert.ToString(SalesInvoicedt1.Rows[i]["StockQuantity"]);
        //        Orders.StockUOM = Convert.ToString(SalesInvoicedt1.Rows[i]["StockUOM"]);
        //        Orders.gvColStockPurchasePrice = "";
        //        Orders.Discount = Convert.ToString(SalesInvoicedt1.Rows[i]["Discount"]);
        //        Orders.Amount = Convert.ToString(SalesInvoicedt1.Rows[i]["Amount"]);
        //        Orders.TotalAmount = Convert.ToString(SalesInvoicedt1.Rows[i]["TotalAmount"]);
        //        Orders.TaxAmount = Convert.ToString(SalesInvoicedt1.Rows[i]["TaxAmount"]);
        //        Orders.gvColTotalAmountINR = "";
        //        Orders.SalePrice = Convert.ToString(SalesInvoicedt1.Rows[i]["SalePrice"]);

        //        Orders.ProductName = Convert.ToString(SalesInvoicedt1.Rows[i]["Products_Name"]);


        //        //22-03-2017

        //        Orders.ComponentID = Convert.ToString(SalesInvoicedt1.Rows[i]["ComponentID"]);
        //        Orders.ComponentNumber = Convert.ToString(SalesInvoicedt1.Rows[i]["ComponentNumber"]);
        //        Orders.TotalQty = Convert.ToString(SalesInvoicedt1.Rows[i]["TotalQty"]);
        //        Orders.BalanceQty = Convert.ToString(SalesInvoicedt1.Rows[i]["BalanceQty"]);
        //        Orders.IsComponentProduct = Convert.ToString(SalesInvoicedt1.Rows[i]["IsComponentProduct"]);


        //        Orders.ProductDisID = Convert.ToString(SalesInvoicedt1.Rows[i]["ProductID"]);
        //        Orders.Product = Convert.ToString(SalesInvoicedt1.Rows[i]["Products_Name"]);

        //        //22-03-2017

        //        if (!string.IsNullOrEmpty(Convert.ToString(SalesInvoicedt1.Rows[i]["Quotation_No"])))//Added on 15-02-2017
        //        { Orders.Quotation_No = Convert.ToInt64(SalesInvoicedt1.Rows[i]["Quotation_No"]); }
        //        else
        //        { Orders.Quotation_No = 0; }
        //        if (dtC.Contains("Quotation"))
        //        {
        //            Orders.Quotation_Num = Convert.ToString(SalesInvoicedt1.Rows[i]["Quotation"]);//subhabrata on 21-02-2017
        //        }

        //        // Mapping With Warehouse with Product Srl No

        //        string strQuoteDetails_Id = Convert.ToString(SalesInvoicedt1.Rows[i]["QuotationID"]).Trim();

        //        if (ImplementTaxonTagging(Convert.ToInt32(strQuoteDetails_Id), Convert.ToInt32(Orders.SrlNo)))
        //        {
        //            Orders.TaxAmount = "0.00";
        //        }


        //        if (tempWarehouse != null && tempWarehouse.Rows.Count > 0)
        //        {
        //            var rows = tempWarehouse.Select("InvoiceDetails_Id ='" + strQuoteDetails_Id + "'");
        //            foreach (var row in rows)
        //            {
        //                row["Product_SrlNo"] = Convert.ToString(i + 1);
        //            }
        //            tempWarehouse.AcceptChanges();
        //        }


        //        //if (tempWarehouse != null && tempWarehouse.Rows.Count > 0)
        //        //{
        //        //    var rows = tempWarehouse.Select("QuoteDetails_Id ='" + strQuoteDetails_Id + "'");
        //        //    foreach (var row in rows)
        //        //    {
        //        //        row["Product_SrlNo"] = Convert.ToString(i + 1);
        //        //    }
        //        //    tempWarehouse.AcceptChanges();
        //        //}

        //        //if (tempProductTax != null && tempProductTax.Rows.Count > 0)
        //        //{
        //        //    var taxrows = tempProductTax.Select("ProductTax_ProductId ='" + strQuoteDetails_Id + "'");
        //        //    foreach (var row in taxrows)
        //        //    {
        //        //        row["SlNo"] = Convert.ToString(i + 1);
        //        //    }
        //        //    tempProductTax.AcceptChanges();
        //        //}

        //        // End


        //        OrderList.Add(Orders);
        //    }


        //    if (tempWarehouse != null && tempWarehouse.Rows.Count > 0)
        //    {
        //        tempWarehouse.Columns.Remove("InvoiceDetails_Id");
        //        Session["PR_WarehouseData"] = tempWarehouse;

        //        // GrdWarehousePC.DataSource = tempWarehouse.DefaultView;
        //        // GrdWarehousePC.DataBind();
        //    }
        //    else
        //    {
        //        Session["PR_WarehouseData"] = null;
        //        GrdWarehousePC.DataSource = null;
        //        GrdWarehousePC.DataBind();
        //    }

        //    return OrderList;
        //}

        public IEnumerable GetInvoiceInfo(DataTable SalesInvoicedt1, string Order_Id)
        {
            List<SR> OrderList = new List<SR>();
            DataColumnCollection dtC = SalesInvoicedt1.Columns;
            CreateDataTaxTable();



            for (int i = 0; i < SalesInvoicedt1.Rows.Count; i++)
            {
                SR Orders = new SR();

                Orders.SrlNo = Convert.ToString(i + 1);
                Orders.QuotationID = Convert.ToString(i + 1);
                Orders.ProductID = Convert.ToString(SalesInvoicedt1.Rows[i]["ProductID"]);
                Orders.Description = Convert.ToString(SalesInvoicedt1.Rows[i]["Description"]);
                Orders.Quantity = Convert.ToString(SalesInvoicedt1.Rows[i]["Quantity"]);
                // Orders.Quantity = Convert.ToString(SalesInvoicedt1.Rows[i]["TotalQty"]);
                Orders.UOM = Convert.ToString(SalesInvoicedt1.Rows[i]["UOM"]);
                Orders.Warehouse = "";
                Orders.StockQuantity = Convert.ToString(SalesInvoicedt1.Rows[i]["StockQuantity"]);
                Orders.StockUOM = Convert.ToString(SalesInvoicedt1.Rows[i]["StockUOM"]);
                Orders.gvColStockPurchasePrice = "";
                Orders.Discount = Convert.ToString(SalesInvoicedt1.Rows[i]["Discount"]);
                Orders.Amount = Convert.ToString(SalesInvoicedt1.Rows[i]["Amount"]);
                Orders.TotalAmount = Convert.ToString(SalesInvoicedt1.Rows[i]["TotalAmount"]);
                Orders.TaxAmount = Convert.ToString(SalesInvoicedt1.Rows[i]["TaxAmount"]);
                Orders.gvColTotalAmountINR = "";
                Orders.SalePrice = Convert.ToString(SalesInvoicedt1.Rows[i]["SalePrice"]);

                Orders.ProductName = Convert.ToString(SalesInvoicedt1.Rows[i]["ProductName"]);


                //22-03-2017

                Orders.ComponentID = Convert.ToString(SalesInvoicedt1.Rows[i]["ComponentID"]);
                Orders.ComponentNumber = Convert.ToString(SalesInvoicedt1.Rows[i]["ComponentNumber"]);
                Orders.TotalQty = Convert.ToString(SalesInvoicedt1.Rows[i]["TotalQty"]);
                Orders.BalanceQty = Convert.ToString(SalesInvoicedt1.Rows[i]["BalanceQty"]);
                Orders.IsComponentProduct = Convert.ToString(SalesInvoicedt1.Rows[i]["IsComponentProduct"]);


                Orders.ProductDisID = Convert.ToString(SalesInvoicedt1.Rows[i]["ProductID"]);
                Orders.Product = Convert.ToString(SalesInvoicedt1.Rows[i]["ProductName"]);

                //22-03-2017

                // Mapping With Warehouse with Product Srl No

                String QuoComponent = "";
                List<object> QuoList = lookup_quotation.GridView.GetSelectedFieldValues("ComponentID");
                foreach (object Quo in QuoList)
                {
                    QuoComponent += "," + Quo;
                }
                QuoComponent = QuoComponent.TrimStart(',');
                string[] eachPurchaseInvoice = QuoComponent.Split(',');
                string strQuoteDetails_Id = Convert.ToString(SalesInvoicedt1.Rows[i]["QuotationID"]).Trim();

                if (ImplementTaxonTagging(Convert.ToInt32(strQuoteDetails_Id), Convert.ToInt32(Orders.SrlNo)))
                {
                    Orders.TaxAmount = "0.00";
                }

                // main tax integrate with respect to first tag purchase invoice
                if (eachPurchaseInvoice.Length == 1)
                {
                    DataTable tempMainTax = objPurchaseReturnBL.GetTaxDetailsPI(QuoComponent);
                    if (tempMainTax != null && tempMainTax.Rows.Count > 0)
                    {
                        Session["PR_TaxDetails"] = tempMainTax;

                    }

                }
                else { Session["PR_TaxDetails"] = null; }
                //  main tax integrate with respect to first tag purchase invoice
                //warehouse

                string commaSeparatedString = String.Join(",", SalesInvoicedt1.AsEnumerable().Select(x => x.Field<Int64>("QuotationID").ToString()).ToArray());
                DataTable tempWarehouse = GetInvoiceWarehouse(commaSeparatedString);
                if (tempWarehouse != null && tempWarehouse.Rows.Count > 0)
                {
                    var rows = tempWarehouse.Select("InvoiceDetails_Id ='" + strQuoteDetails_Id + "'");
                    foreach (var row in rows)
                    {
                        row["Product_SrlNo"] = Convert.ToString(i + 1);
                    }
                    tempWarehouse.AcceptChanges();
                }

                if (tempWarehouse != null && tempWarehouse.Rows.Count > 0)
                {
                    tempWarehouse.Columns.Remove("InvoiceDetails_Id");
                    Session["PR_WarehouseData"] = tempWarehouse;

                    // GrdWarehousePC.DataSource = tempWarehouse.DefaultView;
                    // GrdWarehousePC.DataBind();
                }
                else
                {
                    Session["PR_WarehouseData"] = null;
                    GrdWarehousePC.DataSource = null;
                    GrdWarehousePC.DataBind();
                }
                //warehouse

                //if (QuoComponent =="")
                //{






                //    if (!string.IsNullOrEmpty(Convert.ToString(SalesInvoicedt1.Rows[i]["Quotation_No"])))//Added on 15-02-2017
                //    { Orders.Quotation_No = Convert.ToInt64(SalesInvoicedt1.Rows[i]["Quotation_No"]); }
                //    else
                //    { Orders.Quotation_No = 0; }
                //    if (dtC.Contains("Quotation"))
                //    {
                //        Orders.Quotation_Num = Convert.ToString(SalesInvoicedt1.Rows[i]["Quotation"]);//subhabrata on 21-02-2017
                //    }

                //}




                OrderList.Add(Orders);
            }


            BindSessionByDatatable(SalesInvoicedt1);
            return OrderList;
        }

        #region kaushik/SessionBind
        //
        public bool BindSessionByDatatable(DataTable dt)
        {
            bool IsSuccess = false;
            DataTable SlsOreturnDT = new DataTable();


            SlsOreturnDT.Columns.Add("SrlNo", typeof(string));
            SlsOreturnDT.Columns.Add("QuotationID", typeof(string));
            SlsOreturnDT.Columns.Add("ProductID", typeof(string));
            SlsOreturnDT.Columns.Add("Description", typeof(string));
            SlsOreturnDT.Columns.Add("Quantity", typeof(string));
            SlsOreturnDT.Columns.Add("UOM", typeof(string));
            SlsOreturnDT.Columns.Add("Warehouse", typeof(string));
            SlsOreturnDT.Columns.Add("StockQuantity", typeof(string));
            SlsOreturnDT.Columns.Add("StockUOM", typeof(string));
            SlsOreturnDT.Columns.Add("SalePrice", typeof(string));
            SlsOreturnDT.Columns.Add("Discount", typeof(string));
            SlsOreturnDT.Columns.Add("Amount", typeof(string));
            SlsOreturnDT.Columns.Add("TaxAmount", typeof(string));
            SlsOreturnDT.Columns.Add("TotalAmount", typeof(string));
            SlsOreturnDT.Columns.Add("Status", typeof(string));
            SlsOreturnDT.Columns.Add("ProductName", typeof(string));
            SlsOreturnDT.Columns.Add("ComponentID", typeof(string));
            SlsOreturnDT.Columns.Add("ComponentNumber", typeof(string));
            SlsOreturnDT.Columns.Add("TotalQty", typeof(string));
            SlsOreturnDT.Columns.Add("BalanceQty", typeof(string));
            SlsOreturnDT.Columns.Add("IsComponentProduct", typeof(string));
            SlsOreturnDT.Columns.Add("ProductDisID", typeof(string));
            SlsOreturnDT.Columns.Add("Product", typeof(string));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsSuccess = true;
                //  DataColumnCollection dtC = dt.Columns;




                // Quotationdt.Rows.Add(SrlNo, QuotationID, ProductDetails, Description, Quantity, UOM, Warehouse, StockQuantity, StockUOM, SalePrice, Discount, Amount, TaxAmount, TotalAmount, "U", ProductName, ComponentID, ComponentNumber, TotalQty, BalanceQty, IsComponentProduct, ProductDisID, Product);
                SlsOreturnDT.Rows.Add(Convert.ToString(i + 1),
                    Convert.ToString(dt.Rows[i]["QuotationID"]),
                    Convert.ToString(dt.Rows[i]["ProductID"]),
                    Convert.ToString(dt.Rows[i]["Description"]),
                    Convert.ToString(dt.Rows[i]["Quantity"]),
                    Convert.ToString(dt.Rows[i]["UOM"]),
                    "", Convert.ToString(dt.Rows[i]["StockQuantity"]),
                    Convert.ToString(dt.Rows[i]["StockUOM"]),
                                Convert.ToString(dt.Rows[i]["SalePrice"]),
                               Convert.ToString(dt.Rows[i]["Discount"]),
                    Convert.ToString(dt.Rows[i]["Amount"]),
                    Convert.ToString(dt.Rows[i]["TaxAmount"]),
                    Convert.ToString(dt.Rows[i]["TotalAmount"]), "U",
                    Convert.ToString(dt.Rows[i]["ProductName"]),
                    Convert.ToString(dt.Rows[i]["ComponentID"]),
                  Convert.ToString(dt.Rows[i]["ComponentNumber"]),
                   Convert.ToString(dt.Rows[i]["TotalQty"]),
                   Convert.ToString(dt.Rows[i]["BalanceQty"]),
                    Convert.ToString(dt.Rows[i]["IsComponentProduct"]),
                    Convert.ToString(dt.Rows[i]["ProductID"]),
                       Convert.ToString(dt.Rows[i]["ProductName"])


                               );
            }

            Session["PR_QuotationDetails"] = SlsOreturnDT;

            return IsSuccess;
        }//End
        #endregion

        public DataTable GetInvoiceWarehouse(string strInvoiceList)
        {
            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("prc_PurchaseReturn_Details");
                proc.AddVarcharPara("@Action", 500, "InvoiceWarehouseByQuotation");
                proc.AddVarcharPara("@InvoiceList", 3000, strInvoiceList);
                dt = proc.GetTable();

                string strNewVal = "", strOldVal = "";
                DataTable tempdt = dt.Copy();
                foreach (DataRow drr in tempdt.Rows)
                {
                    strNewVal = Convert.ToString(drr["InvoiceWarehouse_Id"]);

                    if (strNewVal == strOldVal)
                    {
                        drr["WarehouseName"] = "";
                        drr["viewWarehouseName"] = "";
                        drr["Quantity"] = "";
                        drr["viewQuantity"] = "";
                        drr["BatchNo"] = "";
                        drr["viewBatchNo"] = "";
                        drr["SalesUOMName"] = "";
                        drr["SalesQuantity"] = "";
                        drr["StkUOMName"] = "";
                        drr["StkQuantity"] = "";
                        drr["ConversionMultiplier"] = "";
                        drr["AvailableQty"] = "";
                        drr["BalancrStk"] = "";
                        drr["MfgDate"] = "";
                        drr["viewMFGDate"] = "";
                        drr["ExpiryDate"] = "";
                        drr["viewExpiryDate"] = "";
                    }

                    strOldVal = strNewVal;
                }

                Session["LoopPRWarehouse"] = Convert.ToString(Convert.ToInt32(strNewVal) + 1);
                tempdt.Columns.Remove("InvoiceWarehouse_Id");
                return tempdt;
            }
            catch
            {
                return null;
            }
        }

        protected void BindLookUp(string Customer, string SalesReturnDate, string branchId)
        {

            string strReturnID = Convert.ToString(Session["PR_ReturnID"]);
          //  string branchId = Convert.ToString(ddl_Branch.SelectedValue);
           // var type = Convert.ToString(rdl_SaleInvoice.SelectedValue);
            var type = "PGRN";
            DataTable ComponentTable = objPurchaseReturnBL.GetComponent(Customer, SalesReturnDate, strReturnID, branchId,type);
            lookup_quotation.GridView.Selection.CancelSelection();
            lookup_quotation.DataSource = ComponentTable;
            lookup_quotation.DataBind();

            Session["PR_ComponentData"] = ComponentTable;



        }




        //protected void acbpCrpUdf_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        //{
        //    if (Request.QueryString["key"] == "ADD")
        //    {
        //        if (reCat.isAllMandetoryDone((DataTable)Session["UdfDataOnAdd"], "PR") == false)
        //        {
        //            acbpCrpUdf.JSProperties["cpUDF"] = "false";

        //        }
        //        else
        //        {
        //            acbpCrpUdf.JSProperties["cpUDF"] = "true";
        //        }




        //    }
        //    else
        //    {
        //        acbpCrpUdf.JSProperties["cpUDF"] = "true";


        //    }
        //}
        protected void acbpCrpUdf_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            //BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);

            BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine();

            if (Request.QueryString["key"] == "ADD")
            {
                if (reCat.isAllMandetoryDone((DataTable)Session["UdfDataOnAdd"], "PR") == false)
                {
                    acbpCrpUdf.JSProperties["cpUDF"] = "false";

                }
                else
                {
                    acbpCrpUdf.JSProperties["cpUDF"] = "true";
                }


                acbpCrpUdf.JSProperties["cpTransport"] = "true";





                // BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
                DataTable DT = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='Transporter_PRMandatory' AND IsActive=1");
                if (DT != null && DT.Rows.Count > 0)
                {
                    string IsMandatory = Convert.ToString(DT.Rows[0]["Variable_Value"]).Trim();
                    if (Convert.ToString(Session["TransporterVisibilty"]).Trim() == "Yes")
                    {
                        if (IsMandatory == "Yes")
                        {

                            if (hfControlData.Value.Trim() == "")
                            {
                                acbpCrpUdf.JSProperties["cpTransport"] = "false";
                            }

                            else { acbpCrpUdf.JSProperties["cpTransport"] = "true"; }
                        }
                    }
                    else { acbpCrpUdf.JSProperties["cpTransport"] = "true"; }
                }

            }
            else
            {
                acbpCrpUdf.JSProperties["cpUDF"] = "true";
                acbpCrpUdf.JSProperties["cpTransport"] = "true";

                DataTable DT = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='Transporter_PRMandatory' AND IsActive=1");
                if (DT != null && DT.Rows.Count > 0)
                {
                    string IsMandatory = Convert.ToString(DT.Rows[0]["Variable_Value"]).Trim();
                    // objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
                     objEngine = new BusinessLogicLayer.DBEngine();

                    DataTable DTVisible = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='Show_Transporter' AND IsActive=1");
                    if (Convert.ToString(DTVisible.Rows[0]["Variable_Value"]).Trim() == "Yes")
                    {
                        if (IsMandatory == "Yes")
                        {
                            if (VehicleDetailsControl.GetControlValue("cmbTransporter") == "")
                            {
                                acbpCrpUdf.JSProperties["cpTransport"] = "false";
                            }

                            else { acbpCrpUdf.JSProperties["cpTransport"] = "true"; }
                        }
                    }
                    else { acbpCrpUdf.JSProperties["cpTransport"] = "true"; }
                }

            }
        }

    }
}