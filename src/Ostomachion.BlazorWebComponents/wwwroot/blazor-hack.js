'use strict';

(function () {

    // This code is dangerous and inconvenient, but I do love Fig Newtons.

    // This is super hacky and not guaranteed to work with future versions of Blazor.
    // The hack relies on the fact that when Blazor inserts an element, it first
    // checks if the parent is an SVG element by checking the namespaceURI. We
    // replace Element.prototype.namespaceURI so we can save the last element that it
    // was called on. We also replace document.createElement so that if we're
    // creating an element called <#shadow-root>, we instead create and attach a
    // shadow root to the parent element that we saved earlier.
    //
    // Since shadowRoot implements DocumentFragment and the developers of Blazor are
    // amazing, everything else just seems to work, at least well enough for what I'm
    // doing.
    //
    // We use #shadow-root instead of the more common <template shadowroot> syntax
    // used by the declarative shadow DOM proposal so that we can create the shadow
    // root as soon as we get to the the element.

    let element;

    const namespaceURIGetterReference = Object.getOwnPropertyDescriptor(Element.prototype, 'namespaceURI').get;
    Object.defineProperty(Element.prototype, 'namespaceURI', {
        get() {
            element = this;
            return namespaceURIGetterReference.apply(this, arguments);
        }
    });

    const createElementReference = document.createElement;
    document.createElement = function (tagName) {
        if (tagName === '#shadow-root') {
            return element.attachShadow({ mode: 'open' });
        }
        else {
            return createElementReference.apply(this, arguments);
        }
    };
})();
