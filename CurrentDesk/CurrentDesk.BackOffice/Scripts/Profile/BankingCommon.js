/*************************************************************************************
*File Name     : BankingCommon.js
*Author        : Chinmoy Dey
*Copyright     : Mindfire Solutions
*Date          : 1st Feb 2013
*Modified Date : 1st Feb 2013
*Description   : This file contains common banking related edit functionalities
*************************************************************************************/


//Reseting Bank Information
function resetBankInfo(bankID) {

    var attrib = '#' + bankID;

    $(attrib + 'txtBankName').val($(attrib + '-lblBankName').html());
    $(attrib + 'txtAccountNumber').val($(attrib + '-lblAccountNumber').html());
    $(attrib + 'txtBicSwiftCode').val($(attrib + '-lblBicOrSwiftCode').html());
    $(attrib + 'txtBankAddressLine1').val($(attrib + '-lblBankAddLine1').html());
    $(attrib + 'txtBankAddressLine2').val($(attrib + '-lblBankAddLine2').html());
    $(attrib + "drpReceivingBankInfoId option:contains(" + $(attrib + '-lblReceivingBankInfoId').html() + ")").attr('selected', true);
    $(attrib + 'txtReceivingBankInfo').val($(attrib + '-lblReceivingBankInfo').html());
    $(attrib + 'txtBankCity').val($(attrib + '-lblBankCity').html());
    $(attrib + "drpBankCountry option:contains(" + $(attrib + '-lblBankCountry').html() + ")").attr('selected', true);
    $(attrib + 'txtBankPostalCode').val($(attrib + '-lblBankPostalCode').html());

    $("." + bankID + "chzn-select").chosen();
    $("." + bankID + "bankSelect").chosen();
}


//Function to check valid new bank infos and display errors
function checkValidNewBankInfoValues() {
    //Remove previous errors
    $('.requiredError').remove();
    var valid = true;

    if ($('#Modal-BankName').val() == '') {
        $('#Modal-BankName').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#Modal-AccountNum').val() == '') {
        $('#Modal-AccountNum').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#Modal-Swift').val() == '') {
        $('#Modal-Swift').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#Modal-ReceivingBankInfo').val() == '') {
        $('#Modal-ReceivingBankInfo').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#Modal-Address').val() == '') {
        $('#Modal-Address').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#Modal-City').val() == '') {
        $('#Modal-City').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#Modal-Zip').val() == '') {
        $('#Modal-Zip').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#drpNewRecievingBankInfo').val() == 0 || $('#drpNewRecievingBankInfo').val() == undefined) {
        $('#drpNewRecievingBankInfo').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#drpNewBankCountry').val() == 0 || $('#drpNewBankCountry').val() == undefined) {
        $('#drpNewBankCountry').after('<span class="requiredError">*</span>');
        valid = false;
    }
    return valid;
}

//Function to check valid edit bank infos and give required errors
function checkValidEditBankInfoValues(bankID) {
    //Remove previous errors
    $('.requiredError').remove();
    var valid = true;

    if ($('#' + bankID + 'txtBankName').val() == '') {
        $('#' + bankID + 'txtBankName').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#' + bankID + 'txtAccountNumber').val() == '') {
        $('#' + bankID + 'txtAccountNumber').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#' + bankID + 'txtBicSwiftCode').val() == '') {
        $('#' + bankID + 'txtBicSwiftCode').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#' + bankID + 'txtReceivingBankInfo').val() == '') {
        $('#' + bankID + 'txtReceivingBankInfo').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#' + bankID + 'txtBankAddressLine1').val() == '') {
        $('#' + bankID + 'txtBankAddressLine1').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#' + bankID + 'txtBankCity').val() == '') {
        $('#' + bankID + 'txtBankCity').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#' + bankID + 'txtBankPostalCode').val() == '') {
        $('#' + bankID + 'txtBankPostalCode').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#' + bankID + 'drpReceivingBankInfoId').val() == 0 || $('#' + bankID + 'drpReceivingBankInfoId').val() == undefined) {
        $('#' + bankID + 'drpReceivingBankInfoId').after('<span class="requiredError">*</span>');
        valid = false;
    }
    if ($('#' + bankID + 'drpBankCountry').val() == 0 || $('#' + bankID + 'drpBankCountry').val() == undefined) {
        $('#' + bankID + 'drpBankCountry').after('<span class="requiredError">*</span>');
        valid = false;
    }
    return valid;
}

//Handling Image When Image is Missing
function imgError(image) {
    image.onerror = "";
    image.src = "../Images/avatar.png";
    return true;
}