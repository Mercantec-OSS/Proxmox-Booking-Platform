<!-- Component to show list of clusters with vcenter and hosts -->

<script>
  import { vmListStore, clusterListStore, selectedBookingStatus, userStore } from '$lib/utils/store';
  import { Badge } from '$lib/components/ui/badge';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import * as Table from '$lib/components/ui/table';
  import * as Tabs from '$lib/components/ui/tabs';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
  import { ArrowUpRight } from 'lucide-svelte';

  let userAuthed = $userStore.role !== 'Student';

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
</script>

{#if $vmListStore.length === 0 && $clusterListStore.length === 0}
  <!-- Card displaying user has no bookings -->
  <div class="flex flex-1 items-center justify-center rounded-lg border border-dashed shadow-sm">
    <div class="flex flex-col items-center gap-1 text-center">
      <h3 class="text-2xl font-bold tracking-tight">You have no server bookings</h3>
      <p class="text-muted-foreground text-sm">Get started by creating a new booking using the button below.</p>
      <Button class="mt-4">Create Booking</Button>
    </div>
  </div>
{:else}
  <!-- Virtual machine booking table  -->
  <Card.Root class="flex-grow">
    <Card.Header>
      <Card.Title>Bookings</Card.Title>
      <Card.Description>View and manage your cluster and virtual machine bookings</Card.Description>
    </Card.Header>
    <Card.Content>
      <Table.Root>
        <Table.Header>
          <Table.Row>
            <Table.Head class="table-cell">Type</Table.Head>
            <Table.Head class="table-cell">Template</Table.Head>
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
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
          <Table.Row>
            <Table.Cell class="table-cell">
              <div class="flex gap-x-3 items-center">
                <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                Virtual machine
              </div>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">ubuntu_2404_server_template</Badge>
            </Table.Cell>

            <Table.Cell>
              <Badge variant="outline">Confirmed</Badge>
            </Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell"><a href="/user/id">admin user</a></Table.Cell>
            <Table.Cell class="table-cell">Sep 20 09:05 AM</Table.Cell>
            <Table.Cell class="table-cell">Sep 22 02:00 AM</Table.Cell>
            <Table.Cell
              ><Button href="/booking/id" size="sm" class="ml-auto gap-1">
                View
                <ArrowUpRight class="h-4 w-4" />
              </Button></Table.Cell
            >
          </Table.Row>
        </Table.Body>
      </Table.Root>
    </Card.Content>
    <Card.Footer>
      <div class="text-muted-foreground text-xs">
        Showing <strong>1-32</strong> of <strong>32</strong> bookings
      </div>
    </Card.Footer>
  </Card.Root>
{/if}
