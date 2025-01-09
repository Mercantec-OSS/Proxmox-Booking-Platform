<script>
  import * as Dialog from '$lib/components/ui/dialog';
  import { Textarea } from '$lib/components/ui/textarea/index.js';
  import { Label } from '$lib/components/ui/label';
  import { Button } from '$lib/components/ui/button/index.js';
  import { Trash2, LoaderCircle } from 'lucide-svelte';
  import { toast } from 'svelte-sonner';
  import { vmListStore, selectedBookingStore } from '$lib/utils/store';
  import { vmService } from '$lib/services/vm-service';

  let { vmStatusDialogOpen } = $props();
  let loadingDelete = $state(false);
  let loadingAccept = $state(false);

  /**
   * Syncs local booking state with backend data
   * Updates both selected booking and bookinglist stores
   */
  async function refreshBooking() {
    try {
      const updatedBooking = await vmService.getVMBookingById($selectedBookingStore.id);
      selectedBookingStore.set(updatedBooking);

      vmListStore.update((bookings) => {
        const updatedBookings = bookings.map((booking) => (booking.id === $selectedBookingStore.id ? { ...booking, ...updatedBooking } : booking));
        return updatedBookings;
      });
    } catch (error) {
      toast.error(error.message);
    }
  }

  /**
   * Handles accepting a VM booking request
   * Updates UI state and refreshes data on success
   */
  async function handleAcceptBooking(id) {
    loadingAccept = true;
    try {
      await vmService.acceptVMBooking(id);
      await refreshBooking(id);
      toast.success(`Accepted booking`);
    } catch (error) {
      toast.error(error.message);
    } finally {
      vmStatusDialogOpen = false;
      loadingAccept = false;
    }
  }

  /**
   * Handles VM booking deletion
   * Refreshes booking list after successful deletion
   */
  async function handleDeleteBooking(id) {
    loadingDelete = true;
    try {
      await vmService.deleteVMBooking(id);
      vmListStore.set(await vmService.getVMBookings());
      toast.success(`Booking deleted`);
    } catch (error) {
      toast.error(error.message);
    } finally {
      vmStatusDialogOpen = false;
      loadingDelete = false;
    }
  }
</script>

<Dialog.Root bind:open={vmStatusDialogOpen}>
  <Dialog.Content class="bg-primary-foreground">
    <Dialog.Header>
      <Dialog.Title>Booking status</Dialog.Title>
      <Dialog.Description>Accept or delete a booking request</Dialog.Description>
    </Dialog.Header>
    <div class="grid gap-1.5">
      <Label for="comment">Comment about booking</Label>
      <Textarea disabled id="comment" bind:value={$selectedBookingStore.message} />
    </div>
    <Dialog.Footer>
      <Button disabled={loadingDelete} variant="outline" onclick={() => handleDeleteBooking($selectedBookingStore.id)}>
        {#if loadingDelete}
          <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
        {:else}
          <Trash2 class="mr-2 h-4 w-4" />
        {/if}
        Delete
      </Button>
      {#if new Date() < new Date($selectedBookingStore.expiredAt)}
        <Button disabled={loadingAccept} onclick={() => handleAcceptBooking($selectedBookingStore.id)}>
          {#if loadingAccept}
            <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
          {/if}
          Accept
        </Button>
      {/if}
    </Dialog.Footer>
  </Dialog.Content>
</Dialog.Root>
