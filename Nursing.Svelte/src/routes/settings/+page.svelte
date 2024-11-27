<script lang="ts">
    import { settings } from "$lib/stores/settingsStore.svelte";
    // import { NotificationManager } from "$lib/utils/notifications";
    import { db } from "$lib/db/mod";
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

        await db.sessions.clear();
        lastSession.side = undefined;
        lastSession.startTime = null;
        lastSession.save();
    }

    let hours = $state(settings.estimatedInterval / 60);
    $effect(() => {
        settings.estimatedInterval = hours * 60;
        settings.save();
    });
</script>

<ConfirmDialog bind:this={dialog} />

<div class="container">
    <h1>Settings</h1>

    <!-- <section>
        <h2>Notifications</h2>
    </section> -->

    <section>
        <h2>Sessions</h2>
        <div class="row mt-3 mb-3">
            <div class="col-6">
                <input bind:value={hours} type="number" step="0.5" class="form-control" />
            </div>
        </div>
    </section>

    <section>
        <h2>Data</h2>
        <button class="btn btn-danger" onclick={async () => await clear()}
            >Clear Data</button
        >
    </section>
</div>
