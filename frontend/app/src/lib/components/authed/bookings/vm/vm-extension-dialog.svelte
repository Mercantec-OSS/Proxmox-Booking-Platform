<script>
  import { vmListStore, selectedBookingStore } from '$lib/utils/store';
  import { vmService } from '$lib/services/vm-service';
  import * as Dialog from '$lib/components/ui/dialog';
  import { Textarea } from '$lib/components/ui/textarea/index.js';
  import { Label } from '$lib/components/ui/label';
  import { Button } from '$lib/components/ui/button/index.js';
  import { LoaderCircle, CalendarIcon } from 'lucide-svelte';
  import { toast } from 'svelte-sonner';
  import { cn } from '$lib/utils/utils.js';
  import { Calendar } from '$lib/components/ui/calendar';
  import * as Popover from '$lib/components/ui/popover';
  import { CalendarDate, today, getLocalTimeZone } from '@internationalized/date';

  let { vmExtensionDialogOpen } = $props();
  let loadingCreate = $state(false);

  /* Creates Date object from store's expiration timestamp */
  let expiredAtDate = $derived(new Date($selectedBookingStore.expiredAt));

  /* State management for calendar date selection and formatting */
  let calendarDatePicked = $state(null);
  let calendarDateFormated = $derived(calendarDatePicked ? new Date(calendarDatePicked).toLocaleDateString(undefined, { dateStyle: 'long' }) : null);

  /* Extension request form state */
  let bookingExtensionInput = $state({
    bookingId: null,
    message: null,
    newExpiringAt: null
  });

  /* Synchronizes local state with backend after mutations */
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

  /* Validates and submits booking extension request */
  async function handleExtendBooking() {
    if (!calendarDatePicked) {
      toast.error('Please select a new expire date');
      return;
    }

    if (!bookingExtensionInput.message) {
      toast.error('Please add a comment about the extension');
      return;
    }

    bookingExtensionInput.bookingId = $selectedBookingStore.id;
    bookingExtensionInput.newExpiringAt = new Date(calendarDatePicked).toISOString();
    loadingCreate = true;

    try {
      await vmService.extendVmBooking(bookingExtensionInput);
      await refreshBooking();
      toast.success(`Extension request created`);
    } catch (error) {
      toast.error(error.message);
    } finally {
      vmExtensionDialogOpen = false;
      loadingCreate = false;
      calendarDatePicked = null;
    }
  }
</script>

<Dialog.Root bind:open={vmExtensionDialogOpen}>
  <Dialog.Content class="bg-primary-foreground">
    <Dialog.Header>
      <Dialog.Title>Create Booking extension</Dialog.Title>
    </Dialog.Header>
    <div class="flex flex-col gap-y-4">
      <Popover.Root>
        <Popover.Trigger asChild let:builder>
          <Button variant="outline" class={cn('justify-start text-left font-normal', !calendarDatePicked && 'text-muted-foreground')} builders={[builder]}>
            <CalendarIcon class="mr-2 h-4 w-4" />
            {calendarDatePicked ? calendarDateFormated : 'Pick a date'}
          </Button>
        </Popover.Trigger>
        <Popover.Content class="w-auto p-0">
          <Calendar
            bind:value={calendarDatePicked}
            initialFocus
            calendarLabel="Booking expire date"
            minValue={new CalendarDate(expiredAtDate.getFullYear(), expiredAtDate.getMonth() + 1, expiredAtDate.getDate() + 1)}
            maxValue={new CalendarDate(
              today(getLocalTimeZone()).year + (today(getLocalTimeZone()).month + 6 > 12 ? 1 : 0),
              (today(getLocalTimeZone()).month + 6) % 12 || 12,
              today(getLocalTimeZone()).day
            )}
          />
        </Popover.Content>
      </Popover.Root>

      <div class="grid gap-1.5">
        <Label for="comment">Comment about booking extension</Label>
        <Textarea id="comment" bind:value={bookingExtensionInput.message} />
      </div>
    </div>
    <Dialog.Footer>
      <Button variant="outline" onclick={() => (vmExtensionDialogOpen = false)}>Cancel</Button>
      <Button disabled={loadingCreate} onclick={handleExtendBooking}>
        {#if loadingCreate}
          <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
        {/if}
        Create request
      </Button>
    </Dialog.Footer>
  </Dialog.Content>
</Dialog.Root>
