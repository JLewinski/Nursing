<script lang="ts">
    import { timerStore } from '$lib/stores/timerStore';
    import { formatDuration } from '$lib/utils/timeCalculations';
    
    interface Props {
        side: 'left' | 'right';
    }

    let { side }: Props = $props();
    
    let isActive = $derived($timerStore.activeTimer === side);
    const durationStore = timerStore.getDuration(side);
    
    function handleClick() {
        timerStore.toggleTimer(side);
    }
</script>

<div class="timer-container">
    <span class="label">{side}</span>
    <button 
        class="timer-circle {isActive ? 'active' : ''}"
        onclick={handleClick}
        aria-label="{isActive ? 'Stop' : 'Start'} {side} timer"
    >
        <div class="timer-display">
            {#if $durationStore}
                {formatDuration($durationStore)}
            {:else}
                0:00
            {/if}
        </div>
    </button>
</div>

<style>
    .timer-container {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 1rem;
    }

    .label {
        text-transform: capitalize;
        font-size: 1.2rem;
        font-weight: 500;
    }

    .timer-circle {
        width: 150px;
        height: 150px;
        border-radius: 50%;
        border: none;
        background: var(--primary-dark);
        display: grid;
        place-items: center;
        cursor: pointer;
        transition: all 0.3s ease;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }

    .timer-circle:hover {
        transform: scale(1.05);
    }

    .timer-circle.active {
        background: var(--primary-light);
        color: white;
        animation: pulse 2s infinite;
    }

    .timer-display {
        font-size: 2rem;
        font-weight: bold;
        font-family: monospace;
    }

    @keyframes pulse {
        0% { transform: scale(1); }
        50% { transform: scale(1.05); }
        100% { transform: scale(1); }
    }
</style>