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
  import { ChevronLeft, ArrowRight, Trash, Check, LoaderCircle } from 'lucide-svelte';

  // Props and data initialization
  const { data } = $props();
  const { vmData, userInfo, errorMessage } = data;

  // State management
  let loadingDelete = $state(false);
  let loadingAccept = $state(false);

  // Initialize selected booking store
  $effect(() => {
    selectedBookingStore.set(vmData);
  });

  // Constants
  const metrics = ['CPU', 'Memory', 'Disk'];
  const dateFormatter = new Intl.DateTimeFormat(undefined, {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true
  });

  // Derived values
  let userAuthed = $derived(userInfo.role === 'Admin' || userInfo.role === 'Teacher');
  let users = $derived([
    { label: 'Owner', user: $selectedBookingStore?.owner },
    { label: 'Teacher', user: $selectedBookingStore?.assigned }
  ]);

  const formatDateTime = (date) => dateFormatter.format(new Date(date)).replace(',', '');

  async function fetchVmCreds() {
    try {
      const { ip, username, password } = await vmService.getVmInfo($selectedBookingStore.uuid);
      selectedBookingStore.update((store) => ({ ...store, ip, username, password }));
    } catch (error) {
      toast.error(error.message);
    }
  }

  async function handleAcceptBooking() {
    if (loadingAccept) return;

    try {
      loadingAccept = true;
      await vmService.acceptVMBooking($selectedBookingStore.id);
      $selectedBookingStore.isAccepted = true;
      toast.success('Accepted booking');
    } catch (error) {
      toast.error(error.message);
    } finally {
      loadingAccept = false;
    }
  }

  async function handleDeleteBooking() {
    if (loadingDelete) return;

    try {
      loadingDelete = true;
      await vmService.deleteVMBooking($selectedBookingStore.id);
      toast.success('Booking deleted');
      goto('/');
    } catch (error) {
      toast.error(error.message);
    } finally {
      loadingDelete = false;
    }
  }

  function copyToClipboard(text) {
    navigator.clipboard.writeText(text);
    toast.success('Copied to clipboard');
  }

  function checkErrors() {
    if (errorMessage) {
      toast.error(errorMessage);
      goto('/');
    }
  }

  afterNavigate(() => {
    checkErrors();
    if (!$selectedBookingStore.ip) {
      fetchVmCreds();
    }
  });
</script>

