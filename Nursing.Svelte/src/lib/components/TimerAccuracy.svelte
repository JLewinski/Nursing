<script lang="ts">
    import { onMount, onDestroy } from 'svelte';
    import { timerStore } from '$lib/stores/timerStore';
    import { wakeLockManager } from '$lib/utils/wakelock';

    let lastVisibilityChange = Date.now();
    let rafId: number;

    function updateTimer() {
        const now = Date.now();
        if (document.hidden) {
            // Store the time difference for accuracy
            lastVisibilityChange = now;
        }
        
        // Request next frame if timer is active
        if ($timerStore.activeTimer) {
            rafId = requestAnimationFrame(updateTimer);
        }
    }

    onMount(() => {
        document.addEventListener('visibilitychange', updateTimer);
        if ($timerStore.activeTimer) {
            wakeLockManager.requestWakeLock();
            rafId = requestAnimationFrame(updateTimer);
        }
    });

    onDestroy(() => {
        document.removeEventListener('visibilitychange', updateTimer);
        if (rafId) cancelAnimationFrame(rafId);
        wakeLockManager.releaseWakeLock();
    });
</script>