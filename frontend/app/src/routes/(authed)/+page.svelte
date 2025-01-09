<script>
  import BookingList from '$lib/components/authed/bookings/booking-list.svelte';
  import VcenterInfo from '$lib/components/authed/vcenter-info.svelte';
  import { clusterListStore, vmListStore, userStore } from '$lib/utils/store';
  import { onMount, onDestroy } from 'svelte';
  import { clusterService } from '$lib/services/cluster-service';
  import { vmService } from '$lib/services/vm-service';

  const { data } = $props();
  let bookingUpdateIntervalId;
  let bookingUpdateInterval;
  let userAuthed = $state(data.userInfo.role !== 'Student');

  /* Update stores (global vars) to the data returned from the fetch requests in SSR */
  clusterListStore.set(data.clusterData);
  vmListStore.set(data.vmData);
  userStore.set(data.userInfo);

  async function fetchBookings() {
    if (userAuthed) {
      clusterListStore.set(await clusterService.getClusterBookings());
    }
    vmListStore.set(await vmService.getVMBookings());
  }

  /* Make an interval that refreshes all bookings every 10 seconds */
  onMount(async () => {
    bookingUpdateInterval = setTimeout(() => {
      fetchBookings();
      bookingUpdateIntervalId = setInterval(fetchBookings, 10000);
    }, 10000);
  });

  /* Clear the refresh booking interval when a user leaves the page */
  onDestroy(() => {
    clearTimeout(bookingUpdateInterval);
    clearInterval(bookingUpdateIntervalId);
  });
</script>

<main class="flex flex-1 flex-col gap-4 p-4 lg:gap-6 lg:p-6">
  <h1 class="text-lg font-semibold md:text-2xl">Booking Overview</h1>
  <VcenterInfo />
  <!-- Table of all the user's bookings -->
  <BookingList />
</main>
