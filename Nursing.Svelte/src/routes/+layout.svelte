<script lang="ts">
    import '../app.css';
    interface Props {
        children?: import('svelte').Snippet;
    }

    let { children }: Props = $props();
    // TODO: Import navigation components
</script>

<nav>
    <!-- TODO: Add navigation -->
</nav>

<main>
    {@render children?.()}
</main>