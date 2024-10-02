import { clusterService } from '$lib/services/cluster-service';
import { vmService } from '$lib/services/vm-service';

export async function load({ parent, cookies }) {
  const { userInfo } = await parent();
  let clusterData = [];
  let vmData = [];

  try {
    const token = cookies.get('token');

    // Always fetch VM data
    const vmDataPromise = vmService.getVMBookingsBackend(token);

    // Fetch cluster data if users is an admin or teacher
    const clusterDataPromise = userInfo.role === 'Admin' || userInfo.role === 'Teacher' ? clusterService.getClusterBookingsBackend(token) : Promise.resolve([]);

    // Wait for both promises to resolve
    [vmData, clusterData] = await Promise.all([vmDataPromise, clusterDataPromise]);
  } catch (error) {
    console.error(error);
  }

  return {
    clusterData,
    vmData
  };
}
