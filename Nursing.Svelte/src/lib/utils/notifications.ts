export class NotificationManager {
    private static instance: NotificationManager;
    
    private constructor() {
        // Singleton pattern
    }

    static getInstance(): NotificationManager {
        if (!NotificationManager.instance) {
            NotificationManager.instance = new NotificationManager();
        }
        return NotificationManager.instance;
    }

    async requestPermission(): Promise<boolean> {
        if (!('Notification' in window)) {
            console.warn('This browser does not support notifications.');
            return false;
        }

        try {
            const permission = await Notification.requestPermission();
            return permission === 'granted';
        } catch (error) {
            console.error('Error requesting notification permission:', error);
            return false;
        }
    }

    getPermissionState(): NotificationPermission {
        if (!('Notification' in window)) {
            return 'denied';
        }
        return Notification.permission;
    }

    async sendLocalNotification(title: string, options: NotificationOptions = {}): Promise<boolean> {
        if (this.getPermissionState() !== 'granted') {
            console.warn('Notifications not granted');
            return false;
        }

        try {
            // Try to use Service Worker registration if available for better mobile support
            if ('serviceWorker' in navigator) {
                const registration = await navigator.serviceWorker.ready;
                if (registration) {
                    await registration.showNotification(title, {
                        icon: '/pwa-192x192.png',
                        badge: '/pwa-192x192.png',
                        // vibrate: [100, 50, 100],
                        ...options
                    });
                    return true;
                }
            }

            // Fallback to standard Notification API
            new Notification(title, {
                icon: '/pwa-192x192.png',
                ...options
            });
            return true;
        } catch (error) {
            console.error('Error sending notification:', error);
            return false;
        }
    }

    // Stub for Push API subscription
    async subscribeToPushNotifications(): Promise<PushSubscription | null> {
        console.log('Push subscription not yet implemented (Phase 4)');
        return null;
    }
}

export const notificationManager = NotificationManager.getInstance();