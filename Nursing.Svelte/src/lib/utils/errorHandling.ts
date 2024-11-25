export class AppError extends Error {
    constructor(
        message: string,
        public code: string,
        public context?: Record<string, unknown>
    ) {
        super(message);
        this.name = 'AppError';
    }
}

export function handleError(error: unknown): void {
    // TODO: Implement error logging and user notification
    console.error(error);
}