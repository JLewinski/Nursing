<script lang="ts">
    import { timerStore, calculateDuration } from '$lib/stores/timerStore.svelte';
    import { formatDuration } from '$lib/utils/timeCalculations';

    interface Props {
        side: 'left' | 'right' | 'total';
    }

    let { side }: Props = $props();

    let duration = $state(0);
    setInterval(() => {
        duration = calculateDuration(timerStore.events, side);
    }, 100);

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
