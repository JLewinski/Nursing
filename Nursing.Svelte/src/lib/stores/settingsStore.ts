import { writable } from 'svelte/store';

interface Settings {
    theme: 'light' | 'dark' | 'system';
    estimatedInterval: number; // in minutes
    notifications: {
        enabled: boolean;
        reminderInterval: number; // in minutes
    };
}

const STORAGE_KEY = 'nursing-settings';

const defaultSettings: Settings = {
    theme: 'system',
    estimatedInterval: 180,
    notifications: {
        enabled: false,
        reminderInterval: 180
    }
};

function createSettingsStore() {
    // Load initial settings from localStorage
    const initialSettings = (() => {
        if (typeof localStorage !== 'undefined') {
            const stored = localStorage.getItem(STORAGE_KEY);
            if (stored) {
                return JSON.parse(stored) as Settings;
            }
        }
        return defaultSettings;
    })();

    const { subscribe, set, update } = writable<Settings>(initialSettings);

    return {
        subscribe,
        update: (newSettings: Partial<Settings>) => update(current => {
            const updated = { ...current, ...newSettings };
            if (typeof localStorage !== 'undefined') {
                localStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
            }
            return updated;
        }),
        reset: () => {
            set(defaultSettings);
            if (typeof localStorage !== 'undefined') {
                localStorage.setItem(STORAGE_KEY, JSON.stringify(defaultSettings));
            }
        }
    };
}

export const settings = createSettingsStore();