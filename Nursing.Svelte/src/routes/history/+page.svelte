<script lang="ts">
    import { Database, type DBSession } from "$lib/db/mod";
    import { onMount } from "svelte";
    import { Grid } from "gridjs";
    import { Chart, type ChartConfiguration } from "chart.js/auto";
    import "./grid.css";
    import { formatDuration } from "$lib/utils/timeCalculations";
    import Tabs from "$lib/components/bootstrap/tabs/tabs.svelte";

    let sessions: DBSession[] = [];
    let lineCanvas: HTMLCanvasElement;
    let pieContainer: HTMLCanvasElement;
    let gridElement: HTMLDivElement;

    function parseDuration(duration: string): number {
        const [minutes, seconds] = duration.split(":").map(Number);
        return minutes * 60 + seconds;
    }

    onMount(async () => {
        const db = new Database();
        sessions = await db.getAllSessions();

        // Initialize Grid.js
        new Grid({
            columns: [
                {
                    name: "Date",
                    formatter: (cell) =>
                        (cell as Date).toLocaleDateString(),
                },
                {
                    name: "Start Time",
                    formatter: (cell) =>
                        (cell as Date).toLocaleTimeString([], {
                            hour: "2-digit",
                            minute: "2-digit",
                        }),
                },
                {
                    name: "End Time",
                    formatter: (cell) =>
                        (cell as Date).toLocaleTimeString([], {
                            hour: "2-digit",
                            minute: "2-digit",
                        }),
                },
                {
                    name: "Left Duration",
                    formatter: (cell) => formatDuration(cell as number),
                },
                {
                    name: "Right Duration",
                    formatter: (cell) => formatDuration(cell as number),
                }
            ],
            data: sessions.map((s) => [
                new Date(s.startTime),
                new Date(s.startTime),
                new Date(s.endTime),
                s.leftDuration,
                s.rightDuration,
            ]),
            sort: true,
            autoWidth: true,
        }).render(gridElement);

        const ctx = lineCanvas.getContext("2d");
        if (ctx) {
            new Chart(ctx, {
                type: "line",
                data: {
                    labels: sessions.map((s) => {
                        const date = new Date(s.startTime);
                        return `${date.toLocaleDateString()} ${date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}`;
                    }),
                    datasets: [
                        {
                            label: "Left Duration",
                            data: sessions.map((s) => s.leftDuration),
                            borderColor: "rgb(75, 192, 192)",
                        },
                        {
                            label: "Right Duration",
                            data: sessions.map((s) => s.rightDuration),
                            borderColor: "rgb(255, 99, 132)",
                        },
                        {
                            label: "Total Duration",
                            data: sessions.map(
                                (s) => s.leftDuration + s.rightDuration,
                            ),
                            borderColor: "rgb(54, 162, 235)",
                        },
                    ],
                },
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true,
                        },
                    },
                },
            });
        }

        const pieCtx = pieContainer.getContext("2d");
        if (pieCtx) {
            new Chart(pieCtx, {
                type: "pie",
                data: {
                    labels: ["Left Duration", "Right Duration"],
                    datasets: [
                        {
                            data: [
                                sessions.reduce(
                                    (acc, s) => acc + s.leftDuration,
                                    0,
                                ),
                                sessions.reduce(
                                    (acc, s) => acc + s.rightDuration,
                                    0,
                                ),
                            ],
                            backgroundColor: [
                                "rgb(75, 192, 192)",
                                "rgb(255, 99, 132)",
                            ],
                        },
                    ],
                },
                options: {
                    responsive: true,
                },
            });
        }
    });
</script>

<div class="history-page">
    <h1>Nursing History</h1>

    <Tabs tabs={["Grid", "Line", "Pie"]}>
        {#snippet gridTab()}
            <div class="grid-container" bind:this={gridElement}></div>
        {/snippet}
        {#snippet lineTab()}
            <div class="chart-container">
                <canvas bind:this={lineCanvas}></canvas>
            </div>
        {/snippet}
        {#snippet pieTab()}
            <div class="chart-container">
                <canvas bind:this={pieContainer}></canvas>
            </div>
        {/snippet}
    </Tabs>
</div>

<style>
    .history-page {
        padding: 1rem;
    }
    .chart-container {
        margin: 2rem 0;
        height: 300px;
    }
    .grid-container {
        margin: 2rem 0;
    }
</style>
