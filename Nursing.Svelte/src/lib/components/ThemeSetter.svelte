<script lang="ts">
    import { settings } from '$lib/stores/settingsStore';
    let selectedTheme = $derived.by(() => {
        if (settings.theme === 'system') {
            return 'dark'; //todo: check system theme
        }
        return system.theme;
    });
</script>

<div data-theme="{selectedTheme">
    {$props.children}
</div>