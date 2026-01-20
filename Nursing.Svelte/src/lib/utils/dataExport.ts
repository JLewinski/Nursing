import type { DBSession } from '$lib/db/mod';
import { Database } from '$lib/db/mod';

interface ExportData {
    version: string;
    timestamp: string;
    sessions: DBSession[];
    settings: Record<string, unknown>;
}

export class DataExport {
    static async exportData(): Promise<string> {
        const db = new Database();
        await db.init();
        const sessions = await db.getAllSessions();

        const exportData: ExportData = {
            version: '1.0',
            timestamp: new Date().toISOString(),
            sessions,
            settings: {}
        };

        return JSON.stringify(exportData, null, 2);
    }

    static async importData(jsonData: string): Promise<void> {
        try {
            const data: ExportData = JSON.parse(jsonData);
            const db = new Database();
            await db.init();

            // Validate data version and structure
            if (!this.validateImportData(data)) {
                throw new Error('Invalid import data format');
            }

            // Import sessions
            for (const session of data.sessions) {
                await db.saveSession(session);
            }
        } catch (error) {
            const message = error instanceof Error ? error.message : String(error);
            throw new Error(`Import failed: ${message}`);
        }
    }

    private static validateImportData(data: unknown): boolean {
        // TODO: Implement validation logic
        return true;
    }
}