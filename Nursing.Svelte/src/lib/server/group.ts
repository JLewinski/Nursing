import { db } from '$lib/server/db';
import * as table from './db/schema';
import { aliasedTable, eq, or } from 'drizzle-orm';

export async function group(user1: string, user2: string) {

    if (user1 === user2) {
        return [];
    }

    const userTable2 = aliasedTable(table.userGroup, 'user2');
    const existing = await db
        .select({ groupId: table.group.id, user1: table.userGroup.userId, user2: userTable2.userId })
        .from(table.group)
        .leftJoin(table.userGroup, eq(table.group.id, table.userGroup.groupId))
        .leftJoin(userTable2, eq(table.group.id, table.userGroup.groupId))
        .where(or(eq(table.userGroup.userId, user1), eq(userTable2.userId, user2)))

    const sameGroups = existing.filter(x => x.user1 === user1 && x.user2 === user2);

    if (sameGroups.length) {
        return sameGroups.map(x => x.groupId);
    }

    const groupIds = [] as string[];
    const additions = [] as { userId: string, groupId: string }[];

    for (const group of existing.filter(x => x.user1 === user1).map(x => x.groupId)) {
        if (groupIds.includes(group)) {
            continue;
        }
        groupIds.push(group);
        additions.push({ userId: user2, groupId: group });
    }

    for (const group of existing.filter(x => x.user2 === user2).map(x => x.groupId)) {
        if (groupIds.includes(group)) {
            continue;
        }
        groupIds.push(group);
        additions.push({ userId: user1, groupId: group });
    }

    if (groupIds.length === 0) {
        const newGroup = await db.insert(table.group).values({ id: '' }).returning({ id: table.group.id });
        const groupId = newGroup[0].id;
        groupIds.push(groupId);
        additions.push({ userId: user1, groupId });
        additions.push({ userId: user2, groupId });
    }

    await db.insert(table.userGroup).values(additions);

    return groupIds;
}

export async function getGroups(userId: string) {
    const userTable2 = aliasedTable(table.userGroup, 'user2');
    const existing = await db
        .select({ userId: userTable2.userId })
        .from(table.group)
        .leftJoin(table.userGroup, eq(table.group.id, table.userGroup.groupId))
        .leftJoin(userTable2, eq(table.group.id, table.userGroup.groupId))
        .where(or(eq(table.userGroup.userId, userId)));
    
    return existing.map(x => x.userId);
}