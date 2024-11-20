interface FeedingDto {
    Id: string;
    Started: string;
    TotalTime: string;
    RightBreastTotal: string;
    LeftBreastTotal: string;
    Deleted: string | null;
    LastUpdated: string;
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

export function migrateOldData() {
    const data = getOldData();
    const migratedData = migrateArray(data);
    return migratedData;
}

export function parseTimeSpan(timeSpan: string): number {
    // TimeSpan format: "0.00:00:00:000"
    // Remove the "0." prefix if it exists
    const cleanTimeSpan = timeSpan.replace(/^0\./, "");
    const [hours, minutes, seconds, milliseconds] = cleanTimeSpan.split(":");

    return ((Number(hours) * 60 + Number(minutes)) * 60 + Number(seconds)) *
            1000 +
        (Number(milliseconds) * Math.pow(10, 3 - milliseconds.length));
}

function getOldData() {
    // Get all keys from localStorage
    const keys = Object.keys(localStorage).filter((key) =>
        key.startsWith("Data.")
    );
    const allFeedings: FeedingDto[] = [];

    // Collect all feedings
    for (const key of keys) {
        const feedingsJson = localStorage.getItem(key);
        if (feedingsJson) {
            const feedings = JSON.parse(feedingsJson);
            allFeedings.push(...feedings);
        }
    }

    return allFeedings;
}

function migrateArray(data: FeedingDto[]) {
    return data.map(migrateData);
}

export function migrateData(feeding: FeedingDto) {
    const session: Session = {
        id: feeding.Id,
        startTime: new Date(feeding.Started),
        duration: parseTimeSpan(feeding.TotalTime),
        rightDuration: parseTimeSpan(feeding.RightBreastTotal),
        leftDuration: parseTimeSpan(feeding.LeftBreastTotal),
        deleted: feeding.Deleted ? new Date(feeding.Deleted) : undefined,
        lastUpdated: new Date(feeding.LastUpdated),
    };

    return session;
}
