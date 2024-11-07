<script lang="ts">
  import Timer from '$lib/components/Timer.svelte';
  import { timerStore } from '$lib/stores/timerStore';
</script>

<div class="timer-container">
    <Timer side="left" />
    <Timer side="right" />
    {#if $timerStore.activeTimer !== undefined}
        <button on:click={() => timerStore.reset()}>Reset</button>
        <button on:click={() => timerStore.reset()}>Finish</button>
    {/if}
</div>



<style>
    .timer-container {
        display: grid;
        grid-template-columns: 1fr 1fr;
        grid-template-rows: auto auto;
        gap: 1rem;
        padding: 1rem;
    }
</style>
