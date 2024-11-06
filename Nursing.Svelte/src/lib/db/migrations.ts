import { DB_CONFIG } from '$lib/config/constants';

export const migrations = [
    {
        version: 1,
        migrate: async (db: IDBDatabase) => {
            const sessionStore = db.createObjectStore('sessions', { keyPath: 'id' });
            sessionStore.createIndex('startTime', 'startTime');
            sessionStore.createIndex('lastUpdated', 'lastUpdated');
            sessionStore.createIndex('deleted', 'deleted');

            const settingsStore = db.createObjectStore('settings', { keyPath: 'id' });
            const syncStateStore = db.createObjectStore('syncState', { keyPath: 'id' });
        }
    }
    // Future migrations will be added here
];

export async function applyMigrations(db: IDBDatabase, oldVersion: number): Promise<void> {
    for (const migration of migrations) {
        if (migration.version > oldVersion) {
            await migration.migrate(db);
        }
    }
}