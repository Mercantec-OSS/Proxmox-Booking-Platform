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

export const clusterService = {
  async createClusterBooking(bookingDetails) {
    return await clientApi.post('cluster-booking/create', { json: bookingDetails }).text();
  },

  async getClusterBookings() {
    return await clientApi.get('cluster-booking/all').json();
  },

  async getClusterBookingsBackend(cookie) {
    return await backendApi
      .get('cluster-booking/all', {
        headers: {
          Cookie: `token=${cookie}`
        }
      })
      .json();
  },

  async getClusterBookingsByUserBackend(cookie, id) {
    return await backendApi
      .get(`cluster-booking/owner/${id}`, {
        headers: {
          Cookie: `token=${cookie}`
        }
      })
      .json();
  },

  async getClusterBookingsByUser(id) {
    return await backendApi.get(`cluster-booking/owner/${id}`).json();
  },

  async getClusterBookingById(id) {
    return await clientApi.get(`cluster-booking/${id}`).json();
  },

  async getClusterBookingByIdBackend(cookie, id) {
    return await backendApi
      .get(`cluster-booking/${id}`, {
        headers: {
          Cookie: `token=${cookie}`
        }
      })
      .json();
  },

  async getClustersAvailableCount() {
    return await clientApi.get('vcenter/all/available').json();
  },

  async getClustersAvailableCountBackend(cookie) {
    return await backendApi
      .get('vcenter/all/available', {
        headers: {
          Cookie: `token=${cookie}`
        }
      })
      .json();
  },

  async deleteClusterBooking(id) {
    return await clientApi.delete(`cluster-booking/delete/${id}`).text();
  },

  // Below is endpoint for cluster actions
  async installVcenters(id) {
    return await clientApi.get(`script/cluster/vcenter/install-by-booking-id/${id}`).json();
  },

  async resetHosts(id) {
    return await clientApi.get(`script/cluster/host/reset-by-booking-id/${id}`).json();
  },

  async resetAndInstall(id) {
    return await clientApi.get(`script/cluster/vcenter/reset-and-install-by-booking-id/${id}`).json();
  }
};
