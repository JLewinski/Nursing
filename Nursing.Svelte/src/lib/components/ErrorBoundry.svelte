<script lang="ts">
    import { onError } from 'svelte';
    import { handleError } from '$lib/utils/errorHandling';

    let error: Error | null = null;

    onError((e) => {
        error = e.error;
        handleError(e.error);
    });
</script>

{#if error}
    <div class="error-boundary">
        <h3>Something went wrong</h3>
        <p>The app encountered an error. Please try refreshing the page.</p>
        <button on:click={() => window.location.reload()}>
            Refresh Page
        </button>
    </div>
{:else}
    <slot />
{/if}

<style>
    .error-boundary {
        padding: 2rem;
        text-align: center;
        color: var(--error-color);
    }
</style>