import { migrateOldData } from "$lib/db/migration_old.ts";

export const migrations = [
    {
        version: 1,
        migrate: (db: IDBDatabase) => {
            const sessionStore = db.createObjectStore("sessions", {
                keyPath: "id",
            });
            sessionStore.createIndex("startTime", "startTime");
            sessionStore.createIndex("lastUpdated", "lastUpdated");
            sessionStore.createIndex("deleted", "deleted", { unique: false }); // Modified index for datetime

            migrateOldData().forEach((session) => sessionStore.add(session));
        },
    },
    // Future migrations will be added here
];

export function applyMigrations(db: IDBDatabase, oldVersion: number) {
    for (const migration of migrations) {
        if (migration.version > oldVersion) {
            migration.migrate(db);
        }
    }
}
