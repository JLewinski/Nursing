export interface TimerEvent {
    id: string;
    type: 'start' | 'stop';
    timer: 'left' | 'right';
    timestamp: string;
}

export interface Session {
    id: string;
    timerEvents: TimerEvent[];
    startTime: string;
    endTime: string;
    lastUpdated: string;
    created: string;
    deleted?: string;
}

export interface SyncState {
    lastSyncTimestamp: string | null;
    syncStatus: 'idle' | 'syncing' | 'error';
    error?: string;
}