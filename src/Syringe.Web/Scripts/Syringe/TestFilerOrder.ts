module Syringe.Web {
    export class TestFilerOrder {
        public Filename: string;
        public Tests: TestPostion[];

        constructor() {
            this.Tests = new Array<TestPostion>();
        }
    }
}