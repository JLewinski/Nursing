<script lang="ts">
    import "../app.css";
    import Installation from "$lib/components/Installation.svelte";
    import { setTimerState } from "$lib/stores/timerStore.svelte";

    interface Props {
        children?: import("svelte").Snippet;
    }

    let { children }: Props = $props();

    setTimerState();
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
            <a href="/account" class="nav-link">
                <span class="bi bi-person"></span>
                <span class="nav-text">Account</span>
            </a>
        </div>
    </nav>
</div>

<style>
    .bi-list-nested-nav-menu {
        background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' fill='%23ffd59a' class='bi bi-list-nested' viewBox='0 0 16 16'%3E%3Cpath fill-rule='evenodd' d='M4.5 11.5A.5.5 0 0 1 5 11h10a.5.5 0 0 1 0 1H5a.5.5 0 0 1-.5-.5zm-2-4A.5.5 0 0 1 3 7h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm-2-4A.5.5 0 0 1 1 3h10a.5.5 0 0 1 0 1H1a.5.5 0 0 1-.5-.5z'/%3E%3C/svg%3E");
    }

    .nav-row {
        display: flex;
        flex-direction: row;
        width: 100%;
        justify-content: space-around;
    }

    .nav-dock .nav-link {
        display: flex;
        flex-direction: column;
        align-items: center;
        text-decoration: none;
        color: inherit;
    }

    .nav-dock .nav-link:hover {
        text-decoration: underline;
    }

    .nav-dock .nav-link span {
        font-size: 1.5rem;
    }

    .nav-dock .nav-text {
        display: none;
    }

    .main-content {
        padding-bottom: 3.5rem; /* Height of the nav-dock */
    }

    @media (min-width: 768px) {
        .nav-dock .nav-link {
            flex-direction: row;
        }

        .nav-dock .nav-text {
            display: inline;
            margin-left: 0.5rem;
        }
    }
</style>
