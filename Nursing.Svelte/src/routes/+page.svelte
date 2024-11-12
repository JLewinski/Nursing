<script lang="ts">
    import Timer from "$lib/components/Timer.svelte";
    import { timerStore, calculateDuration } from "$lib/stores/timerStore.svelte";
    import { formatDuration } from "$lib/utils/timeCalculations";
    import { lastSession } from "$lib/stores/lastSessionStore";
    import { settings } from "$lib/stores/settingsStore";
    import { Database } from "$lib/db/mod";
    import { v4 } from "uuid";

    const db = new Database();
    
    let startEvent = $derived.by(() => timerStore.events.length ? timerStore.events[0] : null);

    function reset(){
        timerStore.activeTimer = undefined;
        timerStore.events = [];
    }

    function finishSession() {
        if (!startEvent) return;

        const lastSide = timerStore.events[timerStore.events.length - 1].timer;

        lastSession.update({
            startTime: new Date(timerStore.events[0].timestamp),
            side: lastSide,
        });

        const now = new Date().toISOString();
        const session = {
            created: startEvent.timestamp,
            startTime: startEvent.timestamp,
            endTime: now,
            lastSide: lastSide,
            lastUpdated: now,
            leftDuration: formatDuration(
                calculateDuration(timerStore.events, "left"),
            ),
            rightDuration: formatDuration(
                calculateDuration(timerStore.events, "right"),
            ),
            id: v4(),
        };

        db.saveSession(session).then(reset);
    }
</script>
<div class="timer-container">
    {#if timerStore.activeTimer === undefined && $lastSession.startTime}
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
    {#if timerStore.activeTimer !== undefined}
        <button onclick={reset}>Reset</button>
        <button onclick={finishSession}>Finish</button>
    {/if}
</div>

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
