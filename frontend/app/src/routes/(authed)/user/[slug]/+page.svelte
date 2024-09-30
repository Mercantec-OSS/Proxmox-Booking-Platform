<script>
  import UserInfo from '$lib/components/authed/user/user-info.svelte';
  import BookingList from '$lib/components/authed/bookings/booking-list.svelte';
  import { clusterListStore, vmListStore, userStore } from '$lib/utils/store';
  import { toast } from 'svelte-sonner';
  import { goto } from '$app/navigation';
  import { afterNavigate } from '$app/navigation';

  export let data;

  $: userStore.set(data.userInfo);
  $: clusterListStore.set(data.clusterData);
  $: vmListStore.set(data.vmData);
  $: userAuthed = $userStore.role === 'Admin' || $userStore.role === 'Teacher';

  // Function to check for and display errors
  function checkErrors() {
    if (data.errorMessage || !data.userData) {
      toast.error(error.message);

      if (data.errorMessage === 'Invalid user ID' || data.errorMessage === 'User not found') {
        goto('/');
      }
    }

    // Show toast if user has no bookings
    if (userAuthed && $clusterListStore.length === 0 && $vmListStore.length === 0) {
      toast.error(`User has no bookings`);
    }
  }

  afterNavigate(() => {
    checkErrors();
  });
</script>

<main class="flex flex-col flex-grow rounded-3xl bg-background">
  <div class="mx-auto mt-10">
    {#if data.userData}
      <UserInfo user={data.userData} />
    {/if}
  </div>

  {#if $userStore.role === 'Admin' || $userStore.role === 'Teacher'}
    <!-- Button to open create booking drawer and filter bookings -->
    <div class="flex flex-col md:w-3/4 md:mx-auto my-10">
      <!-- List of all bookings -->
      <div class="flex flex-wrap justify-center gap-5 w-full mt-3">
        <BookingList />
      </div>
    </div>
  {:else}
    <div class="flex-grow flex justify-center items-center">
      <div class="flex flex-col gap-y-6 items-center justify-center">
        <img src="/images/wumpus.gif" alt="Wumpus Beyond Curious" class="w-1/4 h-auto" />
        <p class="text-muted-foreground select-none">You are not authorized to view {data.userData.name}'s bookings</p>
      </div>
    </div>
  {/if}
</main>
