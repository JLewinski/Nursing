<script lang="ts">
    import "../app.scss";
    import "./nav.css";
    import Installation from "$lib/components/Installation.svelte";
    import { setTimerState } from "$lib/stores/timerStore.svelte";
    import { syncStore } from "$lib/stores/syncStore.svelte";

    interface Props {
        children?: import("svelte").Snippet;
    }

    let { children }: Props = $props();

    setTimerState();

    let syncBadgeClass = $derived(
        {
            syncing: "bg-info",
            idle: "bg-success",
            error: "bg-danger",
            na: "bg-secondary",
        }[syncStore.status],
    );
</script>

<div>
    <Installation />
    <main class="main-content">
        {@render children?.()}
    </main>

    <nav class="nav-dock">
        <div class="nav-row">
            <a href="/" class="nav-link">
                <span class="bi bi-house"></span>
                <span class="nav-text">Home</span>
            </a>
            <a href="/history" class="nav-link">
                <span class="bi bi-clock-history"></span>
                <span class="nav-text">History</span>
            </a>
            <a href="/settings" class="nav-link">
                <span class="bi bi-gear"></span>
                <span class="nav-text">Settings</span>
            </a>
            <a href="/account" class="nav-link position-relative">
                <span class="bi bi-person"></span>
                <span
                    class="position-absolute top-0 start-100 translate-middle p-1 border border-light rounded-circle {syncBadgeClass}"
                >
                    <span class="visually-hidden">Sync status</span>
                </span>
                <span class="nav-text">Account</span>
            </a>
        </div>
    </nav>
</div>
