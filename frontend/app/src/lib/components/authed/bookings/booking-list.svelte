<!-- Component to show list of clusters with vcenter and hosts -->

<script>
  import { SquareChevronRight, SquareCheck, SquareMinus, SquareX, AlignLeft, School, Notebook } from 'lucide-svelte';
  import { Badge } from '$lib/components/ui/badge';
  import * as Table from '$lib/components/ui/table/index.js';
  import VMInfoDialog from '$lib/components/authed/bookings/vm/vm-info-dialog.svelte';
  import VMStatusDialog from '$lib/components/authed/bookings/vm/vm-status-dialog.svelte';
  import VMExtensionRequestDialog from '$lib/components/authed/bookings/vm/vm-extension-request-dialog.svelte';
  import ClusterInfoDialog from '$lib/components/authed/bookings/cluster/cluster-info-dialog.svelte';
  import { vmListStore, clusterListStore, selectedBookingStore, selectedBookingStatus, selectedBookingPreview, selectedBookingTypes, userStore } from '$lib/utils/store';

  let userAuthed = $userStore.role !== 'Student';

  let vmInfoDialogOpen;
  let vmStatusDialogOpen;
  let vmExtensionRequestDialogOpen;
  let clusterInfoDialogOpen;

  function openDialog(type, booking) {
    selectedBookingStore.set(booking);

    if (type === 'VM') {
      let allAccepted = booking.extentions.every((extention) => extention.isAccepted);

      // Open extension request dialog if there is new request
      if (booking.extentions.length > 0 && !allAccepted && userAuthed) {
        let notAccepted = booking.extentions.filter((extention) => !extention.isAccepted);
        booking.activeExtension = notAccepted[0]; // need to REFACTOR

        vmExtensionRequestDialogOpen = true;
      }
      // Open booking request dialog if it's pending
      else if (!booking.isAccepted && userAuthed) {
        vmStatusDialogOpen = true;
      }
      // Open booking info dialog if it's accepted
      else {
        vmInfoDialogOpen = true;
      }
    } else if (type === 'Cluster') {
      clusterInfoDialogOpen = true;
    }
  }

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

  function shouldRenderBooking(type, booking) {
    const isExpired = new Date() > new Date(booking.expiredAt);

    // Don't render if cluster is not selected
    if (type === 'Cluster' && $selectedBookingTypes.some((option) => option.title === 'Clusters' && !option.selected)) {
      return false;
    }
    1;

    // Don't render if the type of booking is not selected
    if (
      (type === 'Cluster' && !$selectedBookingTypes.find((option) => option.title === 'Clusters')?.selected) ||
      (type === 'VM' && !$selectedBookingTypes.find((option) => option.title === 'Virtual Machines')?.selected)
    ) {
      return false;
    }

    // Don't render if the booking is expired or not accepted and 'other' is false
    if ((isExpired || !booking.isAccepted) && !$selectedBookingStatus.other) {
      return false;
    }

    // Don't render if the booking is accepted and 'confirmed' is false
    if (booking.isAccepted && !isExpired && !$selectedBookingStatus.confirmed) {
      return false;
    }

    return true;
  }

  function getTotalEsxiHosts(booking) {
    return booking.vCenters.reduce((total, vCenter) => {
      return total + (vCenter.esxiHosts ? vCenter.esxiHosts.length : 0);
    }, 0);
  }
</script>

<!-- Dialogs components -->
<VMInfoDialog bind:vmInfoDialogOpen></VMInfoDialog>
<VMStatusDialog bind:vmStatusDialogOpen></VMStatusDialog>
<VMExtensionRequestDialog bind:vmExtensionRequestDialogOpen></VMExtensionRequestDialog>
<ClusterInfoDialog bind:clusterInfoDialogOpen></ClusterInfoDialog>

