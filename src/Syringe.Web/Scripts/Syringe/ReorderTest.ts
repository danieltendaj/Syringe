/// <reference path="../typings/jquery/jquery.d.ts" />
 
module Syringe.Web {
    // declared Sortable so the sortable library can be used
    declare var Sortable: any;

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
                       
                    }).on("shown.bs.modal", function (e) {
                        var el = document.getElementById('reorderedTestsList');
                        var sortable = Sortable.create(el);
                    });
                });
            });

            $(document).on("click", "#saveOrder", function (e) {
                e.preventDefault();
                $(this).hide();

                var tests = new Array<TestPostion>();
                var filename = $(this).data("filename");

                $("#reorderedTestsList li").each(function (e) {
                    tests.push(new TestPostion($(this).data("original"), ""));
                });

                $.post("/TestFile/ReorderTests", { "filename": filename, "tests": tests }, function(e) {
                    if (e) {
                        window.location.reload();
                    }
                });
            });
        };
    }
}

new Syringe.Web.ReorderTests();