'use strict';

window.blazorWebComponents = {
    initialize: function () {

        // This code is dangerous and inconvenient, but I do love Fig Newtons.

        // This is super hacky and not guaranteed to work with future versions of Blazor.
        // The hack relies on the fact that when Blazor inserts an element, it first
        // checks if the parent is an SVG element by checking the namespaceURI. We
        // replace the getter for Element.prototype.namespaceURI so we can save the last
        // element that it was called on. We also replace document.createElement so that
        // if we're tyring to create an element called '#shadow-root (open)' or
        // '#shadow-root (closed)', we instead create and attach a shadow root to the
        // parent element that we saved earlier in the namesapceURI getter.
        //
        // Since shadowRoot implements DocumentFragment and the developers of Blazor are
        // amazing, everything else just seems to work, at least well enough for what I'm
        // doing.
        //
        // We use #shadow-root instead of the more common <template shadowroot> syntax
        // used by the declarative shadow DOM proposal so that we can create the shadow
        // root as soon as we get to the the element.

        const sourceEventSymbol = Symbol('sourceEvent');
        let element;

        const namespaceURIRef = Object.getOwnPropertyDescriptor(Element.prototype, 'namespaceURI').get;
        Object.defineProperty(Element.prototype, 'namespaceURI', {
            get() {
                element = this;
                return namespaceURIRef.apply(this, arguments);
            }
        });

        const createElementRef = HTMLDocument.prototype.createElement;
        HTMLDocument.prototype.createElement = function (tagName) {
            if (tagName === '#shadow-root (open)') {
                const shadowRoot = element.attachShadow({ mode: 'open' });
                attachEvents(shadowRoot, element);
                return shadowRoot;
            }
            else if (tagName === '#shadow-root (closed)') {
                const shadowRoot = element.attachShadow({ mode: 'closed' });
                attachEvents(shadowRoot, element);
                return shadowRoot;
            }
            else {
                return createElementRef.apply(this, arguments);
            }

            // For events that aren't composed, we need to re-dispatch them once
            // they hit the shadow root. We also store the original event's
            // composedPath and target on the new event to trick Blazor.
            function attachEvents(shadowRoot) {
                // TODO: Should we do the same thing to other non-composed events?
                // The change event is used internally by Blazor so we need this one.
                shadowRoot.addEventListener('change', function (e) {
                    if (!e.composed) {
                        const newEvent = new Event('change', {
                            bubbles: e.bubbles,
                            cancelable: e.cancelable,
                            composed: false
                        });
                        newEvent[sourceEventSymbol] = e;
                        shadowRoot.host.dispatchEvent(newEvent);
                    }
                });
            }
        };

        // We re-dispatch non-composed events once they hit the shadow root.
        // But we need to use give Blazor the original target and composedPath.
        const composedPathRef = Event.prototype.composedPath;
        Event.prototype.composedPath = function () {
            const value = composedPathRef.apply(this, arguments);
            if (this.hasOwnProperty(sourceEventSymbol)) {
                return this[sourceEventSymbol].composedPath().concat(value);
            }
            else {
                return value;
            }
        };
        // TODO: Figure out why Blazor's onfoo:preventDefault isn't working.
        const props = ['target', 'defaultPrevented', 'cancelBubble'];
        for (const prop of props) {
            const ref = Object.getOwnPropertyDescriptor(Event.prototype, prop).get;
            Object.defineProperty(Event.prototype, prop, {
                get() {
                    if (this.hasOwnProperty(sourceEventSymbol)) {
                        console.group();
                        console.log('shadow ' + prop);
                        console.dir(this);
                        console.dir(this[sourceEventSymbol]);
                        console.groupEnd();
                        return this[sourceEventSymbol][prop];
                    }
                    else {
                        return ref.apply(this, arguments);
                    }
                }
            });
        }

        // Element references may be hidden in a shadow DOM.
        // Hijack document.querySelector so that Blazor can still find them.
        const querySelectorRef = HTMLDocument.prototype.querySelector;
        HTMLDocument.prototype.querySelector = function (selectors) {
            // This is the attribute pattern that Blazor uses for element references.
            // Only look into shadow roots if we're looking for an element reference for the document root.
            if (/^\[_bl_\w+\]$/.test(selectors)) {
                return querySelectorDeep(this, arguments);
            }
            else {
                return querySelectorRef.apply(this, arguments);
            }

            function querySelectorDeep(root, args) {
                const lightResult = root instanceof HTMLDocument ? querySelectorRef.apply(root, args) : root.querySelector(...args);
                if (lightResult !== null) {
                    return lightResult;
                }

                for (var el of root.querySelectorAll('*')) {
                    if (el.shadowRoot) {
                        const result = querySelectorDeep(el.shadowRoot, args);
                        if (result !== null) {
                            return result;
                        }
                    }
                }

                return null;
            }
        };
    },

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

window.blazorWebComponents.initialize();
