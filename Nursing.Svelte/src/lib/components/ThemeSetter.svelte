<script lang="ts">
    import { settings } from '$lib/stores/settingsStore';
    import type { Snippet } from 'svelte';

    $effect(() => {
        let theme = $settings.theme;
        if (theme === 'system'){
            theme = globalThis.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        }
        document.documentElement.setAttribute('data-theme', theme);
    })
</script>