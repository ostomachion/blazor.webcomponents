'use strict';

window.blazorWebComponents = {
    defineWebComponent: function (name) {
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
