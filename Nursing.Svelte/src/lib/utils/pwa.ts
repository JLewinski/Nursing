import { registerServiceWorker } from './serviceWorkerRegistration.ts';

export class PWAManager {
    private static instance: PWAManager;
    private deferredInstallPrompt: any = null;

    private constructor() {
        if (typeof window !== 'undefined') {
            window.addEventListener('beforeinstallprompt', (e) => {
                e.preventDefault();
                this.deferredInstallPrompt = e;
            });
        }
    }

    static getInstance(): PWAManager {
        if (!PWAManager.instance) {
            PWAManager.instance = new PWAManager();
        }
        return PWAManager.instance;
    }

    async init(): Promise<void> {
        await registerServiceWorker();
        await this.checkForUpdates();
    }

    async promptInstall(): Promise<boolean> {
        if (!this.deferredInstallPrompt) {
            return false;
        }

        const result = await this.deferredInstallPrompt.prompt();
        this.deferredInstallPrompt = null;
        return result.outcome === 'accepted';
    }

    private async checkForUpdates(): Promise<void> {
        // TODO: Implement update check logic
    }
}

export const pwaManager = PWAManager.getInstance();