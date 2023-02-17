function registerBlazorWebComponent(elementName, templateContent) {
    if (!customElements.get(elementName)) {
        customElements.define(elementName, class extends HTMLElement {
            constructor() {
                super();

                this.attachShadow({ mode: 'open' });
                const template = document.createElement('template');
                template.innerHTML = templateContent;
                this.shadowRoot.append(...template.content.childNodes);
            }
        });
    }
}
