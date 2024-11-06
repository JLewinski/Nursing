import { writable } from 'svelte/store';

interface TimerEvent {
    type: 'start' | 'stop';
    timer: 'left' | 'right';
    timestamp: string;
}

interface TimerState {
    events: TimerEvent[];
    activeTimer: 'left' | 'right' | null;
}

// TODO: Implement timer logic
export const timerStore = writable<TimerState>({
    events: [],
    activeTimer: null
});