<script lang="ts">
    import type { Session } from '$lib/types';
    import { formatDate, formatTime } from '$lib/utils/dateFormatters';
    import { calculateDuration } from '$lib/utils/timeCalculations';

    export let session: Session;
    
    $: duration = calculateDuration(session.timerEvents);
</script>

<div class="session-card">
    <div class="session-header">
        <span class="date">{formatDate(new Date(session.startTime))}</span>
        <span class="time">{formatTime(new Date(session.startTime))}</span>
    </div>
    <div class="session-details">
        <!-- TODO: Implement session details display -->
    </div>
</div>

<style>
    .session-card {
        padding: 1rem;
        background: var(--surface-color);
        border-radius: 0.5rem;
        box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }
</style>