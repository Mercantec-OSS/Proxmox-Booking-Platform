import { clusterService } from '$lib/services/cluster-service';
import { vmService } from '$lib/services/vm-service';
import { userService } from '$lib/services/user-service';

export async function load({ cookies }) {
  let vmTemplates = [];
  let listofUsers = [];
  let vmsAvailable;
  let clustersAvailable;

  try {
    // Run all fetches concurrently
    const token = cookies.get('token');
    const [templateResults, vmsResults, usersResults, clustersResults] = await Promise.all([
      vmService.getVMTemplatesBackend(token),
      vmService.getVMsAvailableCountBackend(token),
      userService.getAllUsersBackend(token),
      clusterService.getClustersAvailableCountBackend(token)
    ]);

    // Assign results
    vmTemplates = templateResults;
    vmsAvailable = vmsResults.length;
    listofUsers = usersResults;
    clustersAvailable = clustersResults.length;
  } catch (error) {
    console.error(error);
  }

  return {
    vmTemplates,
    vmsAvailable,
    clustersAvailable,
    listofUsers
  };
}
