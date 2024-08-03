var toggleTheme = isDarkTheme => {
    const htmlTag = document.getElementsByTagName('html')[0];
    htmlTag.setAttribute('data-bs-theme', isDarkTheme ? 'dark' : 'light');
};