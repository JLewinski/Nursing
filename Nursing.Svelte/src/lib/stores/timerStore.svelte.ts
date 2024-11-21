import { getContext, setContext } from "svelte";

const LOCAL_STORAGE_KEY = "timerStore";

interface TimerEvent {
    type: "start" | "stop";
    timer: "left" | "right";
    timestamp: string;
}

interface ITimerState {
    events: TimerEvent[];
    activeTimer: "left" | "right" | null | undefined;
}

const defaultTimerState: ITimerState = {
    events: [],
    activeTimer: undefined,
};

export class TimerState implements ITimerState {
    events: TimerEvent[] = $state([]);
    activeTimer: "left" | "right" | null | undefined = $state(undefined);

    constructor() {
        this.load();
    }

    toggle(side: "left" | "right") {
        const now = new Date().toISOString();
        if (this.activeTimer) {
            this.events.push({
                type: "stop",
                timer: this.activeTimer,
                timestamp: now,
            });
        }

        if (this.activeTimer != side) {
            this.activeTimer = side;
            this.events.push({
                type: "start",
                timer: side,
                timestamp: now,
            });
            this.activeTimer = side;
        } else {
            this.activeTimer = null;
        }

        this.save();
    }

    calculateDuration(timer: "left" | "right" | "total") {
        let duration = 0;
        let lastStart: string | null = null;

        for (const event of this.events) {
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

    reset() {
        this.events = [];
        this.activeTimer = undefined;
        this.save();
    }

    private load() {
        const localData = localStorage.getItem(LOCAL_STORAGE_KEY);
        let data: ITimerState;
        if (localData) {
            data = JSON.parse(localData) as ITimerState;
        } else if (!this.loadOld()) {
            data = defaultTimerState;
        } else {
            return;
        }

        this.events = data.events;
        this.activeTimer = data.activeTimer;
    }

    private loadOld() {
        const old = localStorage.getItem("NursingCache");
        if (!old) {
            return false;
        }

        const data = JSON.parse(old) as {
            CurrentFeeding: {
                LeftBreast: {
                    StartTime: string;
                    EndTime: string | null;
                }[]; // Array type can be more specific if needed
                RightBreast: {
                    StartTime: string;
                    EndTime: string | null;
                }[]; // Array type can be more specific if needed
                IsStarted: boolean;
                IsFinished: boolean;
                Id: string;
                LeftBreastTotal: string;
                RightBreastTotal: string;
                TotalTime: string;
                Started: string;
                Finished: string | null;
                LastIsLeft: boolean;
                LastUpdated: string;
                Deleted: string | null;
            };
            LastStart: string;
            LastWasLeft: boolean;
        };

        if (!data.CurrentFeeding.IsStarted || data.CurrentFeeding.IsFinished) {
            return false;
        }

        const leftEvents = data.CurrentFeeding.LeftBreast.map((item) => ({
            timer: "left" as "left" | "right",
            start: item.StartTime,
            end: item.EndTime,
        }));

        const rightEvents = data.CurrentFeeding.RightBreast.map((item) => ({
            timer: "right" as "left" | "right",
            start: item.StartTime,
            end: item.EndTime,
        }));

        const oldEvents = leftEvents.concat(rightEvents);
        oldEvents.sort((a, b) =>
            new Date(a.start).getTime() - new Date(b.start).getTime()
        );

        for (const event of oldEvents) {
            this.events.push({
                timer: event.timer,
                type: "start",
                timestamp: event.start,
            });
            if (event.end) {
                this.events.push({
                    timer: event.timer,
                    type: "stop",
                    timestamp: event.end,
                });
            }
        }

        if (this.events[this.events.length - 1].type === "start") {
            this.activeTimer = this.events[this.events.length - 1].timer;
        } else {
            this.activeTimer = null;
        }

        this.save();

        return true;
    }

    private save() {
        const data = {
            events: this.events,
            activeTimer: this.activeTimer,
        };

        localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(data));
    }
}

export function setTimerState() {
    return setContext(LOCAL_STORAGE_KEY, new TimerState());
}

export function getTimerState() {
    return getContext<ReturnType<typeof setTimerState>>(LOCAL_STORAGE_KEY);
}
