<script>
  import { vmService } from '$lib/services/vm-service';
  import { selectedBookingStore } from '$lib/utils/store';
  import VMActionsDropdown from '$lib/components/authed/bookings/vm/vm-actions-dropdown.svelte';
  import { toast } from 'svelte-sonner';
  import { goto } from '$app/navigation';
  import { afterNavigate } from '$app/navigation';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import { Badge } from '$lib/components/ui/badge';
  import { Label } from '$lib/components/ui/label';
  import * as Avatar from '$lib/components/ui/avatar/index.js';
  import { Textarea } from '$lib/components/ui/textarea';
  import { Skeleton } from '$lib/components/ui/skeleton';
  import { ChevronLeft, ArrowRight, Trash, Check } from 'lucide-svelte';

  export let data;
  let creds = { ip: '', username: '', password: '' };
  $: vm = data.vmData;
  $: selectedBookingStore.set(data.vmData);
  $: userAuthed = data.userInfo.role === 'Admin' || data.userInfo.role === 'Teacher';

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

  async function fetchVmCreds() {
    try {
      const vmInfo = await vmService.getVmInfo(vm.uuid);
      creds = vmInfo;
    } catch (error) {
      toast.error(error.message);
    }
  }

  async function handleAcceptBooking() {
    try {
      loadingAccept = true;
      await vmService.acceptVMBooking(vm.id);

      vm.isAccepted = true;
      toast.success(`Accepted booking`);
    } catch (error) {
      toast.error(error.message);
    } finally {
      loadingAccept = false;
    }
  }

  async function handleDeleteBooking() {
    try {
      loadingDelete = true;
      await vmService.deleteVMBooking(vm.id);

      goto('/');
      toast.success(`Booking deleted`);
    } catch (error) {
      toast.error(error.message);
    } finally {
      loadingDelete = false;
    }
  }

  // Function to check for and display errors
  function checkErrors() {
    if (data.errorMessage) {
      toast.error(data.errorMessage);
      goto('/');
    }
  }

  afterNavigate(() => {
    checkErrors();
    fetchVmCreds();
  });
</script>

