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
    const userId = params.slug;

    // Define the promises we'll always run
    const fetchPromises = [userService.getUserByIdBackend(token, userId)];

    // If user is teacher or administrator, add booking fetches
    if (userInfo.role === 'Admin' || userInfo.role === 'Teacher') {
      fetchPromises.push(clusterService.getClusterBookingsByUserBackend(token, userId), vmService.getVMBookingsByUserBackend(token, userId));
    }

    // Run all fetches concurrently
    const results = await Promise.all(fetchPromises);

    // Always assign user data (first result)
    userData = results[0];

    // Assign booking data if fetched
    if (results.length > 1) {
      [, clusterData, vmData] = results;
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
