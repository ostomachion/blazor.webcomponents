export function initialize() {
    document.getElementsByTagName('main')[0].addEventListener('click', e => {
        console.log(e.composed);
        console.log(e.composedPath());
    });
}
