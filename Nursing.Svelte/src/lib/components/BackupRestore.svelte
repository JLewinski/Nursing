<script lang="ts">
    import { Database } from '$lib/db';
    import { handleError } from '$lib/utils/errorHandling';
    
    async function createBackup() {
        try {
            const db = new Database();
            const sessions = await db.getAllSessions();
            const settings = await db.getSettings();
            
            const backup = {
                version: 1,
                timestamp: new Date().toISOString(),
                data: { sessions, settings }
            };
            
            const blob = new Blob([JSON.stringify(backup)], { type: 'application/json' });
            const url = URL.createObjectURL(blob);
            
            const a = document.createElement('a');
            a.href = url;
            a.download = `nursing-backup-${backup.timestamp}.json`;
            a.click();
            
            URL.revokeObjectURL(url);
        } catch (error) {
            handleError(error);
        }
    }
</script>

<div class="backup-restore">
    <button on:click={createBackup}>Create Backup</button>
    <div class="restore-section">
        <h3>Restore Data</h3>
        <input 
            type="file" 
            accept=".json"
            on:change={(e) => {
                // TODO: Implement restore functionality
            }}
        />
    </div>
</div>