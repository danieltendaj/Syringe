/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/signalr/signalr.d.ts" />
/// <reference path="../../typings/Hubs.d.ts" />
module Syringe.Web {
    export class Progress {
        private proxy: Syringe.Service.Controllers.Hubs.TaskMonitorHub;
        private signalRUrl: string;
        private totalTests: number;
        private completedTests: number;

        constructor(signalRUrl: string) {
            this.signalRUrl = signalRUrl;
        }

        poll(taskId: number) {
            var that = this;

            setInterval(() =>
            {
                $.getJSON("/Json/PollTaskStatus",
                    { "taskId": taskId },
                    function (data, textStatus, jqXHR)
                    {
                        console.log(data);

                        if (data.IsFinished === true) {
                            window.location.href = `/Results/ViewResult/${data.ResultGuid}`;
                        }

                        if (data.TotalTests > 0) {
                            var percentage = (data.CurrentItem / data.TotalTests) * 100;
                            $(".progress-bar").css("width", percentage + "%");
                            $(".progress-bar .sr-only").text(`${percentage}% Complete`);
                        }

                    });
            },
            500);
        }


        monitor(taskId: number) {
            if (taskId === 0) {
                throw Error("Task ID was 0.");
            }

            $.connection.logging = false;
            $.connection.hub.url = this.signalRUrl;

            this.proxy = $.connection.taskMonitorHub;

            $.connection.hub.start().done(() =>
            {
                this.totalTests = 0;
                this.completedTests = 0;

                this.proxy.server.startMonitoringTask(taskId).done(taskState =>
                {
                    this.totalTests = taskState.TotalTests;
                    console.log(`Started monitoring task ${taskId}. There are ${taskState.TotalTests} tests.`);
                });
            });

            this.proxy.client.onTestFileGuid = (guid: string) => {
                if (guid) {
                    //window.location.href = `/Results/ViewResult/${guid}`;
                }
            };

            this.proxy.client.onTaskCompleted = (taskInfo: Syringe.Service.Controllers.Hubs.CompletedTaskInfo) => {
                ++this.completedTests;

                console.log(`Completed task ${taskInfo.Position} (${this.completedTests} of ${this.totalTests}).`);

                if (this.totalTests > 0) {
                    var percentage = (this.completedTests / this.totalTests) * 100;
                    $(".progress-bar").css("width", percentage + "%");
                    $(".progress-bar .sr-only").text(`${percentage}% Complete`);
                }

                var selector = `#test-${taskInfo.Position}`;
                var $selector = $(selector);

                // Change background color
                var resultClass = taskInfo.Success ? "panel-success" : "panel-danger";
                $selector.addClass(resultClass);
            };
        }
    }
}