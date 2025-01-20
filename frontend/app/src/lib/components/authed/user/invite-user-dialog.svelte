<script>
  import { userService } from '$lib/services/user-service';
  import { userStore } from '$lib/utils/store';
  import * as Dialog from '$lib/components/ui/dialog';
  import { Label } from '$lib/components/ui/label';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Select from '$lib/components/ui/select/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { LoaderCircle } from 'lucide-svelte';
  import { toast } from 'svelte-sonner';

  let { inviteUserDialogOpen = $bindable() } = $props();

  let loadingStates = {
    inviteUser: false
  };

  let inviteUserInput = $state({
    email: null,
    role: null
  });

  /* Validates and submits booking extension request */
  async function handleInviteUser() {
    if (!inviteUserInput.email) {
      toast.error('Please add an email');
      return;
    }

    if (!inviteUserInput.role) {
      toast.error('Please select a role');
      return;
    }

    loadingStates.inviteUser = true;

    try {
      await userService.inviteUser(inviteUserInput);
      toast.success(`${inviteUserInput.email} invited`);
      inviteUserDialogOpen = false;
      inviteUserInput = {
        email: null,
        role: null
      };
    } catch (error) {
      toast.error(error.message);
    } finally {
      loadingStates.inviteUser = false;
    }
  }
</script>

<Dialog.Root bind:open={inviteUserDialogOpen}>
  <Dialog.Content class="sm:max-w-[425px]">
    <Dialog.Header>
      <Dialog.Title>Invite User</Dialog.Title>
      <Dialog.Description>Invite a new user by entering their email and selecting a role.</Dialog.Description>
    </Dialog.Header>
    <div class="grid gap-4 py-4">
      <div class="grid grid-cols-4 items-center gap-4">
        <Label for="Email" class="text-right">Email</Label>
        <Input id="Email" placeholder="user@mercantec.dk" bind:value={inviteUserInput.email} class="col-span-3" />
      </div>
      <div class="grid grid-cols-4 items-center gap-4">
        <Label for="Role" class="text-right">Role</Label>
        <div class="col-span-3">
          <Select.Root type="single" name="Select Role" bind:value={inviteUserInput.role}>
            <Select.Trigger class="w-full">
              {inviteUserInput.role ?? 'Select a role'}
            </Select.Trigger>
            <Select.Content>
              <Select.Group>
                <Select.GroupHeading>Roles</Select.GroupHeading>
                {#if $userStore.role === 'Admin'}
                  <Select.Item value="Admin" label="Admin">Admin</Select.Item>
                {/if}
                <Select.Item value="Teacher" label="Teacher">Teacher</Select.Item>
              </Select.Group>
            </Select.Content>
          </Select.Root>
        </div>
      </div>
    </div>
    <Dialog.Footer>
      <Button variant="outline" onmousedown={() => (inviteUserDialogOpen = false)}>Cancel</Button>
      <Button type="submit" disabled={loadingStates.inviteUser} onmousedown={handleInviteUser}>
        {#if loadingStates.inviteUser}
          <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
        {/if}
        Invite
      </Button>
    </Dialog.Footer>
  </Dialog.Content>
</Dialog.Root>
