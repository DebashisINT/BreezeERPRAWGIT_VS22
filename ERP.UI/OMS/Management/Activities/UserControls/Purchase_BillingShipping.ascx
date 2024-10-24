﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Purchase_BillingShipping.ascx.cs" Inherits="ERP.OMS.Management.Activities.UserControls.Purchase_BillingShipping" %>


<style>
    .headText {
    margin: 0;
    padding: 5px 15px 5px;
    background: #0177c1;
    margin-bottom: 5px;
    color: #fff;
    font-size: 12px !important;
    border-radius: 12px;
}
    #DivBilling [class^="col-md"], #DivShipping [class^="col-md"] {
    padding-top: 2px;
    padding-bottom: 2px;
}
    .lowSize {
    font-size: 12px !important;
}

    .mandt {
        position: absolute;
        right: -18px;
        top: 4px;
    }

</style>
<%--D:\BreeZeErp 6-04-2020\BreezeERP-GIT\ERP.UI\OMS\Management\Activities\JS\PurchaseBillingShipping.js--%>
<script src="/OMS/Management/Activities/JS/PurchaseBillingShipping.js?v2.0"></script>
<div>
    <table>
        <tr>
            <td></td>
            <td></td>
        </tr>
    </table>
    <div class="row">
        <div class="col-md-6 mbot5" id="DivBilling">
            <h5 class="headText" >Our Billing Address</h5>
            <div class="clearfix" style="background: #fff; border: 1px solid #ccc; padding: 0px 0 10px 0;">
                
                <div style="padding-right: 8px">
                   <div class="col-md-4" style="height: auto;">
                        <asp:Label ID="LblType" runat="server" Text="Select Address:" CssClass="newLbl"></asp:Label>
                    </div>
                    <div class="col-md-8">
                        <dxe:ASPxButtonEdit ID="ASPxButtonEdit1" runat="server" ReadOnly="true" ClientInstanceName="txtSelectBillingAdd" Width="100%">
                            <Buttons>
                                <dxe:EditButton>
                                </dxe:EditButton>
                            </Buttons>
                            <ClientSideEvents ButtonClick="function(s,e) {SelectBillingAddClick(s,e);}" KeyDown="function(s,e){SelectBillingKeyDown(s,e);}" />
                        </dxe:ASPxButtonEdit>
                    </div> 
                    <div class="clear"></div>

                     <div class="col-md-4" style="height: auto; margin-bottom: 5px;" runat="server" id="dvlblcntperson">
                        Contact Person: 
                    </div>
                    <div class="col-md-8" runat="server" id="dvtxtCntperson">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtCntperson" MaxLength="80" ClientInstanceName="ctxtCntperson"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                           
                        </div>
                    </div>
                    <div class="clear"></div>

                    <div class="col-md-4" style="height: auto; margin-bottom: 5px;">
                        Address1: <span style="color: red;">*</span>
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtAddress1" MaxLength="80" ClientInstanceName="ctxtAddress1"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="badd1" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="col-md-4" style="height: auto; margin-bottom: 5px;">
                        <%--Code--%>
                                                                            Address2:
                                                                           

                    </div>
                    <%--Start of Address2 --%>
                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtAddress2" MaxLength="80" ClientInstanceName="ctxtAddress2"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="col-md-4" style="height: auto; margin-bottom: 5px;">
                        Address3: 
                    </div>
                    <%--Start of Address3 --%>

                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtAddress3" MaxLength="80" ClientInstanceName="ctxtAddress3"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <%--Start of Landmark --%>
                    <div class="col-md-4" style="height: auto; margin-bottom: 5px;">
                        <%--Code--%>
                                                                            Landmark:
                                                                             

                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtlandmark" MaxLength="80" ClientInstanceName="ctxtlandmark"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                        </div>
                    </div>
                    <div class="clear"></div>

                    <%--  //chinmoy add Phone field start --%>

                     <div class="col-md-4" style="height: auto; margin-bottom: 5px;" runat="server" id="dvlblPhone">

                        <asp:Label ID="lblPhone" runat="server" Text="Phone:" CssClass="newLbl"></asp:Label>

                      <%-- <span style="color: red;">*</span>--%>
                    </div>
                    <div class="col-md-8" runat="server" id="dvtxtPhone">

                        <div class="Left_Content relative">
                             <input type="text" id="Salesbillingphone" runat="server"  maxlength="10" onkeypress="javascript:return isNumber(event)"/>
                           <%-- <dxe:ASPxTextBox ID="txtPhone" MaxLength="80" ClientInstanceName="ctxtPhone"
                                runat="server" Width="100%">
                                  <MaskSettings Mask="&lt;-0..9999999999&gt;" AllowMouseWheel="false" />
                            </dxe:ASPxTextBox>--%>
                          <%--  <span id="billingPhone" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>--%>
                        </div>
                    </div>
                    <div class="clear"></div>

                    <%--//End--%>
                       <%--  //chinmoy add Phone field start --%>

                     <div class="col-md-4" style="height: auto; margin-bottom: 5px;" runat="server" id="dvlblemail">

                        <asp:Label ID="lblEmail" runat="server" Text="Email:" CssClass="newLbl"></asp:Label>

                       
                    </div>
                    <div class="col-md-8" runat="server" id="dvtxtemail">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtEmail" MaxLength="80" ClientInstanceName="ctxtEmail"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="billingEmail" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <div class="clear"></div>

                    <%--//End--%>


                    <%--start of Pin/Zip.--%>
                    <div class="col-md-4" style="height: auto;">
                        <%--Type--%>
                        <asp:Label ID="Label8" runat="server" Text="Pin/Zip (6 Characters):" CssClass="newLbl"></asp:Label><span style="color: red;">*</span>
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">

                            <dxe:ASPxTextBox ID="txtbillingPin" ClientInstanceName="ctxtbillingPin"
                                runat="server" Width="100%">
                                <%--<MaskSettings Mask="&lt;-0..999999&gt;" AllowMouseWheel="false" />--%>
                                <ClientSideEvents LostFocus="BillingPinChange" GotFocus="BillingPinGotFocus" />
                            </dxe:ASPxTextBox>
                            <asp:HiddenField ID="hdBillingPin" runat="server"></asp:HiddenField>
                             <asp:HiddenField ID="hdOldBillingPinCode"  runat="server" />

                            <span id="bpin" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>

                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="col-md-4" style="height: auto;">
                        <%--Type--%>
                        <asp:Label ID="Label2" runat="server" Text="Country:" CssClass="newLbl"></asp:Label><span style="color: red;">*</span>
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtbillingCountry" ClientEnabled="false" MaxLength="80" ClientInstanceName="ctxtbillingCountry"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="billingcountry" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <%--End of State--%>
                    <div class="col-md-4" style="height: auto;">
                        <%--Type--%>
                        <asp:Label ID="Label4" runat="server" Text="State:" CssClass="newLbl"></asp:Label><span style="color: red;">*</span>
                    </div>
                    <div class="col-md-8">
                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtbillingState" ClientEnabled="false" MaxLength="80" ClientInstanceName="ctxtbillingState"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="billingstate" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>

                    </div>
                    <div class="clear"></div>
                    <%--start of City/district.--%>
                    <div class="col-md-4" style="height: auto;">
                        <asp:Label ID="Label6" runat="server" Text="District:" CssClass="newLbl"></asp:Label><span style="color: red;">*</span>
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtbillingCity" ClientEnabled="false" MaxLength="80" ClientInstanceName="ctxtbillingCity"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="billingcity" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>

                    </div>
                    <div class="clear"></div>
                    <%--start of Area--%>
                    <div class="col-md-4" style="height: auto;">
                        <%--Type--%>
                        <asp:Label ID="Label10" runat="server" Text="Area:" CssClass="newLbl"></asp:Label>
                    </div>
                    <div class="col-md-8">
                        <dxe:ASPxButtonEdit ID="txtSelectBillingArea" runat="server" ReadOnly="true" ClientInstanceName="ctxtSelectBillingArea" Width="100%">
                            <Buttons>
                                <dxe:EditButton>
                                </dxe:EditButton>
                            </Buttons>
                            <ClientSideEvents ButtonClick="function(s,e){SelectBillingAreaClick();}" KeyDown="function(s,e){SelectBillingAreaKeyDown(s,e);}" />
                        </dxe:ASPxButtonEdit>
                    </div>
                    
                    <div class="clear"></div>
                    <div id="BillingGstDiv">
                        <div class="col-md-4" style="height: auto;">
                            <asp:Label ID="LabelBillingGst" runat="server" Text="GSTIN:" CssClass="newLbl"></asp:Label>
                        </div>
                        <div class="col-md-8">
                            <div class="Left_Content">
                                <ul class="nestedinput">
                                    <li>
                                        <dxe:ASPxTextBox ID="txtBillingGSTIN1" ClientInstanceName="ctxtBillingGSTIN1" MaxLength="2" runat="server" Width="33px" ReadOnly="true">
                                        </dxe:ASPxTextBox>
                                    </li>
                                    <li class="dash">-</li>
                                    <li>
                                        <dxe:ASPxTextBox ID="txtBillingGSTIN2" ClientInstanceName="ctxtBillingGSTIN2" MaxLength="10" runat="server" Width="90px" ReadOnly="true">
                                        </dxe:ASPxTextBox>
                                    </li>
                                    <li class="dash">-</li>
                                    <li>
                                        <dxe:ASPxTextBox ID="txtBillingGSTIN3" ClientInstanceName="ctxtBillingGSTIN3" MaxLength="3" runat="server" Width="50px" ReadOnly="true">
                                        </dxe:ASPxTextBox>
                                    </li>
                                </ul>
                            </div>
                        </div>
                        <div class="clear"></div>
                    </div>



                    <div class="Left_Content relative">
                        <asp:HiddenField ID="hdStateIdBilling" runat="server" />

                    </div>




                    <div class="Left_Content relative">
                        <asp:HiddenField ID="hdStateCodeBilling" runat="server" />

                    </div>




                    <div class="Left_Content relative">
                        <asp:HiddenField ID="hdCountryIdBilling" runat="server" />

                    </div>

                    <div class="Left_Content relative">
                        <asp:HiddenField ID="hdCityIdBilling" runat="server" />

                    </div>
                    <div class="Left_Content relative">
                        <asp:HiddenField ID="hdAreaIdBilling" runat="server" />

                    </div>
                    <div class="clear"></div>
                    <div id="lblShipToSame" class="col-md-12" style="height: auto;">

                        <a class="[ form-group ]" id="shiptosame" href="javascript:void(0)" onclick="javascript: BillingCheckChange()">
                            <div class="[ btn-group ]">
                                <span for="fancy-checkbox-success" class="[ btn btn-primary active lowSize ]">
                                    Ship To Same Address >>
                                </span>
                            </div>
                        </a>

                    </div>





                </div>
            </div>
        </div>


        <div class="col-md-6 mbot5" id="DivShipping">
            <h5 class="headText" >Our Shipping Address</h5>
            <div class="clearfix" style="background: #fff; border: 1px solid #ccc; padding: 0px 0 10px 0;">

                
                <div style="padding-right: 8px">
                   <div id="divShipToParty" runat="server">
                    <div class="col-md-4" style="height: auto;">
                        <label>Ship to party:</label>
                    </div>
                    <div class="col-md-8">
                        <dxe:ASPxButtonEdit ID="txtShipToPartyShippingAdd" runat="server" ReadOnly="true" ClientInstanceName="ctxtShipToPartyShippingAdd" Width="100%">
                            <Buttons>
                                <dxe:EditButton>
                                </dxe:EditButton>
                            </Buttons>
                            <ClientSideEvents ButtonClick="function(s,e){ShipToPartyAddClick();}" KeyDown="function(s,e){ShipToPartyAddkeydown(s,e);}" />
                        </dxe:ASPxButtonEdit>
                          <span id="shipToParty" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                    </div>
                    </div>
                    <asp:HiddenField ID="hdShipToParty" runat="server" />
                    <asp:Hiddenfield ID="hdIsShiptopartyVissible" runat="server" />



                    <div class="col-md-4" style="height: auto;">
                        <label>Select Address:</label>
                    </div>
                    <div class="col-md-8">
                        <dxe:ASPxButtonEdit ID="txtSelectShippingAdd" runat="server" ReadOnly="true" ClientInstanceName="ctxtSelectShippingAdd" Width="100%">
                            <Buttons>
                                <dxe:EditButton>
                                </dxe:EditButton>
                            </Buttons>
                            <ClientSideEvents ButtonClick="function(s,e){SelectShippingAddClick(s,e);}" KeyDown="function(s,e){SelectShippingAddKeyDown(s,e);}" />
                        </dxe:ASPxButtonEdit>
                    </div>
                    <div class="clear"></div>

                     <div class="col-md-4" style="height: auto; margin-bottom: 5px;" runat="server" id="dvshiplblCntPerson">
                        Contact Person: 
                    </div>
                    <div class="col-md-8" runat="server" id="dvshiptxtCntPerson">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtshipCntPerson" MaxLength="80" ClientInstanceName="ctxtshipCntPerson"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                           
                        </div>
                    </div>
                    <div class="clear"></div>

                  <div class="col-md-4" style="height: auto; margin-bottom: 5px;">
                        Address1: <span style="color: red;">*</span>

                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtsAddress1" MaxLength="80" ClientInstanceName="ctxtsAddress1"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="sadd1" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="col-md-4" style="height: auto; margin-bottom: 5px;">
                        Address2:
                                                                           
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content">
                            <dxe:ASPxTextBox ID="txtsAddress2" MaxLength="80" ClientInstanceName="ctxtsAddress2"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="col-md-4" style="height: auto; margin-bottom: 5px;">
                        Address3: 
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content">
                            <dxe:ASPxTextBox ID="txtsAddress3" MaxLength="80" ClientInstanceName="ctxtsAddress3"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="col-md-4" style="height: auto; margin-bottom: 5px;">
                        Landmark: 
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content">
                            <dxe:ASPxTextBox ID="txtslandmark" MaxLength="80" ClientInstanceName="ctxtslandmark"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <%--  //chinmoy add Phone field start --%>

                     <div class="col-md-4" style="height: auto; margin-bottom: 5px;" runat="server" id="dvshiplblPhone">

                        <asp:Label ID="lblshiipPhone" runat="server" Text="Phone:" CssClass="newLbl"></asp:Label>

                      <%-- <span style="color: red;">*</span>--%>
                    </div>
                    <div class="col-md-8" runat="server" id="dvshiptxtPhone">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtshipPhone" MaxLength="80" ClientInstanceName="ctxtshipPhone"
                                runat="server" Width="100%">
                                  <MaskSettings Mask="&lt;-0..9999999999&gt;" AllowMouseWheel="false" />
                            </dxe:ASPxTextBox>
                            <span id="ShippingPhone" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <div class="clear"></div>

                    <%--//End--%>
                       <%--  //chinmoy add Phone field start --%>

                     <div class="col-md-4" style="height: auto; margin-bottom: 5px;" runat="server" id="dvshiplblemail">

                        <asp:Label ID="lbl" runat="server" Text="Email:" CssClass="newLbl"></asp:Label>

                       
                    </div>
                    <div class="col-md-8" runat="server" id="dvshiptxtemail">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtshipEmail" MaxLength="80" ClientInstanceName="ctxtshipEmail"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="shippingEmail" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <div class="clear"></div>

                    <%--//End--%>

                    <div class="col-md-4" style="height: auto;">
                        <%--Type--%>
                        <asp:Label ID="Label9" runat="server" Text="Pin/Zip (6 Characters):" CssClass="newLbl"></asp:Label><span style="color: red;">*</span>
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">

                            <dxe:ASPxTextBox ID="txtShippingPin" MaxLength="20" ClientInstanceName="ctxtShippingPin"
                                runat="server" Width="100%">
                                <%--<MaskSettings Mask="&lt;-0..999999&gt;" AllowMouseWheel="false" />--%>
                                <ClientSideEvents LostFocus="ShippingPinChange" GotFocus="ShippingPinGotFocus"  />
                            </dxe:ASPxTextBox>
                            <asp:HiddenField ID="hdShippingPin" runat="server"></asp:HiddenField>
                            <asp:HiddenField ID="hdOldShippingPinId" runat="server" />
                            <asp:HiddenField ID="hdOldShippingPinCode"  runat="server" />


                            <span id="spin" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <%--End of Pin/Zip.--%>




                    <div class="col-md-4" style="height: auto;">

                        <asp:Label ID="Label3" runat="server" Text="Country:" CssClass="newLbl"></asp:Label><span style="color: red;">*</span>
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtshippingCountry" ClientEnabled="false" MaxLength="80" ClientInstanceName="ctxtshippingCountry"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="shippingcountry" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <%--End of Country--%>
                    <div class="col-md-4" style="height: auto;">
                        <%--Type--%>
                        <asp:Label ID="Label5" runat="server" Text="State:" CssClass="newLbl"></asp:Label><span style="color: red;">*</span>
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtshippingState" ClientEnabled="false" MaxLength="80" ClientInstanceName="ctxtshippingState"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="shippingstate" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <div class="clear"></div>

                    <%--End of State--%>
                    <div class="col-md-4" style="height: auto;">
                        <%--Type--%>
                        <asp:Label ID="Label7" runat="server" Text="District:" CssClass="newLbl"></asp:Label><span style="color: red;">*</span>
                    </div>
                    <div class="col-md-8">

                        <div class="Left_Content relative">
                            <dxe:ASPxTextBox ID="txtshippingCity" ClientEnabled="false" MaxLength="80" ClientInstanceName="ctxtshippingCity"
                                runat="server" Width="100%">
                            </dxe:ASPxTextBox>
                            <span id="shippingcity" style="display: none" class="mandt">
                                <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                            </span>
                        </div>
                    </div>
                    <%--End of City/District--%>
                    <div class="clear"></div>
                    <div class="col-md-4" style="height: auto;">
                        <%--Type--%>
                        <asp:Label ID="Label11" runat="server" Text="Area:" CssClass="newLbl"></asp:Label>
                    </div>
                    <div class="col-md-8">
                        <dxe:ASPxButtonEdit ID="txtSelectShippingArea" runat="server" ReadOnly="true" ClientInstanceName="ctxtSelectShippingArea" Width="100%">
                            <Buttons>
                                <dxe:EditButton>
                                </dxe:EditButton>
                            </Buttons>
                            <ClientSideEvents ButtonClick="function(s,e){SelectShippingAreaClick();}" KeyDown="function(s,e){SelectShippingAreaKeyDown(s,e);}" />
                        </dxe:ASPxButtonEdit>
                    </div>


                    
                    <div class="clear"></div>
                    <div id="ShippingGstDiv">
                        <div class="col-md-4" style="height: auto;">
                            <asp:Label ID="LabelGST" runat="server" Text="GSTIN:" CssClass="newLbl"></asp:Label>
                        </div>
                        <div class="col-md-8">
                            <div class="Left_Content">
                                <ul class="nestedinput">
                                    <li>
                                        <dxe:ASPxTextBox ID="txtShippingGSTIN1" ClientInstanceName="ctxtShippingGSTIN1" MaxLength="2" runat="server" Width="33px" ReadOnly="true">
                                        </dxe:ASPxTextBox>
                                    </li>
                                    <li class="dash">-</li>
                                    <li>
                                        <dxe:ASPxTextBox ID="txtShippingGSTIN2" ClientInstanceName="ctxtShippingGSTIN2" MaxLength="10" runat="server" Width="90px" ReadOnly="true">
                                        </dxe:ASPxTextBox>
                                    </li>
                                    <li class="dash">-</li>
                                    <li>
                                        <dxe:ASPxTextBox ID="txtShippingGSTIN3" ClientInstanceName="ctxtShippingGSTIN3" MaxLength="3" runat="server" Width="50px" ReadOnly="true">
                                        </dxe:ASPxTextBox>
                                    </li>
                                </ul>
                              
                            </div>
                        </div>
                        <div class="clear"></div>
                    </div>

                    <div class="Left_Content relative">
                        <asp:HiddenField ID="hdStateCodeShipping" runat="server" />
                    </div>



                    <div class="Left_Content relative">
                        <asp:HiddenField ID="hdCountryIdShipping" runat="server" />

                    </div>
                         <div class="Left_Content relative">
                        <asp:HiddenField ID="hfVendorGSTIN" runat="server" />
                          </div>


               
                        <asp:HiddenField ID="hdStateIdShipping" runat="server" />

                    </div>
                    <div class="Left_Content relative">
                        <asp:HiddenField ID="hdCityIdShipping" runat="server" />

                    </div>
                    <div class="Left_Content relative">
                        <asp:HiddenField ID="hdAreaIdShipping" runat="server" />

                    </div>



                    <div class="clear"></div>
                    <div id="lblBillToSame" class="col-md-12" style="height: auto;">


                        <a class="[ form-group ]" id="billtoSame" href="javascript:void(0)" onclick="javascript: ShippingCheckChange()">
                            <div class="[ btn-group ]">

                                <span for="fancy-checkbox-successShipping" class="[ btn btn-primary active lowSize  ]">
                                    << Bill To Same Address
                                </span>
                            </div>
                        </a>


                    </div>




                </div>

            </div>
        </div>
    </div>
    <%--End of Address Type--%>




    <%--End of Area--%>


    <div class="clear"></div>
      <div id="dvBillShipMessage" class="col-md-4 pdLeft0" style="display:none;">
    <label id="BillShipMessage" style="color:red;"><b>Cannot modify billing/shipping as the tax already been calculated</b></label>
   </div>
     <div class="clear"></div>
    <div class="col-md-3 pdLeft0" style="padding-top: 10px">
        <%-- <button class="btn btn-primary">OK</button> ValidationGroup="Address"--%>

        <dxe:ASPxButton ID="btnSave_SalesBillingShiping" ClientInstanceName="cbtnSave_SalesBillingShiping" runat="server"
            AutoPostBack="False" Text="OK" CssClass="btn btn-primary" UseSubmitBehavior="false">
            <ClientSideEvents Click="function (s, e) {ValidationBillingShipping();}" />
        </dxe:ASPxButton>

    </div>
      


 

