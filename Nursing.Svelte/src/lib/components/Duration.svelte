<script lang="ts">
    import { getTimerState } from '$lib/stores/timerStore.svelte';
    import { formatDuration } from '$lib/utils/timeCalculations';
    import { onDestroy } from 'svelte';

    const timerState = getTimerState();
    interface Props {
        side: 'left' | 'right' | 'total';
    }

    let { side }: Props = $props();

    let duration = $state(0);
    const intervalKey = setInterval(() => {
        duration = timerState.calculateDuration(side);
    }, 100);

    onDestroy(() => {
        clearInterval(intervalKey);
    });

</script>

<div class="timer-display">
    {#if duration}
        {formatDuration(duration)}
    {:else}
        0:00
    {/if}
</div>

<style>
    .timer-display {
        font-size: 2rem;
        font-weight: bold;
        font-family: monospace;
    }
</style>
