import type { Session } from '$lib/types';
import { Database } from '$lib/db/mod';

interface ExportData {
    version: string;
    timestamp: string;
    sessions: Session[];
    settings: Record<string, unknown>;
}

export class DataExport {
    static async exportData(): Promise<string> {
        const db = new Database();
        const sessions = await db.getAllSessions();
        const settings = await db.getSettings();

        const exportData: ExportData = {
            version: '1.0',
            timestamp: new Date().toISOString(),
            sessions,
            settings
        };

        return JSON.stringify(exportData, null, 2);
    }

    static async importData(jsonData: string): Promise<void> {
        try {
            const data: ExportData = JSON.parse(jsonData);
            const db = new Database();

            // Validate data version and structure
            if (!this.validateImportData(data)) {
                throw new Error('Invalid import data format');
            }

            await db.importData(data);
        } catch (error) {
            throw new Error(`Import failed: ${error.message}`);
        }
    }

    private static validateImportData(data: unknown): boolean {
        // TODO: Implement validation logic
        return true;
    }
}