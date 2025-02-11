export default function debounce(func, timeout = 3000){
    let timer = null;

    return function callthislater() {
        const args = arguments;
        clearTimeout(timer);
        timer = setTimeout(() => {
            func.apply(this, args);
            }, timeout);
    };
}