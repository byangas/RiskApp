
export  function formatNumber(number) {
    if (!number || isNaN(number))
        return "";
    return number.toLocaleString();
}

export function formatCurrency(number) {
    return "$" + formatNumber(number)
}
// cleans a string with non-number values.
// e.g. "44,4444" will be "44444". the resulting
// string will then be converted to a number type
export function numberFromString(stringValue) {
    if(!stringValue)
        return stringValue
    let asNumber = stringValue.replaceAll(/[^0-9]/g, "")
    //if contains no valid numbers
    if (asNumber.length === 0 || isNaN(asNumber)) {
        asNumber = undefined
    } else {
        asNumber = parseInt(asNumber)
    }
    return asNumber;
}


export function decimalFromString(stringValue) {
    if(!stringValue)
        return stringValue
    let matches = stringValue.match(/[0-9]*.[0-9]*/g)
    let asNumber = matches[0]
    //if contains no valid numbers
    if (asNumber.length === 0 || isNaN(asNumber)) {
        asNumber = undefined
    } else {
        asNumber = parseFloat(asNumber)
    }
    return asNumber;
}