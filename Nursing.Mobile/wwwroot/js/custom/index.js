//@ts-check

/**
 * 
 * @param {boolean} isDarkTheme
 */
function toggleTheme(isDarkTheme) {
    const htmlTag = document.getElementsByTagName('html')[0];
    htmlTag.setAttribute('data-bs-theme', isDarkTheme ? 'dark' : 'light');
};

function isDarkPreferred() {
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
}