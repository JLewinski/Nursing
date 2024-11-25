
export interface TimerEvent {
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