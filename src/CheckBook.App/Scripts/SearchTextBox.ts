

var dotvvm = dotvvm || {};

ko.bindingHandlers["doneTyping"] = <KnockoutBindingHandler>{
    init(element: any, valueAccessor: () => any, allBindingsAccessor?: KnockoutAllBindingsAccessor, viewModel?: any, bindingContext?: KnockoutBindingContext) {
        var doneTypingInterval = Number(element.attributes["delay"].value);
        var typingTimer;
        var $element = $(element);
        //on keyup, start the countdown
        $element.on('keyup', () => {
            clearTimeout(typingTimer);
            typingTimer = setTimeout((e) => {
                var fnc = valueAccessor();
                fnc(element, e);
            }, doneTypingInterval);
        });

        //on keydown, clear the countdown 
        $element.on('keydown', () => {
            clearTimeout(typingTimer);
        });
    }
};
