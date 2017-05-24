var dotvvm = dotvvm || {};
ko.bindingHandlers["doneTyping"] = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var doneTypingInterval = Number(element.attributes["delay"].value);
        var typingTimer;
        var $element = $(element);
        //on keyup, start the countdown
        $element.on('keyup', function () {
            clearTimeout(typingTimer);
            typingTimer = setTimeout(function (e) {
                var fnc = valueAccessor();
                fnc(element, e);
            }, doneTypingInterval);
        });
        //on keydown, clear the countdown 
        $element.on('keydown', function () {
            clearTimeout(typingTimer);
        });
    }
};
//# sourceMappingURL=SearchTextBox.js.map