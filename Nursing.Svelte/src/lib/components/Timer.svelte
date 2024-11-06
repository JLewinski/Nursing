<script lang="ts">
    let { side } = $props<{ side: 'left' | 'right' }>();
    import { timerStore } from '$lib/stores/timerStore';
    
    // TODO: Implement timer logic and UI
</script>

<div class="timer {side}">
    <button>Start {side}</button>
    <div class="timer-display">
        00:00
    </div>
</div>

<style>
    .timer {
        padding: 1rem;
        border-radius: 0.5rem;
        background: var(--surface-color);
    }
</style>