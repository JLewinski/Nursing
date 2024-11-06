<script lang="ts">
    import { page } from '$app/stores';
    import Installation from './Installation.svelte';
    import OfflineIndicator from './OfflineIndicator.svelte';
</script>

<header>
    <nav>
        <a href="/" class:active={$page.url.pathname === '/'}>Timer</a>
        <a href="/history" class:active={$page.url.pathname === '/history'}>History</a>
        <a href="/settings" class:active={$page.url.pathname === '/settings'}>Settings</a>
    </nav>
    <Installation />
    <OfflineIndicator />
</header>

<style>
    header {
        position: sticky;
        top: 0;
        background: var(--surface-color);
        padding: 1rem;
        box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    nav {
        display: flex;
        gap: 1rem;
    }

    .active {
        color: var(--primary-color);
        font-weight: bold;
    }
</style>