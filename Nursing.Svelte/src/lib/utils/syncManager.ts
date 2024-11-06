import type { Session } from '$lib/types';
import { Database } from '$lib/db';
import { syncState } from '$lib/stores/syncStore';

export class SyncManager {
    private db: Database;
    private syncInProgress = false;

    constructor() {
        this.db = new Database();
    }

    async registerSyncTask(): Promise<void> {
        if ('serviceWorker' in navigator && 'sync' in registration) {
            const registration = await navigator.serviceWorker.ready;
            try {
                await registration.sync.register('sync-sessions');
            } catch (error) {
                console.error('Background sync registration failed:', error);
            }
        }
    }

    async syncData(): Promise<void> {
        if (this.syncInProgress) return;

        this.syncInProgress = true;
        syncState.set({ lastSync: null, status: 'syncing' });

        try {
            const sessions = await this.db.getUnsynced();
            // TODO: Implement sync with backend
            
            syncState.set({
                lastSync: new Date().toISOString(),
                status: 'idle'
            });
        } catch (error) {
            syncState.set({
                lastSync: null,
                status: 'error',
                error: error.message
            });
        } finally {
            this.syncInProgress = false;
        }
    }
}