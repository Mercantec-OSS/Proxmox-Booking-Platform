<script>
  import BookingList from '$lib/components/authed/bookings/booking-list.svelte';
  import { clusterListStore, vmListStore, userStore } from '$lib/utils/store';
  import { onMount, onDestroy } from 'svelte';
  import { clusterService } from '$lib/services/cluster-service';
  import { vmService } from '$lib/services/vm-service';

  export let data;
  let bookingUpdateIntervalId;
  let bookingUpdateInterval;
  let userAuthed = data.userInfo.role !== 'Student';

  /* Update stores (global vars) to the data returned from the fetch requests in SSR */
  clusterListStore.set(data.clusterData);
  vmListStore.set(data.vmData);
  userStore.set(data.userInfo);

  async function fetchBookings() {
    if (userAuthed) {
      clusterListStore.set(await clusterService.getClusterBookingsFrontend());
    }
    vmListStore.set(await vmService.getVMBookingsFrontend());
  }

  /* Make an interval that refreshes all bookings every 20 seconds */
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

  <!-- Table of all the user's bookings -->
  <BookingList />
</main>
