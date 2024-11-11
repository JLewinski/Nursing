import { writable } from 'svelte/store';

const STORAGE_KEY = 'lastSessionStore';

const defaultSession: LastSession = {
    side: null,
    startTime: null,
}

interface LastSession {
    side: "left" | "right" | null;
    startTime: Date | null;
}

function createStore() {
    const initialData = (() => {
        const localData = localStorage.getItem('lastSessionStore');
        if (localData) {
            const parsedData = JSON.parse(localData) as { lastSide: "left" | "right" | null, startTime: string };
            return {
                side: parsedData.lastSide,
                startTime: new Date(parsedData.startTime),
            } as LastSession;
        }
        
        return {
            side: null,
            startTime: null,
        } as LastSession;
    })();

    const { subscribe, set, update } = writable<LastSession>(initialData);
    return {
        subscribe,
        update: (newSettings: Partial<LastSession>) => update(current => {
            const updated = { ...current, ...newSettings };
            if (typeof localStorage !== 'undefined') {
                localStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
            }
            return updated;
        }),
        reset: () => {
            set(defaultSession);
            if (typeof localStorage !== 'undefined') {
                localStorage.setItem(STORAGE_KEY, JSON.stringify(defaultSession));
            }
        }
    };
}

export const lastSession = createStore();
