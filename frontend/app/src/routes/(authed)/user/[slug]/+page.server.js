import { userService } from '$lib/services/user-service';
import { clusterService } from '$lib/services/cluster-service';
import { vmService } from '$lib/services/vm-service';

export const load = async ({ parent, params, cookies }) => {
  const { userInfo } = await parent();

  let userData;
  let errorMessage;
  let clusterData = [];
  let vmData = [];

  /* Return error if id is not an int */
  if (!Number.isInteger(Number(params.slug))) {
    errorMessage = 'Invalid user ID';
  } else {
    try {
      /* Fetch user data based on id */
      userData = await userService.getUserById(cookies.get('token'), params.slug);

      // Fetch bookings if user is teacher or administrator
      if (userInfo.role === 'Admin' || userInfo.role === 'Teacher') {
        clusterData = await clusterService.getClusterBookingsByUserBackend(cookies.get('token'), userData.id);
        vmData = await vmService.getVMBookingsByUserBackend(cookies.get('token'), userData.id);
      }
    } catch (error) {
      errorMessage = error.message;
    }
  }

  return {
    userData,
    errorMessage,
    clusterData,
    vmData
  };
};
