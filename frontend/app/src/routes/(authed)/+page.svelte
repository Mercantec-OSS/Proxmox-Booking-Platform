<script>
  import GreetingsComponent from '$lib/components/authed/dashboard/greeting.svelte';
  import BookingList from '$lib/components/authed/bookings/booking-list.svelte';
  import CreateBookingDrawer from '$lib/components/authed/bookings/create-booking-drawer.svelte';
  import BookingFiltering from '$lib/components/authed/bookings/booking-filtering.svelte';
  import { clusterListStore, vmListStore, userStore } from '$lib/utils/store';
  import { onMount, onDestroy } from 'svelte';
  import { clusterService } from '$lib/services/cluster-service';
  import { vmService } from '$lib/services/vm-service';
  import { Button } from '$lib/components/ui/button/index.js';

  export let data;
  let bookingUpdateIntervalId;
  let bookingUpdateInterval;

  /* Create a store for cluster and vm bookings so they can be accessed and modified from everywhere */
  clusterListStore.set(data.clusterData);
  vmListStore.set(data.vmData);

  /* Set user info store */
  userStore.set(data.userInfo);

  async function fetchBookings() {
    if (data.userInfo.role !== 'Student') {
      clusterListStore.set(await clusterService.getClusterBookingsFrontend());
    }
    vmListStore.set(await vmService.getVMBookingsFrontend());
  }

  /* Make an interval that refreshes all bookings every 30 seconds */
  onMount(async () => {
    bookingUpdateInterval = setTimeout(() => {
      fetchBookings();
      bookingUpdateIntervalId = setInterval(fetchBookings, 20000);
    }, 20000);
  });

  /* Clear the refresh booking interval when a user leaves the page */
  onDestroy(() => {
    clearTimeout(bookingUpdateInterval);
    clearInterval(bookingUpdateIntervalId);
  });
</script>

<main class="flex flex-1 flex-col gap-4 p-4 lg:gap-6 lg:p-6">
  <div class="flex items-center">
    <h1 class="text-lg font-semibold md:text-2xl">Booking Overview</h1>
  </div>
  <div class="flex flex-1 items-center justify-center rounded-lg border border-dashed shadow-sm">
    <div class="flex flex-col items-center gap-1 text-center">
      <h3 class="text-2xl font-bold tracking-tight">You have no server bookings</h3>
      <p class="text-muted-foreground text-sm">Start managing your server by creating a new booking.</p>
      <Button class="mt-4">Create Booking</Button>
    </div>
  </div>
</main>
