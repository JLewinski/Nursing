export interface ISyncState {
    lastSync: string | null;
    status: "idle" | "syncing" | "error";
    error?: string;
}

export class SyncState implements ISyncState {
    lastSync: string | null = null;
    status: "idle" | "syncing" | "error" = "idle";
    error?: string;

    constructor() {
        this.load();
    }

    save() {
        const data: ISyncState = {
            lastSync: this.lastSync,
            status: this.status,
            error: this.error,
        };
        localStorage.setItem("syncStore", JSON.stringify(data));
    }

    load() {
        const stored = localStorage.getItem("syncStore");
        if (stored) {
            const data = JSON.parse(stored) as ISyncState;
            this.lastSync = data.lastSync;
            this.status = data.status;
            this.error = data.error;
        }
    }

    async syncData(): Promise<void> {
        this.lastSync = new Date().toISOString();
        this.status = "idle";
        
    }
}

export const syncStore = $state(new SyncState());