<!--Product Modal -->
<div class="modal fade" id="billingAreaModel" role="dialog">
    <div class="modal-dialog">

        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Select Area</h4>
            </div>
            <div class="modal-body">
                <input type="text" onkeydown="areakeydown(event)" id="txtbillingArea" autofocus width="100%" placeholder="Search By Area Name." />

                <div id="billingAreatable">
                    <table border='1' width="100%" class="dynamicPopupTbl">
                        <tr class="HeaderStyle">
                            <th class="hide">id</th>
                            <th>Area</th>
                        </tr>
                    </table>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>

    </div>
</div>

<div class="modal fade" id="shippingAreaModel" role="dialog">
    <div class="modal-dialog">

        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Select Area</h4>
            </div>
            <div class="modal-body">
                <input type="text" onkeydown="areakeydownshipping(event)" id="txtshippingArea" autofocus width="100%" placeholder="Search By Area Name." />

                <div id="shippingAreatable">
                    <table border='1' width="100%" class="dynamicPopupTbl">
                        <tr class="HeaderStyle">
                            <th class="hide">id</th>
                            <th>Area</th>
                        </tr>
                    </table>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>

    </div>
</div>

<div class="modal fade" id="ShiptoPartyModel" role="dialog">
    <div class="modal-dialog">

        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Select Address</h4>
            </div>
            <div class="modal-body">
                <div id="ShiptoPartyModelTable">
                    <table border='1' width="100%" class="dynamicPopupTbl">
                        <tr class="HeaderStyle">
                            <th class="hide">Id</th>
                            <th>Address</th>
                            <th style="width:150px;">Address Type</th>
                          
                        </tr>
                    </table>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>

    </div>
</div>


<div class="modal fade" id="ShippingShipToPartyModel" role="dialog">
    <div class="modal-dialog">

        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Ship to Party</h4>
            </div>
            <div class="modal-body">
                <input type="text" onkeydown="ShipToPartykeydown(event)" id="txtshippingShipToParty" autofocus width="100%" placeholder="Search By Customer Name." />

                <div id="ShippingShipToPartytable">
                    <table border='1' width="100%" class="dynamicPopupTbl">
                        <tr class="HeaderStyle">
                            <th class="hide">Id</th>
                            <th>Customer Name</th>
                            <th>Unique Id</th>
                            <th>Address</th>
                        </tr>
                    </table>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>

    </div>
</div>
<div class="modal fade" id="billingAddressModel" role="dialog">
    <div class="modal-dialog">

        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Select Address</h4>
            </div>
            <div class="modal-body">
                <div id="AddressTableQuotation">
                    <table border='1' width="100%" class="dynamicPopupTbl">
                        <tr class="HeaderStyle">
                            <th class="hide">Id</th>
                            <th>Address</th>
                            <th style="width:150px;">Address Type</th>
                           
                        </tr>
                    </table>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>

    </div>
</div>
