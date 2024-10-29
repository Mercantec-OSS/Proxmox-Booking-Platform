import { clusterService } from '$lib/services/cluster-service';
import { vmService } from '$lib/services/vm-service';
import { userService } from '$lib/services/user-service';

export async function load({ parent, cookies }) {
  const { userInfo } = await parent();
  let vmTemplates = [];
  let listOfUsers = [];
  let clustersAvailable = '?';

  try {
    const token = cookies.get('token');

    // Define the promises for vm templates and list of user fetches
    const fetchPromises = [vmService.getVMTemplatesBackend(token), userService.getAllUsersBackend(token)];

    // Only fetch clusters available if user is Admin or Teacher
    if (userInfo.role === 'Admin' || userInfo.role === 'Teacher') {
      fetchPromises.push(clusterService.getClustersAvailableCountBackend(token));
    }

    // Run all fetches concurrently
    const results = await Promise.all(fetchPromises);

    // Assign results
    [vmTemplates, listOfUsers] = results;

    // sort by display name
    vmTemplates.sort((a, b) => a.displayName.localeCompare(b.displayName));

    // Assign clusters result if it was fetched
    if (results.length > 2) {
      clustersAvailable = results[2].length;
    }
  } catch (error) {
    console.error(error);
  }

  return {
    vmTemplates,
    clustersAvailable,
    listOfUsers
  };
}
