<script>
  import * as Command from '$lib/components/ui/command/index.js';
  import { Button } from '$lib/components/ui/button/index.js';
  import { Search, GraduationCap, Shield, School, LoaderCircle } from 'lucide-svelte';
  import { goto } from '$app/navigation';
  import { toast } from 'svelte-sonner';
  import { userService } from '$lib/services/user-service';

  let listOfUsers = $state([]);
  let open = $state(false);
  let loading = $state(false);
  let loadingProfile = $state(false);
  let selectedUser = $state();

  // Maps roles to priority values for sorting
  const rolePriority = {
    Admin: 1,
    Teacher: 2,
    Student: 3
  };

  // Handles user profile navigation with loading states
  async function redirectUser(id) {
    selectedUser = id;
    try {
      loadingProfile = true;
      await goto(`/user/${id}`);
      open = false;
    } catch (error) {
      toast.error(error.message);
    } finally {
      loadingProfile = false;
      selectedUser = null;
    }
  }

  // Fetches and populates user list with error handling
  async function fetchUsers() {
    loading = true;

    try {
      listOfUsers = await userService.getAllUsers();
      listOfUsers.sort((a, b) => {
        const roleA = rolePriority[a.role] || 99;
        const roleB = rolePriority[b.role] || 99;
        return roleA - roleB;
      });
    } catch (error) {
      toast.error(error.message);
    } finally {
      loading = false;
    }
  }

  // Initialize user data on component mount
  $effect(() => {
    fetchUsers();
  });
</script>

<Button
  variant="outline"
  onmousedown={() => {
    open = true;
  }}
>
  <Search class="mr-2 h-4 w-4" />
  Search for a user
</Button>

<Command.Dialog bind:open>
  <Command.Input placeholder="Search for a user..." />
  <Command.List>
    {#if loading}
      <Command.Loading>Loading...</Command.Loading>
    {/if}
    <Command.Empty>No results found.</Command.Empty>
    <Command.Group>
      {#each listOfUsers as user (user.id)}
        <Command.Item
          onSelect={() => {
            redirectUser(user.id);
          }}
          disabled={loadingProfile && selectedUser === user.id}
        >
          {#if loadingProfile && selectedUser === user.id}
            <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
          {:else if user.role == 'Student'}
            <GraduationCap class="mr-2 h-4 w-4" />
          {:else if user.role == 'Teacher'}
            <School class="mr-2 h-4 w-4" />
          {:else if user.role == 'Admin'}
            <Shield class="mr-2 h-4 w-4" />
          {/if}
          <span>{user.name} {user.surname} </span><span class="hidden">{user.id}</span></Command.Item
        >
      {/each}
    </Command.Group>
    <Command.Separator />
  </Command.List>
</Command.Dialog>
