interface DBSchema {
    sessions: {
        id: string;
        timerEvents: Array<{
            type: 'start' | 'stop';
            timer: 'left' | 'right';
            timestamp: string;
        }>;
        startTime: string;
        endTime: string;
        lastUpdated: string;
    };
}

// TODO: Implement IndexedDB setup and CRUD operations
export class Database {
    async init(): Promise<void> {
        // Initialize IndexedDB
    }

    async saveSession(session: DBSchema['sessions']): Promise<void> {
        // Save session data
    }
}