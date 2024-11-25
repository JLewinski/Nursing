<script lang="ts">
    let isOnline = true;

    function updateOnlineStatus() {
        isOnline = navigator.onLine;
    }

    $: if (typeof window !== 'undefined') {
        window.addEventListener('online', updateOnlineStatus);
        window.addEventListener('offline', updateOnlineStatus);
    }
</script>

{#if !isOnline}
    <div class="offline-indicator">
        You are currently offline
    </div>
{/if}

<style>
    .offline-indicator {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        background: var(--error-color);
        color: white;
        text-align: center;
        padding: 0.5rem;
        z-index: 1000;
    }
</style>