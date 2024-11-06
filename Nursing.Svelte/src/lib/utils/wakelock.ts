export class WakeLockManager {
    private wakeLock: WakeLockSentinel | null = null;

    async requestWakeLock(): Promise<void> {
        if ('wakeLock' in navigator) {
            try {
                this.wakeLock = await navigator.wakeLock.request('screen');
                
                this.wakeLock.addEventListener('release', () => {
                    // TODO: Handle wake lock release
                });
            } catch (err) {
                console.error('Wake Lock error:', err);
            }
        }
    }

    async releaseWakeLock(): Promise<void> {
        if (this.wakeLock) {
            await this.wakeLock.release();
            this.wakeLock = null;
        }
    }
}

export const wakeLockManager = new WakeLockManager();