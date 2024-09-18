///<reference path="../../../node_modules/@types/bootstrap/index.d.ts" />
//@ts-check

/** @type {Record<string, bootstrap.Modal>} */
const bsModals = {};

function closeModal(modalId) {
    console.log('closing modal', modalId);
    const modal = bsModals[modalId];
    modal.hide();
    bsModals[modalId] = undefined;
}

function showModal(modalId) {
    console.log('showing modal', modalId);
    const modal = new bootstrap.Modal(document.getElementById(modalId));
    bsModals[modalId] = modal;
    modal.show();
}