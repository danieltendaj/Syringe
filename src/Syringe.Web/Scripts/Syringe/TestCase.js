(function () {
    function setupButtons() {
        $("#addVerification").click(function (e) {
            e.preventDefault();
            var verificationItem = {
                Description: $("#description").val(),
                Regex: $("#regex").val(),
                VerifyType: $("#verifyType").val()
            };
            $.get("/TestCase/AddVerification", verificationItem, function (data) {
                $("#addVerification").closest('.form-group').before(data);
                $("#description").val('');
                $("#regex").val('');
            });
        });
        $("#addParsedItem").click(function (e) {
            e.preventDefault();
            var parsedResponseItem = {
                Description: $("#parsedDescription").val(),
                Regex: $("#parsedRegex").val(),
            };
            $.get("/TestCase/AddParsedResponseItem", parsedResponseItem, function (data) {
                $("#addParsedItem").closest('.form-group').before(data);
                $("#description").val('');
                $("#regex").val('');
            });
        });
        $("body").on("click", "#removeRow", function (e) {
            e.preventDefault();
            $(this).closest('.form-group').remove();
        });
    }
    $(document).ready(function () {
        setupButtons();
    });
}());
//# sourceMappingURL=Testcase.js.map