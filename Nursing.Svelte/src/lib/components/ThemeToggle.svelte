<script lang="ts">
    import { settings } from '$lib/stores/settingsStore';
    
    function toggleTheme(theme: 'light' | 'dark' | 'system') {
        $settings.theme = theme;
    }
</script>

<div class="theme-toggle">
    <button on:click={() => toggleTheme('light')}>Light</button>
    <button on:click={() => toggleTheme('dark')}>Dark</button>
    <button on:click={() => toggleTheme('system')}>System</button>
</div>