<script lang="ts">
    import Timer from "$lib/components/Timer.svelte";
    import { getTimerState } from "$lib/stores/timerStore.svelte";
    import {
        formatDuration,
        formatLongDuration,
    } from "$lib/utils/timeCalculations";
    import { lastSession } from "$lib/stores/lastSessionStore.svelte";
    import { settings } from "$lib/stores/settingsStore.svelte";
    import { Database } from "$lib/db/mod";
    import { v4 } from "uuid";
    import ConfirmDialog from "$lib/components/ConfirmDialog.svelte";
    import { onDestroy } from "svelte";

    const db = new Database();

    const timerState = getTimerState();

    let startEvent = $derived.by(() =>
        timerState.events.length ? timerState.events[0] : null,
    );

    let nextStartTime = $derived.by(() => {
        if (!lastSession.startTime) return null;

        return new Date(
            lastSession.startTime.getTime() +
                settings.estimatedInterval * 60 * 1000,
        );
    });

    let durationStart = $state("");
    let durationNext = $state("");

    function setDurations(){
        if (lastSession.startTime && nextStartTime) {
            const now = Date.now();
            durationStart = formatLongDuration(
                now - lastSession.startTime.getTime(),
            );
            durationNext = formatLongDuration(nextStartTime.getTime() - now);
        }
    }
    setDurations();

    const intervalKey = setInterval(() => {
        setDurations();
    }, 5000);

    function reset() {
        dialog
            .showConfirmation(
                "Reset",
                "Are you sure you want to reset this feeding?",
                "Reset",
            )
            .then((confirmed) => {
                if (confirmed) {
                    timerState.reset();
                }
            });
    }

    function finishSession() {
        if (!startEvent) return;

        const lastSide = timerState.events[timerState.events.length - 1].timer;

        lastSession.startTime = new Date(timerState.events[0].timestamp);
        lastSession.side = lastSide;
        lastSession.save();

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

        db.saveSession(session).then(() => {
            timerState.reset();
        });
    }
    let dialog: ConfirmDialog;

    onDestroy(() => {
        clearInterval(intervalKey);
    });
</script>

<div class="timer-container">
    {#if timerState.activeTimer === undefined && lastSession.startTime && nextStartTime}
        <div class="h4">
            <div class="p-2">Last</div>
            <div class="bg-body-secondary p-2">
                {lastSession.startTime.toLocaleTimeString([], {
                    hour: "numeric",
                    minute: "2-digit",
                })}
            </div>
            <div class="bg-body-tertiary p-2">
                {durationStart}
            </div>
        </div>
        <div class="h4">
            <div class="p-2">Next</div>
            <div class="bg-body-secondary p-2">
                {nextStartTime.toLocaleTimeString([], {
                    hour: "numeric",
                    minute: "2-digit",
                })}
            </div>
            <div class="bg-body-tertiary p-2">
                {durationNext}
            </div>
        </div>
    {:else}
        <span class="grid-item-centered"></span>
    {/if}
    <Timer side="left" />
    <Timer side="right" />
    {#if timerState.activeTimer !== undefined}
        <button class="btn btn-primary" onclick={reset}>Reset</button>
        <button class="btn btn-primary" onclick={finishSession}>Finish</button>
    {/if}
</div>

<ConfirmDialog bind:this={dialog} />

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
