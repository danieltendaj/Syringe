/// <reference path="../typings/jquery/jquery.d.ts" />
 
module Syringe.Web {

    export class TestPostion {

        public OriginalPostion: number;
        public Description: string;

        constructor(originalPosition: number, description: string) {
            this.OriginalPostion = originalPosition;
            this.Description = description;
        }
    }

    export class TestFilerOrder {
        public Filename: string;
        public Tests: TestPostion[];

    }

    export class ReorderTests {
        constructor() {
            this.init();
        }

        private init() {
            $(document).on("click", ".reorder-test", function (e) {
                e.preventDefault();

                var filename = $(this).data("filename");

                $.get("/TestFile/GetTestsToReorder", { "filename": filename }, html => {
                    bootbox.alert(html, () => {
                    });
                });
            });

            $(document).on("click", "#saveOrder", function (e) {
                e.preventDefault();
                $(this).hide();

                var testFileOrder = new TestFilerOrder();
                testFileOrder.Filename = $(this).data("filename");
                var testPosition = new Array<TestPostion>();
                $("#reorderedTestsList li").each(function(e) {
                    testPosition.push(new TestPostion($(this).data("original"), ""));
                });

                testFileOrder.Tests = testPosition;

                $.post("/TestFile/ReorderTests", { "testFile" : testFileOrder }, function(e) {
                    console.log(e);
                });
            });
        };
    }
}

new Syringe.Web.ReorderTests();