<script lang="ts">
    import { Database, type DBSession } from '$lib/db/mod';
    import { onMount } from 'svelte';
    import { Grid } from "gridjs";
    import { Chart, type ChartConfiguration } from 'chart.js/auto';
    import './grid.css';
    
    let sessions: DBSession[] = [];
    let chartCanvas: HTMLCanvasElement;
    let gridElement: HTMLDivElement;

    function parseDuration(duration: string): number {
        const [minutes, seconds] = duration.split(':').map(Number);
        return minutes * 60 + seconds;
    }

    onMount(async () => {
        const db = new Database();
        sessions = await db.getAllSessions();
        
        // Initialize Grid.js
        new Grid({
            columns: ['Date', 'Start Time', 'End Time', 'Left Duration', 'Right Duration'],
            data: sessions.map(s => [
                new Date(s.startTime).toLocaleDateString(),
                new Date(s.startTime).toLocaleTimeString(),
                new Date(s.endTime).toLocaleTimeString(),
                s.leftDuration,
                s.rightDuration
            ]),
            sort: true,
        }).render(gridElement);

        // Initialize Chart.js
        const ctx = chartCanvas.getContext('2d');
        if (ctx) {
            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: sessions.map(s => new Date(s.startTime).toLocaleDateString()),
                    datasets: [{
                        label: 'Left Duration',
                        data: sessions.map(s => parseDuration(s.leftDuration)),
                        borderColor: 'rgb(75, 192, 192)',
                    }, {
                        label: 'Right Duration',
                        data: sessions.map(s => parseDuration(s.rightDuration)),
                        borderColor: 'rgb(255, 99, 132)',
                    }, {
                        label: 'Total Duration',
                        data: sessions.map(s => parseDuration(s.leftDuration) + parseDuration(s.rightDuration)),
                        borderColor: 'rgb(54, 162, 235)',
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
        }
    });
</script>

<div class="history-page">
    <h1>Nursing History</h1>
    <div class="chart-container">
        <canvas bind:this={chartCanvas}></canvas>
    </div>
    <div class="grid-container" bind:this={gridElement}></div>
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