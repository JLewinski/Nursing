<script lang="ts">
    type BeforeInstallPromptEvent = Event & {
        prompt: () => Promise<void>;
        userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>;
    };

    let deferredPrompt: BeforeInstallPromptEvent | null = null;
    let showPrompt = false;

    window.addEventListener('beforeinstallprompt', (e) => {
        e.preventDefault();
        deferredPrompt = e as BeforeInstallPromptEvent;
        showPrompt = true;
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
        <button on:click={handleInstall} class="install-button">
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
        background: var(--surface-color, #fff);
        padding: 0.5rem 1rem;
        border-radius: 8px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
        z-index: 1000;
    }

    .install-button {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        padding: 0.5rem 1rem;
        border: none;
        border-radius: 4px;
        background: var(--primary-color, #007bff);
        color: white;
        cursor: pointer;
        font-weight: 500;
    }

    .install-button:hover {
        background: var(--primary-color-dark, #0056b3);
    }
</style>