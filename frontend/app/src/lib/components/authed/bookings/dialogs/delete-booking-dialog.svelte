<script>
  import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
  import * as Dialog from '$lib/components/ui/dialog/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { Label } from '$lib/components/ui/label/index.js';
  import { Badge } from '$lib/components/ui/badge/index.js';
  import { toast } from 'svelte-sonner';
  import { vmService } from '$lib/services/vm-service';
  import { vmListStore } from '$lib/utils/store';
  let { listOfBookings = $bindable([]), deleteDialogOpen = $bindable(false) } = $props();

  async function handleDeleteMultiple() {
    try {
      // Delete all selected bookings in parallel
      await Promise.all(
        listOfBookings.map(async (booking) => {
          await vmService.deleteVMBooking(booking.id);
        })
      );
      
      toast.success(`Successfully deleted ${listOfBookings.length} ${listOfBookings.length > 1 ? 'bookings' : 'booking'}`);

      deleteDialogOpen = false;
      listOfBookings = [];
      let bookings = await vmService.getVMBookings();
      vmListStore.set(bookings);
    } catch (error) {
      toast.error(error);
    }
  }
</script>

<Dialog.Root bind:open={deleteDialogOpen}>
  <Dialog.Content class="sm:max-w-[500px]">
    <Dialog.Header>
      <Dialog.Title>Delete {listOfBookings.length > 1 ? 'Bookings' : 'Booking'}</Dialog.Title>
      <Dialog.Description>
        Are you sure you want to delete {listOfBookings.length > 1 ? 'these bookings' : 'this booking'}? This action cannot be undone.
      </Dialog.Description>
    </Dialog.Header>

    {#if listOfBookings.length > 0}
      <div class="py-4 max-h-[300px] overflow-y-auto border rounded-md">
        {#each listOfBookings as booking, i}
          <div class="flex flex-col gap-1 py-2 px-3 border-b last:border-b-0">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2">
                <Badge variant="outline">{booking.id}</Badge>
                <span class="font-medium">{booking.type}</span>
              </div>
              <Badge variant="outline" class="text-xs">
                {booking.isAccepted ? 'Accepted' : 'Pending'}
              </Badge>
            </div>

            <div class="text-sm text-gray-500">
              <div class="flex justify-between">
                <span>Owner: {booking.owner.name} {booking.owner.surname}</span>
                <span>Expires: {new Date(booking.expiredAt).toLocaleDateString()}</span>
              </div>
              {#if booking.message}
                <div class="mt-1 text-xs truncate">{booking.message}</div>
              {/if}
            </div>
          </div>
        {/each}
      </div>
    {/if}

    <Dialog.Footer class="flex justify-between gap-2 mt-4">
      <Button variant="outline" onclick={() => (deleteDialogOpen = false)}>Cancel</Button>
      <Button onclick={handleDeleteMultiple}>
        {listOfBookings.length > 1 ? `Delete ${listOfBookings.length} Bookings` : 'Delete Booking'}
      </Button>
    </Dialog.Footer>
  </Dialog.Content>
</Dialog.Root>
