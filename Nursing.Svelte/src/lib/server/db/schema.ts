import { relations } from 'drizzle-orm';
import { pgTable, serial, text, integer, timestamp, boolean } from 'drizzle-orm/pg-core';

export const user = pgTable('user', {
    id: text('id').primaryKey(),
    username: text('username').notNull().unique(),
    passwordHash: text('password_hash').notNull()
});

export const session = pgTable("session", {
    id: text('id').primaryKey(),
    userId: text('user_id').notNull().references(() => user.id),
    expiresAt: timestamp('expires_at', { withTimezone: true, mode: 'date' }).notNull()
});

export const group = pgTable('userGroup', {
    id: text('id').primaryKey(),
});

export const userGroup = pgTable('userGroupConnection', {
    userId: text('user_id').notNull().references(() => user.id),
    groupId: text('group_id').notNull().references(() => group.id)
});

export const feedingSession = pgTable('feedingSession', {
    id: text('id').primaryKey(),
    userId: text('user_id').notNull().references(() => user.id),
    startTime: timestamp('started_at', { withTimezone: true, mode: 'date' }).notNull(),
    endedTime: timestamp('ended_at', { withTimezone: true, mode: 'date' }).notNull(),
    lastSide: boolean('last_side').notNull(),
    leftDuration: integer('left_duration').notNull(),
    rightDuration: integer('right_duration').notNull(),
    lastUpdated: timestamp('last_updated', { withTimezone: true, mode: 'date' }).notNull(),
    created: timestamp('created', { withTimezone: true, mode: 'date' }).notNull(),
    deleted: timestamp('deleted', { withTimezone: true, mode: 'date' })
});

export type UserSession = typeof session.$inferSelect;

export type User = typeof user.$inferSelect;

export type FeedingSession = typeof feedingSession.$inferSelect;

export type UserGroup = typeof group.$inferSelect;