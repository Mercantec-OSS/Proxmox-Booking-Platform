<script>
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
  import { Button } from '$lib/components/ui/button/index.js';
  import { Wrench, Hammer, Cable, Download, Trash2, Server, RefreshCcw, EllipsisVertical } from 'lucide-svelte';
  import { toast } from 'svelte-sonner';
  import { clusterService } from '$lib/services/cluster-service';
  import { clusterListStore, selectedBookingStore } from '$lib/utils/store';
  import AlertDialog from '$lib/components/authed/alert-dialog.svelte';

  export let ClusterInfoDialogOpen;
  /* Types: "Booking", "vCenter", "ESXi" */
  export let type;
  /* vCenter id or esxi id */
  export let id = null;
  /* Alert dialog */
  let alertDialogOpen = false;
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

  /* Booking action: install vCenter action */
  async function bookingInstallVcenters() {
    const userConfirmed = await promptUser('Confirm Installation of All vCenters', 'You are about to install all vCenters in your booking. Please confirm to proceed.');
    if (!userConfirmed) return;

    try {
      await clusterService.bInstallVcenters(id);
      toast.success(`vCenter instllation started`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* vCenter action: install vCenter action */
  async function vcenterInstallVcenter() {
    const userConfirmed = await promptUser('Confirm vCenter Installation', 'You are about to install this vCenter. Please confirm to proceed.');
    if (!userConfirmed) return;

    try {
      await clusterService.vInstallVcenter(id);
      toast.success(`vCenter instllation started`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* Booking action: reset hosts */
  async function bookingResetHosts() {
    const userConfirmed = await promptUser(
      'Confirm Host Reset in All vCenters',
      'You are about to reset all hosts in all vCenters. This will wipe the hosts machines, impacting all configurations and services currently running on them.'
    );
    if (!userConfirmed) return;

    try {
      await clusterService.bResetHosts(id);
      toast.success(`Reset hosts started`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* vCenter action: reset hosts */
  async function vcenterResetHosts() {
    const userConfirmed = await promptUser(
      'Confirm Host Reset in vCenter',
      'You are about to reset all hosts in the vCenter. This will wipe the hosts machines, impacting all configurations and services currently running on them.'
    );
    if (!userConfirmed) return;

    try {
      await clusterService.vResetHosts(id);
      toast.success(`Reset hosts started`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* ESXi action: reset host */
  async function esxiResetHost() {
    const userConfirmed = await promptUser(
      'Confirm Host Reset',
      'You are about to reset the host. This will wipe the host machine, impacting all configurations and services currently running on it.'
    );
    if (!userConfirmed) return;

    try {
      await clusterService.eResetHost(id);
      toast.success(`Reset hosts started`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* Booking action: reset and install */
  async function bookingResetAndInstall() {
    const userConfirmed = await promptUser(
      'Confirm vCenter and Hosts Reset and Reinstallation',
      'You are about to perform a reset followed by a reinstallation of all the vCenter and their hosts. Please confirm to proceed.'
    );
    if (!userConfirmed) return;

    try {
      await clusterService.bResetAndInstall(id);
      toast.success(`Reset and install started`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* vCenter action: reset and install */
  async function vcenterResetAndInstall() {
    const userConfirmed = await promptUser(
      'Confirm vCenter and Hosts Reset and Reinstallation',
      'You are about to perform a reset followed by a reinstallation of both the vCenter and its hosts. Please confirm to proceed.'
    );
    if (!userConfirmed) return;

    try {
      await clusterService.vResetAndInstall(id);
      toast.success(`Reset and install started`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* vCenter action: create windows vms */
  async function vcenterCreateWindowsVms() {
    const userConfirmed = await promptUser('Confirm Creation of Windows VMs', 'You are about to create Windows virtual machines. Please confirm to proceed.');
    if (!userConfirmed) return;

    try {
      await clusterService.vCreateWindowsVms(id);
      toast.success(`Create windows vms started`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* Refresh specific booking based on id */
  async function refreshBooking() {
    try {
      const updatedBooking = await clusterService.getClusterBookingById($selectedBookingStore.id);

      toast.success(`Refreshed booking #${$selectedBookingStore.id}`);

      selectedBookingStore.set(updatedBooking);

      clusterListStore.update((bookings) => {
        const updatedBookings = bookings.map((booking) => (booking.id === $selectedBookingStore.id ? { ...booking, ...updatedBooking } : booking));
        return updatedBookings;
      });
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* Delete a booking */
  async function deleteBooking() {
    const userConfirmed = await promptUser(
      'Confirm Booking Deletion',
      'You are about to delete this booking. This action will erase all associated servers permanently. Please confirm to proceed with deletion.'
    );
    if (!userConfirmed) return;

    try {
      await clusterService.deleteClusterBooking($selectedBookingStore.id);
      clusterListStore.set(await clusterService.getClusterBookingsFrontend());
      ClusterInfoDialogOpen = false;
      toast.success(`Deleted booking #${$selectedBookingStore.id}`);
    } catch (error) {
      toast.error(error.message);
    }
  }

  /* Download a txt with all relevant information about the booking */
  function handleFileDownload() {
    const booking = $selectedBookingStore;
    const owner = booking.owner;

    let output = `Booking ID: ${booking.id}

Owner: ${owner.name} ${owner.surname} (${owner.role})
Email: ${owner.email}
Amount of Students: ${booking.amountStudents}
Expiration Date: ${new Date(booking.expiredAt).toLocaleString()}
`;

    booking.vCenters.forEach((vCenter, index) => {
      const esxiHosts = vCenter.esxiHosts.map((host) => `  - ESXi Host IP: ${host.ip}, Username: ${host.userName}, Password: ${host.password}`).join('\n');

      output += `
vCenter IP: ${vCenter.ip}
vCenter Username: ${vCenter.userName}
vCenter Password: ${vCenter.password}
ESXi Hosts:
${esxiHosts}
`;
    });

    const blob = new Blob([output], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `cluster_booking_details_${booking.id}.txt`;
    link.click();
    window.URL.revokeObjectURL(url);
  }
</script>

<!-- Alert dialog to confirm or cancel an action -->
<AlertDialog bind:alertTitle bind:alertDescription bind:open={alertDialogOpen} on:notify={handleAnswer} />

<DropdownMenu.Root>
  <DropdownMenu.Trigger asChild let:builder>
    {#if type === 'Booking'}
      <Button variant="outline" class="bg-primary-foreground" builders={[builder]}>Actions</Button>
    {:else if type === 'vCenter' || type === 'ESXi'}
      <Button variant="outline" class="bg-primary-foreground" size="icon" builders={[builder]}><EllipsisVertical /></Button>
    {/if}
  </DropdownMenu.Trigger>
  <DropdownMenu.Content>
    <DropdownMenu.Group>
      <DropdownMenu.Label>{type} actions</DropdownMenu.Label>
      <DropdownMenu.Separator />
      {#if type === 'Booking'}
        {#if new Date() < new Date($selectedBookingStore.expiredAt)}
          <DropdownMenu.Item on:click={bookingInstallVcenters}>
            <Hammer class="mr-2 size-4" />
            <span>Install vCenters</span>
          </DropdownMenu.Item>
          <DropdownMenu.Item on:click={bookingResetHosts}>
            <Wrench class="mr-2 size-4" />
            <span>Reset hosts</span>
          </DropdownMenu.Item>
          <DropdownMenu.Item on:click={bookingResetAndInstall}>
            <Cable class="mr-2 size-4" />
            <span>Reset & install</span>
          </DropdownMenu.Item>
          <DropdownMenu.Separator />
          <DropdownMenu.Item on:click={handleFileDownload}>
            <Download class="mr-2 size-4" />
            <span>Download</span>
          </DropdownMenu.Item>
          <DropdownMenu.Item on:click={refreshBooking}>
            <RefreshCcw class="mr-2 size-4" />
            <span>Refresh</span>
          </DropdownMenu.Item>
          <DropdownMenu.Separator />
        {/if}
        <DropdownMenu.Item on:click={deleteBooking} class="hover:data-[highlighted]:bg-destructive hover:data-[highlighted]:text-white">
          <Trash2 class="mr-2 size-4" />
          <span>Delete</span>
        </DropdownMenu.Item>
      {/if}

      {#if type === 'vCenter'}
        <DropdownMenu.Item on:click={vcenterInstallVcenter}>
          <Hammer class="mr-2 size-4" />
          <span>Install vCenter</span>
        </DropdownMenu.Item>
        <DropdownMenu.Item on:click={vcenterResetHosts}>
          <Wrench class="mr-2 size-4" />
          <span>Reset hosts</span>
        </DropdownMenu.Item>
        <DropdownMenu.Item on:click={vcenterResetAndInstall}>
          <Cable class="mr-2 size-4" />
          <span>Reset & install</span>
        </DropdownMenu.Item>
        <DropdownMenu.Item on:click={vcenterCreateWindowsVms}>
          <Server class="mr-2 size-4" />
          <span>Create Windows vms</span>
        </DropdownMenu.Item>
      {/if}

      {#if type === 'ESXi'}
        <DropdownMenu.Item on:click={esxiResetHost}>
          <Hammer class="mr-2 size-4" />
          <span>Reset host</span>
        </DropdownMenu.Item>
      {/if}
    </DropdownMenu.Group>
  </DropdownMenu.Content>
</DropdownMenu.Root>
