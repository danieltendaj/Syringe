module Syringe.Web {
    export class TestPostion {

        public OriginalPostion: number;
        public Description: string;

        constructor(originalPosition: number, description: string) {
            this.OriginalPostion = originalPosition;
            this.Description = description;
        }
    }
}