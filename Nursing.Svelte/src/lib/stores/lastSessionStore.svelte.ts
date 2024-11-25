const STORAGE_KEY = "lastSessionStore";

interface ILastSession {
    side: "left" | "right" | null | undefined;
    startTime: Date | null;
}

class LastSessionState {
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
        } else {
            const old = localStorage.getItem("NursingCache");
            if (old) {
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

                this.side = data.LastWasLeft ? "left" : "right";
                this.startTime = new Date(data.LastStart);

                this.save();
            }
        }
    }
}

export const lastSession = new LastSessionState();
