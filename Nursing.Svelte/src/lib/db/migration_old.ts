interface FeedingDto {
    Id: string;
    LeftBreastTotal: string;
    RightBreastTotal: string;
    TotalTime: string;
    Started: string;
    Finished: string;
    LastIsLeft: boolean;
    LastUpdated: string;
    Deleted: string | null;
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
    return {
        id: feeding.Id,
        startTime: feeding.Started,
        endTime: feeding.Finished,
        lastSide: feeding.LastIsLeft ? "left" : "right",
        created: new Date().toISOString(),
        rightDuration: parseTimeSpan(feeding.RightBreastTotal),
        leftDuration: parseTimeSpan(feeding.LeftBreastTotal),
        deleted: feeding.Deleted ? feeding.Deleted : undefined,
        lastUpdated: feeding.LastUpdated,
    };
}
