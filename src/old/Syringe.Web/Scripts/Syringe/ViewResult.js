/// <reference path="../typings/jquery/jquery.d.ts" />
var Syringe;
(function (Syringe) {
    var Web;
    (function (Web) {
        var ViewResult = (function () {
            function ViewResult() {
                this.init();
            }
            ViewResult.prototype.init = function () {
                $("#tests-passed-count").on("click", function () {
                    $.each($(".test-failed"), function (count, item) {
                        $(item).hide(250);
                    });
                    $.each($(".test-passed"), function (count, item) {
                        $(item).show(400);
                    });
                });
                $("#all-tests-count").on("click", function () {
                    $.each($(".test-failed, .test-passed"), function (count, item) {
                        $(item).show(400);
                    });
                });
                $("#tests-failed-count").on("click", function () {
                    $.each($(".test-passed"), function (count, item) {
                        $(item).hide(250);
                    });
                    $.each($(".test-failed"), function (count, item) {
                        $(item).show(400);
                    });
                });
            };
            return ViewResult;
        })();
        Web.ViewResult = ViewResult;
    })(Web = Syringe.Web || (Syringe.Web = {}));
})(Syringe || (Syringe = {}));
new Syringe.Web.ViewResult();
//# sourceMappingURL=ViewResult.js.map