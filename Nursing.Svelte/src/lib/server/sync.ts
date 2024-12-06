import { db } from '$lib/server/db';
import type { DBSession } from '$lib/db/mod';
import type { FeedingSession } from './db/schema';
import * as table from './db/schema';
import type { PgTable } from 'drizzle-orm/pg-core';
import { getTableColumns, SQL, sql } from 'drizzle-orm';

function buildConflictUpdateColumns<
    T extends PgTable,
    Q extends keyof T['_']['columns']
>(table: T, columns?: Q[]) {
    const cls = getTableColumns(table);

    if (!columns?.length) {
        columns = Object.keys(cls).filter((key) => key as Q != 'id') as Q[];
    }

    return columns.reduce((acc, column) => {
        const colName = cls[column].name;
        acc[column] = sql.raw(`excluded.${colName}`);

        return acc;
    }, {} as Record<Q, SQL>);
};

export default async function sync(syncDate: Date | null, userId: string, sessions: DBSession[]) {
    const updated = await retrieveUpdatedSessions(syncDate, userId);
    await updateSessions(userId, sessions);
    return updated;
}

async function updateSessions(userId: string, sessions: DBSession[]) {

    if(sessions.length === 0) {
        return;
    }

    const dtos = sessions.map(x => toDto(x, userId));
    console.log('Updating sessions', dtos);
    await db.insert(table.feedingSession).values(dtos).onConflictDoUpdate({
        target: table.feedingSession.id,
        set: buildConflictUpdateColumns(table.feedingSession)
    });
}

async function retrieveUpdatedSessions(syncDate: Date | null, userId: string) {
    const result = await db.query.feedingSession.findMany({
        where: (feedingSession, { eq, gt, and }) => {
            if (syncDate){
                return and(
                    eq(feedingSession.userId, userId),
                    gt(feedingSession.lastUpdated, syncDate)
                );
            }
                
            return eq(feedingSession.userId, userId);
        }
    });

    return result;
}

function toDto(session: DBSession, userId: string): FeedingSession {
    return {
        id: session.id,
        userId: userId,
        startTime: new Date(session.startTime),
        endedTime: new Date(session.endTime),
        lastSide: session.lastSide === "left",
        leftDuration: session.leftDuration,
        rightDuration: session.rightDuration,
        lastUpdated: new Date(session.lastUpdated),
        created: new Date(session.created),
        deleted: session.deleted ? new Date(session.deleted) : null
    };
}