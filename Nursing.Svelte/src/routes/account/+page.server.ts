import * as auth from '$lib/server/auth';
import { fail, json, redirect } from '@sveltejs/kit';
import type { Actions, PageServerLoad } from './$types';

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
	group: async (event) => {
		if (!event.locals.user) {
			return fail(401);
		}

		const data = await event.request.formData();



		return json({ status: 'error', error: 'Not implemented' }, { status: 501 });
	}
};
