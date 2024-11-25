export class TimeZoneHandler {
    private static currentTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

    static detectTimeZoneChange(): boolean {
        const newTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
        const changed = newTimeZone !== this.currentTimeZone;
        this.currentTimeZone = newTimeZone;
        return changed;
    }

    static adjustTimestamp(timestamp: string): string {
        // Convert timestamp to UTC to ensure consistency
        const date = new Date(timestamp);
        return date.toISOString();
    }

    static getLocalTimestamp(): string {
        return new Date().toISOString();
    }
}

// Set up timezone change detection
if (typeof window !== 'undefined') {
    setInterval(() => {
        if (TimeZoneHandler.detectTimeZoneChange()) {
            // TODO: Handle timezone change
            console.log('Timezone changed');
        }
    }, 1000 * 60); // Check every minute
}