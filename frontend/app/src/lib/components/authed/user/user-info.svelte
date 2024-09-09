<!-- Component to show name, surname, email and role on user page -->

<script>
  import * as Avatar from '$lib/components/ui/avatar/index.js';
  import { Badge } from '$lib/components/ui/badge';
  import * as Select from '$lib/components/ui/select';
  import { userService } from '$lib/services/user-service';
  import { toast } from 'svelte-sonner';
  import { userStore } from '$lib/utils/store';

  export let user;

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
  <!-- Avatar -->
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
    <Select.Root
      onSelectedChange={(value) => {
        updateRole(value.value);
      }}
    >
      <Select.Trigger class="w-[180px]" aria-label="Select user role">
        <Select.Value placeholder={user.role} />
      </Select.Trigger>
      <Select.Content>
        <Select.Group>
          <Select.Label>Roles</Select.Label>
          <Select.Item value="Administrator" label="Administrator">Administrator</Select.Item>
          <Select.Item select value="Teacher" label="Teacher">Teacher</Select.Item>
          <Select.Item value="Student" label="Student">Student</Select.Item>
        </Select.Group>
      </Select.Content>
      <Select.Input name="roles" />
    </Select.Root>
  {:else}
    <Badge>{user.role}</Badge>
  {/if}
</div>
