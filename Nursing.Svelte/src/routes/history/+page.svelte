<script lang="ts">
    import { Database } from '$lib/db';
    import type { Session } from '$lib/types';
    import { onMount } from 'svelte';
    
    let sessions: Session[] = [];
    
    onMount(async () => {
        const db = new Database();
        // TODO: Load session history
    });
</script>

<div class="history-page">
    <h1>Nursing History</h1>
    <div class="sessions-list">
        {#each sessions as session}
            <!-- TODO: Session display implementation -->
        {/each}
    </div>
</div>