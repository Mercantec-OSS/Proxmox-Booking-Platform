<!-- Component to show list of clusters with vcenter and hosts -->

<script>
  import { vmListStore, clusterListStore, userStore } from '$lib/utils/store';
  import { Badge } from '$lib/components/ui/badge';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import * as Table from '$lib/components/ui/table';
  import * as Tabs from '$lib/components/ui/tabs';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
  import { ArrowUpRight, CirclePlus, ListRestart, ShieldEllipsis } from 'lucide-svelte';
  import { vmService } from '$lib/services/vm-service';

  $: userAuthed = $userStore.role === 'Admin' || $userStore.role === 'Teacher';
  let activeTab = 'vms';

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

  function countEsxiHosts(cluster) {
    return cluster.vCenters.reduce((total, vCenter) => total + vCenter.esxiHosts.length, 0);
  }
</script>

{#if $vmListStore.length === 0 && $clusterListStore.length === 0}
  <!-- Card displaying user has no bookings -->
  <div class="flex flex-1 items-center justify-center rounded-lg border border-dashed shadow-sm">
    <div class="flex flex-col items-center gap-1 text-center">
      <h3 class="text-2xl font-bold tracking-tight">You have no bookings</h3>
      <p class="text-muted-foreground text-sm">Get started by creating a new booking using the button below.</p>
      <Button href="/create" class="mt-4">Create Booking <CirclePlus class="h-4 w-4 ml-1" /></Button>
    </div>
  </div>
{:else}
  <Card.Root>
    <Card.Header>
      <Card.Title>Bookings</Card.Title>
      <Card.Description>View and manage your cluster and virtual machine bookings</Card.Description>
    </Card.Header>
    <Card.Content>
      <Tabs.Root bind:value={activeTab}>
        <div class="flex justify-between items-center">
          <Tabs.List class="grid w-full md:w-96 grid-cols-2">
            <Tabs.Trigger value="vms">Virtual machines</Tabs.Trigger>
            <Tabs.Trigger value="clusters">Clusters</Tabs.Trigger>
          </Tabs.List>
          <Button class='w-39' href="/create" variant="outline"><CirclePlus class="h-4 w-4 mr-1" /> Create Booking</Button>
        </div>
        {#if userAuthed}
        <div class="flex justify-end items-center">
            <DropdownMenu.Root>
              <DropdownMenu.Trigger>
                <Button variant="outline" class='w-39'><ShieldEllipsis class="h-4 w-4 mr-1" />Teacher actions</Button>
              </DropdownMenu.Trigger>
              <DropdownMenu.Content>
                <DropdownMenu.Item on:click="{vmService.resetVMTemplates}"><ListRestart class="h-4 w-4 mr-1" />Reset templates</DropdownMenu.Item>
              </DropdownMenu.Content>
            </DropdownMenu.Root>
        </div>
            
        {/if}
        <Tabs.Content value="vms">
          <!-- Virtual machine booking table  -->
          <Table.Root>
            <Table.Header>
              <Table.Row>
                <Table.Head class="table-cell"></Table.Head>
                <Table.Head class="table-cell">Template</Table.Head>
                <Table.Head class="table-cell">Note</Table.Head>
                <Table.Head class="table-cell">UUID</Table.Head>
                <Table.Head class="table-cell">Status</Table.Head>
                <Table.Head class="table-cell">Owner</Table.Head>
                <Table.Head class="table-cell">Assigned to</Table.Head>
                <Table.Head class="table-cell">Created at</Table.Head>
                <Table.Head class="table-cell">Expire at</Table.Head>
                <Table.Head>
                  <span class="sr-only">Open booking</span>
                </Table.Head>
              </Table.Row>
            </Table.Header>
            <Table.Body>
              {#each $vmListStore as vm (`${vm.id}-vm`)}
                <Table.Row>
                  <!-- Color indicator to show if vm or cluster -->
                  <Table.Cell class="table-cell">
                    <div class="flex gap-x-3 items-center">
                      <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                    </div>
                  </Table.Cell>
                  <!-- Badge that shows which vm template used -->
                  <Table.Cell>
                    <Badge variant="outline">{vm.type}</Badge>
                  </Table.Cell>
                  <!-- Comment made by user at creation -->
                  <Table.Cell class="max-w-sm lg:max-w-md">
                    <span class="block truncate">
                      "{vm.message}"
                    </span>
                  </Table.Cell>
                  <!-- Shows first 5 chart from machine UUID -->
                  <Table.Cell>
                    <span title="{vm.uuid}">{vm.uuid?.slice(0, 7)}...</span>
                  </Table.Cell>
                  <!-- Shows if booking is accepted or pending -->
                  <Table.Cell>
                    <Badge variant={vm.isAccepted ? 'outline' : 'destructive'}>{vm.isAccepted ? 'Confirmed' : 'Pending'}</Badge>
                  </Table.Cell>
                  <!-- Show who is owner and redirect to their profile on click -->
                  <Table.Cell class="table-cell">
                    <a href="/user/{vm.owner.id}" class="flex items-center gap-1">
                      <span>{vm.owner.name}</span>
                      <ArrowUpRight class="h-4 w-4" />
                    </a>
                  </Table.Cell>
                  <!-- Show who is assigned and redirect to their profile on click -->
                  <Table.Cell class="table-cell">
                    <a href="/user/{vm.assigned.id}" class="flex items-center gap-1">
                      <span>{vm.assigned.name}</span>
                      <ArrowUpRight class="h-4 w-4" />
                    </a>
                  </Table.Cell>
                  <!-- Show when booking was created -->
                  <Table.Cell class="table-cell">{formatDateTime(vm.createdAt)}</Table.Cell>
                  <!-- Show when booking is expiring -->
                  <Table.Cell class="table-cell">{formatDateTime(vm.expiredAt)}</Table.Cell>
                  <!-- Button to open the booking page to show full information about it -->
                  <Table.Cell
                    ><Button href="/booking/vm/{vm.id}" size="sm" class="ml-auto gap-1">
                      View
                      <ArrowUpRight class="h-4 w-4" />
                    </Button></Table.Cell
                  >
                </Table.Row>
              {/each}
            </Table.Body>
          </Table.Root>
        </Tabs.Content>
        <Tabs.Content value="clusters">
          <!-- Cluster booking table  -->
          <Table.Root>
            <Table.Header>
              <Table.Row>
                <Table.Head class="table-cell"></Table.Head>
                <Table.Head class="table-cell">Students</Table.Head>
                <Table.Head class="table-cell">vCenters</Table.Head>
                <Table.Head class="table-cell">ESXi hosts</Table.Head>
                <Table.Head class="table-cell">Owner</Table.Head>
                <Table.Head class="table-cell">Created at</Table.Head>
                <Table.Head class="table-cell">Expire at</Table.Head>
                <Table.Head>
                  <span class="sr-only">Open booking</span>
                </Table.Head>
              </Table.Row>
            </Table.Header>
            <Table.Body>
              {#each $clusterListStore as cluster (`${cluster.id}-cluster`)}
                <Table.Row>
                  <!-- Color indicator to show if vm or cluster -->
                  <Table.Cell class="table-cell">
                    <div class="flex gap-x-3 items-center">
                      <div class="h-9 w-1 rounded-full bg-orange-500"></div>
                    </div>
                  </Table.Cell>
                  <!-- Amount of students -->
                  <Table.Cell>
                    {cluster.amountStudents}x Students
                  </Table.Cell>
                  <!-- Amount of vCenters -->
                  <Table.Cell>
                    {cluster.vCenters.length}x vCenters
                  </Table.Cell>
                  <!-- Amount of  -->
                  <Table.Cell>
                    {countEsxiHosts(cluster)}x ESXi hosts
                  </Table.Cell>
                  <!-- Show who is owner and redirect to their profile on click -->
                  <Table.Cell class="table-cell">
                    <a href="/user/{cluster.owner.id}" class="flex items-center gap-1">
                      <span>{cluster.owner.name}</span>
                      <ArrowUpRight class="h-4 w-4" />
                    </a>
                  </Table.Cell>
                  <!-- Show when booking was created -->
                  <Table.Cell class="table-cell">{formatDateTime(cluster.createdAt)}</Table.Cell>
                  <!-- Show when booking is expiring -->
                  <Table.Cell class="table-cell">{formatDateTime(cluster.expiredAt)}</Table.Cell>
                  <!-- Button to open the booking page to show full information about it -->
                  <Table.Cell
                    ><Button href="/booking/cluster/{cluster.id}" size="sm" class="ml-auto gap-1">
                      View
                      <ArrowUpRight class="h-4 w-4" />
                    </Button></Table.Cell
                  >
                </Table.Row>
              {/each}
            </Table.Body>
          </Table.Root>
        </Tabs.Content>
      </Tabs.Root>
    </Card.Content>
    <Card.Footer>
      <!-- Show about of vms/clusters being shown and total amount combined -->
      <div class="text-muted-foreground text-xs">
        Showing <strong>1-{activeTab === 'vms' ? $vmListStore.length : $clusterListStore.length}</strong> of <strong>{$vmListStore.length + $clusterListStore.length}</strong> bookings
      </div>
    </Card.Footer>
  </Card.Root>
{/if}
