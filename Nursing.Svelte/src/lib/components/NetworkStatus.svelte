<script lang="ts">
    import { onMount } from 'svelte';
    import { syncQueue } from '$lib/utils/syncQueue';
    
    let isOnline = navigator.onLine;
    let syncPending = false;

    onMount(() => {
        window.addEventListener('online', async () => {
            isOnline = true;
            if (syncPending) {
                await syncQueue.processQueue();
                syncPending = false;
            }
        });

        window.addEventListener('offline', () => {
            isOnline = false;
            syncPending = true;
        });

        return () => {
            window.removeEventListener('online', () => {});
            window.removeEventListener('offline', () => {});
        };
    });
</script>

{#if !isOnline}
    <div class="network-status offline">
        Working Offline
        {#if syncPending}
            <span class="sync-pending">Changes will sync when online</span>
        {/if}
    </div>
{/if}

<style>
    .network-status {
        position: fixed;
        bottom: 1rem;
        right: 1rem;
        padding: 0.5rem 1rem;
        border-radius: 0.25rem;
        background: var(--surface-color);
        box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .offline {
        border-left: 4px solid var(--error-color);
    }

    .sync-pending {
        font-size: 0.8rem;
        opacity: 0.8;
    }
</style>