<script lang="ts">
    import type { Snippet } from "svelte";
    import { setContext } from "svelte";

    interface Props {
        children: Snippet;
    }

    let tabTitles = $state([]) as string[];
    let selectedTab = $state("");
    let context = {
        addTitle,
        get selectedTab() {
            return selectedTab;
        },
    };
    setContext("tab", context);

    function addTitle(title: string) {
        if (tabTitles.length === 0) {
            selectedTab = title;
        }

        tabTitles.push(title);
    }

    let { children }: Props = $props();
</script>

<ul class="nav nav-tabs" id="HistoryTabs" role="tablist">
    {#each tabTitles as title}
        <li class="nav-item" role="presentation">
            <button
                class="nav-link"
                class:active={selectedTab == title}
                id="grid-tab"
                type="button"
                role="tab"
                aria-controls="grid"
                aria-selected="true"
                onclick={() => {
                    selectedTab = title;
                }}>{title}</button
            >
        </li>
    {/each}
</ul>
<div class="tab-content">
    {@render children()}
</div>