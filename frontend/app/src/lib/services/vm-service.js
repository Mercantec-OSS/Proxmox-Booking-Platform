import { env } from '$env/dynamic/public';
import ky from 'ky';

const clientApi = ky.create({
  prefixUrl: env.PUBLIC_CLIENT_API,
  timeout: 60000,
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
  timeout: 60000,
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

export const vmService = {
  async createVMBooking(bookingDetails) {
    return await clientApi.post('vm-booking/create', { json: bookingDetails }).text();
  },

  async getVMBookingsBackend(cookie) {
    return await backendApi
      .get('vm-booking/all', {
        headers: {
          Cookie: `token=${cookie}`
        }
      })
      .json();
  },

  async getVMBookingsFrontend() {
    return await clientApi.get('vm-booking/all').json();
  },

  async getVMBookingsByUserBackend(cookie, id) {
    return await backendApi
      .get(`vm-booking/owner/${id}`, {
        headers: {
          Cookie: `token=${cookie}`
        }
      })
      .json();
  },

  async getVMBookingsByUserFrontend(id) {
    return await backendApi.get(`vm-booking/owner/${id}`).json();
  },

  async getVMBookingById(id) {
    return await clientApi.get(`vm-booking/${id}`).json();
  },

  async getVMsAvailableCount() {
    return await clientApi.get('vcenter/all/available').json();
  },

  async deleteVMBooking(id) {
    return await clientApi.delete(`vm-booking/delete/${id}`).text();
  },

  async getVMTemplates() {
    return await clientApi.get('script/vm/templates').json();
  },

  async acceptVMBooking(id) {
    return await clientApi.put(`vm-booking/accept/${id}`).json();
  },

  async extendVmBooking(extendDetails) {
    return await clientApi.post('extention-request/create', { json: extendDetails }).json();
  },

  async acceptExtendVmBooking(id) {
    return await clientApi.put(`extention-request/accept-extention/${id}`).json();
  },

  async getVmInfo(id) {
    return await clientApi.get(`script/vm/get-ip/${id}`).json();    
  },

  async resetVmPower(name) {
    await clientApi.get(`script/vm/reset-power/${name}`).json();    
  }
};
