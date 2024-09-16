<script>
  import GreetingsComponent from '$lib/components/authed/dashboard/greeting.svelte';
  import BookingList from '$lib/components/authed/bookings/booking-list.svelte';
  import CreateBookingDrawer from '$lib/components/authed/bookings/create-booking-drawer.svelte';
  import BookingFiltering from '$lib/components/authed/bookings/booking-filtering.svelte';
  import { clusterListStore, vmListStore, userStore } from '$lib/utils/store';
  import { onMount, onDestroy } from 'svelte';
  import { clusterService } from '$lib/services/cluster-service';
  import { vmService } from '$lib/services/vm-service';

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

<main class="flex flex-grow flex-col bg-background">
  <div class="ml-5 mt-5"></div>

  <!-- Button to open create booking drawer and filter bookings -->
  <div class="flex flex-col md:w-3/4 md:mx-auto my-10">
    <div class="flex gap-x-2">
      <CreateBookingDrawer />
      <BookingFiltering />
    </div>

    <!-- List of all bookings -->
    <div class="flex flex-wrap justify-center gap-5 w-full mt-3">
      <BookingList />
    </div>
  </div>
</main>
