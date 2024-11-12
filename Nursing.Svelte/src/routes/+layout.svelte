<script lang="ts">
    import '../app.css';
    import './nav.css';
    import Installation from '$lib/components/Installation.svelte';
    import ThemeSetter from '$lib/components/ThemeSetter.svelte';
    import { setTimerState } from '$lib/stores/timerStore.svelte';
    
    interface Props {
        children?: import('svelte').Snippet;
    }

    let { children }: Props = $props();

    setTimerState();
</script>

<ThemeSetter>
</ThemeSetter>
<div>
    <Installation />
    <main class="main-content">
        {@render children?.()}
    </main>

    <nav class="nav-dock">
        <div class="nav-row">
            <a href="/" class="nav-link">
                <span class="bi bi-clock-history"></span>
                <span class="nav-text">Home</span>
            </a>
            <a href="/history" class="nav-link">
                <span class="bi bi-list-nested-nav-menu"></span>
                <span class="nav-text">History</span>
            </a>
            <a href="/settings" class="nav-link">
                <span class="bi bi-gear"></span>
                <span class="nav-text">Settings</span>
            </a>
            <a href="/account" class="nav-link">
                <span class="bi bi-person"></span>
                <span class="nav-text">Account</span>
            </a>
        </div>
    </nav>
</div>