<main class="grid flex-1 items-start gap-4 p-4 sm:px-6 md:gap-8">
  {#if $selectedBookingStore && !data.errorMessage}
    <div class="mx-auto grid flex-1 auto-rows-max gap-4">
      <div class="flex flex-wrap items-center gap-4">
        <Button href="/" variant="outline" size="icon" class="size-7">
          <ChevronLeft class="size-4" />
          <span class="sr-only">Back</span>
        </Button>
        <h1 class="flex-1 shrink-0 whitespace-nowrap text-xl font-semibold tracking-tight sm:grow-0">Booking details</h1>
        <Badge variant="outline" class="text-primary border-primary">Virtual machine</Badge>
        <Badge variant={$selectedBookingStore.isAccepted ? 'outline' : 'destructive'} class={$selectedBookingStore.isAccepted ? 'text-primary border-primary' : ''}
          >{$selectedBookingStore.isAccepted ? 'Confirmed' : 'Pending'}</Badge
        >
        <div class="flex items-center gap-2 md:ml-auto">
          <!-- Buttons for teacher/admin to accept or delete booking -->
          {#if !$selectedBookingStore.isAccepted}
            <Button onmousedown={handleDeleteBooking} disabled={loadingDelete} variant="outline" class="border-destructive text-destructive hover:bg-destructive/90 hover:text-white" size="sm">
              {#if loadingDelete}
                <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
              {:else}
                <Trash class="mr-2 h-4 w-4" />
              {/if}
              Delete
            </Button>
          {/if}

          {#if !$selectedBookingStore.isAccepted && userAuthed}
            <Button onmousedown={handleAcceptBooking} disabled={loadingAccept} variant="outline" class="border-green-600 text-green-600 hover:bg-green-600/90 hover:text-white" size="sm">
              {#if loadingAccept}
                <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
              {:else}
                <Check class="mr-2 h-4 w-4" />
              {/if}
              Accept
            </Button>
          {/if}

          {#if $selectedBookingStore.isAccepted}
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
                  <div class="inline-flex flex-wrap gap-x-7 gap-y-4 justify-between text-sm">
                    {#each users as { label, user }}
                      <div class="grid gap-3">
                        <Label for={label.toLowerCase()}>{label}</Label>
                        <a href="/user/{user.id}" class="flex gap-2 items-center hover:font-medium">
                          <Avatar.Root>
                            <Avatar.Fallback>{user.name[0]}{user.surname[0]}</Avatar.Fallback>
                          </Avatar.Root>
                          {user.name}
                          {user.surname}
                        </a>
                      </div>
                    {/each}
                  </div>
                </div>

                <div class="grid gap-3">
                  <Label for="description">Created and expire dates</Label>
                  <div class="flex gap-2 text-sm items-center">
                    <p>{formatDateTime($selectedBookingStore.createdAt)}</p>
                    <ArrowRight class="size-4" />
                    <p>{formatDateTime($selectedBookingStore.expiredAt)}</p>
                  </div>
                </div>

                <div class="text-sm mb-4">
                  <div>Machine UUID</div>
                  <div>
                    {$selectedBookingStore.uuid}
                  </div>
                </div>
                <div class="grid gap-3">
                  <Label for="description">Note</Label>
                  <Textarea class="resize-none" disabled value={$selectedBookingStore.message} />
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
                    <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{$selectedBookingStore.type}</div>
                  </div>
                </div>

                <div class="grid gap-3">
                  <div class="inline-flex flex-wrap gap-x-7 gap-y-4">
                    <!-- VM ip -->
                    <div class="grid gap-3">
                      <Label for="vmIp" class="ml-2 font-bold">IP</Label>
                      {#if $selectedBookingStore.ip}
                        <button
                          class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm cursor-pointer hover:bg-secondary/80"
                          onmousedown={() => copyToClipboard($selectedBookingStore.ip)}>{$selectedBookingStore.ip}</button
                        >
                      {:else}
                        <Skeleton class="w-28 h-9 rounded-lg" />
                      {/if}
                    </div>

                    <!-- VM username -->
                    <div class="grid gap-3">
                      <Label for="vmUsername" class="ml-2 font-bold">Username</Label>
                      {#if $selectedBookingStore.username}
                        <button
                          class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm cursor-pointer hover:bg-secondary/80"
                          onmousedown={() => copyToClipboard($selectedBookingStore.username)}>{$selectedBookingStore.username}</button
                        >
                      {:else}
                        <Skeleton class="w-20 h-9 rounded-lg" />
                      {/if}
                    </div>

                    <!-- VM password -->
                    <div class="grid gap-3">
                      <Label for="vmpassword" class="ml-2 font-bold">Password</Label>
                      {#if $selectedBookingStore.password}
                        <button
                          class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm cursor-pointer hover:bg-secondary/80"
                          onmousedown={() => copyToClipboard($selectedBookingStore.password)}>{$selectedBookingStore.password}</button
                        >
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
          {#each metrics as metric}
            <Card.Root>
              <Card.Header>
                <Card.Title>{metric} usage</Card.Title>
              </Card.Header>
              <Card.Content>
                <div class="grid gap-6">
                  <div class="grid gap-3">
                    <Label for="status">Graph</Label>
                  </div>
                </div>
              </Card.Content>
            </Card.Root>
          {/each}
        </div>
      </div>
    </div>
  {/if}
</main>
