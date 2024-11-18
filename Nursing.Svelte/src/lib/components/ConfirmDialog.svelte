<script lang="ts">
    
    let dialog: HTMLDialogElement;

    let resolve: (value: boolean) => void;

    let titleText = $state('');
    let confirmText = $state('Confirm');
    let messageText = $state('');

    export function showConfirmation(title: string, message: string, confirm = 'Confirm') {
        titleText = title;
        confirmText = confirm;
        messageText = message;
        dialog.showModal();
        return new Promise<boolean>((res) => {
            resolve = res;
        });
    }

    function confirm() {
        resolve(true);
        dialog.close();
    }
</script>

<dialog bind:this={dialog} class="p-0" onclose={() => resolve(false)}>
    <div class="modal-dialog m-0">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">{titleText}</h2>
                <button type="button" class="btn-close" aria-label="Close" onclick={() => dialog.close()}></button>
            </div>
            <div class="modal-body">
                {messageText}
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary mr-2" onclick={() => dialog.close()}>Cancel</button>
                <button class="btn btn-primary" onclick={confirm}>{confirmText}</button>
            </div>
        </div>
    </div>
</dialog>
