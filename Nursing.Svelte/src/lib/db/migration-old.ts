
interface FeedingDto {
    id: string;
    started: string;
    totalTime: string;
    rightBreastTotal: string;
    leftBreastTotal: string;
    deleted?: string;
    lastUpdated: string;
}

interface Session {
    id: string;
    startTime: Date;
    duration: number;
    rightDuration: number;
    leftDuration: number;
    deleted?: Date;
    lastUpdated: Date;
}

export function migrateFromLocalStorage(store: IDBObjectStore) {
    // Check if data was already migrated
    const migrationKey = 'nursing-data-migrated';
    if (localStorage.getItem(migrationKey)) {
        return;
    }

    // Get all keys from localStorage
    const keys = Object.keys(localStorage).filter(key => key.startsWith('Data.'));
    const allFeedings: FeedingDto[] = [];

    // Collect all feedings
    for (const key of keys) {
        const feedingsJson = localStorage.getItem(key);
        if (feedingsJson) {
            const feedings = JSON.parse(feedingsJson);
            allFeedings.push(...feedings);
        }
    }

    // Convert and save to IndexedDB

    for (const feeding of allFeedings) {
        const session: Session = {
            id: feeding.id,
            startTime: new Date(feeding.started),
            duration: parseTimeSpan(feeding.totalTime),
            rightDuration: parseTimeSpan(feeding.rightBreastTotal),
            leftDuration: parseTimeSpan(feeding.leftBreastTotal),
            deleted: feeding.deleted ? new Date(feeding.deleted) : undefined,
            lastUpdated: new Date(feeding.lastUpdated)
        };
        
        store.add(session);
    }

    // Mark migration as complete
    localStorage.setItem(migrationKey, 'true');

    // Optional: Clean up old data
    // for (const key of keys) {
    //     localStorage.removeItem(key);
    // }
}

function parseTimeSpan(timeSpan: string): number {
    // TimeSpan format: "00:00:00"
    const [hours, minutes, seconds] = timeSpan.split(':').map(Number);
    return ((hours * 60 + minutes) * 60 + seconds) * 1000; // Convert to milliseconds
}