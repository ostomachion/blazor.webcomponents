export default class {
    countWords() {
        const text = this.parentNode.innerText || this.parentNode.textContent;
        return text.trim().split(/\s+/g).filter(a => a.trim().length > 0).length ?? 0;
    }
}
