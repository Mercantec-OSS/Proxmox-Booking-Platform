import { clusterService } from '$lib/services/cluster-service';

export const load = async ({ parent, params, cookies }) => {
  const { userInfo } = await parent();

  let errorMessage;
  let clusterData = [];

  /* Return error if id is not an int */
  if (!Number.isInteger(Number(params.slug))) {
    errorMessage = 'Invalid booking ID';
  } else {
    try {
      // Fetch the cluster by id param
      clusterData = await clusterService.getClusterBookingByIdBackend(cookies.get('token'), params.slug);
    } catch (error) {
      errorMessage = error.message;
    }
  }

  return {
    errorMessage,
    clusterData,
    userInfo
  };
};
