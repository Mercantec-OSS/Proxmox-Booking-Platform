import { vmService } from '$lib/services/vm-service';

export async function load({ cookies }) {
  let vmData = [];

  try {
    const token = cookies.get('token');
    vmData = await vmService.getVMBookingsBackend(token);
  } catch (error) {
    console.error(error);
  }

  return {
    vmData
  };
}
