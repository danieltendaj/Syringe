/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/signalr/signalr.d.ts" />
/// <reference path="../typings/Hubs.d.ts" />
module Syringe.Web {
    export class Progress {
        private proxy: Service.Api.Hubs.TaskMonitorHub;
        private signalRUrl: string;
        private totalTests: number;
        private completedTests: number;

        constructor(signalRUrl: string) {
            this.signalRUrl = signalRUrl;
        }

        monitor(taskId: number) {
            if (taskId === 0) {
                throw Error("Task ID was 0.");
            }

            $.connection.hub.logging = true;
            $.connection.hub.url = this.signalRUrl;

            this.proxy = $.connection.taskMonitorHub;

            this.proxy.client.onTestFileGuid = (guid: string) => {
                if (guid) {
                    window.location.href = `/Results/ViewResult/${guid}`;
                }
            };

            this.proxy.client.onTaskCompleted = (taskInfo: Service.Api.Hubs.CompletedTaskInfo) => {
                ++this.completedTests;

                console.log(`Completed task ${taskInfo.Position} (${this.completedTests} of ${this.totalTests}).`);

                if (this.totalTests > 0) {
                    var percentage = (this.completedTests / this.totalTests) * 100;
                    $(".progress-bar").css("width", percentage + "%");
                    $(".progress-bar .sr-only").text(`${percentage}% Complete`);
                }

                var selector = `#test-${taskInfo.Position}`;
                var $selector = $(selector);

                // Url
                var $urlSelector = $(".test-result-url", $selector);
                $urlSelector.html("<a target='_blank' href='" + taskInfo.ActualUrl + "'>" + taskInfo.ActualUrl + "</a>");

                // Change background color
                var resultClass = taskInfo.Success ? "panel-success" : "panel-danger";

                // Exceptions
                if (taskInfo.ExceptionMessage !== null) {
                    $(".test-result-exception", $selector).removeClass("hidden");
                    $(".test-result-exception pre", $selector).text(taskInfo.ExceptionMessage);
                    $("table tr.result-row", $selector).addClass("warning");
                }
                else {
                    // Show HTML/Raw buttons.
                    $(".view-html", $selector).removeClass("hidden");
                    $(".view-raw", $selector).removeClass("hidden");

                    for (var i = 0; i < taskInfo.Verifications.length; i++) {

                        var verificationItemClass = taskInfo.Verifications[i].Success ? "success" : "danger";

                        $("table tr.result-row:eq(" + i + ")", $selector).addClass(verificationItemClass);
                    }
                }



                $selector.addClass(resultClass);
            };

            $.connection.hub.start()
                .done(() => {
                    this.totalTests = 0;
                    this.completedTests = 0;
                    this.proxy.server.startMonitoringTask(taskId)
                        .done(taskState => {
                            this.totalTests = taskState.TotalTests;
                            console.log(`Started monitoring task ${taskId}. There are ${taskState.TotalTests} tests.`);
                        });
                });
        }
    }
}