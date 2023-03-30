export function slotChange() {
    const slots = this.shadowRoot.querySelectorAll('slot');
    const nodes = slots[0].assignedNodes();
    console.log(`Element in Slot "${slots[0].name}" changed to "${nodes[0].outerHTML}".`);
}
