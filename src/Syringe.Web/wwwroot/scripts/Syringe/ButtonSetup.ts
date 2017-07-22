/// <reference path="../typings/bootbox.d.ts" />
/// <reference path="../typings/jquery/jquery.textcomplete.d.ts" />

$(document).ready(function () {
	const rowsToAdd = [
		{ $Button: $("#addVerification"), URL: "/Test/AddAssertion", Prefix: "Assertions" },
		{ $Button: $("#addParsedItem"), URL: "/Test/AddCapturedVariableItem", Prefix: "CapturedVariables" },
		{ $Button: $("#addHeaderItem"), URL: "/Test/AddHeaderItem", Prefix: "Headers" },
		{ $Button: $("#addVariableItem"), URL: "/TestFile/AddVariableItem", Prefix: "Variables" }
	];

	function getFileExtension(fileName: string) {
		return fileName.split(".").pop().toLowerCase();
	}

	function bindVariablesAutoComplete() {
		var availableVariables = $("#available-variables-json");

		if (availableVariables && availableVariables.length === 1) {
			availableVariables = JSON.parse(availableVariables.val());

			$('.variable-compatible').textcomplete([
				{
					words: availableVariables,
					match: /{([\-+\w]*)$/,
					search: function (term: any, callback: any) {
						callback($.map(this.words, function (word) {
							return word.indexOf(term) === 0 ? word : null;
						}));
					},
					index: 1,
					replace: function (word: any) {
						return `{${word}}`;
					}
				}
			], { appendTo: 'body' }).overlay([
				{
					match: /{([\-+\w]*)}/g,
					css: {
						'background-color': '#d8dfea'
					}
				}
			]);
		}
	}

	let rowHandler = new RowHandler(rowsToAdd, bindVariablesAutoComplete);
	rowHandler.setupButtons();

	bindVariablesAutoComplete();

	$("body").on("click", "#removeRow", function (e) {
		e.preventDefault();
		var group = $(this).closest(".group");
		var component = group.closest(".component");
		$(this).closest(".group").remove();

		component.find(".group").each(function (i, ev) {
			$(ev).find("label").each(function () {
				rowHandler.updateElementValue($(this), i, "for");
			});

			$(ev).find("input, select").each(function () {
				rowHandler.updateElementValue($(this), i, "name");
				rowHandler.updateElementValue($(this), i, "id");
			});
		});
	});

	$(".delete-button").on("click", function (e) {
		e.preventDefault();

		var that = $(this);

		var form = that.closest("form");
		bootbox.confirm("Are you sure you want delete?", function (result) {
			if (result) {
				form.submit();
				return true;
			}
		});

		return false;
	});

	$(".copy-file-button").on("click", function (e) {
		e.preventDefault();

		var that = $(this);
		var form = that.closest("form");
		var sourceFileName = form.find("[name='sourceTestFile']").val();
		bootbox.prompt({
			title: "What would you like to call the new file?",
			value: sourceFileName,
			callback: function (result) {
				var canConvert: boolean = false;
				if (result) {
					if (result.toLowerCase() != sourceFileName.toLowerCase()) {
						if (getFileExtension(result) == getFileExtension(sourceFileName)) {
							canConvert = true;
						}
					}
				}

				if (!canConvert) {
					bootbox.alert("Invalid copy name, please try again.");
				}
				else {
					form.find("[name='targetTestFile']").val(result);
					form.submit();
					return true;
				}
			}
		});

		return false;
	});
});