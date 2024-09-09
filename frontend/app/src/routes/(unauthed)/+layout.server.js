import { authCheck } from '$lib/middlewares/auth-check';

/* Check if user is authenticated */
export async function load({ locals, cookies }) {
  await authCheck({ locals, cookies }, 'unauthed');
}
