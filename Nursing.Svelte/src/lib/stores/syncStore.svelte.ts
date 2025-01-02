import { db, type DBSession } from "$lib/db/mod";
import { ResponseError, SyncApi, type FeedingDto, type SyncResult } from "$lib/api";
import { formatDuration, parseDuration } from "$lib/utils/timeCalculations";

export class SyncState {
    lastSync: Date | undefined = $state(undefined);
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
            this.lastSync = undefined;
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

        const client = new SyncApi();
        try {
            const result = await client.sync({
                syncModel: {
                    lastSync: this.lastSync,
                    feedings: data.map(x => ({
                        deleted: x.deleted,
                        finished: x.endTime,
                        id: x.id,
                        lastIsLeft: x.lastSide === 'left',
                        lastUpdated: x.lastUpdated,
                        leftBreastTotal: formatDuration(x.leftDuration),
                        rightBreastTotal: formatDuration(x.rightDuration),
                        started: x.startTime,
                        totalTime: formatDuration(x.leftDuration + x.rightDuration)
                    } as FeedingDto))
                }
            });

            await this.updateLocal(result);
        }
        catch (e) {
            const error = e as ResponseError;
            this.status = 'error';
            this.result = await error.response.text();
            await this.save();
            return;
        }
    }

    private async updateLocal(result: SyncResult) {
        const feedings: DBSession[] = result.feedings.map(session => ({
            created: new Date(),
            endTime: session.finished ?? new Date(),
            id: session.id!,
            lastSide: session.lastIsLeft ? 'left' : 'right',
            lastUpdated: session.lastUpdated!,
            leftDuration: parseDuration(session.leftBreastTotal!),
            rightDuration: parseDuration(session.rightBreastTotal!),
            startTime: session.started!,
            deleted: session.deleted ?? undefined
        }));

        for (const session of feedings) {
            console.log('Updating session', session);
            await db.sessions.put(session);
        }

        this.status = 'idle';
        this.result = 'Sync complete';

        await this.save();
    }

    private async save() {
        if (this.id === undefined || this.status === 'na' || this.lastSync === undefined) {
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
