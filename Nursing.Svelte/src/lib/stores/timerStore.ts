import { writable, derived } from 'svelte/store';

interface TimerEvent {
    type: 'start' | 'stop';
    timer: 'left' | 'right';
    timestamp: string;
}

interface TimerState {
    events: TimerEvent[];
    activeTimer: 'left' | 'right' | null | undefined;
}

function createTimerStore() {
    const { subscribe, set, update } = writable<TimerState>({
        events: [],
        activeTimer: undefined
    });

    

    // Add interval for updating active timer
    let intervalId: number | null = null;

    // Function to start interval
    function startInterval() {
        if (!intervalId) {
            intervalId = setInterval(() => {
                // Trigger store update
                update(state => ({ ...state }));
            }, 1000);
        }
    }

    // Function to stop interval
    function stopInterval() {
        if (intervalId) {
            clearInterval(intervalId);
            intervalId = null;
        }
    }

    const localData = localStorage.getItem('timerStore');
    const savedData = localData ? JSON.parse(localData) as TimerState : null;
    
    if (savedData) {
        set(savedData);
        if(savedData.activeTimer) {
            startInterval();
        }
    }

    return {
        subscribe,
        toggleTimer: (timer: 'left' | 'right') => update(state => {
            const now = new Date().toISOString();
            const events = [...state.events];
            
            // Stop current timer if any
            if (state.activeTimer) {
                events.push({
                    type: 'stop',
                    timer: state.activeTimer,
                    timestamp: now
                });
                stopInterval();
            }

            // Start new timer if it's different from current
            if (state.activeTimer !== timer) {
                events.push({
                    type: 'start',
                    timer,
                    timestamp: now
                });
                startInterval();
            }

            const newState = { events, activeTimer: state.activeTimer !== timer ? timer : null };
            localStorage.setItem('timerStore', JSON.stringify(newState));
            return newState;
        }),
        reset: () => {
            stopInterval();
            localStorage.removeItem('timerStore');
            set({ events: [], activeTimer: undefined });
        },
        getDuration: (timer: 'left' | 'right') => derived({ subscribe }, ($state) => {
            let duration = 0;
            let lastStart: string | null = null;

            for (const event of $state.events) {
                if (event.timer === timer) {
                    if (event.type === 'start') {
                        lastStart = event.timestamp;
                    } else if (event.type === 'stop' && lastStart) {
                        duration += new Date(event.timestamp).getTime() - new Date(lastStart).getTime();
                        lastStart = null;
                    }
                }
            }

            // Add current running time if timer is active
            if (lastStart && $state.activeTimer === timer) {
                duration += new Date().getTime() - new Date(lastStart).getTime();
            }

            return duration;
        })
    };
}

export const timerStore = createTimerStore();