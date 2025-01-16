import { vmService } from '$lib/services/vm-service';
import { userService } from '$lib/services/user-service';

export async function load({ parent, cookies }) {
  const { userInfo } = await parent();
  let vmTemplates = [];
  let listOfUsers = [];

  try {
    const token = cookies.get('token');

    // Define the promises for vm templates and list of user fetches
    const fetchPromises = [vmService.getVMTemplatesBackend(token), userService.getAllUsersBackend(token)];

    // Run all fetches concurrently
    const results = await Promise.all(fetchPromises);

    // Assign results
    [vmTemplates, listOfUsers] = results;

    // sort by display name
    vmTemplates.sort((a, b) => a.displayName.localeCompare(b.displayName));
  } catch (error) {
    console.error(error);
  }

  return {
    vmTemplates,
    listOfUsers
  };
}
