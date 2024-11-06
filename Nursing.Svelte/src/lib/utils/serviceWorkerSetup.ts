interface ServiceWorkerConfig {
    scope: string;
    updateInterval: number;
}

export class ServiceWorkerSetup {
    private static readonly DEFAULT_CONFIG: ServiceWorkerConfig = {
        scope: '/',
        updateInterval: 60 * 60 * 1000 // 1 hour
    };

    static async register(config: Partial<ServiceWorkerConfig> = {}): Promise<void> {
        if (!('serviceWorker' in navigator)) return;

        const finalConfig = { ...this.DEFAULT_CONFIG, ...config };

        try {
            const registration = await (navigator.serviceWorker as ServiceWorkerContainer).register(
                '/service-worker.js',
                { scope: finalConfig.scope }
            );

            // Set up periodic updates
            setInterval(() => {
                registration.update();
            }, finalConfig.updateInterval);

            // Handle updates
            registration.addEventListener('updatefound', () => {
                const newWorker = registration.installing;
                if (!newWorker) return;

                newWorker.addEventListener('statechange', () => {
                    if (!('serviceWorker' in navigator)) return;
                    if (newWorker.state === 'installed' && (navigator.serviceWorker as ServiceWorkerContainer).controller) {
                        // New service worker available
                        this.dispatchUpdateEvent();
                    }
                });
            });

        } catch (error) {
            console.error('Service worker registration failed:', error);
        }
    }

    private static dispatchUpdateEvent(): void {
        window.dispatchEvent(new CustomEvent('swUpdate'));
    }
}