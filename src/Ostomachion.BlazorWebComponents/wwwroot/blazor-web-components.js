'use strict';

window.blazorWebComponents = {
    defineCustomElement: function (name) {
        if (!customElements.get(name)) {
            customElements.define(name,
                class extends HTMLElement {
                    constructor() {
                        super();
                    }
                }
            );
        }
    }
};
