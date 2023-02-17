function registerBlazorWebComponent(elementName, templateHtml, templateCss) {
    if (!customElements.get(elementName)) {
        customElements.define(elementName, class extends HTMLElement {
            constructor() {
                super()
                    .attachShadow({ mode: 'open' })
                    .append(...getTemplateNodes(templateHtml, templateCss));
            }
        });
    }

    function getTemplateNodes(templateHtml, templateCss) {
        const style = document.createElement('style');
        style.textContent = templateCss;

        const template = document.createElement('template');
        template.innerHTML = templateHtml;

        return [style, ...template.content.childNodes];
    }
}