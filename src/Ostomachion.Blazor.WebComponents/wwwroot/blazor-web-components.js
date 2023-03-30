'use strict';

window.blazorWebComponents = {
    registerCustomElement: async function (name, localName, modulePath) {
        if (!customElements.get(name)) {
            let module = null;
            if (modulePath) {
                try {
                    module = await import(modulePath);
                } catch {
                    // Probably means the component isn't used. Just ignore this.
                }
            }

            const base = localName ? document.createElement(localName).constructor : HTMLElement;

            const definition = class extends base {
                constructor() {
                    super();
                    if (module?.constructor) {
                        module.constructor.call(this);
                    }
                }
            };

            if (module !== null) {
                for (const [key, value] of Object.entries(module)) {
                    if (key !== 'constructor') {
                        definition.prototype[key] = value;
                    }
                }
            }

            customElements.define(name, definition, localName ? { extends: localName } : {});
        }
    },
};
