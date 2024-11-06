export type WorkerMessage = {
    type: 'CACHE_UPDATED' | 'SYNC_REQUIRED' | 'TIMER_BACKUP';
    payload?: unknown;
    timestamp: number;
}

export function sendWorkerMessage(message: Omit<WorkerMessage, 'timestamp'>): void {
    if ('serviceWorker' in navigator && navigator.serviceWorker.controller) {
        navigator.serviceWorker.controller.postMessage({
            ...message,
            timestamp: Date.now()
        });
    }
}

export function setupWorkerMessageHandlers(): void {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.addEventListener('message', (event) => {
            const message = event.data as WorkerMessage;
            
            switch (message.type) {
                case 'CACHE_UPDATED':
                    // TODO: Handle cache updates
                    break;
                case 'SYNC_REQUIRED':
                    // TODO: Handle sync requests
                    break;
                case 'TIMER_BACKUP':
                    // TODO: Handle timer state backup
                    break;
            }
        });
    }
}