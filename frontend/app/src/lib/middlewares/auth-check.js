import { authService } from '$lib/services/auth-service';
import { redirect } from '@sveltejs/kit';

/* Check if user is authorized or not and redirect if necessary */
export async function authCheck({ locals, cookies }, checkFor) {
  /* Don't validate token if it's undefined and user is on a public page*/
  if (!locals.userToken && checkFor === 'unauthed') {
    return;
  }

  let userIsAuthed = false;
  let authResponse = null;

  if (locals.userToken) {
    try {
      authResponse = await authService.fetchUser(locals.userToken);
      userIsAuthed = authResponse;
    } catch (error) {
      /* Delete cookie from user's browser if it expired or an error happened */
      cookies.set('token', '', {
        path: '/',
        expires: new Date(0),
        httpOnly: true
      });
    }
  }

  /* Redirect to specified page if user is not supposed to be there */
  if (userIsAuthed && checkFor === 'unauthed') {
    throw redirect(302, '/');
  } else if (!userIsAuthed && checkFor === 'authed') {
    throw redirect(302, '/login');
  }

  return authResponse;
}
