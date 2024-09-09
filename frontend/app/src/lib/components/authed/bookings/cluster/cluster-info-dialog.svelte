<script>
  import * as Dialog from '$lib/components/ui/dialog';
  import * as Tooltip from '$lib/components/ui/tooltip';
  import { ScrollArea } from '$lib/components/ui/scroll-area/index.js';
  import { selectedBookingStore } from '$lib/utils/store';
  import ClusterActionsDropdown from '$lib/components/authed/bookings/cluster/cluster-actions-dropdown.svelte';

  export let clusterInfoDialogOpen = false;

  /* Get days and hours left before booking expires */
  function getTimeDifference(endDate) {
    const diffMs = new Date(endDate) - new Date();
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    const diffHours = Math.floor((diffMs % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    return `${diffDays}d, ${diffHours}h`;
  }
</script>

<!-- Main dialog component -->
<Dialog.Root bind:open={clusterInfoDialogOpen}>
  <Dialog.Content class="max-w-screen-lg">
    <Dialog.Header>
      <!-- Dialog title with status, name, email, start/expire date and an action dropdown -->
      <Dialog.Title>
        <div class="flex flex-wrap items-center justify-between">
          <div class="flex flex-wrap items-center gap-y-2 gap-x-3">
            <!-- Booking active status -->
            <div class="shrink-0 h-3 w-3 rounded-full {new Date() > new Date($selectedBookingStore.expiredAt) ? 'bg-red-500' : 'bg-green-500'}"></div>
            <p>#{$selectedBookingStore.id}</p>

            <!-- Booking owner name -->
            <a href="/user/{$selectedBookingStore.owner.id}" class="py-2 px-3 bg-muted text-muted-foreground shadow-sm rounded-lg"
              >{$selectedBookingStore.owner.name} {$selectedBookingStore.owner.surname}</a
            >

            <!-- Booking owner email -->
            <a href="mailto:{$selectedBookingStore.owner.email}" class="py-2 px-3 bg-muted text-muted-foreground shadow-sm rounded-lg">
              {$selectedBookingStore.owner.email}
            </a>

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
          </div>

          <!-- Action dropdown -->
          <div class="mr-6 mt-2 md:mt-0">
            <ClusterActionsDropdown type="Booking" id={$selectedBookingStore.id} bind:clusterInfoDialogOpen />
          </div>
        </div>
      </Dialog.Title>
      <Dialog.Description>
        <!-- vCenters -->
        {#if $selectedBookingStore.vCenters}
          <ScrollArea class="flex flex-col gap-y-8 h-96">
            {#each $selectedBookingStore.vCenters as vcenter (vcenter.id)}
              <div class="flex flex-col gap-y-4 mb-8 w-full">
                <div>
                  <h3 class="text-xl font-semibold">vCenter:</h3>
                  <div class="flex flex-wrap gap-y-2 gap-x-5 w-full items-center bg-muted text-muted-foreground shadow-sm rounded-2xl p-5">
                    <!-- vCenter information -->
                    <div class="flex flex-wrap justify-between w-full gap-y-2 md:gap-y-0">
                      <div class="flex flex-wrap gap-y-2 md:gap-y-0 gap-x-2 md:gap-x-5 items-center">
                        <div class="shrink-0 h-2 w-2 rounded-full {new Date() > new Date($selectedBookingStore.expiredAt) ? 'bg-red-500' : 'bg-green-500'}"></div>
                        <p class="py-2 px-3 bg-primary-foreground shadow-sm rounded-lg font-semibold">{vcenter.ip}</p>
                        <p class="py-2 px-3 bg-primary-foreground shadow-sm rounded-lg">{vcenter.userName}</p>
                        <p class="py-2 px-3 bg-primary-foreground shadow-sm rounded-lg">
                          {vcenter.password}
                        </p>
                      </div>
                      {#if new Date() < new Date($selectedBookingStore.expiredAt) && $selectedBookingStore.isAccepted}
                        <ClusterActionsDropdown type="vCenter" id={vcenter.id} bind:clusterInfoDialogOpen />
                      {/if}
                    </div>
                  </div>
                </div>

                <!-- ESXi hosts -->
                {#if $selectedBookingStore.exsiHosts}
                  <div>
                    <h3 class="text-xl font-semibold">ESXi hosts:</h3>
                    <div class="flex flex-col gap-y-3">
                      {#each $selectedBookingStore.exsiHosts as host (host.id)}
                        {#if host.vCenterId === vcenter.id}
                          <div class="flex flex-wrap gap-y-2 gap-x-5 w-full items-center bg-muted text-muted-foreground shadow-sm rounded-2xl p-5">
                            <!-- ESXi information -->
                            <div class="flex flex-wrap justify-between w-full gap-y-2 md:gap-y-0">
                              <div class="flex flex-wrap gap-y-2 md:gap-y-0 gap-x-2 md:gap-x-5 items-center">
                                <div class="shrink-0 h-2 w-2 rounded-full {new Date() > new Date($selectedBookingStore.expiredAt) ? 'bg-red-500' : 'bg-green-500'}"></div>
                                <p class="py-2 px-3 bg-primary-foreground shadow-sm rounded-lg font-semibold">{host.ip}</p>
                                <p class="py-2 px-3 bg-primary-foreground shadow-sm rounded-lg">{host.userName}</p>
                                <p class="py-2 px-3 bg-primary-foreground shadow-sm rounded-lg">
                                  {host.password}
                                </p>
                              </div>
                              {#if new Date() < new Date($selectedBookingStore.expiredAt) && $selectedBookingStore.isAccepted}
                                <ClusterActionsDropdown type="ESXi" id={host.id} bind:clusterInfoDialogOpen />
                              {/if}
                            </div>
                          </div>
                        {/if}
                      {/each}
                    </div>
                  </div>
                {/if}
              </div>
            {/each}
          </ScrollArea>
        {/if}
      </Dialog.Description>
    </Dialog.Header>
  </Dialog.Content>
</Dialog.Root>
