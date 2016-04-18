/// <reference path="../typings/jquery/jquery.d.ts" />
$(document).ready(() => {
    $(".environment-selection").each(function() {
        var me = $(this);
        var environmentInput = me.find("input[name=environment]");
        var form = environmentInput.closest("form");
        me.find(".environment-option").click(function () {
            environmentInput.val($(this).attr("data-value"));
            form.submit();
        });
    });
});