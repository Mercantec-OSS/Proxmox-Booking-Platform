import { vmService } from '$lib/services/vm-service';

export const load = async ({ parent, params, cookies }) => {
  const [{ userInfo }, id] = await Promise.all([parent(), Promise.resolve(Number(params.slug))]);

  if (!Number.isInteger(id)) {
    return {
      errorMessage: 'Invalid booking ID',
      vmData: [],
      userInfo
    };
  }

  try {
    const token = cookies.get('token');
    const vmData = await vmService.getVMBookingByIdBackend(token, id);

    if (vmData.uuid && vmData.isAccepted) {
      const creds = await vmService.getVmInfoBackend(token, vmData.uuid);
      const isoList = await vmService.getIsoListBackend(token);
      // const usageInfo = await vmService.getUsageInfoBackend(token, vmData.uuid);
      return { vmData: { ...vmData, ...creds, isoList }, userInfo };
    }

    return { vmData, userInfo };
  } catch (error) {
    return {
      errorMessage: error.message,
      vmData: [],
      userInfo
    };
  }
};
