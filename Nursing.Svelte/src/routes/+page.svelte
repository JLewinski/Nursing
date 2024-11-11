<script lang="ts">
    import Timer from "$lib/components/Timer.svelte";
    import { timerStore } from "$lib/stores/timerStore";
    import { formatDuration } from "$lib/utils/timeCalculations";
    import { lastSession } from "$lib/stores/lastSessionStore";
    import { settings } from "$lib/stores/settingsStore";
    import { Database } from "$lib/db/mod";
    
    const db = new Database();
    const startEvent = timerStore.getStartTime();
    
    function finishSession(){
        if($timerStore.activeTimer === undefined) return;
        
        lastSession.update({ startTime: $startEvent ? new Date($startEvent.timestamp) : null, side: $timerStore.activeTimer });
        
        timerStore.reset();
    }
</script>

<div class="timer-container">
    {#if $timerStore.activeTimer === undefined && $lastSession.startTime}
        <div>
            <div>Last</div>
            <div>{$lastSession.startTime.toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' })}</div>
        </div>
        <div>
            <div>Next</div>
            <div>{new Date($lastSession.startTime.getTime() + $settings.estimatedInterval * 60 * 1000).toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' })}</div>
        </div>
    {:else}
        <span class="grid-item-centered"></span>
    {/if}
    <Timer side="left" />
    <Timer side="right" />
    {#if $timerStore.activeTimer !== undefined}
        <button onclick={() => timerStore.reset()}>Reset</button>
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
