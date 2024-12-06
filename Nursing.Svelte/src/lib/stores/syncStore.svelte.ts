import { db, type DBSession, type DBSyncState } from "../db/mod";

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

        const result = await fetch('/sync', {
            method: 'POST',
            body: JSON.stringify({
                syncDate: lastSync,
                sessions: data
            })
        });

        if (result.ok === false) {
            this.status = 'error';
            this.result = result.statusText;
            await this.save();
            return;
        }

        const resultData = await (async () => {
            const data: { status: 'ok', updates: DBSession[] } | { status: 'error', error: string } = await result.json();

            if (data.status === 'ok') {
                data.updates = data.updates.map(x => {
                    x.startTime = new Date(x.startTime);
                    x.endTime = new Date(x.endTime);
                    x.lastUpdated = new Date(x.lastUpdated);
                    x.created = new Date(x.created);
                    x.deleted = x.deleted ? new Date(x.deleted) : undefined;
                    return x;
                });
            }

            return data;
        })();

        if (resultData.status === 'ok') {
            for (const session of resultData.updates) {
                console.log('Updating session', session);
                await db.sessions.put(session);
            }

            this.status = 'idle';
            this.result = 'Sync complete';
        } else {
            this.status = 'error';
            this.result = resultData.error;
        }

        await this.save();
    }

    private async save() {
        if (this.id === undefined || this.status === 'na' || this.lastSync === null) {
            throw new Error('Sync not in progress');
        }

        await db.syncState.put({
            id: this.id,
            lastSync: this.lastSync,
            syncStatus: this.status,
            result: this.result
        });
    }
}

export const syncStore = new SyncState();
