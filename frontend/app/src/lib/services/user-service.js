import { env } from '$env/dynamic/public';
import ky from 'ky';

const clientApi = ky.create({
  prefixUrl: env.PUBLIC_CLIENT_API,
  timeout: 10000,
  hooks: {
    beforeError: [
      async (error) => {
        const { response } = error;
        if (response?.body) {
          try {
            const { message } = await response.json();
            if (message) {
              error.message = message;
            }
          } catch {
            error.message = 'Something went wrong. Please try again later.';
          }
        }
        return error;
      }
    ]
  }
});

const backendApi = ky.create({
  prefixUrl: env.PUBLIC_BACKEND_API,
  timeout: 10000,
  hooks: {
    beforeError: [
      async (error) => {
        const { response } = error;
        if (response?.body) {
          try {
            const { message } = await response.json();
            if (message) {
              error.message = message;
            }
          } catch {
            error.message = 'Something went wrong. Please try again later.';
          }
        }
        return error;
      }
    ]
  }
});

export const userService = {
  async getUserById(cookie, id) {
    return await backendApi
      .get(`users/${id}`, {
        headers: {
          Cookie: `token=${cookie}`
        }
      })
      .json();
  },

  async getAllUsers() {
    return await clientApi.get('users/all').json();
  },

  async updaterole(user) {
    return await clientApi.put('update/role', { json: user }).json();
  }
};
