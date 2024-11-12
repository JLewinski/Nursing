
interface TimerEvent {
    type: "start" | "stop";
    timer: "left" | "right";
    timestamp: string;
}

interface TimerState {
    events: TimerEvent[];
    activeTimer: "left" | "right" | null | undefined;
}

const defaultTimerState: TimerState = {
    events: [],
    activeTimer: undefined,
};

function getData() {
    const localData = localStorage.getItem("timerStore");
    return localData ? JSON.parse(localData) as TimerState : defaultTimerState;
}

function createTimerStore() {
    let data = $state(getData());
    return data;
}

export const timerStore = createTimerStore();

export function calculateDuration(
    events: TimerEvent[],
    timer: "left" | "right" | "total",
) {
    let duration = 0;
    let lastStart: string | null = null;

    for (const event of events) {
        if (timer === "total" || event.timer === timer) {
            if (event.type === "start") {
                lastStart = event.timestamp;
            } else if (event.type === "stop" && lastStart) {
                duration += new Date(event.timestamp).getTime() -
                    new Date(lastStart).getTime();
                lastStart = null;
            }
        }
    }

    if (lastStart) {
        duration += new Date().getTime() - new Date(lastStart).getTime();
    }

    return duration;
}