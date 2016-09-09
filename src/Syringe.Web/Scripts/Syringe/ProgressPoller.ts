/// <reference path="../typings/jquery/jquery.d.ts" />
module Syringe.Web {
    export class ProgressPoller {
        private intervalTime: number;
        private intervalRef: number;

        constructor() {
            this.intervalTime = 500;
        }

        poll(taskId: number) {

            var that = this;

            this.intervalRef = setInterval(() =>
            {
                $.getJSON("/Json/PollTaskStatus",
                    { "taskId": taskId },
                    function (data, textStatus, jqXHR)
                    {
                        console.log(that.intervalRef +" - " +data);

                        if (data.IsFinished === true) {
                            clearInterval(that.intervalRef);
                            window.location.href = `/Results/ViewResult/${data.ResultGuid}`;
                        }

                        if (data.TotalTests > 0) {
                            var percentage = (data.CurrentItem / data.TotalTests) * 100;
                            $(".progress-bar").css("width", percentage + "%");
                            $(".progress-bar .sr-only").text(`${percentage}% Complete`);
                        }

                    });
            },
            this.intervalTime);
        }
    }
}