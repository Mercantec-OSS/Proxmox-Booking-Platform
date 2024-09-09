export const handle = async ({ event, resolve }) => {
  event.locals.userToken = event.cookies.get('token');
  const response = await resolve(event);
  return response;
};
