<script lang="ts">
    import { timerStore } from '$lib/stores/timerStore.svelte';
    import { lastSession } from '$lib/stores/lastSessionStore';
    import Duration from './Duration.svelte';
    
    interface Props {
        side: 'left' | 'right';
    }

    let { side }: Props = $props();
    
    let isActive = $derived(timerStore.activeTimer === side);
    
    function handleClick() {
        const now = new Date().toISOString();
        if (timerStore.activeTimer) {
            timerStore.events.push({
                timer: timerStore.activeTimer,
                timestamp: now,
                type: 'stop',
            });
        }

        if(timerStore.activeTimer === side) {
            timerStore.activeTimer = null;
            return;
        }

        timerStore.activeTimer = side;
        timerStore.events.push({
            timer: side,
            timestamp: new Date().toISOString(),
            type: 'start',
        });
    }
</script>

<div class="timer-container">
    {#if $lastSession.side === side}
        <span class="badge">Last used</span>
    {:else}
        <span></span>
    {/if}
    <span class="label">{side}</span>
    <button 
        class="timer-circle {isActive ? 'active' : ''}"
        onclick={handleClick}
        aria-label="{isActive ? 'Stop' : 'Start'} {side} timer"
    >
        <Duration {side} />
    </button>
</div>

<style>
    .timer-container {
        display: grid;
        grid-template-rows: 1.5rem auto auto;
        justify-items: center;
        gap: 1rem;
    }

    .badge {
        font-size: 1rem;
        font-weight: 500;
        border: none;
        border-radius: 20%;
        padding: 1%;
        background-color: var(--primary-dark);
        height: min-content;
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

    @keyframes pulse {
        0% { transform: scale(1); }
        50% { transform: scale(1.05); }
        100% { transform: scale(1); }
    }
</style>