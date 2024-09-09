import { clusterService } from '$lib/services/cluster-service';
import { vmService } from '$lib/services/vm-service';

export async function load({ parent, cookies }) {
  const { userInfo } = await parent();

  let clusterData = [];
  let vmData = [];

  try {
    // Get cluster bookings if user is teacher or administrator
    if (userInfo.role === 'Admin' || userInfo.role === 'Teacher') {
      clusterData = await clusterService.getClusterBookingsBackend(cookies.get('token'));
    }

    // Get vm bookings
    vmData = await vmService.getVMBookingsBackend(cookies.get('token'));
  } catch (error) {
    console.error(error);
  }

  return {
    clusterData,
    vmData
  };
}
