import { vmService } from '$lib/services/vm-service';

export const load = async ({ parent, params, cookies }) => {
  const [{ userInfo }, id] = await Promise.all([parent(), Promise.resolve(Number(params.slug))]);

  if (!Number.isInteger(id)) {
    return {
      errorMessage: 'Invalid booking ID',
      vmData: {},
      userInfo
    };
  }

  try {
    const token = cookies.get('token');

    // First fetch vmData
    const vmData = await vmService.getVMBookingByIdBackend(token, id);

    if (vmData.uuid && vmData.isAccepted) {
      const [creds, isoList, usageInfo] = await Promise.all([vmService.getVmInfoBackend(token, vmData.uuid), vmService.getIsoListBackend(token), vmService.getUsageInfoBackend(token, vmData.uuid)]);

      return {
        vmData: { ...vmData, ...creds, isoList, usageInfo },
        userInfo
      };
    }

    return { vmData, userInfo };
  } catch (error) {
    return {
      errorMessage: error.message,
      vmData: {},
      userInfo
    };
  }
};
