export function initialize() {
    document.documentElement.addEventListener('click', e => {
        log('composed: ' + e.composed);
        log('composedPath(): ' + e.composedPath().map(x => '\n    ' + getString(x)).join(''));
        log('');
    });
}

function log(message) {
    console.log(message);
    const el = document.getElementById('log');
    el.value += message + '\n';
}

function getString(obj) {
    if (obj instanceof Window) {
        return 'Window';
    }

    if (obj instanceof Document) {
        return 'Document';
    }

    if (obj instanceof ShadowRoot) {
        return '#shadow-root (' + obj.mode + ')';
    }

    if (obj instanceof Element) {
        let value = obj.tagName.toLowerCase();
        if (obj.id)
            value += '#' + obj.id;

        if (obj.classList.length)
            value += ['foo', 'bar'].map(c => '.' + c).join('');

        return value;
    }

    return 'Unknown';
}