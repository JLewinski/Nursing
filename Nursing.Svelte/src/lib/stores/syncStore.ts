import { writable, get } from 'svelte/store';

interface SyncState {
    lastSync: string | null;
    status: 'idle' | 'syncing' | 'error';
    error?: string;
}

function createSyncStore() {
    const { subscribe, set, update } = writable<SyncState>({
        lastSync: null,
        status: 'idle'
    });

    return {
        subscribe,
        startSync: () => update(state => ({ ...state, status: 'syncing', error: undefined })),
        syncComplete: () => update(state => ({
            ...state,
            status: 'idle',
            lastSync: new Date().toISOString(),
            error: undefined
        })),
        syncError: (error: string) => update(state => ({
            ...state,
            status: 'error',
            error
        })),
        reset: () => set({
            lastSync: null,
            status: 'idle'
        })
    };
}

export const syncStore = createSyncStore();