interface AnalyticsEvent {
    type: 'session_start' | 'session_end' | 'timer_toggle' | 'error';
    timestamp: string;
    metadata?: Record<string, unknown>;
}

class Analytics {
    private events: AnalyticsEvent[] = [];
    private readonly MAX_EVENTS = 100;

    trackEvent(type: AnalyticsEvent['type'], metadata?: Record<string, unknown>) {
        const event: AnalyticsEvent = {
            type,
            timestamp: new Date().toISOString(),
            metadata
        };

        this.events.push(event);
        if (this.events.length > this.MAX_EVENTS) {
            this.events.shift();
        }

        // TODO: Implement local storage persistence
    }

    getEvents(): AnalyticsEvent[] {
        return [...this.events];
    }
}

export const analytics = new Analytics();