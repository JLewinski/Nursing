import type { DBSession } from '$lib/db/mod.js';
import sync from '$lib/server/sync';
import { json } from '@sveltejs/kit';


export async function POST({ request, locals }) {

    if (!locals.user) {
        return json({ status: 'error', error: 'Unauthorized' }, { status: 401 });
    }

    const { syncDate, sessions }: {
        syncDate: Date | null,
        sessions: DBSession[]
    } = await (async () => {
        const data = await request.json();
        return {
            syncDate: data.syncDate ? new Date(data.syncDate) : null,
            sessions: data.sessions
        };
    })();

    try {
        const updates = await sync(syncDate, locals.user.id, sessions);
        return json({ status: 'ok', updates }, { status: 200 });
    } catch (e) {
        console.error('Sync error', e);
        return json({ status: 'error', error: 'Sync error' }, { status: 410 });
    }
}