class RowHandler {
    private rowsToAdd: IRowAdder[];
    private bindVariablesAutoComplete: () => void;

    constructor(i: IRowAdder[], bindVariablesAutoComplete: () => void) {
        this.bindVariablesAutoComplete = bindVariablesAutoComplete;
        this.rowsToAdd = i;
    }

    private addRow = (e) => {
        e.preventDefault();

        var test = e.data.test;
        var bindVariablesAutoComplete = this.bindVariablesAutoComplete;

        $.get(test.URL, html => {
            // get the closest component class, needs a rewrite, too easy to break.
            var panelBody = test.$Button.parent().next();
            var group = panelBody.find(".group").last();
            var rowNumber = 0;

            if (group.length !== 0) {
                var firstInputName = group.find("input:first").attr("name");

                // get the last index number of the row and increment it by 1
                rowNumber = parseInt(firstInputName.match(/\d/g)) + 1;
            }

            // replace the name value with the correct prefix and row number so it can be posted to the server 
            var newHtml = html.replace(/name="/g, "name=\"" + test.Prefix + "[" + rowNumber + "].");
            panelBody.append(newHtml);

            bindVariablesAutoComplete();
        });
    }

    public setupButtons = () => {
        for (let i = 0; i < this.rowsToAdd.length; i++) {
            let testRow = this.rowsToAdd[i];
            testRow.$Button.on("click", { test: testRow }, this.addRow);
        }
    }

    public updateElementValue = ($element, iRowNumber, attribute) => {
        $element.attr(attribute, $element.attr(attribute).replace(/\[\d\]/, "[" + iRowNumber.toString() + "]"));
    }
}


