/// <reference lib="webworker" />
export type {};
declare let self: ServiceWorkerGlobalScope;

const CACHE_NAME = 'nursing-app-cache-v1';

// TODO: Implement cache strategy
self.addEventListener('install', (event) => {
    // Cache static assets
});

self.addEventListener('fetch', (event) => {
    // Handle fetch events
});

self.addEventListener('push', (event) => {
    // Handle push notifications
});