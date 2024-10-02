<script>
  import { vmService } from '$lib/services/vm-service';
  import VMExtensionDialog from '$lib/components/authed/bookings/vm/vm-extension-dialog.svelte';
  import { vmListStore, selectedBookingStore } from '$lib/utils/store';
  import AlertDialog from '$lib/components/authed/alert-dialog.svelte';
  import { Download, Trash2, RefreshCcw, CalendarPlus, Zap, ChevronDown, MonitorUp } from 'lucide-svelte';
  import { toast } from 'svelte-sonner';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
  import { Button } from '$lib/components/ui/button/index.js';
  import { goto } from '$app/navigation';

  let vmExtensionDialogOpen;
  let open = false;

  /* Alert dialog */
  let alertDialogOpen;
  let alertTitle = null;
  let alertDescription = null;
  let resolveAction;

  /* Promt user with alert before executing an action */
  /* Run when the user presses confirm or cancel using on:notify on the alert component */
  function handleAnswer(event) {
    alertDialogOpen = false;
    // Resolve the prompUser promise when the event is handled
    if (resolveAction) {
      resolveAction(event.detail.confirmed);
    }
  }

  /* Show the alert dialog with inputted text and return/resolve with true or false depending on what button the user clicked */
  function promptUser(title, description) {
    return new Promise((resolve) => {
      alertTitle = title;
      alertDescription = description;
      alertDialogOpen = true;
      resolveAction = resolve;
    });
  }

  /* Refresh specific booking based on id */
  async function refreshBooking() {
    try {
      const updatedBooking = await vmService.getVMBookingById($selectedBookingStore.id);

      toast.success(`Refreshed booking details`);

      selectedBookingStore.set(updatedBooking);

      vmListStore.update((bookings) => {
        const updatedBookings = bookings.map((booking) => (booking.id === $selectedBookingStore.id ? { ...booking, ...updatedBooking } : booking));
        return updatedBookings;
      });
    } catch (error) {
      toast.error(error.message);
      if (error.message === 'Booking not found') goto('/');
    }
  }

  /* Delete a booking */
  async function deleteBooking() {
    const userConfirmed = await promptUser(
      'Confirm Booking Deletion',
      'You are about to delete this booking. This action will erase the associated server permanently. Please confirm to proceed with deletion.'
    );
    if (!userConfirmed) return;

    try {
      await vmService.deleteVMBooking($selectedBookingStore.id);
      vmListStore.set(await vmService.getVMBookings());
      goto('/');
      toast.success(`Deleted booking #${$selectedBookingStore.id}`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* Download a txt with all relevant information about the booking */
  function handleFileDownload() {
    let output = '';

    // Add the booking details
    output += `Booking ID: ${$selectedBookingStore.id}\n`;
    output += '\n';

    // Add server login details
    output += `Server Details:\n`;
    output += `- Ip: ${$selectedBookingStore.ip}\n`;
    output += `- Password: ${$selectedBookingStore.password}\n`;
    output += `- Template: ${$selectedBookingStore.type.name}\n`;
    output += `- UUID: ${$selectedBookingStore.uuid}\n`;
    output += '\n';

    // Add owner details
    output += 'Owner:\n';
    output += `- Profile: ${window.location.href}user/${$selectedBookingStore.owner.id}\n`;
    output += `- Name: ${$selectedBookingStore.owner.name} ${$selectedBookingStore.owner.surname}\n`;
    output += '\n';

    // Add teacher assigned to details
    output += 'Teacher assigned to:\n';
    output += `- Profile: ${window.location.href}user/${$selectedBookingStore.assigned.id}\n`;
    output += `- Name: ${$selectedBookingStore.assigned.name} ${$selectedBookingStore.assigned.surname}\n`;
    output += '\n';

    // Add booking dates
    output += `Created At: ${new Date($selectedBookingStore.createdAt).toLocaleString(undefined, { dateStyle: 'full', timeStyle: 'long' })}\n`;
    output += `Expired At: ${new Date($selectedBookingStore.expiredAt).toLocaleString(undefined, { dateStyle: 'full', timeStyle: 'long' })}\n`;

    const blob = new Blob([output], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `vm_${$selectedBookingStore.id}.txt`;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  function handleResetPower() {
    vmService.resetVmPower($selectedBookingStore.uuid);
  }
</script>

<!-- Alert dialog to confirm or cancel an action -->
<AlertDialog bind:alertTitle bind:alertDescription bind:open={alertDialogOpen} on:notify={handleAnswer} />

<!-- Dialog to request extension of booking -->
<VMExtensionDialog bind:vmExtensionDialogOpen></VMExtensionDialog>

<DropdownMenu.Root bind:open>
  <DropdownMenu.Trigger asChild let:builder>
    <Button variant="outline" size="sm" class="border-indigo-500 text-indigo-500 hover:text-indigo-500" builders={[builder]}
      >Actions <ChevronDown class="size-4 ml-1 transition duration-100 {open ? 'rotate-180' : ''}" /></Button
    >
  </DropdownMenu.Trigger>
  <DropdownMenu.Content>
    <DropdownMenu.Group>
      <DropdownMenu.Label>Booking actions</DropdownMenu.Label>
      <DropdownMenu.Separator />
      <DropdownMenu.Item
        on:click={() => {
          window.open(`/api/web-console/${$selectedBookingStore.uuid}`, '_blank', 'noopener,noreferrer');
        }}
      >
        <MonitorUp class="mr-2 size-4" />
        <span>Web console</span>
      </DropdownMenu.Item>
      <DropdownMenu.Item on:click={handleResetPower}>
        <Zap class="mr-2 size-4" />
        <span>Reset power</span>
      </DropdownMenu.Item>
      <DropdownMenu.Separator />
      <DropdownMenu.Item
        on:click={() => {
          vmExtensionDialogOpen = true;
        }}
      >
        <CalendarPlus class="mr-2 size-4" />
        <span>Extend booking</span>
      </DropdownMenu.Item>
      <DropdownMenu.Item on:click={handleFileDownload}>
        <Download class="mr-2 size-4" />
        <span>Download</span>
      </DropdownMenu.Item>
      <DropdownMenu.Item on:click={refreshBooking}>
        <RefreshCcw class="mr-2 size-4" />
        <span>Refresh</span>
      </DropdownMenu.Item>
      <DropdownMenu.Separator />
      <DropdownMenu.Item on:click={deleteBooking} class="hover:data-[highlighted]:bg-destructive hover:data-[highlighted]:text-white">
        <Trash2 class="mr-2 size-4" />
        <span>Delete</span>
      </DropdownMenu.Item>
    </DropdownMenu.Group>
  </DropdownMenu.Content>
</DropdownMenu.Root>