<!-- Booking list container -->
{#if $selectedBookingPreview === 'card'}
  {#each [...$clusterListStore, ...$vmListStore] as booking (`${booking.id}, ${booking.createdAt}`)}
    {#if shouldRenderBooking($vmListStore.includes(booking) ? 'VM' : 'Cluster', booking)}
      <!-- Card booking preview -->
      <button
        on:click={() => openDialog($clusterListStore.includes(booking) ? 'Cluster' : 'VM', booking)}
        class="aspect-[2/1.1] w-full max-w-96 md:w-96 rounded-lg bg-primary-foreground shadow-md relative p-4 text-left"
        disabled={!$clusterListStore.includes(booking) && !booking.isAccepted && !userAuthed}
      >
        <div class="flex justify-between items-start">
          <div class="flex items-center">
            <div class="w-1 h-20 rounded-xl mr-3 {$vmListStore.includes(booking) ? 'bg-indigo-500' : 'bg-orange-500'}"></div>
            <div class="flex flex-col">
              <div class="font-bold text-xl">{booking.owner.name} {booking.owner.surname}</div>
              <div class="flex space-x-2">
                {#if $vmListStore.includes(booking)}
                  <Badge variant="secondary">{booking.type}</Badge>
                  <Badge variant="secondary"><School class="mr-2 size-4" />{booking.assigned.name} {booking.assigned.surname}</Badge>
                {:else}
                  <Badge variant="secondary">{booking.vCenters.length}x vCenters</Badge>
                  <Badge variant="secondary">{getTotalEsxiHosts(booking)}x ESXi hosts</Badge>
                {/if}
              </div>
              <div class="mt-2">
                {#if booking.message}
                  <Badge variant="secondary"
                    ><Notebook class="mr-2 size-4" />
                    {booking.message.slice(0, 80)}
                    {#if booking.message.length > 80}
                      ...
                    {/if}
                  </Badge>
                {/if}
              </div>
            </div>
          </div>
        </div>
        <div class="mt-4">
          <div class="flex justify-between items-center">
            <div>
              <div class="text-sm text-muted-foreground">Start Time</div>
              <div class="text-sm">{formatDateTime(booking.createdAt)}</div>
            </div>
            <SquareChevronRight class="text-muted-foreground"></SquareChevronRight>
            <div>
              <div class="text-sm text-muted-foreground">End Time</div>
              <div class="text-sm">{formatDateTime(booking.expiredAt)}</div>
            </div>
          </div>
          <div class="mt-7 flex items-center">
            <div class="flex items-center text-sm text-muted-foreground">
              <AlignLeft class="size-4 mr-2 {$vmListStore.includes(booking) ? 'text-indigo-500' : 'text-orange-500'}" />

              {#if $vmListStore.includes(booking)}
                <p>Virtual machine</p>
              {:else}
                <p>Cluster</p>
              {/if}
            </div>
            <div class="ml-auto">
              {#if new Date() > new Date(booking.expiredAt)}
                <Badge variant="destructive" class="font-extrabold"><SquareX class="mr-1 h-4 w-4" />Expired</Badge>
              {:else if booking.isAccepted || booking.isAccepted == null}
                <Badge variant="success" class="font-extrabold"><SquareCheck class="mr-1 h-4 w-4" />Confirmed</Badge>
              {:else}
                <Badge variant="pending" class="font-extrabold"><SquareMinus class="mr-1 h-4 w-4" />Pending</Badge>
              {/if}
            </div>
          </div>
        </div>
      </button>
    {/if}
  {/each}
{:else if $selectedBookingPreview === 'table'}
  <!-- Table booking preview -->
  <Table.Root>
    <Table.Header>
      <Table.Row>
        <Table.Head>Type</Table.Head>
        <Table.Head>Status</Table.Head>
        <Table.Head>Owner</Table.Head>
        <Table.Head>Assigned to</Table.Head>
        <Table.Head>Created date</Table.Head>
        <Table.Head>Expire date</Table.Head>
      </Table.Row>
    </Table.Header>
    <Table.Body>
      {#each [...$clusterListStore, ...$vmListStore] as booking (`${booking.id}, ${booking.createdAt}`)}
        {#if shouldRenderBooking($vmListStore.includes(booking) ? 'VM' : 'Cluster', booking)}
          <Table.Row on:click={() => openDialog($clusterListStore.includes(booking) ? 'Cluster' : 'VM', booking)} class="hover:cursor-pointer">
            <Table.Cell class="font-medium">
              {#if $clusterListStore.includes(booking)}
                <div class="flex items-center gap-x-2">
                  <div class="w-1 h-6 rounded-full bg-orange-500"></div>
                  Cluster
                </div>
              {:else}
                <div class="flex items-center gap-x-2">
                  <div class="w-1 h-6 rounded-full bg-indigo-500"></div>
                  Virtual machine
                </div>
              {/if}
            </Table.Cell>
            <Table.Cell>
              {#if new Date() > new Date(booking.expiredAt)}
                Expired
              {:else if booking.isAccepted}
                Confirmed
              {:else}
                Pending
              {/if}
            </Table.Cell>
            <Table.Cell>{booking.owner.name} {booking.owner.surname}</Table.Cell>
            {#if $vmListStore.includes(booking)}
              <Table.Cell>{booking.assigned.name} {booking.assigned.surname}</Table.Cell>
            {:else}
              <Table.Cell></Table.Cell>
            {/if}
            <Table.Cell>{formatDateTime(booking.createdAt)}</Table.Cell>
            <Table.Cell>{formatDateTime(booking.expiredAt)}</Table.Cell>
          </Table.Row>
        {/if}
      {/each}
    </Table.Body>
  </Table.Root>
{/if}
