export function selectAll(input) {
    input.setSelectionRange(0, input.value.length);
}

export function updateWidth(input, span) {
    input.style.width = span.clientWidth + 'px';
}
