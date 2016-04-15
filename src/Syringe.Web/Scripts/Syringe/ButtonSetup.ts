/// <reference path="../typings/bootbox.d.ts" />
$(document).ready(function () {
    const rowsToAdd = [
        { $Button: $("#addVerification"), URL: "/Test/AddAssertion", Prefix: "Assertions" },
        { $Button: $("#addParsedItem"), URL: "/Test/AddCapturedVariableItem", Prefix: "CapturedVariables" },
        { $Button: $("#addHeaderItem"), URL: "/Test/AddHeaderItem", Prefix: "Headers" },
        { $Button: $("#addVariableItem"), URL: "/TestFile/AddVariableItem", Prefix: "Variables" }
    ];
    let rowHandler = new RowHandler(rowsToAdd);
    rowHandler.setupButtons();

    $("body").on("click", "#removeRow", function (e) {
        e.preventDefault();
        var formGroup = $(this).closest(".form-group");
        var parentPanelBody = formGroup.closest(".panel-body");
        $(this).closest(".form-group").remove();

        parentPanelBody.find(".form-group").each(function (i, ev) {
            $(ev).find("label").each(function () {
                rowHandler.updateElementValue($(this), i, "for");
            });

            $(ev).find("input, select").each(function () {

                rowHandler.updateElementValue($(this), i, "name");
                rowHandler.updateElementValue($(this), i, "id");

            });

        });

    });

    $(".delete-button").on("click", function () {
        var success = false;
        bootbox.confirm("Are you sure you want delete?", function (result) {
            success = result;
        });

        return success;
    });
});