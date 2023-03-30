export function handleClick(element) {
    const items = Array.from(this.querySelectorAll('li'));
    const descriptions = Array.from(this.querySelectorAll('p'));

    items.forEach(item => {
        item.style.backgroundColor = 'white';
    });

    descriptions.forEach(description => {
        updateDisplay(description, element);
    });
}

function updateDisplay(description, item) {
    description.removeAttribute('slot');

    if (description.getAttribute('data-name') === item.textContent) {
        description.setAttribute('slot', 'choice');
        item.style.backgroundColor = '#bad0e4';
    }
}
