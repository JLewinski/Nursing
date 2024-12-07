<script lang="ts">
	import { enhance } from "$app/forms";
	import type { PageServerData } from "./$types";
	import { syncStore } from "$lib/stores/syncStore.svelte";
	import { db } from "$lib/db/mod";
	import { onMount } from "svelte";
	import { liveQuery } from "dexie";

	let { data }: { data: PageServerData } = $props();
	let startDateString = $state(
		(() => {
			const date = new Date();
			date.setDate(date.getDate() - 7);
			date.setHours(0, 0, 0, 0);
			return date.toISOString().split("T")[0];
		})(),
	);

	let sessions = $derived(getSessions(parseDateString(startDateString)));

	function getSessions(startDate: Date) {
		return liveQuery(() => {
			return db.sessions
				.where("lastUpdated")
				.aboveOrEqual(startDate)
				.toArray();
		});
	}

	function parseDateString(dateString: string): Date {
		const [year, month, day] = dateString
			.split("-")
			.map((x) => Number.parseInt(x));
		return new Date(year, month - 1, day);
	}

	function syncData() {
		syncStore.syncData();
	}

	let status = $derived(syncStore.status);
</script>

<div class="container">
	<label for="startDate" class="form-label">Start Date</label>
	<input type="date" class="form-control" bind:value={startDateString} />

	<h1>Hi, {data.user.username}!</h1>
	<p>Your user ID is {data.user.id}.</p>
	<p>Last sync date is {syncStore.lastSync?.toLocaleString()}</p>
	<p>Sync status is {status}</p>
	<form method="post" action="?/logout" use:enhance>
		<button class="btn btn-danger">Sign out</button>
	</form>
	<button class="btn btn-primary" onclick={syncData}>Sync data</button>

	<form class="py-4" method="post" action="?/group">
		<input class="form-control" name="userId" type="text" />
		<button class="btn btn-primary mt-4">Join Group</button>
	</form>
</div>
