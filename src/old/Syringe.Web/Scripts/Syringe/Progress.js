/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/signalr/signalr.d.ts" />
/// <reference path="../typings/Hubs.d.ts" />
var Syringe;
(function (Syringe) {
    var Web;
    (function (Web) {
        var Progress = (function () {
            function Progress(signalRUrl) {
                this.signalRUrl = signalRUrl;
            }
            Progress.prototype.monitor = function (taskId) {
                var _this = this;
                if (taskId === 0) {
                    throw Error("Task ID was 0.");
                }
                $.connection.hub.logging = true;
                $.connection.hub.url = this.signalRUrl;
                this.proxy = $.connection.taskMonitorHub;
                this.proxy.client.onTaskCompleted = function (taskInfo) {
                    ++_this.completedCases;
                    console.log("Completed task " + taskInfo.CaseId + " (" + _this.completedCases + " of " + _this.totalCases + ").");
                    if (_this.totalCases > 0) {
                        var percentage = (_this.completedCases / _this.totalCases) * 100;
                        $(".progress-bar").css("width", percentage + "%");
                        $(".progress-bar .sr-only").text(percentage + "% Complete");
                    }
                    var selector = "#case-" + taskInfo.CaseId;
                    var $selector = $(selector);
                    // Url
                    var $urlSelector = $(".case-result-url", $selector);
                    $urlSelector.text(taskInfo.ActualUrl);
                    // Change background color
                    var resultClass = taskInfo.Success ? "panel-success" : "panel-warning";
                    // Exceptions
                    if (taskInfo.ExceptionMessage !== null) {
                        resultClass = "panel-danger";
                        $(".case-result-exception", $selector).removeClass("hidden");
                        $(".case-result-exception textarea", $selector).text(taskInfo.ExceptionMessage);
                    }
                    else {
                        // Show HTML/Raw buttons.
                        $(".view-html", $selector).removeClass("hidden");
                        $(".view-raw", $selector).removeClass("hidden");
                    }
                    $selector.addClass(resultClass);
                };
                $.connection.hub.start()
                    .done(function () {
                    _this.totalCases = 0;
                    _this.completedCases = 0;
                    _this.proxy.server.startMonitoringTask(taskId)
                        .done(function (taskState) {
                        _this.totalCases = taskState.TotalCases;
                        console.log("Started monitoring task " + taskId + ". There are " + taskState.TotalCases + " cases.");
                    });
                });
            };
            return Progress;
        })();
        Web.Progress = Progress;
    })(Web = Syringe.Web || (Syringe.Web = {}));
})(Syringe || (Syringe = {}));
//# sourceMappingURL=Progress.js.map