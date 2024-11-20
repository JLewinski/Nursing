const STORAGE_KEY = 'lastSessionStore';

interface ILastSession {
    side: "left" | "right" | null | undefined;
    startTime: Date | null;
}

class LastSessionState{
    side: "left" | "right" | null | undefined = $state(undefined);
    startTime: Date | null = $state(null);

    constructor() {
        this.load();
    }

    save() {
        const data: ILastSession = {
            side: this.side,
            startTime: this.startTime,
        };
        localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
    }

    load() {
        const stored = localStorage.getItem(STORAGE_KEY);
        if (stored) {
            const data = JSON.parse(stored) as {
                side: "left" | "right" | null | undefined;
                startTime: string;
            };
            this.side = data.side;
            this.startTime = new Date(data.startTime);
        }
    }
}

export const lastSession = new LastSessionState();
