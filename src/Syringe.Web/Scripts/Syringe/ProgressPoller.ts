/// <reference path="../typings/jquery/jquery.d.ts" />
module Syringe.Web {
    export class ProgressPoller {
        private intervalTime: number;

        constructor() {
            this.intervalTime = 500;
        }

        poll(taskId: number) {

            var intervalRef = setInterval(() =>
            {
                $.getJSON("/Json/PollTaskStatus",
                    { "taskId": taskId },
                    (data, textStatus, jqXhr) => {
                        console.log("setInterval ref is: " + intervalRef);
                        console.log(data);

                        if (data.IsFinished === true) {
                            clearInterval(intervalRef);
                            console.log("Finished!");
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