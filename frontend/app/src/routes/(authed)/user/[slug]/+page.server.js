import { vmService } from '$lib/services/vm-service';

export const load = async ({ params, cookies }) => {
  let userData;
  let errorMessage;
  let vmData = [];

  /* Return early if id is not an int */
  if (!Number.isInteger(Number(params.slug))) {
    return {
      userData,
      errorMessage: 'Invalid user ID',
      vmData
    };
  }

  try {
    const token = cookies.get('token');
    const userId = params.slug;

   vmData = vmService.getVMBookingsByUserBackend(token, userId);
  } catch (error) {
    errorMessage = error.message;
  }

  return {
    userData,
    errorMessage,
    vmData
  };
};
