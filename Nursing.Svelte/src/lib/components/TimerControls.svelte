<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import { formatDuration } from '$lib/utils/timeCalculations';
    
    export let side: 'left' | 'right';
    export let isActive = false;
    export let duration = 0;
    
    const dispatch = createEventDispatcher();
</script>

<div class="timer-controls {side}">
    <button 
        class="timer-button {isActive ? 'active' : ''}"
        on:click={() => dispatch('toggle')}
    >
        {isActive ? 'Stop' : 'Start'}
    </button>
    
    <div class="timer-display">
        {formatDuration(duration)}
    </div>
</div>

<style>
    .timer-controls {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 1rem;
    }

    .timer-button {
        padding: 1rem 2rem;
        border-radius: 9999px;
        background: var(--primary-color);
        color: white;
        border: none;
    }

    .timer-button.active {
        background: var(--error-color);
    }
</style>