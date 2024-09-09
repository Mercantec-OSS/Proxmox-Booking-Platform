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

export const authService = {
  async register(credentials) {
    return clientApi.post('authorization/create/student', { json: credentials }).json();
  },

  async login(credentials) {
    return clientApi.post('authorization/login', { json: credentials }).json();
  },

  async logout(token) {
    await clientApi.delete(`authorization/logout`);
  },

  async fetchUser(token) {
    return backendApi
      .get(`authorization/check-session`, {
        headers: {
          Cookie: `token=${token}`
        }
      })
      .json();
  }
};
