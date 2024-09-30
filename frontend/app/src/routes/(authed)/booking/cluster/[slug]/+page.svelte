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
  import { Textarea } from '$lib/components/ui/textarea';
  import { Skeleton } from '$lib/components/ui/skeleton';
  import { ChevronLeft, ArrowRight, Trash, Check } from 'lucide-svelte';

  export let data;
  $: cluster = data.clusterData;
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
  {#if cluster && !data.errorMessage}
    <div class="mx-auto grid flex-1 auto-rows-max gap-4">
      <div class="flex flex-wrap items-center gap-4">
        <Button href="/" variant="outline" size="icon" class="h-7 w-7">
          <ChevronLeft class="h-4 w-4" />
          <span class="sr-only">Back</span>
        </Button>
        <h1 class="flex-1 shrink-0 whitespace-nowrap text-xl font-semibold tracking-tight sm:grow-0">Booking details</h1>
        <Badge variant="outline" class="text-orange-500 border-orange-500">Cluster</Badge>
        <div class="flex items-center gap-2 md:ml-auto">
          <ClusterActionsDropdown type="Booking" />
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
                      <a href="/user/{cluster.owner.id}" class="flex gap-2 items-center hover:font-medium">
                        <Avatar.Root>
                          <Avatar.Fallback>{cluster.owner.name[0]}{cluster.owner.surname[0]}</Avatar.Fallback>
                        </Avatar.Root>
                        {cluster.owner.name}
                        {cluster.owner.surname}
                      </a>
                    </div>
                  </div>
                </div>
                <div class="grid gap-3">
                  <Label for="description">Created and expire dates</Label>
                  <div class="flex gap-2 text-sm items-center">
                    <p>{formatDateTime(cluster.createdAt)}</p>
                    <ArrowRight class="size-4" />
                    <p>{formatDateTime(cluster.expiredAt)}</p>
                  </div>
                </div>
              </div>
            </Card.Content>
          </Card.Root>

          <!-- vCenter and exsi host cards -->
          {#each cluster.vCenters as vCenter (vCenter.id)}
            <Card.Root>
              <Card.Header>
                <Card.Title>vCenter</Card.Title>
                <Card.Description>Details about the vCenter and it's ESXi hosts</Card.Description>
              </Card.Header>
              <Card.Content>
                <div class="grid gap-6">
                  <div class="grid gap-3">
                    <div class="inline-flex gap-7">
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

                  <!-- exsi hosts -->
                  {#each vCenter.esxiHosts as exsiHost (exsiHost.id)}
                    <Card.Root>
                      <Card.Header>
                        <Card.Title>ESXi host</Card.Title>
                        <Card.Description>ESXI credentials and actions</Card.Description>
                      </Card.Header>
                      <Card.Content>
                        <div class="inline-flex gap-7">
                          <!-- Cluster ip -->
                          <div class="grid gap-3">
                            <Label for="vmIp" class="ml-2 font-bold">IP</Label>
                            <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{exsiHost.ip}</div>
                          </div>

                          <!-- Cluster username -->
                          <div class="grid gap-3">
                            <Label for="vmUsername" class="ml-2 font-bold">Username</Label>
                            <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{exsiHost.userName}</div>
                          </div>

                          <!-- Cluster password -->
                          <div class="grid gap-3">
                            <Label for="vmpassword" class="ml-2 font-bold">Password</Label>
                            <div class="py-2 px-3 rounded-lg bg-secondary text-secondary-foreground text-sm">{exsiHost.password}</div>
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
