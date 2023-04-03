export function getObservedAttributes() {
    return ['c', 'l'];
}

export function connectedCallback() {
    console.log('Custom square element added to page.');
    updateStyle(this);
}

export function disconnectedCallback() {
    console.log('Custom square element removed from page.');
}

export function adoptedCallback() {
    console.log('Custom square element moved to new page.');
}

export function attributeChangedCallback(name, oldValue, newValue) {
    console.log('Custom square element attributes changed.');
    updateStyle(this);
}

function updateStyle(elem) {
    const shadow = elem.shadowRoot;
    shadow.querySelector('style').textContent = `
        div {
            width: ${elem.getAttribute('l')}px;
            height: ${elem.getAttribute('l')}px;
            background-color: ${elem.getAttribute('c')};
        }
        `;
}
