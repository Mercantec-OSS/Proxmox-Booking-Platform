<script>
  import * as Command from '$lib/components/ui/command/index.js';
  import { Button } from '$lib/components/ui/button/index.js';
  import { Search, GraduationCap, Shield, School, LoaderCircle } from 'lucide-svelte';
  import { goto } from '$app/navigation';
  import { toast } from 'svelte-sonner';
  import { userService } from '$lib/services/user-service';
  import { onMount } from 'svelte';

  let listOfUsers = [];
  let open = false;
  let loading = false;
  let loadingProfile = false;
  let selectedUser;

  // Role priority map
  const rolePriority = {
    Admin: 1,
    Teacher: 2,
    Student: 3
  };

  // Sort listOfUsers by role based on rolePriority map
  $: listOfUsers.sort((a, b) => {
    // Get the role priority, or assign a high number if role is unknown to ensure they sort last
    const roleA = rolePriority[a.role] || 99;
    const roleB = rolePriority[b.role] || 99;
    return roleA - roleB;
  });

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

  async function fetchUsers() {
    loading = true;

    try {
      listOfUsers = await userService.getAllUsers();
    } catch (error) {
      toast.error(error.message);
    } finally {
      loading = false;
    }
  }

  onMount(() => {
    fetchUsers();
  });
</script>

<Button
  variant="outline"
  on:click={() => {
    open = true;
  }}
>
  <Search class="mr-2 h-4 w-4" />
  Search for user
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
