import { Userinfo } from '$lib/api/Userinfo';

export const load = () => {
    const client = new Userinfo({ baseUrl: '/api' });
    return {
        userInfo: client.userInformation()
    };
};