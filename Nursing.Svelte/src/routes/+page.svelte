<script lang="ts">
  // let count = $state(1);
  import Timer from '$lib/components/Timer.svelte';
  import { timerStore } from '$lib/stores/timerStore';
</script>
<!-- 
<button
  onclick={() => {
    count++
  }}>need hug right now {"🤗".repeat(count)}</button
> -->

<div class="timer-container">
    <!-- TODO: Implement dual timer interface -->
    <Timer side="left" />
    <Timer side="right" />
</div>

<style>
    .timer-container {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 1rem;
        padding: 1rem;
    }
</style>
