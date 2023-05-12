'use strict';

window.blazorWebComponents = {
    registerCustomElement: async function (name, localName, modulePath) {
        if (!customElements.get(name)) {
            let ModuleClass = null;
            let test = null;
            if (modulePath) {
                try {
                    test = await import(modulePath);
                    ModuleClass = test.default;
                } catch {
                    // Probably means the component isn't used. Just ignore this.
                }
            }

            const base = localName ? document.createElement(localName).constructor : HTMLElement;

            class Definition extends base {
                constructor() {
                    super();
                    if (ModuleClass !== null) {
                        Object.assign(this, new ModuleClass());
                    }
                }
            }

            if (ModuleClass !== null) {
                for (const key of Object.getOwnPropertyNames(ModuleClass.prototype)) {
                    if (key !== 'constructor') {
                        Definition.prototype[key] = ModuleClass.prototype[key];
                    }
                }
            }

            customElements.define(name, Definition, localName ? { extends: localName } : {});
        }
    },

    invokeMethod: (customElement, name, args) => customElement[name](...args),
};
