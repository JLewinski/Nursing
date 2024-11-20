<script lang="ts">
    import { settings } from "$lib/stores/settingsStore.svelte";
    import { NotificationManager } from "$lib/utils/notifications";
    import { Database } from "$lib/db/mod";
    import { lastSession } from "$lib/stores/lastSessionStore.svelte";
    import ConfirmDialog from "$lib/components/ConfirmDialog.svelte";

    let dialog: ConfirmDialog;
    async function clear() {
        const confirmed = await dialog.showConfirmation(
            "Clear Data",
            "Are you sure you want to clear all data?",
            "Clear",
        );
        if (!confirmed) return;

        await new Database().clearSessions();
        lastSession.side = undefined;
        lastSession.startTime = null;
        lastSession.save();
    }
</script>

<ConfirmDialog bind:this={dialog} />

<div class="settings-container">
    <h1>Settings</h1>

    <section>
        <h2>Notifications</h2>
        <!-- TODO: Notification settings -->
    </section>

    <section>
        <h2>Data</h2>
        <button class="btn btn-danger" onclick={async () => await clear()}
            >Clear Data</button
        >
    </section>
</div>
