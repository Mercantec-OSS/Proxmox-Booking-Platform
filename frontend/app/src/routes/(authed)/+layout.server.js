import { authCheck } from '$lib/middlewares/auth-check';

export async function load({ locals, cookies }) {
  const userInfo = await authCheck({ locals, cookies }, 'authed');
  return {
    userInfo
  };
}