<main class="grid flex-1 items-start gap-4 p-4 sm:px-6 md:gap-8">
  {#if vm && !data.errorMessage}
    <div class="mx-auto grid flex-1 auto-rows-max gap-4">
      <div class="flex flex-wrap items-center gap-4">
        <Button href="/" variant="outline" size="icon" class="h-7 w-7">
          <ChevronLeft class="h-4 w-4" />
          <span class="sr-only">Back</span>
        </Button>
        <h1 class="flex-1 shrink-0 whitespace-nowrap text-xl font-semibold tracking-tight sm:grow-0">Booking details</h1>
        <Badge variant="outline" class="text-indigo-500 border-indigo-500">Virtual machine</Badge>
        <Badge variant={vm.isAccepted ? 'outline' : 'destructive'} class={vm.isAccepted ? 'text-indigo-500 border-indigo-500' : ''}>{vm.isAccepted ? 'Confirmed' : 'Pending'}</Badge>
        <div class="flex items-center gap-2 md:ml-auto">
          <!-- Buttons for teacher/admin to accept or delete booking -->
          {#if !vm.isAccepted}
            <Button on:click={handleDeleteBooking} variant="outline" class="border-destructive text-destructive hover:bg-destructive/90 hover:text-white" size="sm">
              <Trash class="mr-2 h-4 w-4" />
              Delete
            </Button>
          {/if}

          {#if !vm.isAccepted && userAuthed}
            <Button on:click={handleAcceptBooking} variant="outline" class="border-green-600 text-green-600 hover:bg-green-600/90 hover:text-white" size="sm">
              <Check class="mr-2 h-4 w-4" />
              Accept
            </Button>
          {/if}

          {#if vm.isAccepted}
            <VMActionsDropdown />
          {/if}
        </div>
      </div>
      <div class="grid gap-4 md:grid-cols-[1fr_250px] lg:grid-cols-3 lg:gap-8">
        <div class="grid auto-rows-max items-start gap-4 lg:col-span-2 lg:gap-8">
          <Card.Root>
            <Card.Header>
              <Card.Title>Details</Card.Title>
              <Card.Description>Information about the booking and it's owner</Card.Description>
            </Card.Header>
            <Card.Content>
              <div class="grid gap-6">
                <div class="grid gap-3">
                  <div class="inline-flex justify-between text-sm">
                    <!-- Owner of the booking -->
                    <div class="grid gap-3">
                      <Label for="owner">Owner</Label>
                      <a href="/user/{vm.owner.id}" class="flex gap-2 items-center hover:font-medium">
                        <Avatar.Root>
                          <Avatar.Fallback>{vm.owner.name[0]}{vm.owner.surname[0]}</Avatar.Fallback>
                        </Avatar.Root>
                        {vm.owner.name}
                        {vm.owner.surname}
                      </a>
                    </div>

                    <!-- Teacher assigned to -->
                    <div class="grid gap-3">
                      <Label for="teacher">Teacher</Label>
                      <a href="/user/{vm.assigned.id}" class="flex gap-2 items-center hover:font-medium">
                        <Avatar.Root>
                          <Avatar.Fallback>{vm.assigned.name[0]}{vm.assigned.surname[0]}</Avatar.Fallback>
                        </Avatar.Root>
                        {vm.assigned.name}
                        {vm.assigned.surname}
                      </a>
                    </div>
                  </div>
                </div>
                <div class="grid gap-3">
                  <Label for="description">Created and expire dates</Label>
                  <div class="flex gap-2 text-sm items-center">
                    <p>{formatDateTime(vm.createdAt)}</p>
                    <ArrowRight class="size-4" />
                    <p>{formatDateTime(vm.expiredAt)}</p>
                  </div>
                </div>
                <div class="grid gap-3">
                  <Label for="description">Note</Label>
                  <Textarea class="resize-none" disabled value={vm.message} />
                </div>
              </div>
            </Card.Content>
          </Card.Root>
          <Card.Root>
            <Card.Header>
              <Card.Title>Virtual machine</Card.Title>
              <Card.Description>Details about template and credentials</Card.Description>
            </Card.Header>
            <Card.Content>
              <div class="grid gap-6">
                <div class="inline-flex">
                  <div class="grid gap-3">
                    <Label for="vmType" class="ml-2 font-bold">Template</Label>
                    <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{vm.type}</div>
                  </div>
                </div>

                <div class="grid gap-3">
                  <div class="inline-flex gap-7">
                    <!-- VM ip -->
                    <div class="grid gap-3">
                      <Label for="vmIp" class="ml-2 font-bold">IP</Label>
                      {#if creds.ip}
                        <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{creds.ip}</div>
                      {:else}
                        <Skeleton class="w-28 h-9 rounded-lg" />
                      {/if}
                    </div>

                    <!-- VM username -->
                    <div class="grid gap-3">
                      <Label for="vmUsername" class="ml-2 font-bold">Username</Label>
                      {#if creds.username}
                        <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{creds.username}</div>
                      {:else}
                        <Skeleton class="w-20 h-9 rounded-lg" />
                      {/if}
                    </div>

                    <!-- VM password -->
                    <div class="grid gap-3">
                      <Label for="vmpassword" class="ml-2 font-bold">Password</Label>
                      {#if creds.password}
                        <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{creds.password}</div>
                      {:else}
                        <Skeleton class="w-24 h-9 rounded-lg" />
                      {/if}
                    </div>
                  </div>
                </div>
              </div>
            </Card.Content>
          </Card.Root>
        </div>
        <div class="grid auto-rows-max items-start gap-4 lg:gap-8">
          <Card.Root>
            <Card.Header>
              <Card.Title>CPU usage</Card.Title>
            </Card.Header>
            <Card.Content>
              <div class="grid gap-6">
                <div class="grid gap-3">
                  <Label for="status">Graph</Label>
                </div>
              </div>
            </Card.Content>
          </Card.Root>
          <Card.Root>
            <Card.Header>
              <Card.Title>Memory usage</Card.Title>
            </Card.Header>
            <Card.Content>
              <div class="grid gap-6">
                <div class="grid gap-3">
                  <Label for="status">Graph</Label>
                </div>
              </div>
            </Card.Content>
          </Card.Root>
          <Card.Root>
            <Card.Header>
              <Card.Title>Disk usage</Card.Title>
            </Card.Header>
            <Card.Content>
              <div class="grid gap-6">
                <div class="grid gap-3">
                  <Label for="status">Graph</Label>
                </div>
              </div>
            </Card.Content>
          </Card.Root>
        </div>
      </div>
    </div>
  {/if}
</main>
