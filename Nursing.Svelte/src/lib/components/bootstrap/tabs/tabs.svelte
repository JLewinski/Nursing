<script lang="ts">
    import type { Snippet } from "svelte";
    import TabContent from "./tabContent.svelte";

    let { tabs, ...children } = $props();

    let tabTitles = $derived(tabs as string[]);

    // svelte-ignore state_referenced_locally
    let selectedTab = $state(tabTitles[0]);
    let tabContent = Object.keys(children).map(key => children[key] as Snippet);
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
    {#each tabContent as tab, index}
        <TabContent title={tabTitles[index]} {selectedTab}>
            {@render tab()}
        </TabContent>
    {/each}
</div>
