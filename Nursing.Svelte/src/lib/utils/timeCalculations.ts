import type { TimerEvent } from "$lib/types/index.ts";

export function calculateDuration(events: TimerEvent[]): number {
    let duration = 0;
    const sortedEvents = [...events].sort((a, b) => a.timestamp - b.timestamp);
    const timerStates = new Map<"left" | "right", number>();

    for (const event of sortedEvents) {
        if (event.type === "start") {
            timerStates.set(event.timer, event.timestamp);
        } else if (event.type === "stop") {
            const startTime = timerStates.get(event.timer);
            if (startTime !== undefined) {
                duration += event.timestamp - startTime;
                timerStates.delete(event.timer);
            }
        }
    }
    return duration;
}

export function formatLongDuration(milliseconds: number) {
    const totalMinues = Math.floor(milliseconds / 60000);
    const hours = Math.floor(totalMinues / 60);
    const minutes = totalMinues % 60;

    return `${hours} hrs ${minutes.toString().padStart(2, "0")} mins`;
}

export function formatDuration(milliseconds: number): string {
    const totalSeconds = Math.floor(milliseconds / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;
    return `${minutes}:${seconds.toString().padStart(2, "0")}`;
}

export function getActiveTimers(events: TimerEvent[]): ("left" | "right")[] {
    const sortedEvents = [...events].sort((a, b) => a.timestamp - b.timestamp);
    const activeTimers = new Set<"left" | "right">();

    for (const event of sortedEvents) {
        if (event.type === "start") {
            activeTimers.add(event.timer);
        } else if (event.type === "stop") {
            activeTimers.delete(event.timer);
        }
    }

    return Array.from(activeTimers);
}

export function parseDuration(durationStr: string): number {
    const [minutes, seconds] = durationStr.split(":").map(Number);
    return (minutes * 60 + seconds) * 1000;
}
