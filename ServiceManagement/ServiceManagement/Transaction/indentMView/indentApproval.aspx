﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="indentApproval.aspx.cs" Inherits="ServiceManagement.ServiceManagement.Transaction.indentMView.indentApproval" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,800;1,900&display=swap" rel="stylesheet" />
    <script src="/assests/js/jquery.min.js"></script>
   <style>
        body {
            font-family: 'Poppins', sans-serif;
        }
        .box {
            padding:15px;
            background:#ddd;
            padding: 15px;
            background: #f1f1f1;
            border-radius: 7px;
            border: 1px solid #e3e3e3;
            margin-bottom:20px;
        }
        .hdtec {
            display:inline-block;
            min-width:100px;
            font-weight:500;
            color: #26509d;
        }
        .box p {
            margin-top:5px;
            margin-bottom:8px;
            font-size:14px
        }
        .tableContent {
            margin-bottom:50px;
        }
        .tableContent table {
            width:100%
        }
        .tableContent table>thead>tr>th {
            text-align:left;
            font-size:13px;
            font-weight:500;
            background:blue;
            color:#fff;
            padding:4px 5px
        }
        .tableContent table>tbody>tr>td {
            font-size:13px;
            padding:4px 5px
        }
        .table-striped tbody tr:nth-of-type(odd) {
              background-color: rgba(0,0,0,.05);
        }
        .table-striped th, .table-striped td{
            border: 1px solid #dee2e6;
        }
        .table-striped th {
            border-color:#0000af
        }
        .clltoActions {
            position:fixed;
            bottom:0;
            left:0;
            width:100%;
            text-align:center;
        
            background: #fff;
        }
        .clltoActions .btn:first-child {
            margin-right:8px
        }
        .btn {
            display: inline-block;
            font-weight: 400;
            text-align: center;
            white-space: nowrap;
            vertical-align: middle;
            -webkit-user-select: none;
            -moz-user-select: none;
            -ms-user-select: none;
            user-select: none;
            border: 1px solid transparent;
            padding: 0.375rem 0.75rem;
            font-size: 1rem;
            line-height: 1.5;
            border-radius: 0.25rem;
            transition: color .15s ease-in-out,background-color .15s ease-in-out,border-color .15s ease-in-out,box-shadow .15s ease-in-out;
            cursor:pointer;
        }
        .btn.done {
            color: #fff;
            background-color: #28a745;
            border-color: #28a745;
        }
        .btn.done:hover {
            color: #fff;
            background-color: #218838;
            border-color: #1e7e34;
        }
        .btn.reject {
            color: #fff;
            background-color: #dc3545;
            border-color: #dc3545;
        }
        .btn.reject:hover {
            color: #fff;
            background-color: #c82333;
            border-color: #bd2130;
        }
        .form-control{
            width:100%
        }
        .inp{
            border:1px solid #C1C1C1;
            padding:3px 10px;
            border-radius:4px;
            display:inline-block;
            margin-bottom:5px
        }
    </style>
    <script>
        function convertDate(date){
            var d= new Date(date)
            console.log(d)
        }
        $(function () {

            var url_string = window.location.href;
            var url = new URL(url_string);
            var id = url.searchParams.get("id");

            //var AU = url.searchParams.get("AU");
            var dbname = url.searchParams.get("UniqueKey");
            $("#indentIdHidden").val(id)
           // console.log(AU);
            var obj = {
                Indent_Id: id,
                dbname: dbname
            }
            var obj2 = {
                Indent_Id: id
            }
            $.ajax({
                type: "POST",
                url: "indentApproval.aspx/GetData",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(obj),
                //async: false,
                success: function (data) {
                    var selectedData = data.d;
                    //console.log('GetData', data)
                    var topData = data.d;
                    convertDate(topData[0].Indent_RequisitionDateTimeFormat)
                    $("#DocumentNo").text(topData[0].Indent_RequisitionNumber);
                    $("#Purpose").text(topData[0].Indent_Purpose);
                    $("#Branch").text(topData[0].Indent_branch);
                    var date = topData[0].Indent_RequisitionDateTimeFormat;
                    var dateAct = date.split(" ");
                    var MDate = dateAct[0];
                    var datearray = MDate.split("/");
                    var newdate = datearray[1] + '/' + datearray[0] + '/' + datearray[2];
                    $("#PostingDate").text(newdate);

                    $.ajax({
                        type: "POST",
                        url: "indentApproval.aspx/GetListData",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(obj2),
                        //async: false,
                        success: function (data) {
                            console.log('GetListData', data)
                            var GetListData = data.d;                       
                            createRows(GetListData)
                        }

                    });
                    HideDoneRej(topData[0].ApprovalSettings);
                    AddRejButtons(topData[0].PurchaseIndent_ApproveStatus)
                }

            });
            
        })
        function AddRejButtons(data) {
            if (data == "Pending Approval") {
                $("#AddRejButtons").show();
                $("btnDone").attr("disabled", false);
                $("btnReject").attr("disabled", false);
            }
            else {
                $("#AddRejButtons").hide();
                $("btnDone").attr("disabled", true);
                $("btnReject").attr("disabled", true);
            }
        }
        function HideDoneRej(data) {
            if (data != "Yes") {
                //alert(data.d)
                $("#AddRejButtons").hide();
                $("btnDone").attr("disabled", true);
                $("btnReject").attr("disabled", true);
            } else {
                $("#AddRejButtons").show();
                $("btnDone").removeAttr("disabled");
                $("btnReject").removeAttr("disabled");
            }
        }

            function createRows(data) {
                console.log("calling", data)
                var tableRow;
                for (i = 0; i < data.length; i++) {
                    tableRow += "<tr>";
                    tableRow += "<td>" + data[i].IndentDetailsId + "</td>"
                    tableRow += "<td>" + data[i].ProductName + "</td>"
                    //Mantis Issue 25241
                    //tableRow += "<td ><input type='number' value='" + parseInt(data[i].Quantity).toFixed(4) + "' style='Width:90px !important' /></td>"
                    tableRow += "<td>" + parseInt(data[i].Quantity).toFixed(4) + "</td>"
                    // End of Mantis Issue 25241
                    tableRow += "<td>" + data[i].UOM + "</td>"
                    tableRow += "</tr>"
                }

            
            $("#problemsTable").html(tableRow);           
            $('#problemsTable td:nth-child(1)').hide();
        }
        


        function DoneProblemTable() {
            //$("#hdnAppRej").val("Done");
            if ($("#appRemarks").val() == "") {
                alert("Please enter remarks.");
                return false;
            }
            else {
                $("#hdnAppRej").val(1);
                TableToArray();
            }
            
            //alert($("#hdnAppRej").val())
        }
        function RejectProblemTable() {
            //$("#hdnAppRej").val("Reject");
            if ($("#appRemarks").val() == "") {
                alert("Please enter remarks.");
                return false;
            }
            else {
                $("#hdnAppRej").val(2);
                TableToArray();
            }
            
            //alert($("#hdnAppRej").val())
        }
        var myTableArray = [];
        function TableToArray() {
            myTableArray = [];
            $("#problemsTable tr").each(function () {
                var arrayOfThisRow = [];
                var tableData = $(this).find('td');
                if (tableData.length > 0) {
                    tableData.each(function () {
                        var cell = $(this).closest('td');
                        var cellIndex = cell[0].cellIndex
                        if (cellIndex==2) {
                            arrayOfThisRow.push($(this).find("input").val());
                        }
                        else {
                            arrayOfThisRow.push($(this).text());
                        }
                    });
                    myTableArray.push(arrayOfThisRow);
                    console.log("myTableArray", myTableArray)
                    //alert(myTableArray.toString())
                    
                }
            });
            DoneRejectSubmit();
        }
        function DoneRejectSubmit() {
            //var actionBox = $(".actionBox").val();
            //if (actionBox == '') {
            //    alert('required')
            //    return;
            //}
            var validate = true;
            var AppRej = $("#hdnAppRej").val();

            //var selection = document.querySelectorAll('.actionBox');
            if (AppRej == 'Done') {
                //selection.forEach(function (item) {
                //    if (item.value == '') {
                //        alert('Service Action cannot be blank.');
                //        validate = false;
                //    }

                //});
            }
            
            if (validate == true) {
                //var AppRej = $("#hdnAppRej").val();
                var url_string = window.location.href;
                var url = new URL(url_string);
                var Indent_Id = url.searchParams.get("id");
                var Indent_ApprovalRemarks = $("#appRemarks").val();
                //rev Pratik
                //var user_id = url.searchParams.get("user_id");
                //End of rev Pratik
                //var strArr = myTableArray.toString();
                //alert(myTableArray)
                //console.log(myTableArray)
                //console.log("A",JSON.stringify({ AppRej: AppRej, ReceiptChallan_ID: RecChallan_id, myTableArray: myTableArray }))
                //var mAr = [{ a: "29", b: "POWER OFF / DEAD", c: "FCBBSCGKB", d: "1" }, { a: "29", b: "POWER OFF / DEAD", c: "FCBBSCGKB", d: "1" }]
               
                $.ajax({
                    type: "POST",
                    url: "indentApproval.aspx/JobStatusUpdate",
                    data: JSON.stringify({ AppRej: AppRej, Indent_ApprovalRemarks: Indent_ApprovalRemarks, udtIndentDetailsAction: myTableArray, Indent_Id: Indent_Id }),
                    //data: JSON.stringify({ AppRej: AppRej, ReceiptChallan_ID: RecChallan_id }),
                    async: false,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        //console.log(response);
                        //alert(response.d)
                        //alert(response)
                        if (response.d == "Success") {
                            //jAlert("Saved Successfully.", "Alert", function () {
                            //    //window.location.href = "serviceDataList.aspx";

                            //});
                            if (AppRej=='Reject')
                                alert("Rejected Successfully.");
                            else
                                alert("Done Successfully.");

                            //var ispopup = url.searchParams.get("UniqueKey");
                          //  if (!ispopup) { window.location.href = "serviceDataList.aspx"; }

                            HideDoneRej();
                            //window.location.reload();
                        }
                        else {
                            //alert(response.d);
                            alert("Done/Reject Failed.");
                            return
                        }
                    },
                    error: function (response) {
                        console.log(response);
                    }
                });
            }
            
        }
        
    </script>
    <style>
        .table-striped>thead>tr>th {
            white-space:nowrap
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" id="hdnAppRej" />
        <input type="hidden" id="appRemarksHidden" />
        <input type="hidden" id="indentIdHidden" />
        <div>
            <div class="box">
                <div>
                    <span class="hdtec">Document No : </span> 
                    <span id="DocumentNo" class="inp"></span>
                </div>
                <div>
                    <span class="hdtec">Posting Date: </span> 
                    <span id="PostingDate" class="inp"></span>
                </div>
                <div>
                    <span class="hdtec">Purpose : </span> 
                    <span id="Purpose" class="inp"></span>
                </div>
                <div>
                    <span class="hdtec">Branch : </span> 
                    <span id="Branch" class="inp"></span>
                </div>
                <div>
                    <span class="hdtec">Approve/Reject Remarks: </span> 
                    <div><textarea id="appRemarks" rows="2" class="form-control"></textarea></div>
                </div>
                
            </div>
            <div class="tableContent">
                <table class="table-striped" cellspacing="0" cellspadding="0">
                    <thead >
                        <tr>
                            <th style="width:70%">Product Name</th>
                            <th style="width:50px !important">Quantity</th>
                            <th>UOM</th>
                        </tr>
                    </thead>
                    <tbody id="problemsTable">
                        <!-- Dynamic table data -->
                       
                        
                    </tbody>
                </table>
            </div>
            <div class="clltoActions" id="AddRejButtons">
                <button type="button" id="btnDone" class="btn done" onclick="DoneProblemTable()">Approve</button>
                <button type="button" id="btnReject" class="btn reject" onclick="RejectProblemTable()">Reject</button>
            </div>
        </div>
    </form>
</body>
</html>
