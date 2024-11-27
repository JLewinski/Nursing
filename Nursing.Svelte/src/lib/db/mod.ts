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
        id: string;
        lastSyncTimestamp: string;
        syncStatus: "idle" | "syncing" | "error";
    };
}

export type DBSession = DBSchema["sessions"];
export const db = new Dexie("NursingDB") as Dexie & {
    sessions: EntityTable<DBSchema["sessions"]>;
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
