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
        const template = document.createElement('template');
        template.innerHTML = (templateCss ? `<style>\n${templateCss}\n</style>\n` : '') + templateHtml;
        return template.content.childNodes;
    }
}