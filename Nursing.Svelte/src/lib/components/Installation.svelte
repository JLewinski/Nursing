<script lang="ts">
    import { onMount } from 'svelte';
    

    type BeforeInstallPromptEvent = Event & {
        prompt: () => Promise<void>;
        userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>;
    };

    let deferredPrompt: BeforeInstallPromptEvent | null = null;
    let showPrompt = $state(false);

    onMount(() => {
        if (typeof window !== 'undefined') {
            window.addEventListener('beforeinstallprompt', (e) => {
                e.preventDefault();
                deferredPrompt = e as BeforeInstallPromptEvent;
                showPrompt = true;
            });
        }
    });

    async function handleInstall() {
        if (!deferredPrompt) return;
        
        deferredPrompt.prompt();
        const { outcome } = await deferredPrompt.userChoice;
        
        if (outcome === 'accepted') {
            showPrompt = false;
            deferredPrompt = null;
        }
    }
</script>

{#if showPrompt}
    <div class="install-prompt">
        <button onclick={handleInstall} class="btn btn-primary">
            <span class="bi bi-download"></span>
            Install App
        </button>
    </div>
{/if}

<style>
    .install-prompt {
        position: fixed;
        bottom: 70px;
        left: 50%;
        transform: translateX(-50%);
        padding: 0.5rem 1rem;
        border-radius: 8px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
        z-index: 1000;
    }
</style>