<script lang="ts">
    import Timer from "$lib/components/Timer.svelte";
    import { timerStore } from "$lib/stores/timerStore";
    import { formatDuration } from "$lib/utils/timeCalculations";
    import { lastSession } from "$lib/stores/lastSessionStore.svelte";
    import { settings } from "$lib/stores/settingsStore";
</script>

<div class="timer-container">
    {#if $timerStore.activeTimer === undefined && lastSession.lastStartTime}
        <div>
            <span>Last</span>
            <span>{lastSession.lastStartTime.toLocaleTimeString()}</span>
        </div>
        <div>
            <span>Next</span>
            <span></span>
        </div>
    {:else}
        <span class="grid-item-centered"></span>
    {/if}
    <Timer side="left" />
    <Timer side="right" />
    {#if $timerStore.activeTimer !== undefined}
        <button onclick={() => timerStore.reset()}>Reset</button>
        <button onclick={() => timerStore.reset()}>Finish</button>
    {/if}
</div>

<style>
    .timer-container {
        display: grid;
        grid-template-columns: 1fr 1fr;
        grid-template-rows: auto auto auto;
        gap: 1rem;
        padding: 1rem;
    }

    .grid-item-centered {
        grid-column: 1 / span 2;
        text-align: center;
    }
</style>
