
export function shimValueAsNumber() {
    /* Some browsers may have trouble retrieving the number type
    of an input value. This short script performs a quick test, and repairs
    the functionality if necessary. Load before attempting to use the
    `valueAsNumber` property on input elements. */


    var a = document.createElement("input");
    a.setAttribute("type", "number");
    a.setAttribute("value", 2319);

    if ("valueAsNumber" in a && a.value !== a.valueAsNumber) {
        if ("defineProperty" in Object && "getPrototypeOf" in Object) {
            Object.defineProperty(Object.getPrototypeOf(a), "valueAsNumber", {
                get: function () {
                    return parseInt(this.value, 10);
                }
            });
        }
    }

}