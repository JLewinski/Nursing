///<reference path="../../../node_modules/@types/bootstrap/index.d.ts" />
//@ts-check

var themeManager = {
    /**
     * 
     * @param {boolean} isDarkTheme
     */
    toggleTheme(isDarkTheme) {
        const htmlTag = document.getElementsByTagName('html')[0];
        htmlTag.setAttribute('data-bs-theme', isDarkTheme ? 'dark' : 'light');
    },

    isDarkPreferred() {
        return window.matchMedia('(prefers-color-scheme: dark)').matches;
    }
};

var loadingModal = (() => {

    class LoadingModal {
        /** @type {bootstrap.Modal} */
        #loadingModal;

        init() {
            if (this.#loadingModal) {
                return;
            }
            this.#loadingModal = new bootstrap.Modal(document.getElementById('LoadingModal'));
        }

        /**
         * 
         * @param {string} message
         */
        showLoadingModal(message) {
            this.init();
            document.getElementById('LoadingTitle').textContent = message;
            this.#loadingModal.show();
        }

        hideLoadingModal() {
            this.#loadingModal?.hide();
        }
    }

    return new LoadingModal()
})();