var lastFocusedInputIndex;

dotvvm.events.beforePostback.subscribe(function () {
    var focusedElement = $("input[type=text]:focus");
    if (focusedElement.length > 0) {
        lastFocusedInputIndex = $("input[type=text]").index(focusedElement[0]);
    } else {
        lastFocusedInputIndex = -1;
    }
});
dotvvm.events.afterPostback.subscribe(function () {
    if (lastFocusedInputIndex >= 0) {
        var inputs = $("input[type=text]");
        if (lastFocusedInputIndex < inputs.length) {
            window.setTimeout(function () {
                inputs[lastFocusedInputIndex].focus();
            }, 0);
        }
    }
});