﻿using DevExpress.Web;
using DevExpress.Web.Mvc;
using EntityLayer.CommonELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessLogicLayer;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;
using DevExpress.XtraPrinting;
using DevExpress.Export;
using System.Drawing;
using Reports.Model;
using DataAccessLayer;

namespace Reports.Reports.GridReports
{
    public partial class BranchWHWiseSummary : System.Web.UI.Page
    {
        DateTime dtFrom;
        DateTime dtTo;
        BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(string.Empty);
        public EntityLayer.CommonELS.UserRightsForPage rights = new UserRightsForPage();

        ExcelFile objExcel = new ExcelFile();
        DataTable CompanyInfo = new DataTable();
        DataTable dtExport = new DataTable();
        DataTable dtReportHeader = new DataTable();
        DataTable dtReportFooter = new DataTable();

        DataTable dtProductTotal = null;
        string ProductTotalOpQty = "";
        string ProductTotalAltOpQty = "";
        string ProductTotalInQty = "";
        string ProductTotalAltInQty = "";
        string ProductTotalOutQty = "";
        string ProductTotalAltOutQty = "";
        string ProductTotalBalQty = "";
        string ProductTotalAltBalQty = "";
        string ProductTotalDesc = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/GridReports/BranchWHWiseSummary.aspx");
            if (!IsPostBack)
            {
                Session["chk_branwhsumtotal"] = 0;
                string GridHeader = "";
                BusinessLogicLayer.Reports GridHeaderDet = new BusinessLogicLayer.Reports();
                RptHeading.Text = "Branch/Warehouse Wise Stock - Summary";
                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), true, false, false, false, false, false);
                CompName.Text = GridHeader;

                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, true, false, false, false, false);
                CompAdd.Text = GridHeader;

                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, false, true, false, false, false);
                CompOth.Text = GridHeader;

                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, false, false, true, false, false);
                CompPh.Text = GridHeader;

                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, false, false, false, false, true);
                CompAccPrd.Text = GridHeader;

                Session["SI_ComponentData"] = null;
                Session["IsBranWHWiseSumFilter"] = null;
                Session["SI_ComponentData_Branch"] = null;
                Session["branwhsumWarehouseData"] = null;
                dtFrom = DateTime.Now;
                dtTo = DateTime.Now;
                ASPxFromDate.Text = dtFrom.ToString("dd-MM-yyyy");
                ASPxToDate.Text = dtTo.ToString("dd-MM-yyyy");
                ASPxFromDate.Value = DateTime.Now;
                ASPxToDate.Value = DateTime.Now;
                ShowGrid.Columns[7].Visible = false;
                ShowGrid.Columns[9].Visible = false;
                ShowGrid.Columns[11].Visible = false;
                ShowGrid.Columns[13].Visible = false;
                ShowGrid.Columns[15].Visible = false;

                dtFrom = Convert.ToDateTime(ASPxFromDate.Date);
                dtTo = Convert.ToDateTime(ASPxToDate.Date);

                string FROMDATE = dtFrom.ToString("yyyy-MM-dd");
                string TODATE = dtTo.ToString("yyyy-MM-dd");

                Date_finyearwise(Convert.ToString(Session["LastFinYear"]));
                BranchHoOffice();
                Session["BranchNames"] = null;
            }
            else
            {

                dtFrom = Convert.ToDateTime(ASPxFromDate.Date);
                dtTo = Convert.ToDateTime(ASPxToDate.Date);
            }
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string sPath = Convert.ToString(HttpContext.Current.Request.Url);
                oDBEngine.Call_CheckPageaccessebility(sPath);
            }
        }
        public void BranchHoOffice()
        {
            CommonBL bll1 = new CommonBL();
            DataTable stbill = new DataTable();
            DataTable dtBranchChild = new DataTable();
            stbill = bll1.GetBranchheadoffice(Convert.ToString(HttpContext.Current.Session["userbranchHierarchy"]), "HO");
            if (stbill.Rows.Count > 0)
            {
                ddlbranchHO.DataSource = stbill;
                ddlbranchHO.DataTextField = "Code";
                ddlbranchHO.DataValueField = "branch_id";
                ddlbranchHO.DataBind();
                dtBranchChild = GetChildBranch(Convert.ToString(HttpContext.Current.Session["userbranchHierarchy"]));
                if (dtBranchChild.Rows.Count > 0)
                {
                    ddlbranchHO.Items.Insert(0, new ListItem("All", "All"));
                }
            }
        }

        public DataTable GetChildBranch(string CHILDBRANCH)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            SqlCommand cmd = new SqlCommand("PRC_FINDCHILDBRANCH_REPORT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CHILDBRANCH", CHILDBRANCH);
            cmd.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);
            cmd.Dispose();
            con.Dispose();
            return dt;
        }

        #region Export
        public void cmbExport_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 Filter = int.Parse(Convert.ToString(drdExport.SelectedItem.Value));
            if (Convert.ToString(Session["IsBranWHWiseSumFilter"]) == "Y")
            {
                if (Filter != 0)
                {
                    bindexport(Filter);
                }
            }
            else
            {
                BranchHoOffice();
            }
        }

        public void bindexport(int Filter)
        {
            string filename = "Branch_Warehouse_Wise_Stock_Summary";
            exporter.FileName = filename;
            if (Filter == 1 || Filter == 2)
            {
                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
                con.Open();
                string selectQuery = "SELECT BRANCHDESC,WHDESC,PRODCODE,PRODDESC,CLASSDESC,BRANDNAME,STOCKUOM,ALTSTOCKUOM,OP_QTY,OP_ALTQTY,IN_QTY,ALTIN_QTY,OUT_QTY,ALTOUT_QTY,BALQTY,BALALTQTY FROM BRANCHWHWISESTOCKDETSUM_REPORT Where USERID=" + Convert.ToInt32(Session["userid"]) + " AND PRODCODE<>'Gross Total :' AND REPORTTYPE='Summary' order by SEQ";
                SqlDataAdapter myCommand = new SqlDataAdapter(selectQuery, con);

                // Create and fill a DataSet.
                DataSet ds = new DataSet();
                myCommand.Fill(ds, "Main");
                myCommand = new SqlDataAdapter("SELECT PRODCODE,OP_QTY,OP_ALTQTY,IN_QTY,ALTIN_QTY,OUT_QTY,ALTOUT_QTY,BALQTY,BALALTQTY FROM BRANCHWHWISESTOCKDETSUM_REPORT Where USERID=" + Convert.ToInt32(Session["userid"]) + " AND PRODCODE='Gross Total :' AND REPORTTYPE='Summary'", con);
                myCommand.Fill(ds, "GrossTotal");
                myCommand.Dispose();
                con.Dispose();
                Session["exportbranwhstocksumdataset"] = ds;

                dtExport = ds.Tables[0].Copy();
                dtExport.Clear();
                dtExport.Columns.Add(new DataColumn("Unit", typeof(string)));
                dtExport.Columns.Add(new DataColumn("Warehouse", typeof(string)));
                dtExport.Columns.Add(new DataColumn("Item Code", typeof(string)));
                dtExport.Columns.Add(new DataColumn("Item Name", typeof(string)));
                dtExport.Columns.Add(new DataColumn("Class", typeof(string)));
                dtExport.Columns.Add(new DataColumn("Brand", typeof(string)));
                dtExport.Columns.Add(new DataColumn("Stock Unit", typeof(string)));
                dtExport.Columns.Add(new DataColumn("Alternate Unit", typeof(string)));
                dtExport.Columns.Add(new DataColumn("Opening Qty.", typeof(decimal)));
                dtExport.Columns.Add(new DataColumn("Opening Alt. Qty.", typeof(decimal)));
                dtExport.Columns.Add(new DataColumn("Received Qty.", typeof(decimal)));
                dtExport.Columns.Add(new DataColumn("Recd. Alt. Qty.", typeof(decimal)));
                dtExport.Columns.Add(new DataColumn("Delivered Qty.", typeof(decimal)));
                dtExport.Columns.Add(new DataColumn("Delv. Alt. Qty.", typeof(decimal)));
                dtExport.Columns.Add(new DataColumn("Balance Qty.", typeof(decimal)));
                dtExport.Columns.Add(new DataColumn("Balance Alt. Qty.", typeof(decimal)));

                foreach (DataRow dr1 in ds.Tables[0].Rows)
                {
                    DataRow row2 = dtExport.NewRow();

                    row2["Unit"] = dr1["BRANCHDESC"];
                    row2["Warehouse"] = dr1["WHDESC"];
                    row2["Item Code"] = dr1["PRODCODE"];
                    row2["Item Name"] = dr1["PRODDESC"];
                    row2["Class"] = dr1["CLASSDESC"];
                    row2["Brand"] = dr1["BRANDNAME"];
                    row2["Stock Unit"] = dr1["STOCKUOM"];
                    row2["Alternate Unit"] = dr1["ALTSTOCKUOM"];
                    row2["Opening Qty."] = dr1["OP_QTY"];
                    row2["Opening Alt. Qty."] = dr1["OP_ALTQTY"];
                    row2["Received Qty."] = dr1["IN_QTY"];
                    row2["Recd. Alt. Qty."] = dr1["ALTIN_QTY"];
                    row2["Delivered Qty."] = dr1["OUT_QTY"];
                    row2["Delv. Alt. Qty."] = dr1["ALTOUT_QTY"];
                    row2["Balance Qty."] = dr1["BALQTY"];
                    row2["Balance Alt. Qty."] = dr1["BALALTQTY"];

                    dtExport.Rows.Add(row2);
                }

                if (chkAltQtyUOM.Checked == false)
                {
                    dtExport.Columns.Remove("Alternate Unit");
                    dtExport.Columns.Remove("Opening Alt. Qty.");
                    dtExport.Columns.Remove("Recd. Alt. Qty.");
                    dtExport.Columns.Remove("Delv. Alt. Qty.");
                    dtExport.Columns.Remove("Balance Alt. Qty.");
                }

                dtExport.Columns.Remove("BRANCHDESC");
                dtExport.Columns.Remove("WHDESC");
                dtExport.Columns.Remove("PRODCODE");
                dtExport.Columns.Remove("PRODDESC");
                dtExport.Columns.Remove("CLASSDESC");
                dtExport.Columns.Remove("BRANDNAME");
                dtExport.Columns.Remove("STOCKUOM");
                dtExport.Columns.Remove("ALTSTOCKUOM");
                dtExport.Columns.Remove("OP_QTY");
                dtExport.Columns.Remove("OP_ALTQTY");
                dtExport.Columns.Remove("IN_QTY");
                dtExport.Columns.Remove("ALTIN_QTY");
                dtExport.Columns.Remove("OUT_QTY");
                dtExport.Columns.Remove("ALTOUT_QTY");
                dtExport.Columns.Remove("BALQTY");
                dtExport.Columns.Remove("BALALTQTY");

                DataRow row3 = dtExport.NewRow();
                row3["Item Code"] = ds.Tables[1].Rows[0]["PRODCODE"].ToString();
                row3["Opening Qty."] = ds.Tables[1].Rows[0]["OP_QTY"].ToString();
                if (chkAltQtyUOM.Checked == true)
                    row3["Opening Alt. Qty."] = ds.Tables[1].Rows[0]["OP_ALTQTY"].ToString();
                row3["Received Qty."] = ds.Tables[1].Rows[0]["IN_QTY"].ToString();
                if (chkAltQtyUOM.Checked == true)
                    row3["Recd. Alt. Qty."] = ds.Tables[1].Rows[0]["ALTIN_QTY"].ToString();
                row3["Delivered Qty."] = ds.Tables[1].Rows[0]["OUT_QTY"].ToString();
                if (chkAltQtyUOM.Checked == true)
                    row3["Delv. Alt. Qty."] = ds.Tables[1].Rows[0]["ALTOUT_QTY"].ToString();
                row3["Balance Qty."] = ds.Tables[1].Rows[0]["BALQTY"].ToString();
                if (chkAltQtyUOM.Checked == true)
                    row3["Balance Alt. Qty."] = ds.Tables[1].Rows[0]["BALALTQTY"].ToString();
                dtExport.Rows.Add(row3);

                //For Excel/PDF Header
                BusinessLogicLayer.Reports GridHeaderDet = new BusinessLogicLayer.Reports();
                dtReportHeader.Columns.Add(new DataColumn("Header", typeof(String)));

                string GridHeader = "";
                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), true, false, false, false, false, false);
                DataRow HeaderRow = dtReportHeader.NewRow();
                HeaderRow[0] = GridHeader.ToString();
                dtReportHeader.Rows.Add(HeaderRow);
                DataRow HeaderRow1 = dtReportHeader.NewRow();
                HeaderRow1[0] = Convert.ToString(Session["BranchNames"]);
                dtReportHeader.Rows.Add(HeaderRow1);
                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, true, false, false, false, false);
                DataRow HeaderRow2 = dtReportHeader.NewRow();
                HeaderRow2[0] = GridHeader.ToString();
                dtReportHeader.Rows.Add(HeaderRow2);
                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, false, true, false, false, false);
                DataRow HeaderRow3 = dtReportHeader.NewRow();
                HeaderRow3[0] = GridHeader.ToString();
                dtReportHeader.Rows.Add(HeaderRow3);
                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, false, false, true, false, false);
                DataRow HeaderRow4 = dtReportHeader.NewRow();
                HeaderRow4[0] = GridHeader.ToString();
                dtReportHeader.Rows.Add(HeaderRow4);
                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, false, false, false, false, true);
                DataRow HeaderRow5 = dtReportHeader.NewRow();
                HeaderRow5[0] = GridHeader.ToString();
                dtReportHeader.Rows.Add(HeaderRow5);
                DataRow HeaderRow6 = dtReportHeader.NewRow();
                HeaderRow6[0] = "Branch/Warehouse Wise Stock - Summary";
                dtReportHeader.Rows.Add(HeaderRow6);
                DataRow HeaderRow7 = dtReportHeader.NewRow();
                HeaderRow7[0] = "For the period: " + Convert.ToDateTime(ASPxFromDate.Date).ToString("dd-MM-yyyy") + " To " + Convert.ToDateTime(ASPxToDate.Date).ToString("dd-MM-yyyy");
                dtReportHeader.Rows.Add(HeaderRow7);

                //For Excel/PDF Footer
                dtReportFooter.Columns.Add(new DataColumn("Footer", typeof(String))); //0
                DataRow FooterRow1 = dtReportFooter.NewRow();
                dtReportFooter.Rows.Add(FooterRow1);
                DataRow FooterRow2 = dtReportFooter.NewRow();
                dtReportFooter.Rows.Add(FooterRow2);
                DataRow FooterRow = dtReportFooter.NewRow();
                FooterRow[0] = "* * *  End Of Report * * *   ";
                dtReportFooter.Rows.Add(FooterRow);
            }
            else
            {
                exporter.PageHeader.Font.Size = 10;
                exporter.PageHeader.Font.Name = "Tahoma";
                exporter.GridViewID = "ShowGrid";
            }

            switch (Filter)
            {
                case 1:
                    objExcel.ExportToExcelforExcel(dtExport, "Branch_Warehouse_Wise_Stock_Summary", "Total of : ", "Total of : ", dtReportHeader, dtReportFooter);
                    break;
                case 2:
                    objExcel.ExportToPDF(dtExport, "Branch_Warehouse_Wise_Stock_Summary", "Total of : ", "Total of : ", dtReportHeader, dtReportFooter);
                    break;
                case 3:
                    exporter.WriteCsvToResponse();
                    break;
                default:
                    return;
            }

        }

        #endregion

        #region =======================Branch/Warehouse wise Stock Data =========================
        protected void CallbackPanel_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string IsBranWHWiseSumFilter = Convert.ToString(hfIsBranWHWiseSumFilter.Value);
            Session["IsBranWHWiseSumFilter"] = IsBranWHWiseSumFilter;

            dtFrom = Convert.ToDateTime(ASPxFromDate.Date);
            dtTo = Convert.ToDateTime(ASPxToDate.Date);

            string FROMDATE = dtFrom.ToString("yyyy-MM-dd");
            string TODATE = dtTo.ToString("yyyy-MM-dd");

            string BRANCH_ID = "";

            string BranchList = "";
            List<object> BranchList1 = lookup_branch.GridView.GetSelectedFieldValues("ID");
            foreach (object Branch in BranchList1)
            {
                BranchList += "," + Branch;
            }
            BRANCH_ID = BranchList.TrimStart(',');

            string WH_ID = "";

            string WHID = "";
            List<object> WhidList = lookup_warehouse.GridView.GetSelectedFieldValues("ID");
            foreach (object WH in WhidList)
            {
                WHID += "," + WH;
            }
            WH_ID = WHID.TrimStart(',');

            string BRANCH_NAME = "";
            string BranchNameComponent = "";
            List<object> BranchNameList = lookup_branch.GridView.GetSelectedFieldValues("branch_description");
            foreach (object BranchName in BranchNameList)
            {
                BranchNameComponent += "," + BranchName;
            }
            if (BranchNameList.Count > 1 || BranchNameList.Count == 0)
            {
                BRANCH_NAME = "Multiple Branch Selected";
                Session["BranchNames"] = BRANCH_NAME;
            }
            else
            {
                BRANCH_NAME = BranchNameComponent.TrimStart(',');
                Session["BranchNames"] = "For Unit : " + BRANCH_NAME + " ";
            }
            CallbackPanel.JSProperties["cpBranchNames"] = Convert.ToString(Session["BranchNames"]);

            Task PopulateStockTrialDataTask = new Task(() => GetBranchWHWiseStockkdata(FROMDATE, TODATE, BRANCH_ID, WH_ID));
            PopulateStockTrialDataTask.RunSynchronously();

        }

        public void GetBranchWHWiseStockkdata(string FROMDATE, string TODATE, string BRANCH_ID, string WHID)
        {
            try
            {
                DataTable ds = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_BRANCHWHWISESTOCKDETSUM_REPORT");
                proc.AddPara("@COMPANYID", Convert.ToString(Session["LastCompany"]));
                proc.AddPara("@FINYEAR", Convert.ToString(Session["LastFinYear"]));
                proc.AddPara("@FROMDATE", FROMDATE);
                proc.AddPara("@TODATE", TODATE);
                proc.AddPara("@BRANCHID", BRANCH_ID);
                proc.AddPara("@PRODUCT_ID", hdncWiseProductId.Value);
                proc.AddPara("@WAREHOUSE_ID", WHID);
                proc.AddPara("@CLASS", hdnClassId.Value);
                proc.AddPara("@BRAND", hdnBranndId.Value);
                proc.AddPara("@SHOWALTQTYUOM", (chkAltQtyUOM.Checked) ? "1" : "0");
                //Rev Debashis 0025145
                proc.AddPara("@PROJECT_ID", "");
                //End of Rev Debashis 0025145
                proc.AddPara("@REPORTTYPE", "Summary");
                proc.AddPara("@USERID", Convert.ToInt32(Session["userid"]));

                ds = proc.GetTable();

            }
            catch (Exception ex)
            {
            }
        }

        protected void ShowGrid_SummaryDisplayText(object sender, ASPxGridViewSummaryDisplayTextEventArgs e)
        {
            if (Convert.ToString(Session["IsBranWHWiseSumFilter"]) == "Y")
            {
                dtProductTotal = oDBEngine.GetDataTable("SELECT PRODCODE,OP_QTY,OP_ALTQTY,IN_QTY,ALTIN_QTY,OUT_QTY,ALTOUT_QTY,BALQTY,BALALTQTY FROM BRANCHWHWISESTOCKDETSUM_REPORT Where USERID=" + Convert.ToInt32(Session["userid"]) + " AND PRODCODE='Gross Total :' AND REPORTTYPE='Summary'");
                ProductTotalDesc = dtProductTotal.Rows[0][0].ToString();
                ProductTotalOpQty = dtProductTotal.Rows[0][1].ToString();
                ProductTotalAltOpQty = dtProductTotal.Rows[0][2].ToString();
                ProductTotalInQty = dtProductTotal.Rows[0][3].ToString();
                ProductTotalAltInQty = dtProductTotal.Rows[0][4].ToString();
                ProductTotalOutQty = dtProductTotal.Rows[0][5].ToString();
                ProductTotalAltOutQty = dtProductTotal.Rows[0][6].ToString();
                ProductTotalBalQty = dtProductTotal.Rows[0][7].ToString();
                ProductTotalAltBalQty = dtProductTotal.Rows[0][8].ToString();
            }

            string summaryTAG = (e.Item as ASPxSummaryItem).Tag;
            if (e.IsTotalSummary == true)
            {
                switch (summaryTAG)
                {
                    case "Item_Prod":
                        e.Text = ProductTotalDesc;
                        break;
                    case "Item_OpQty":
                        e.Text = ProductTotalOpQty;
                        break;
                    case "Item_AltOpQty":
                        e.Text = ProductTotalAltOpQty;
                        break;
                    case "Item_InQty":
                        e.Text = ProductTotalInQty;
                        break;
                    case "Item_AltInQty":
                        e.Text = ProductTotalAltInQty;
                        break;
                    case "Item_OutQty":
                        e.Text = ProductTotalOutQty;
                        break;
                    case "Item_AltOutQty":
                        e.Text = ProductTotalAltOutQty;
                        break;
                    case "Item_BalQty":
                        e.Text = ProductTotalBalQty;
                        break;
                    case "Item_AltBalQty":
                        e.Text = ProductTotalAltBalQty;
                        break;
                }
            }
        }

        protected void ShowGrid_HtmlFooterCellPrepared(object sender, ASPxGridViewTableFooterCellEventArgs e)
        {
            if (e.Column.Caption != "Item Code")
            {
                e.Cell.Style["text-align"] = "right";
            }
        }

        protected void ShowGrid_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            if (Convert.ToString(e.CellValue).Contains("Total of : "))
            {
                Session["chk_branwhsumtotal"] = 1;
            }
            if (Convert.ToInt32(Session["chk_branwhsumtotal"]) == 1)
            {
                e.Cell.Font.Bold = true;
                e.Cell.BackColor = Color.DarkSeaGreen;
            }

            if (chkAltQtyUOM.Checked == true)
            {
                if (e.DataColumn.FieldName == "BALALTQTY")
                {
                    Session["chk_branwhsumtotal"] = 0;
                }
            }
            else
            {
                if (e.DataColumn.FieldName == "BALQTY")
                {
                    Session["chk_branwhsumtotal"] = 0;
                }
            }
        }
        #endregion

        public void Date_finyearwise(string Finyear)
        {
            CommonBL stkdb = new CommonBL();
            DataTable dtstkdb = new DataTable();

            dtstkdb = stkdb.GetDateFinancila(Finyear);
            if (dtstkdb.Rows.Count > 0)
            {
                ASPxFromDate.MaxDate = Convert.ToDateTime((dtstkdb.Rows[0]["FinYear_EndDate"]));
                ASPxFromDate.MinDate = Convert.ToDateTime((dtstkdb.Rows[0]["FinYear_StartDate"]));

                ASPxToDate.MaxDate = Convert.ToDateTime((dtstkdb.Rows[0]["FinYear_EndDate"]));
                ASPxToDate.MinDate = Convert.ToDateTime((dtstkdb.Rows[0]["FinYear_StartDate"]));

                DateTime MaximumDate = Convert.ToDateTime((dtstkdb.Rows[0]["FinYear_EndDate"]));
                DateTime MinimumDate = Convert.ToDateTime((dtstkdb.Rows[0]["FinYear_StartDate"]));

                DateTime TodayDate = Convert.ToDateTime(DateTime.Now);
                DateTime FinYearEndDate = MaximumDate;

                if (TodayDate > FinYearEndDate)
                {
                    ASPxToDate.Date = FinYearEndDate;
                    ASPxFromDate.Date = MinimumDate;
                }
                else
                {
                    ASPxToDate.Date = TodayDate;
                    ASPxFromDate.Date = MinimumDate;
                }
            }
        }

        #region Branch Populate

        protected void Componentbranch_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            if (e.Parameter.Split('~')[0] == "BindComponentGrid")
            {
                DataTable ComponentTable = new DataTable();
                string Hoid = e.Parameter.Split('~')[1];
                if (Hoid != "All")
                {
                    ComponentTable = GetBranch(Convert.ToString(HttpContext.Current.Session["userbranchHierarchy"]), Hoid);
                }
                else
                {
                    ComponentTable = oDBEngine.GetDataTable("select * from (select branch_id as ID,branch_description,branch_code from tbl_master_branch a where a.branch_id=1  union all select branch_id as ID,branch_description,branch_code from tbl_master_branch b where b.branch_parentId=1) a order by branch_description");
                }

                if (ComponentTable.Rows.Count > 0)
                {
                    Session["SI_ComponentData_Branch"] = ComponentTable;
                    lookup_branch.DataSource = ComponentTable;
                    lookup_branch.DataBind();
                }
                else
                {
                    Session["SI_ComponentData_Branch"] = ComponentTable;
                    lookup_branch.DataSource = null;
                    lookup_branch.DataBind();
                }
            }
        }

        public DataTable GetBranch(string BRANCH_ID, string Ho)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            SqlCommand cmd = new SqlCommand("GetFinancerBranchfetchhowise", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Branch", BRANCH_ID);
            cmd.Parameters.AddWithValue("@Hoid", Ho);
            cmd.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);
            cmd.Dispose();
            con.Dispose();

            return dt;
        }

        protected void lookup_branch_DataBinding(object sender, EventArgs e)
        {
            if (Session["SI_ComponentData_Branch"] != null)
            {
                lookup_branch.DataSource = (DataTable)Session["SI_ComponentData_Branch"];
            }
        }

        #endregion

        #region Warehouse Populate

        protected void Componentwarehouse_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            if (e.Parameter.Split('~')[0] == "BindWarehouseGrid")
            {
                DataTable WarehouseTable = new DataTable();
                WarehouseTable = GetWarehouse();

                if (WarehouseTable.Rows.Count > 0)
                {
                    Session["branwhsumWarehouseData"] = WarehouseTable;
                    lookup_warehouse.DataSource = WarehouseTable;
                    lookup_warehouse.DataBind();
                }
                else
                {
                    Session["branwhsumWarehouseData"] = WarehouseTable;
                    lookup_warehouse.DataSource = null;
                    lookup_warehouse.DataBind();
                }
            }
        }

        public DataTable GetWarehouse()
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            SqlCommand cmd = new SqlCommand("PRC_WAREHOUSESELECTION_REPORT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);
            cmd.Dispose();
            con.Dispose();

            return dt;
        }

        protected void lookup_warehouse_DataBinding(object sender, EventArgs e)
        {
            if (Session["branwhsumWarehouseData"] != null)
            {
                lookup_warehouse.DataSource = (DataTable)Session["branwhsumWarehouseData"];
            }
        }

        #endregion

        #region LinQ
        protected void GenerateEntityServerModeDataSource_Selecting(object sender, DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs e)
        {
            e.KeyExpression = "SEQ";
            SqlConnection connectionString = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            string Userid = Convert.ToString(HttpContext.Current.Session["userid"]);
            ReportSourceDataContext dc = new ReportSourceDataContext(connectionString);

            if (Convert.ToString(Session["IsBranWHWiseSumFilter"]) == "Y")
            {
                var q = from d in dc.BRANCHWHWISESTOCKDETSUM_REPORTs
                        where Convert.ToString(d.USERID) == Userid && Convert.ToString(d.REPORTTYPE) == "Summary" && Convert.ToString(d.PRODCODE) != "Gross Total :"
                        orderby d.SEQ
                        select d;
                e.QueryableSource = q;
            }
            else
            {
                var q = from d in dc.BRANCHWHWISESTOCKDETSUM_REPORTs
                        where Convert.ToString(d.SEQ) == "0"
                        orderby d.SEQ
                        select d;
                e.QueryableSource = q;
            }

            if (chkAltQtyUOM.Checked == true)
            {
                ShowGrid.Columns[7].Visible = true;
                ShowGrid.Columns[9].Visible = true;
                ShowGrid.Columns[11].Visible = true;
                ShowGrid.Columns[13].Visible = true;
                ShowGrid.Columns[15].Visible = true;
            }
            else
            {
                ShowGrid.Columns[7].Visible = false;
                ShowGrid.Columns[9].Visible = false;
                ShowGrid.Columns[11].Visible = false;
                ShowGrid.Columns[13].Visible = false;
                ShowGrid.Columns[15].Visible = false;
            }
        }
        #endregion
    }
}