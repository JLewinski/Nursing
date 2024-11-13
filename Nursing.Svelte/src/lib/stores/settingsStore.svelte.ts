const STORAGE_KEY = "nursing-settings";

export class SettingsState implements ISettings {
    theme: "light" | "dark" | "system" = $state("system");
    estimatedInterval: number = $state(180);
    notifications: {
        enabled: boolean;
        reminderInterval: number;
    } = $state({ enabled: false, reminderInterval: 180 });

    constructor() {
        this.load();
    }

    save() {
        const data: ISettings = {
            theme: this.theme,
            estimatedInterval: this.estimatedInterval,
            notifications: this.notifications,
        };
        localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
    }

    load() {
        const stored = localStorage.getItem(STORAGE_KEY);
        if (stored) {
            const data = JSON.parse(stored) as ISettings;
            this.theme = data.theme;
            this.estimatedInterval = data.estimatedInterval;
            this.notifications = data.notifications;
        }
    }
}

interface ISettings {
    theme: "light" | "dark" | "system";
    estimatedInterval: number; // in minutes
    notifications: {
        enabled: boolean;
        reminderInterval: number; // in minutes
    };
}

export const settings = new SettingsState();
