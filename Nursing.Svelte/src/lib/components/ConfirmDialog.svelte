<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    
    export let title = 'Confirm';
    export let message = '';
    export let show = false;
    
    const dispatch = createEventDispatcher();
</script>

{#if show}
    <div class="dialog-overlay">
        <div class="dialog">
            <h3>{title}</h3>
            <p>{message}</p>
            <div class="actions">
                <button 
                    class="secondary"
                    on:click={() => dispatch('cancel')}
                >
                    Cancel
                </button>
                <button 
                    class="primary"
                    on:click={() => dispatch('confirm')}
                >
                    Confirm
                </button>
            </div>
        </div>
    </div>
{/if}

<style>
    .dialog-overlay {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0, 0, 0, 0.5);
        display: grid;
        place-items: center;
        z-index: 1000;
    }

    .dialog {
        background: var(--surface-color);
        padding: 1.5rem;
        border-radius: 0.5rem;
        max-width: 90vw;
        width: 400px;
    }
</style>