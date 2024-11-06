import { writable } from 'svelte/store';

interface Settings {
    theme: 'light' | 'dark' | 'system';
    estimatedInterval: number; // in minutes
    notifications: {
        enabled: boolean;
        reminderInterval: number; // in minutes
    };
}

const defaultSettings: Settings = {
    theme: 'system',
    estimatedInterval: 180,
    notifications: {
        enabled: false,
        reminderInterval: 180
    }
};

// TODO: Implement settings persistence
export const settings = writable<Settings>(defaultSettings);