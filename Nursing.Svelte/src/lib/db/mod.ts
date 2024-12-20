import { applyMigrations } from './migrations.ts';

interface DBSchema {
    sessions: {
        id: string;
        startTime: string;
        endTime: string;
        lastSide: 'left' | 'right';
        leftDuration: number;
        rightDuration: number;
        lastUpdated: string;
        created: string;
        deleted?: string;
    };
    syncState: {
        id: string;
        lastSyncTimestamp: string;
        syncStatus: 'idle' | 'syncing' | 'error';
    };
}

export type DBSession = DBSchema['sessions'];

export class Database {
    private db: IDBDatabase | null = null;
    private readonly DB_NAME = 'NursingDB';
    private readonly DB_VERSION = 1;

    // deno-lint-ignore require-await
    async init(): Promise<void> {
        if (this.db) return;

        return new Promise((resolve, reject) => {
            const request = indexedDB.open(this.DB_NAME, this.DB_VERSION);

            request.onerror = () => reject(request.error);
            request.onsuccess = () => {
                this.db = request.result;
                resolve();
            };
            request.onupgradeneeded = (event) => {
                const db = request.result;
                const oldVersion = event.oldVersion;
                applyMigrations(db, oldVersion);
            };
        });
    }

    async deleteSession(id: string): Promise<void> {
        await this.ensureInit();
        const session = await this.getSession(id);
        if (!session) return;

        session.deleted = new Date().toISOString();
        return this.put('sessions', session);
    }

    async removeSession(id: string): Promise<void> {
        await this.ensureInit();
        return new Promise((resolve, reject) => {
            const transaction = this.db!.transaction('sessions', 'readwrite');
            const objectStore = transaction.objectStore('sessions');
            const request = objectStore.delete(id);

            request.onerror = () => reject(request.error);
            request.onsuccess = () => resolve();
        });
    }

    async clearSessions(): Promise<void> {
        await this.ensureInit();
        return new Promise((resolve, reject) => {
            const transaction = this.db!.transaction('sessions', 'readwrite');
            const objectStore = transaction.objectStore('sessions');
            const request = objectStore.clear();

            request.onerror = () => reject(request.error);
            request.onsuccess = () => resolve();
        });
    }

    async saveSession(session: DBSchema['sessions']): Promise<void> {
        await this.ensureInit();
        return this.put('sessions', session);
    }

    async getSession(id: string): Promise<DBSchema['sessions'] | undefined> {
        await this.ensureInit();
        return this.get('sessions', id);
    }

    async getAllSessions(): Promise<DBSchema['sessions'][]> {
        await this.ensureInit();
        return this.getAll('sessions');
    }

    async getActiveSession(): Promise<DBSchema['sessions'] | undefined> {
        await this.ensureInit();
        const sessions = await this.getAll('sessions');
        return sessions.find(session => !session.endTime && !session.deleted);
    }

    async saveSyncState(state: DBSchema['syncState']): Promise<void> {
        await this.ensureInit();
        return this.put('syncState', state);
    }

    async getSyncState(id: string): Promise<DBSchema['syncState'] | undefined> {
        await this.ensureInit();
        return this.get('syncState', id);
    }

    private async ensureInit(): Promise<void> {
        if (!this.db) {
            await this.init();
        }
    }

    private put<T extends keyof DBSchema>(store: T, value: DBSchema[T]): Promise<void> {
        return new Promise((resolve, reject) => {
            const transaction = this.db!.transaction(store, 'readwrite');
            const objectStore = transaction.objectStore(store);
            const request = objectStore.put(value);

            request.onerror = () => reject(request.error);
            request.onsuccess = () => resolve();
        });
    }

    private get<T extends keyof DBSchema>(store: T, key: string): Promise<DBSchema[T] | undefined> {
        return new Promise((resolve, reject) => {
            const transaction = this.db!.transaction(store, 'readonly');
            const objectStore = transaction.objectStore(store);
            const request = objectStore.get(key);

            request.onerror = () => reject(request.error);
            request.onsuccess = () => resolve(request.result);
        });
    }

    private getAll<T extends keyof DBSchema>(store: T): Promise<DBSchema[T][]> {
        return new Promise((resolve, reject) => {
            const transaction = this.db!.transaction(store, 'readonly');
            const objectStore = transaction.objectStore(store);
            const request = objectStore.getAll();

            request.onerror = () => reject(request.error);
            request.onsuccess = () => resolve(request.result);
        });
    }
}