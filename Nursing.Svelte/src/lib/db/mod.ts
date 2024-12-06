import { Dexie, type EntityTable } from "dexie";

interface DBSchema {
    sessions: {
        id: string;
        startTime: Date;
        endTime: Date;
        lastSide: "left" | "right";
        leftDuration: number;
        rightDuration: number;
        lastUpdated: Date;
        created: Date;
        deleted?: Date;
    };
    syncState: {
        id?: number;
        lastSync: Date;
        syncStatus: "idle" | "syncing" | "error";
        result?: string;
    };
}

export type DBSession = DBSchema["sessions"];
export type DBSyncState = DBSchema["syncState"];
export const db = new Dexie("NursingDB") as Dexie & {
    sessions: EntityTable<DBSchema["sessions"]>;
    syncState: EntityTable<DBSchema["syncState"]>;
};

db.version(0.2).stores({
    sessions: "id,deleted,lastUpdated,startTime",
}).upgrade((tx) => {
    return tx.table("sessions").toCollection().modify((session) => {
        session.endTime = new Date(session.endTime);
        session.created = new Date(session.created);
        session.startTime = new Date(session.startTime);
        session.lastUpdated = new Date(session.lastUpdated);
        session.deleted = session.deleted
            ? new Date(session.deleted)
            : undefined;
    });
});

db.version(1).stores({
    syncState: "++id,lastSync",
});