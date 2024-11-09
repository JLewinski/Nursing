/// <reference lib="webworker" />
export type {};
declare let self: ServiceWorkerGlobalScope;

const CACHE_NAME = 'nursing-app-cache-v1';
const STATIC_ASSETS = [
    '/',
    '/index.html',
    '/manifest.json',
    '/favicon.ico'
];

self.addEventListener('install', (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => {
            return cache.addAll(STATIC_ASSETS);
        })
    );
});

self.addEventListener('fetch', (event) => {
    const isApiRequest = event.request.url.includes('/api/');

    if (isApiRequest) {
        return; // Let API requests go through normally
    }

    event.respondWith(
        caches.match(event.request)
            .then(response => response || fetch(event.request)
                .then(response => {
                    const responseClone = response.clone();
                    caches.open(CACHE_NAME)
                        .then(cache => cache.put(event.request, responseClone));
                    return response;
                })
            )
    );
});

self.addEventListener('push', (event) => {
    const options = {
        body: event.data?.text() ?? 'Timer notification',
        icon: '/favicon.ico',
        badge: '/icon-192.png',
        vibrate: [100, 50, 100],
        data: {
            dateOfArrival: Date.now(),
            primaryKey: 1
        }
    };

    event.waitUntil(
        self.registration.showNotification('Nursing Timer', options)
    );
});