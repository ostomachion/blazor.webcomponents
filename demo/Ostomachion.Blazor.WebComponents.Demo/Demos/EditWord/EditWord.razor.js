export default class {
    selectAll(input) {
        input.setSelectionRange(0, input.value.length);
    }

    updateWidth(input, span) {
        input.style.width = span.clientWidth + 'px';
    }
}
