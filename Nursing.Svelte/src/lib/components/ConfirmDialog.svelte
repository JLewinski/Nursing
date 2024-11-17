<script lang="ts">
    
    interface Props {
        children: import("svelte").Snippet;
    }

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

<dialog bind:this={dialog} onclose={() => resolve(false)}>
    <div class="dialog-content">
        <div class="dialog-header">
            <h2>{titleText}</h2>
        </div>
        <div class="dialog-body">
            {messageText}
        </div>
        <div class="dialog-footer">
            <button class="btn btn-secondary mr-2" onclick={() => dialog.close()}>Cancel</button>
            <button class="btn btn-primary" onclick={confirm}>{confirmText}</button>
        </div>
    </div>
</dialog>
