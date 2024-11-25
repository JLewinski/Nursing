export async function registerServiceWorker() {
    if ("serviceWorker" in navigator) {
        try {
            const serviceWorker = navigator.serviceWorker as ServiceWorkerContainer;
            const registration = await serviceWorker.register(
                "/service-worker.js",
                { scope: "/" },
            );

            registration.addEventListener("updatefound", () => {
                // TODO: Handle service worker updates
            });

            return registration;
        } catch (error) {
            console.error("Service worker registration failed:", error);
        }
    }
}

export async function unregisterServiceWorker() {
    if ("serviceWorker" in navigator) {
        const serviceWorker = navigator.serviceWorker as ServiceWorkerContainer;
        const registration = await serviceWorker.getRegistration();
        if (registration) {
            await registration.unregister();
        }
    }
}
