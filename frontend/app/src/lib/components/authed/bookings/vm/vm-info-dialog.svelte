<script>
  import { vmService } from '$lib/services/vm-service';
  import * as Dialog from '$lib/components/ui/dialog';
  import * as Tooltip from '$lib/components/ui/tooltip';
  import { ScrollArea } from '$lib/components/ui/scroll-area/index.js';
  import { selectedBookingStore } from '$lib/utils/store';
  import VMActionsDropdown from '$lib/components/authed/bookings/vm/vm-actions-dropdown.svelte';
  import { toast } from 'svelte-sonner';
  import { Skeleton } from '$lib/components/ui/skeleton';

  export let vmInfoDialogOpen = false;
  let isLoading = false;

  // Fetch VM info when the booking dialog opens
  $: vmInfoDialogOpen, handleGetVmInfo();

  /* Get days and hours left before booking expires */
  function getTimeDifference(endDate) {
    const diffMs = new Date(endDate) - new Date();
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    const diffHours = Math.floor((diffMs % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    return `${diffDays}d, ${diffHours}h`;
  }

  /* Get VM ip, username and password */
  async function handleGetVmInfo() {
    if (!vmInfoDialogOpen) {
      return;
    }

    try {
      isLoading = true;

      const { ip, username, password } = await vmService.getVmInfo($selectedBookingStore.uuid);
      $selectedBookingStore = {
        ...$selectedBookingStore,
        ip,
        username,
        password
      };
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }
</script>

<!-- Main dialog component -->
<Dialog.Root bind:open={vmInfoDialogOpen}>
  <Dialog.Content class="max-w-screen-lg">
    <Dialog.Header>
      <!-- Dialog title with status, name, email, start/expire date and an action dropdown -->
      <Dialog.Title>
        <div class="flex flex-wrap items-center justify-between">
          <div class="flex flex-wrap items-center gap-y-2 gap-x-3 grow">
            <!-- Booking active status -->
            <div class="shrink-0 h-3 w-3 rounded-full {new Date() > new Date($selectedBookingStore.expiredAt) ? 'bg-red-500' : 'bg-green-500'}"></div>
            <p>#{$selectedBookingStore.id}</p>

            <!-- Booking owner name -->
            <a href="/user/{$selectedBookingStore.owner.id}" class="py-2 px-3 bg-muted text-muted-foreground shadow-sm rounded-lg"
              >{$selectedBookingStore.owner.name} {$selectedBookingStore.owner.surname}</a
            >

            <!-- Booking VM template -->
            <div class="py-2 px-3 bg-muted text-muted-foreground shadow-sm rounded-lg">
              {$selectedBookingStore.type}
            </div>

            <!-- Booking internal UUID -->
            <div class="py-2 px-3 bg-muted text-muted-foreground shadow-sm rounded-lg">
              {$selectedBookingStore.uuid.slice(0, 7)}
            </div>

            <!-- start and expire date with time difference in tooltip -->
            <Tooltip.Root openDelay={150}>
              <Tooltip.Trigger
                ><p class="py-2 px-3 bg-muted text-muted-foreground shadow-sm rounded-lg">
                  {new Date($selectedBookingStore.createdAt).toLocaleDateString({ calendar: 'full' })} - {new Date($selectedBookingStore.expiredAt).toLocaleDateString({ calendar: 'full' })}
                </p></Tooltip.Trigger
              >
              <Tooltip.Content>
                <p>
                  {#if new Date() < new Date($selectedBookingStore.expiredAt)}
                    {getTimeDifference($selectedBookingStore.expiredAt)}
                  {:else}
                    expired
                  {/if}
                </p>
              </Tooltip.Content>
            </Tooltip.Root>
            <!-- Action dropdown -->
            <div class="mr-4 mt-0 mt-0 grow flex justify-end">
              <VMActionsDropdown bind:vmInfoDialogOpen />
            </div>
          </div>
        </div>
      </Dialog.Title>
      <Dialog.Description>
        <!-- vCenters -->
        {#if $selectedBookingStore}
          <ScrollArea class="flex flex-col gap-y-8 h-96">
            <div class="flex flex-wrap gap-y-2 gap-x-5 w-full items-center bg-muted text-muted-foreground shadow-sm rounded-2xl p-5">
              <!-- VM information -->
              <div class="flex flex-wrap justify-between w-full gap-y-2 md:gap-y-0">
                <div class="flex flex-wrap gap-y-2 md:gap-y-0 gap-x-2 md:gap-x-5 items-center">
                  <div class="shrink-0 h-2 w-2 rounded-full {new Date() > new Date($selectedBookingStore.expiredAt) ? 'bg-red-500' : 'bg-green-500'}"></div>
                  {#if !$selectedBookingStore.ip && !$selectedBookingStore.username && !$selectedBookingStore.password}
                    <Skeleton class="h-7 w-28 bg-input" />
                    <Skeleton class="h-7 w-28 bg-input" />
                    <Skeleton class="h-7 w-28 bg-input" />
                  {:else}
                    <p class="py-2 px-3 bg-primary-foreground shadow-sm rounded-lg font-semibold">{$selectedBookingStore.ip}</p>
                    <p class="py-2 px-3 bg-primary-foreground shadow-sm rounded-lg">{$selectedBookingStore.username}</p>
                    <p class="py-2 px-3 bg-primary-foreground shadow-sm rounded-lg">
                      {$selectedBookingStore.password}
                    </p>
                  {/if}
                </div>
              </div>
              <div>
                <b>
                  Note:
                </b>
                {$selectedBookingStore.message}
              </div>
            </div>
          </ScrollArea>
        {/if}
      </Dialog.Description>
    </Dialog.Header>
  </Dialog.Content>
</Dialog.Root>
