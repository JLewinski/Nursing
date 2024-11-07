<script lang="ts">
    import { onMount } from 'svelte';
    let updateAvailable = false;
    let registration: ServiceWorkerRegistration | undefined = undefined;

    async function checkForUpdates() {
        if (!registration) return;
        try {
            await registration.update();
        } catch (error) {
            console.error('Update check failed:', error);
        }
    }

    function handleUpdate() {
        if (registration?.waiting) {
            registration.waiting.postMessage({ type: 'SKIP_WAITING' });
            window.location.reload();
        }
    }

    onMount(async () => {
        if ('serviceWorker' in navigator) {
            registration = await navigator.serviceWorker.getRegistration();
            registration?.addEventListener('updatefound', () => {
                updateAvailable = true;
            });
        }
    });
</script>

{#if updateAvailable}
    <div class="update-banner">
        <span>New version available!</span>
        <button on:click={handleUpdate}>Update Now</button>
    </div>
{/if}

<style>
    .update-banner {
        position: fixed;
        bottom: 0;
        left: 0;
        right: 0;
        padding: 1rem;
        background: var(--primary-color);
        color: white;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }
</style>