
interface IElvisDelayedAfterKeyItemProcedure {
    element: HTMLElement;
    func: () => void;
}

interface IElvisDelayedAfterKeyItem {
    element: HTMLElement;
    handler: number;
}

var dotvvm = dotvvm || {};

ko.bindingHandlers["elvisDelayedAfterKey"] = <KnockoutBindingHandler>{
    onchangeFunctions: <IElvisDelayedAfterKeyItemProcedure[]>[],
    timeouts: <IElvisDelayedAfterKeyItem[]>[],
    init(element: any, valueAccessor: () => any, allBindingsAccessor?: KnockoutAllBindingsAccessor, viewModel?: any, bindingContext?: KnockoutBindingContext) {
        var obs = valueAccessor();
        var tmp = obs();
        $(element).keyup((e) => {
            var onchangeFunctions = <IElvisDelayedAfterKeyItemProcedure[]>(<any>ko.bindingHandlers["elvisDelayedAfterKey"]).onchangeFunctions;
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
                onchangeFunctions.push(<IElvisDelayedAfterKeyItemProcedure>{
                    func: changeFunc,
                    element: element
                });
            }
            var timeouts = <IElvisDelayedAfterKeyItem[]>(<any>ko.bindingHandlers["elvisDelayedAfterKey"]).timeouts;
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
