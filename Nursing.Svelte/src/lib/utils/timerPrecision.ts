import type { TimerEvent } from '$lib/types';

export class TimerPrecision {
    private static readonly PRECISION_CHECK_INTERVAL = 1000; // 1 second
    private static timeOffset = 0;

    static init(): void {
        // Check system time drift
        setInterval(() => {
            const expectedTime = Date.now();
            const actualTime = performance.now();
            this.timeOffset = expectedTime - actualTime;
        }, this.PRECISION_CHECK_INTERVAL);
    }

    static getPreciseTimestamp(): string {
        const now = Date.now() - this.timeOffset;
        return new Date(now).toISOString();
    }

    static calculateDuration(events: TimerEvent[]): number {
        let duration = 0;
        let startTime: number | null = null;

        for (const event of events) {
            if (event.type === 'start') {
                startTime = new Date(event.timestamp).getTime();
            } else if (event.type === 'stop' && startTime) {
                duration += new Date(event.timestamp).getTime() - startTime;
                startTime = null;
            }
        }

        return duration;
    }
}