/// <reference path="../typings/jquery/jquery.d.ts" />

module Syringe.Web {
    export class ReorderTests {
        constructor() {
            this.init();
        }

        private init() {
            $(document).on("click", ".reorder-test", function (e) {
                e.preventDefault();

                var filename = $(this).data("filename");

                $.get("/TestFile/ReorderTests", { "filename": filename }, html => {
                    bootbox.alert(html, () => {



                    });
                });
            });     
        };
    }
}

new Syringe.Web.ReorderTests();