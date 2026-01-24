<script lang="ts">
    import { settings } from '$lib/stores/settingsStore.svelte';
    import { notificationManager } from '$lib/utils/notifications';
    import { onMount } from 'svelte';

    let permissionStatus: NotificationPermission = $state('default');

    onMount(() => {
        permissionStatus = notificationManager.getPermissionState();
        // Sync enabled state with permission (if permission revoked, disable setting)
        if (permissionStatus === 'denied' && settings.notifications.enabled) {
             settings.notifications.enabled = false;
             settings.save();
        }
    });

    async function requestPermission() {
        const granted = await notificationManager.requestPermission();
        permissionStatus = notificationManager.getPermissionState();
        if (granted) {
            settings.notifications.enabled = true;
            settings.save();
        }
    }

    async function sendTest() {
        await notificationManager.sendLocalNotification('Test Notification', {
            body: 'This is a test notification from Nursing App',
            tag: 'test-notification'
        });
    }

    function toggleNotifications() {
        if (settings.notifications.enabled) {
            // User wants to disable
            settings.notifications.enabled = false;
            settings.save();
        } else {
            // User wants to enable
            if (permissionStatus === 'granted') {
                settings.notifications.enabled = true;
                settings.save();
            } else {
                requestPermission();
            }
        }
    }
</script>

<div class="card mb-3">
    <div class="card-body">
        <h5 class="card-title">Notification Settings</h5>
        
        <div class="mb-3 form-check form-switch">
            <input 
                class="form-check-input" 
                type="checkbox" 
                role="switch" 
                id="notifSwitch"
                checked={settings.notifications.enabled}
                onchange={toggleNotifications}
                disabled={permissionStatus === 'denied'}
            >
            <label class="form-check-label" for="notifSwitch">
                Enable Reminder Notifications
            </label>
        </div>

        {#if permissionStatus === 'denied'}
            <div class="alert alert-warning">
                Notifications are blocked in your browser settings. Please enable them manually in your browser site settings to use this feature.
            </div>
        {/if}

        {#if settings.notifications.enabled}
            <div class="mb-3">
                <label for="reminderInterval" class="form-label">Remind me after (minutes)</label>
                <input 
                    type="number" 
                    class="form-control" 
                    id="reminderInterval" 
                    bind:value={settings.notifications.reminderInterval}
                    onchange={() => settings.save()}
                    min="1"
                >
                <div class="form-text">
                    We'll send you a notification {settings.notifications.reminderInterval} minutes after your session ends.
                </div>
            </div>

            <button class="btn btn-outline-secondary btn-sm" onclick={sendTest}>
                Send Test Notification
            </button>
        {/if}
    </div>
</div>