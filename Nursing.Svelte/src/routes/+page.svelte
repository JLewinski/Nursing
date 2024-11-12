<script lang="ts">
    import Timer from "$lib/components/Timer.svelte";
    import { getTimerState } from "$lib/stores/timerStore.svelte";
    import { formatDuration } from "$lib/utils/timeCalculations";
    import { lastSession } from "$lib/stores/lastSessionStore";
    import { settings } from "$lib/stores/settingsStore";
    import { Database } from "$lib/db/mod";
    import { v4 } from "uuid";
    import ConfirmDialog from "$lib/components/ConfirmDialog.svelte";

    const db = new Database();

    const timerState = getTimerState();

    console.log(timerState);
    let startEvent = $derived.by(() =>
        timerState.events.length ? timerState.events[0] : null,
    );

    function reset() {
        dialog.showConfirmation('Reset', 'Are you sure you want to reset this feeding?', 'Reset').then((confirmed) => {
            if (confirmed) {
                timerState.reset();
            }
        });
    }

    function finishSession() {
        if (!startEvent) return;

        const lastSide = timerState.events[timerState.events.length - 1].timer;

        lastSession.update({
            startTime: new Date(timerState.events[0].timestamp),
            side: lastSide,
        });

        const now = new Date().toISOString();
        const session = {
            created: startEvent.timestamp,
            startTime: startEvent.timestamp,
            endTime: now,
            lastSide: lastSide,
            lastUpdated: now,
            leftDuration: formatDuration(timerState.calculateDuration("left")),
            rightDuration: formatDuration(
                timerState.calculateDuration("right"),
            ),
            id: v4(),
        };

        db.saveSession(session).then(reset);
    }
    let dialog: ConfirmDialog;
</script>

<div class="timer-container">
    {#if timerState.activeTimer === undefined && $lastSession.startTime}
        <div>
            <div>Last</div>
            <div>
                {$lastSession.startTime.toLocaleTimeString([], {
                    hour: "numeric",
                    minute: "2-digit",
                })}
            </div>
        </div>
        <div>
            <div>Next</div>
            <div>
                {new Date(
                    $lastSession.startTime.getTime() +
                        $settings.estimatedInterval * 60 * 1000,
                ).toLocaleTimeString([], {
                    hour: "numeric",
                    minute: "2-digit",
                })}
            </div>
        </div>
    {:else}
        <span class="grid-item-centered"></span>
    {/if}
    <Timer side="left" />
    <Timer side="right" />
    {#if timerState.activeTimer !== undefined}
        <button class="primary" onclick={reset}>Reset</button>
        <button class="primary" onclick={finishSession}>Finish</button>
    {/if}
</div>

<ConfirmDialog bind:this={dialog}/>

<style>
    .timer-container {
        display: grid;
        grid-template-columns: 1fr 1fr;
        grid-template-rows: auto auto auto;
        gap: 1rem;
        padding: 1rem;
        text-align: center;
    }

    .grid-item-centered {
        grid-column: 1 / span 2;
    }
</style>
