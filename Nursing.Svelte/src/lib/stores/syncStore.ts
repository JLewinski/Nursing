import { writable } from 'svelte/store';

interface SyncState {
    lastSync: string | null;
    status: 'idle' | 'syncing' | 'error';
    error?: string;
}

// TODO: Implement sync logic
export const syncState = writable<SyncState>({
    lastSync: null,
    status: 'idle'
});