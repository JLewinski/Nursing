<script lang="ts">
    import { settings } from '$lib/stores/settingsStore';
    import { NotificationManager } from '$lib/utils/notifications';
    import ThemeToggle from '$lib/components/ThemeToggle.svelte';
</script>

<div class="settings-container">
    <h1>Settings</h1>
    
    <section>
        <h2>Theme</h2>
        <ThemeToggle />
    </section>
    
    <section>
        <h2>Notifications</h2>
        <!-- TODO: Notification settings -->
    </section>
</div>