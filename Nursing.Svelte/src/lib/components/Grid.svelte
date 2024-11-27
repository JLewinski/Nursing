<script lang="ts">
    import { createEventDispatcher } from "svelte";
    import { Grid } from "gridjs";
    import type { Config as UserConfig } from "gridjs";
    import type { Action } from "svelte/action";

    interface Props {
        width?: UserConfig["width"];
        height?: UserConfig["height"];
        autoWidth?: UserConfig["autoWidth"];
        fixedHeader?: UserConfig["fixedHeader"];
        resizable?: UserConfig["resizable"];
        from?: UserConfig["from"];
        language?: UserConfig["language"];
        search?: UserConfig["search"];
        sort?: UserConfig["sort"];
        pagination?: UserConfig["pagination"];
        server?: UserConfig["server"];
        columns?: UserConfig["columns"];
        data?: UserConfig["data"];
        plugins?: UserConfig["plugins"];
        style?: UserConfig["style"];
        className?: UserConfig["className"];
        class?: string;
        instance?: Grid;
    }

    let {
        width = "100%",
        height = "auto",
        autoWidth = true,
        fixedHeader = false,
        resizable = false,
        from = undefined,
        language = undefined,
        search = false,
        sort = false,
        pagination = false,
        server = undefined,
        columns = undefined,
        data = undefined,
        plugins = undefined,
        style = {},
        className = {},
        class: articalClass = "",
        instance = $bindable(),
    }: Props = $props();

    instance = new Grid({
        from,
        data,
        columns,
        server,
        search,
        sort,
        pagination,
        language,
        width,
        height,
        autoWidth,
        plugins,
        fixedHeader,
        resizable,
        style,
        className,
    });

    let node: Element = $state() as Element;
    const dispatch = createEventDispatcher();

    // https://github.com/grid-js/gridjs/blob/master/src/view/table/events.ts
    instance.on("cellClick", (...args) => dispatch("cellClick", { ...args }));
    instance.on("rowClick", (...args) => dispatch("rowClick", { ...args }));

    // https://github.com/grid-js/gridjs/blob/master/src/view/events.ts
    instance.on("beforeLoad", () => dispatch("beforeLoad"));
    instance.on("load", (data) => dispatch("load", { ...data }));
    instance.on("ready", () => dispatch("ready"));

    $effect(() => {
        if (node) {
            instance
                .updateConfig({
                    from,
                    data,
                    columns,
                    server,
                    search,
                    sort,
                    pagination,
                    language,
                    width,
                    height,
                    autoWidth,
                    fixedHeader,
                    style,
                    className,
                    resizable,
                })
                .forceRender();
        }
    });

    const renderGrid: Action = (node) => {
        instance.render(node);
    };
</script>

<article class={articalClass} bind:this={node} use:renderGrid></article>
