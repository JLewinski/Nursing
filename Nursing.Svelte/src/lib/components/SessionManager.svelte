<script lang="ts">
    import { onMount } from 'svelte';
    import { timerStore } from '$lib/stores/timerStore';
    import type { Session, TimerEvent } from '$lib/types';
    import { Database } from '$lib/db';
    
    let currentSession: Session | null = null;
    const db = new Database();

    async function createSession() {
        currentSession = {
            id: crypto.randomUUID(),
            timerEvents: [],
            startTime: new Date().toISOString(),
            endTime: '',
            lastUpdated: new Date().toISOString(),
            created: new Date().toISOString()
        };
        await db.saveSession(currentSession);
    }

    async function updateSession(event: TimerEvent) {
        if (!currentSession) return;
        
        currentSession.timerEvents.push(event);
        currentSession.lastUpdated = new Date().toISOString();
        
        if (event.type === 'stop' && !isAnyTimerActive()) {
            currentSession.endTime = new Date().toISOString();
        }
        
        await db.saveSession(currentSession);
    }

    function isAnyTimerActive(): boolean {
        return $timerStore.activeTimer !== null;
    }

    onMount(async () => {
        // Resume active session if exists
        currentSession = await db.getActiveSession();
    });
</script>

{#if currentSession}
    <slot session={currentSession} />
{/if}