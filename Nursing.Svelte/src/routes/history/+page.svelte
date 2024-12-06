<script lang="ts">
    import { db } from "$lib/db/mod";
    import Grid from "$lib/components/Grid.svelte";
    import { Grid as GridJs } from "gridjs";
    import { Chart } from "chart.js/auto";
    import "./grid.css";
    import { formatDuration } from "$lib/utils/timeCalculations";
    import Tabs from "$lib/components/bootstrap/tabs/tabs.svelte";
    import TabContent from "$lib/components/bootstrap/tabs/tabContent.svelte";
    import { liveQuery } from "dexie";

    function parseDateString(dateString: string): Date {
        const [year, month, day] = dateString
            .split("-")
            .map((x) => Number.parseInt(x));
        return new Date(year, month - 1, day);
    }

    let startDateString = $state(
        (() => {
            const date = new Date();
            date.setDate(date.getDate() - 7);
            date.setHours(0, 0, 0, 0);
            return date.toISOString().split("T")[0];
        })(),
    );

    let endDate = $state(new Date());

    function getSessions(startDate: Date) {
        return liveQuery(() => {
            return db.sessions
                .orderBy("startTime")
                .filter(
                    (s) => s.startTime >= startDate && s.startTime <= endDate,
                )
                .toArray();
        });
    }

    let sessions = $derived(getSessions(parseDateString(startDateString)));

    let lineCanvas: HTMLCanvasElement;
    let pieContainer: HTMLCanvasElement;

    let gridData = $derived(
        $sessions
            ?.map((s) => [
                s.startTime,
                s.startTime,
                s.endTime,
                s.leftDuration,
                s.rightDuration,
            ])
            .reverse() ?? [],
    );

    $effect(() => {
        if (!$sessions) return;
        console.log('line')

        const ctx = lineCanvas.getContext("2d");
        if (ctx) {
            new Chart(ctx, {
                type: "line",
                data: {
                    labels: $sessions?.map((s) => {
                        const date = new Date(s.startTime);
                        return `${date.toLocaleDateString()} ${date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}`;
                    }),
                    datasets: [
                        {
                            label: "Left Duration",
                            data: $sessions?.map((s) => s.leftDuration),
                            borderColor: "rgb(75, 192, 192)",
                        },
                        {
                            label: "Right Duration",
                            data: $sessions?.map((s) => s.rightDuration),
                            borderColor: "rgb(255, 99, 132)",
                        },
                        {
                            label: "Total Duration",
                            data: $sessions?.map(
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
    });

    $effect(() => {
        if (!$sessions) return;

        const pieCtx = pieContainer.getContext("2d");
        if (pieCtx) {
            new Chart(pieCtx, {
                type: "pie",
                data: {
                    labels: ["Left Duration", "Right Duration"],
                    datasets: [
                        {
                            data: [
                                $sessions?.reduce(
                                    (acc, s) => acc + s.leftDuration,
                                    0,
                                ),
                                $sessions?.reduce(
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

    <div class="row">
        <div class="col">
            <label for="startDate" class="form-label">Start Date</label>
            <input
                type="date"
                class="form-control"
                bind:value={startDateString}
            />
        </div>
        <div class="col">
            <label for="endDate" class="form-label">End Date</label>
            <input type="date" class="form-control" bind:value={endDate} />
        </div>
    </div>

    <Tabs>
        <TabContent title="Grid">
            <Grid
                data={gridData}
                columns={[
                    {
                        name: "Date",
                        formatter: (cell: Date) => cell.toLocaleDateString(),
                    },
                    {
                        name: "Start Time",
                        formatter: (cell: Date) =>
                            cell.toLocaleTimeString([], {
                                hour: "2-digit",
                                minute: "2-digit",
                            }),
                    },
                    {
                        name: "End Time",
                        formatter: (cell: Date) =>
                            cell.toLocaleTimeString([], {
                                hour: "2-digit",
                                minute: "2-digit",
                            }),
                    },
                    {
                        name: "Left Duration",
                        formatter: (cell: number) => formatDuration(cell),
                    },
                    {
                        name: "Right Duration",
                        formatter: (cell: number) => formatDuration(cell),
                    },
                ]}
                sort={true}
                autoWidth={true}
            />
        </TabContent>
        <TabContent title="Line">
            <div class="chart-container">
                <canvas bind:this={lineCanvas}></canvas>
            </div>
        </TabContent>
        <TabContent title="Pie">
            <div class="chart-container">
                <canvas bind:this={pieContainer}></canvas>
            </div>
        </TabContent>
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
</style>
