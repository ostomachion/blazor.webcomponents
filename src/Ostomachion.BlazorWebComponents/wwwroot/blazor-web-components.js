'use strict';

window.blazorWebComponents = {
    defineCustomElement: function (name, localName) {
        if (!customElements.get(name)) {
            const base = localName ? document.createElement(localName).constructor : HTMLElement;
            const definition = class extends base {
                constructor() {
                    super();
                }
            };
            customElements.define(name, definition, localName ? { extends: localName } : {});
        }
    }
};
