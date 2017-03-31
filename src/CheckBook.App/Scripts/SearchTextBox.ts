
interface IdelayedAfterKeyItemProcedure {
    element: HTMLElement;
    func: () => void;
}

interface IdelayedAfterKeyItem {
    element: HTMLElement;
    handler: number;
}

var dotvvm = dotvvm || {};

ko.bindingHandlers["delayedAfterKey"] = <KnockoutBindingHandler>{
    onchangeFunctions: <IdelayedAfterKeyItemProcedure[]>[],
    timeouts: <IdelayedAfterKeyItem[]>[],
    init(element: any, valueAccessor: () => any, allBindingsAccessor?: KnockoutAllBindingsAccessor, viewModel?: any, bindingContext?: KnockoutBindingContext) {
        var obs = valueAccessor();
        var tmp = obs();
        $(element).keyup((e) => {
            var onchangeFunctions = <IdelayedAfterKeyItemProcedure[]>(<any>ko.bindingHandlers["delayedAfterKey"]).onchangeFunctions;
            var changeFunc = <(e) => void>null;
            for (var funcReg of onchangeFunctions) {
                if (funcReg.element === element) {
                    changeFunc = funcReg.func;
                    break;
                }
            }
            if (!changeFunc) {
                changeFunc = element.onchange;
                element.onchange = null;
                onchangeFunctions.push(<IdelayedAfterKeyItemProcedure>{
                    func: changeFunc,
                    element: element
                });
            }
            var timeouts = <IdelayedAfterKeyItem[]>(<any>ko.bindingHandlers["delayedAfterKey"]).timeouts;
            var resolvedTimeout = null;
            for (var timeout of timeouts) {
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
            } else {
                resolvedTimeout = { element: element, handler: null };
                timeouts.push(resolvedTimeout);
            }
            //update value
            obs(element.value);
            // suppress
            if (e.keyCode !== 13) {
                resolvedTimeout.handler = setTimeout(() => {
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
