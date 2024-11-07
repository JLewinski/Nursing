<script lang="ts">
    import { onMount } from 'svelte';
    import { syncStore } from '$lib/stores/syncStore';
    import { SyncManager } from '$lib/utils/syncManager';
    import LoadingSpinner from './LoadingSpinner.svelte';

    const syncManager = new SyncManager();
    let lastSyncAttempt: Date | null = null;

    async function triggerSync() {
        lastSyncAttempt = new Date();
        await syncManager.syncData();
    }

    onMount(() => {
        if (navigator.onLine) {
            syncManager.registerSyncTask();
        }
    });
</script>

<div class="sync-status">
    {#if $syncStore.status === 'syncing'}
        <LoadingSpinner size="1rem" />
        <span>Syncing...</span>
    {:else if $syncStore.status === 'error'}
        <span class="error">Sync failed</span>
        <button on:click={triggerSync}>Retry</button>
    {:else}
        <button on:click={triggerSync}>
            Sync Now
        </button>
    {/if}
</div>

<style>
    .sync-status {
        display: flex;
        align-items: center;
        gap: 0.5rem;
    }

    .error {
        color: var(--error-color);
    }
</style>