<script>
  import * as Dialog from '$lib/components/ui/dialog';
  import { Textarea } from '$lib/components/ui/textarea/index.js';
  import { Label } from '$lib/components/ui/label';
  import { Button } from '$lib/components/ui/button/index.js';
  import { LoaderCircle, SquareChevronRight } from 'lucide-svelte';
  import { toast } from 'svelte-sonner';
  import { vmListStore, selectedBookingStore } from '$lib/utils/store';
  import { vmService } from '$lib/services/vm-service';

  let { vmExtensionRequestDialogOpen } = $props();
  let loadingAccept = $state(false);

  /**
   * Refreshes booking data from API and updates stores
   * Shows error toast if refresh fails
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
   * Accepts booking extension and refreshes data
   * @param {number} id - Extension request ID
   */
  async function handleAcceptExtension(id) {
    vmExtensionRequestDialogOpen = false;
    loadingAccept = true;
    try {
      await vmService.acceptExtendVmBooking(id);
      await refreshBooking(id);
      toast.success(`Accepted booking extension`);
    } catch (error) {
      toast.error(error.message);
    } finally {
      vmExtensionRequestDialogOpen = false;
      loadingAccept = false;
    }
  }

  /**
   * Formats date to "01 Jan 11:59 PM" style string
   * @param {string} date - ISO date string
   */
  function formatDateTime(date) {
    const options = {
      day: '2-digit',
      month: 'short',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    };
    return new Date(date).toLocaleString(undefined, options).replace(',', '');
  }
</script>

<Dialog.Root bind:open={vmExtensionRequestDialogOpen}>
  <Dialog.Content class="bg-primary-foreground">
    <Dialog.Header>
      <Dialog.Title>Booking extension request</Dialog.Title>
    </Dialog.Header>
    <div class="flex flex-col gap-y-4">
      <div class="flex justify-between items-center">
        <div>
          <div class="text-sm text-muted-foreground">Current expire date</div>
          <div class="text-sm">{formatDateTime($selectedBookingStore.expiredAt)}</div>
        </div>
        <SquareChevronRight class="text-muted-foreground"></SquareChevronRight>
        <div>
          <div class="text-sm text-muted-foreground">New expire date</div>
          <div class="text-sm">{formatDateTime($selectedBookingStore.activeExtension.newExpiringAt)}</div>
        </div>
      </div>

      <div class="grid gap-1.5">
        <Label for="comment">Comment about booking extension</Label>
        <Textarea disabled id="comment" bind:value={$selectedBookingStore.activeExtension.message} />
      </div>
      <Dialog.Footer>
        <Button variant="outline" onclick={() => (vmExtensionRequestDialogOpen = false)}>Cancel</Button>
        {#if new Date() < new Date($selectedBookingStore.expiredAt)}
          <Button disabled={loadingAccept} onclick={() => handleAcceptExtension($selectedBookingStore.activeExtension.id)}>
            {#if loadingAccept}
              <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
            {/if}
            Accept
          </Button>
        {/if}
      </Dialog.Footer>
    </div>
  </Dialog.Content>
</Dialog.Root>
