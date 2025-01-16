<script>
  import * as Avatar from '$lib/components/ui/avatar/index.js';
  import { Badge } from '$lib/components/ui/badge';
  import * as Select from '$lib/components/ui/select';
  import { userService } from '$lib/services/user-service';
  import { toast } from 'svelte-sonner';
  import { userStore } from '$lib/utils/store';

  let { user } = $props();

  let selectedRole = $state(user.role);

  $effect(() => {
    if (selectedRole === user.role) return;
    updateRole(selectedRole);
  });

  async function updateRole(role) {
    user.role = role;
    try {
      await userService.updaterole(user);
      toast.success(`Changed ${user.name}'s role to ${role}`);
    } catch (error) {
      toast.error(error.message);
    }
  }
</script>

<div class="flex flex-wrap gap-x-5 items-center">
  <Avatar.Root class="select-none">
    <Avatar.Fallback>{user.name[0]}{user.surname[0]}</Avatar.Fallback>
  </Avatar.Root>
  <!-- Name and surname -->
  <p class="font-medium">{user.name} {user.surname}</p>
  <!-- Email -->
  <a class="font-medium" href="mailto:{user.email}">{user.email}</a>
  <p>{new Date(user.creationAt).toLocaleDateString(undefined, { dateStyle: 'long' })}</p>
  <!-- Role -->
  {#if $userStore.role === 'Admin'}
    <Select.Root type="single" bind:value={selectedRole}>
      <Select.Trigger class="w-[180px]" aria-label="Select user role">
        {selectedRole}
      </Select.Trigger>
      <Select.Content>
        <Select.Item value="Admin">Admin</Select.Item>
        <Select.Item value="Teacher">Teacher</Select.Item>
        <Select.Item value="Student">Student</Select.Item>
      </Select.Content>
    </Select.Root>
  {:else}
    <Badge>{user.role}</Badge>
  {/if}
</div>
