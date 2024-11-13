import { Database } from "$lib/db/mod.ts";
import { syncStore } from "$lib/stores/syncStore.svelte.ts";

export class SyncManager {
    private db: Database;
    private syncInProgress = false;

    constructor() {
        this.db = new Database();
    }

    async registerSyncTask(): Promise<void> {
        if ("serviceWorker" in navigator) {
            
            const registration = await navigator.serviceWorker.ready;
            if ("sync" in registration) {
                try {
                    await registration.sync.register("sync-sessions");
                } catch (error) {
                    console.error("Background sync registration failed:", error);
                }
            }
        }
    }

    async syncData(): Promise<void> {
        if (this.syncInProgress) return;
        await syncStore.syncData();
    }
}
