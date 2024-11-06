export function formatDate(date: Date): string {
    return new Intl.DateTimeFormat('default', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    }).format(date);
}

export function formatTime(date: Date): string {
    return new Intl.DateTimeFormat('default', {
        hour: '2-digit',
        minute: '2-digit'
    }).format(date);
}

export function getRelativeTimeString(date: Date): string {
    // TODO: Implement relative time formatting
    return '';
}