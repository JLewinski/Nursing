import { db } from "../db/mod";

export class SyncState {
    lastSync: Date | null = $state(null);
    status: "idle" | "syncing" | "error" | "na" = $state("na");
    result?: string;
    id?: number;

    constructor() {
        this.load();
    }

    async load() {
        const data = await db.syncState.orderBy("id").last();
        if (!data) {
            this.result = undefined;
            this.status = "na";
            this.lastSync = null;
        }
        else {
            this.result = data.result;
            this.status = data.syncStatus;
            this.lastSync = data.lastSync;
            this.id = data.id;
        }
    }

    async syncData(): Promise<void> {

        if (this.status === "syncing") {
            console.log('Sync already in progress');
            return;
        }
        this.status = "syncing";

        const lastSync = this.lastSync;

        this.lastSync = new Date();

        await db.syncState.add({
            lastSync: new Date(),
            syncStatus: 'syncing',
            result: undefined
        });

        await this.load();

        let data;
        if (lastSync === null) {
            data = await db.sessions.toArray();
        } else {
            data = await db.sessions.where('lastUpdated').aboveOrEqual(lastSync).toArray();
        }

        const result = await fetch('/account?/sync', {
            method: 'POST',
            body: JSON.stringify({
                syncDate: lastSync,
                sessions: data
            }),
        });

        console.log('Sync result', await result.json());

        if (result.ok) {
            await this.syncComplete('Sync successful', true);
        } else {
            await this.syncComplete('Sync failed', false);
        }

    }

    async syncComplete(result: string, isSuccess: boolean): Promise<void> {

        if (this.status !== "syncing" || this.id === undefined) {
            throw new Error('Sync not in progress');
        }

        await db.syncState.where('id').equals(this.id).modify({
            syncStatus: isSuccess ? 'idle' : 'error',
            result: result
        });

        await this.load();

    }
}

export const syncStore = new SyncState();
