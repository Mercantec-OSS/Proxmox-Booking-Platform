import { vmService } from '$lib/services/vm-service';

export async function load({ cookies }) {
  let vmData = [];
  let vcenterInfo = null;
  const token = cookies.get('token');

  try {
    [vmData, vcenterInfo] = await Promise.all([
      vmService.getVMBookingsBackend(token).catch((error) => {
        console.error(error);
        return [];
      }),
      vmService.getVcenterInfoBackend(token).catch((error) => {
        console.error(error);
        return null;
      })
    ]);
  } catch (error) {
    console.error(error);
  }

  return {
    vmData,
    vcenterInfo
  };
}
