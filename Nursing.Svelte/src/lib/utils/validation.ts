import type { Session, TimerEvent } from '$lib/types';

export function validateSession(session: Partial<Session>): boolean {
    if (!session.id || !session.timerEvents || !Array.isArray(session.timerEvents)) {
        return false;
    }

    return session.timerEvents.every(validateTimerEvent);
}

export function validateTimerEvent(event: TimerEvent): boolean {
    return (
        !!event.timestamp &&
        !!event.type &&
        !!event.timer &&
        ['start', 'stop'].includes(event.type) &&
        ['left', 'right'].includes(event.timer)
    );
}

export function validateTimestamp(timestamp: string): boolean {
    const date = new Date(timestamp);
    return date.toString() !== 'Invalid Date';
}