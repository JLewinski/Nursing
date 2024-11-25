<script lang="ts">
    import type { Snippet } from "svelte";

    interface Props{ 
        selectedTab: string; 
        title: string; 
        children: Snippet; 
    }

    let {
        selectedTab,
        title,
        children,
    }: Props = $props();

    let active = $derived(selectedTab == title);
</script>

<div class="tab-pane" class:active role="tabpanel">
    {@render children()}
</div>
