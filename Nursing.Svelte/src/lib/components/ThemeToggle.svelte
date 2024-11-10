<script lang="ts">
    import { settings } from '$lib/stores/settingsStore';
    
    function toggleTheme(newTheme: 'light' | 'dark' | 'system') {
        settings.update({theme: newTheme});
    }
</script>

<div class="theme-toggle">
    {#each ['light', 'dark', 'system'] as mode}
        <button 
            class:active={$settings.theme === mode}
            onclick={() => toggleTheme(mode as 'light' | 'dark' | 'system')}>
            {mode}
        </button>
    {/each}
</div>

<style>
    .theme-toggle {
        display: flex;
        gap: 0.5rem;
    }
    
    button {
        padding: 0.5rem 1rem;
        border: 1px solid var(--border-color);
        background: var(--surface-2);
        color: var(--text-color);
        border-radius: 0.25rem;
    }
    
    button.active {
        background: var(--primary-color);
        color: var(--surface-color);
    }
</style>