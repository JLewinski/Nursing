<script lang="ts">
    import { onMount } from 'svelte';
    import { timerStore } from '$lib/stores/timerStore';
    import type { Session, TimerEvent } from '$lib/types';
    import { Database } from '$lib/db/mod';
    interface Props {
        children?: import('svelte').Snippet<[any]>;
    }

    let { children }: Props = $props();
    
    let currentSession: Session | null = $state(null);
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
        const activeSession = await db.getActiveSession();
        if (activeSession) {
            currentSession = activeSession as Session;
        }
    });
</script>

{#if currentSession}
    {@render children?.({ session: currentSession, })}
{/if}