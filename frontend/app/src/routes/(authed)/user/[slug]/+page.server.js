import { userService } from '$lib/services/user-service';
import { clusterService } from '$lib/services/cluster-service';
import { vmService } from '$lib/services/vm-service';

export const load = async ({ parent, params, cookies }) => {
  const { userInfo } = await parent();
  let userData;
  let errorMessage;
  let clusterData = [];
  let vmData = [];

  /* Return early if id is not an int */
  if (!Number.isInteger(Number(params.slug))) {
    return {
      userData,
      errorMessage: 'Invalid user ID',
      clusterData,
      vmData
    };
  }

  try {
    const token = cookies.get('token');

    // First, fetch user data
    userData = await userService.getUserByIdBackend(token, params.slug);

    // If user is teacher or administrator, fetch bookings concurrently
    if (userInfo.role === 'Admin' || userInfo.role === 'Teacher') {
      const [clusterResults, vmResults] = await Promise.all([clusterService.getClusterBookingsByUserBackend(token, userData.id), vmService.getVMBookingsByUserBackend(token, userData.id)]);

      clusterData = clusterResults;
      vmData = vmResults;
    }
  } catch (error) {
    errorMessage = error.message;
  }

  return {
    userData,
    errorMessage,
    clusterData,
    vmData
  };
};
