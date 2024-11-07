export interface TimerEvent {
    id: string;
    type: 'start' | 'stop';
    timer: 'left' | 'right';
    timestamp: number;
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

interface FeedingDto {
    id: string;
    started: string;
    totalTime: string;
    rightBreastTotal: string;
    leftBreastTotal: string;
    deleted?: string;
    lastUpdated: string;
}