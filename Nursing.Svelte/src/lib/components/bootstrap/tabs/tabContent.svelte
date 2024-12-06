<script lang="ts">
    import { getContext, type Snippet } from "svelte";

    interface Props {
        title: string;
        children: Snippet;
    }

    let { title, children }: Props = $props();

    let context = getContext<{
        addTitle: (title: string) => void;
        readonly selectedTab: string;
    }>("tab");

    context.addTitle(title);

    let active = $derived(context.selectedTab == title);

</script>
<div class="tab-pane" class:active role="tabpanel">
    {@render children()}
</div>
