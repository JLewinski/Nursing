<script lang="ts">
    import { onMount, onDestroy } from 'svelte';
    import { Database } from '$lib/db';
    import { settings } from '$lib/stores/settingsStore';
    
    let backupInterval: number;
    const BACKUP_INTERVAL = 1000 * 60 * 60; // 1 hour

    async function performBackup() {
        try {
            const db = new Database();
            const data = await db.getAllData();
            localStorage.setItem('nursing-backup', JSON.stringify({
                timestamp: new Date().toISOString(),
                data
            }));
        } catch (error) {
            console.error('Backup failed:', error);
        }
    }

    onMount(() => {
        backupInterval = window.setInterval(performBackup, BACKUP_INTERVAL);
        performBackup();
    });

    onDestroy(() => {
        if (backupInterval) {
            clearInterval(backupInterval);
        }
    });
</script>