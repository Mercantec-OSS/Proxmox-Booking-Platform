import { vmService } from '$lib/services/vm-service';

export const load = async ({ parent, params, cookies }) => {
  const { userInfo } = await parent();

  let userData;
  let errorMessage;
  let clusterData = [];
  let vmData = [];

  /* Return error if id is not an int */
  if (!Number.isInteger(Number(params.slug))) {
    errorMessage = 'Invalid booking ID';
  } else {
    try {
      // Fetch the virtual machine by id param
      vmData = await vmService.getVMBookingByIdBackend(cookies.get('token'), params.slug);
    } catch (error) {
      errorMessage = error.message;
    }
  }

  return {
    errorMessage,
    vmData,
    userInfo
  };
};
