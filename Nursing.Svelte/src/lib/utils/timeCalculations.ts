import type { TimerEvent } from '$lib/types';

export function calculateDuration(events: TimerEvent[]): number {
    // TODO: Implement precise duration calculation
    return 0;
}

export function formatDuration(milliseconds: number): string {
    // TODO: Implement duration formatting
    return '00:00';
}

export function getActiveTimers(events: TimerEvent[]): ('left' | 'right')[] {
    // TODO: Implement active timer detection
    return [];
}