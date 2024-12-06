import * as auth from '$lib/server/auth';
import { fail, redirect } from '@sveltejs/kit';
import type { Actions, PageServerLoad } from './$types';
import sync from '$lib/server/sync';

export const load: PageServerLoad = async (event) => {
	if (!event.locals.user) {
		return redirect(302, '/account/login');
	}
	return { user: event.locals.user };
};

export const actions: Actions = {
	logout: async (event) => {
		if (!event.locals.session) {
			return fail(401);
		}
		await auth.invalidateSession(event.locals.session.id);
		auth.deleteSessionTokenCookie(event);

		return redirect(302, '/account/login');
	},
	sync: async (event) => {

		if (!event.locals.user) {
			return fail(401);
		}
		const data = await event.request.json();
		try {
			const syncDate = data.syncDate ? new Date(data.syncDate) : null;
			const updates = await sync(syncDate, event.locals.user.id, data.sessions);
			return {
				status: 200,
				body: { status: 'ok', syncDate: data.syncDate, updates },

			};
		} catch (e) {
			console.error('Sync error', e);
			return fail(410, { temp: data });
		}
	}
};
