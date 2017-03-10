var dotvvm = dotvvm || {};
ko.bindingHandlers["elvisDelayedAfterKey"] = {
    onchangeFunctions: [],
    timeouts: [],
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var obs = valueAccessor();
        var tmp = obs();
        $(element).keyup(function (e) {
            var onchangeFunctions = ko.bindingHandlers["elvisDelayedAfterKey"].onchangeFunctions;
            var changeFunc = null;
            for (var _i = 0, onchangeFunctions_1 = onchangeFunctions; _i < onchangeFunctions_1.length; _i++) {
                var funcReg = onchangeFunctions_1[_i];
                if (funcReg.element === element) {
                    changeFunc = funcReg.func;
                    break;
                }
            }
            if (!changeFunc) {
                changeFunc = element.onchange;
                element.onchange = null;
                onchangeFunctions.push({
                    func: changeFunc,
                    element: element
                });
            }
            var timeouts = ko.bindingHandlers["elvisDelayedAfterKey"].timeouts;
            var resolvedTimeout = null;
            for (var _a = 0, timeouts_1 = timeouts; _a < timeouts_1.length; _a++) {
                var timeout = timeouts_1[_a];
                if (timeout.element === element) {
                    resolvedTimeout = timeout;
                    break;
                }
            }
            if (resolvedTimeout) {
                if (resolvedTimeout.handler) {
                    window.clearTimeout(resolvedTimeout.handler);
                    resolvedTimeout.handler = null;
                }
            }
            else {
                resolvedTimeout = { element: element, handler: null };
                timeouts.push(resolvedTimeout);
            }
            //update value
            obs(element.value);
            // suppress
            if (e.keyCode !== 13) {
                resolvedTimeout.handler = setTimeout(function () {
                    if (changeFunc) {
                        if (!dotvvm.isPostbackRunning()) {
                            changeFunc.call(element, e);
                        }
                    }
                    resolvedTimeout.handler = null;
                }, 400);
            }
        });
    }
};
//# sourceMappingURL=SearchTextBox.js.map