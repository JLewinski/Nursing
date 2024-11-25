import { Database } from "../db/mod.ts";

export class StorageManager {
    private static readonly STORAGE_THRESHOLD = 0.9; // 90% of available space

    static async checkStorageQuota(): Promise<boolean> {
        if ('storage' in navigator && 'estimate' in navigator.storage) {
            const estimate = await navigator.storage.estimate();
            if (estimate.usage && estimate.quota) {
                return (estimate.usage / estimate.quota) < this.STORAGE_THRESHOLD;
            }
        }
        return true;
    }

    static async cleanupOldData(): Promise<void> {
        const db = new Database();
        const threshold = new Date();
        threshold.setDate(threshold.getDate() - 30); // Keep last 30 days

        // TODO: Implement data cleanup
        // await db.removeSessionsBefore(threshold);
    }

    static async ensureStorageAvailable(): Promise<boolean> {
        const hasSpace = await this.checkStorageQuota();
        if (!hasSpace) {
            await this.cleanupOldData();
            return this.checkStorageQuota();
        }
        return true;
    }
}