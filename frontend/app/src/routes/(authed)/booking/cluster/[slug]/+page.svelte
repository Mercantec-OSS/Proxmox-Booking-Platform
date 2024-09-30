<script>
  import { clusterService } from '$lib/services/cluster-service';
  import { selectedBookingStore } from '$lib/utils/store';
  import ClusterActionsDropdown from '$lib/components/authed/bookings/cluster/cluster-actions-dropdown.svelte';
  import { toast } from 'svelte-sonner';
  import { goto } from '$app/navigation';
  import { afterNavigate } from '$app/navigation';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import { Badge } from '$lib/components/ui/badge';
  import { Label } from '$lib/components/ui/label';
  import * as Avatar from '$lib/components/ui/avatar/index.js';
  import { ChevronLeft, ArrowRight } from 'lucide-svelte';

  export let data;
  $: selectedBookingStore.set(data.clusterData);

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

  // Function to check for and display errors
  function checkErrors() {
    if (data.errorMessage) {
      toast.error(data.errorMessage);
      goto('/');
    }
  }

  afterNavigate(() => {
    checkErrors();
  });
</script>

<main class="grid flex-1 items-start gap-4 p-4 sm:px-6 md:gap-8">
  {#if $selectedBookingStore && !data.errorMessage}
    <div class="grid mx-auto auto-rows-max gap-4">
      <div class="flex flex-wrap items-center gap-4">
        <Button href="/" variant="outline" size="icon" class="h-7 w-7">
          <ChevronLeft class="h-4 w-4" />
          <span class="sr-only">Back</span>
        </Button>
        <h1 class="flex-1 shrink-0 whitespace-nowrap text-xl font-semibold tracking-tight sm:grow-0">Booking details</h1>
        <Badge variant="outline" class="text-orange-500 border-orange-500">Cluster</Badge>
        <div class="flex items-center gap-2 md:ml-auto">
          <ClusterActionsDropdown />
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
                  <!-- Owner of the booking -->
                  <Label for="owner">Owner</Label>
                  <a href="/user/{$selectedBookingStore.owner.id}" class="flex gap-2 items-center text-sm hover:font-semibold">
                    <Avatar.Root>
                      <Avatar.Fallback>{$selectedBookingStore.owner.name[0]}{$selectedBookingStore.owner.surname[0]}</Avatar.Fallback>
                    </Avatar.Root>
                    {$selectedBookingStore.owner.name}
                    {$selectedBookingStore.owner.surname}
                  </a>
                </div>
                <div class="grid gap-3">
                  <Label for="description">Created and expire dates</Label>
                  <div class="flex gap-2 text-sm items-center">
                    <p>{formatDateTime($selectedBookingStore.createdAt)}</p>
                    <ArrowRight class="size-4" />
                    <p>{formatDateTime($selectedBookingStore.expiredAt)}</p>
                  </div>
                </div>
              </div>
            </Card.Content>
          </Card.Root>

          <!-- vCenter and esxi host cards -->
          {#each $selectedBookingStore.vCenters as vCenter (vCenter.id)}
            <Card.Root>
              <Card.Header>
                <Card.Title>vCenter</Card.Title>
                <Card.Description>Details about the vCenter and it's ESXi hosts</Card.Description>
              </Card.Header>
              <Card.Content>
                <div class="grid gap-6">
                  <div class="grid gap-3">
                    <div class="flex flex-wrap gap-x-7 gap-y-4">
                      <!-- Cluster ip -->
                      <div class="grid gap-3">
                        <Label for="vmIp" class="ml-2 font-bold">IP</Label>
                        <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{vCenter.ip}</div>
                      </div>

                      <!-- Cluster username -->
                      <div class="grid gap-3">
                        <Label for="vmUsername" class="ml-2 font-bold">Username</Label>
                        <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{vCenter.userName}</div>
                      </div>

                      <!-- Cluster password -->
                      <div class="grid gap-3">
                        <Label for="vmpassword" class="ml-2 font-bold">Password</Label>
                        <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{vCenter.password}</div>
                      </div>
                    </div>
                  </div>

                  <!-- esxi hosts -->
                  {#each vCenter.esxiHosts as esxiHost (esxiHost.id)}
                    <Card.Root>
                      <Card.Header>
                        <Card.Title>ESXi host</Card.Title>
                        <Card.Description>ESXI credentials and actions</Card.Description>
                      </Card.Header>
                      <Card.Content>
                        <div class="inline-flex flex-wrap gap-x-7 gap-y-4">
                          <!-- Cluster ip -->
                          <div class="grid gap-3">
                            <Label for="vmIp" class="ml-2 font-bold">IP</Label>
                            <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{esxiHost.ip}</div>
                          </div>

                          <!-- Cluster username -->
                          <div class="grid gap-3">
                            <Label for="vmUsername" class="ml-2 font-bold">Username</Label>
                            <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{esxiHost.userName}</div>
                          </div>

                          <!-- Cluster password -->
                          <div class="grid gap-3">
                            <Label for="vmpassword" class="ml-2 font-bold">Password</Label>
                            <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{esxiHost.password}</div>
                          </div>
                        </div>
                      </Card.Content>
                    </Card.Root>
                  {/each}
                </div>
              </Card.Content>
            </Card.Root>
          {/each}
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
