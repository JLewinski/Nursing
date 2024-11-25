export const APP_VERSION = '1.0.0';

export const DB_CONFIG = {
    name: 'NursingDB',
    version: 1,
    stores: {
        sessions: 'id,startTime,lastUpdated,deleted',
        settings: 'id',
        syncState: 'id'
    }
};

export const TIMER_CONFIG = {
    updateInterval: 1000,
    maxDuration: 3600000 // 1 hour in milliseconds
};

export const NOTIFICATION_CONFIG = {
    defaultInterval: 180, // 3 hours in minutes
    reminderTitle: 'Nursing Timer Reminder',
    reminderBody: 'Time to start a new nursing session'
};