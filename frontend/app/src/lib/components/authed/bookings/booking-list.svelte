<script>
  import { vmListStore, userStore } from '$lib/utils/store';
  import { Badge } from '$lib/components/ui/badge';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import * as Table from '$lib/components/ui/table';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
  import { ArrowUpRight, CirclePlus, ListRestart, ShieldEllipsis } from 'lucide-svelte';
  import { vmService } from '$lib/services/vm-service';

  let userAuthed = $derived($userStore.role === 'Admin' || $userStore.role === 'Teacher');

  const dateTimeFormatter = new Intl.DateTimeFormat(undefined, {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true
  });

  function formatDateTime(date) {
    return dateTimeFormatter.format(new Date(date)).replace(',', '');
  }
</script>

{#if $vmListStore.length === 0}
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
      <Card.Description>View and manage your virtual machine bookings</Card.Description>
    </Card.Header>
    <Card.Content>
      <div class="flex space-x-2 justify-end pb-6">
        <Button class="w-39" href="/create" variant="outline"><CirclePlus class="h-4 w-4 mr-1" /> Create Booking</Button>
        {#if userAuthed}
          <DropdownMenu.Root>
            <DropdownMenu.Trigger>
              <Button variant="outline" class="w-39"><ShieldEllipsis class="h-4 w-4 mr-1" />Teacher actions</Button>
            </DropdownMenu.Trigger>
            <DropdownMenu.Content>
              <DropdownMenu.Item onmousedown={vmService.resetVMTemplates}><ListRestart class="h-4 w-4 mr-1" />Reset templates</DropdownMenu.Item>
            </DropdownMenu.Content>
          </DropdownMenu.Root>
        {/if}
      </div>
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
          {#each $vmListStore as { id, type, message, uuid, isAccepted, owner, assigned, createdAt, expiredAt } (id)}
            <Table.Row>
              <Table.Cell class="table-cell">
                <div class="flex gap-x-3 items-center">
                  <div class="h-9 w-1 rounded-full bg-indigo-500"></div>
                </div>
              </Table.Cell>
              <Table.Cell>
                <Badge variant="outline">{type}</Badge>
              </Table.Cell>
              <Table.Cell class="max-w-sm lg:max-w-md">
                <span class="block truncate">
                  "{message}"
                </span>
              </Table.Cell>
              <Table.Cell>
                <span title={uuid}>{uuid?.slice(0, 7)}...</span>
              </Table.Cell>
              <Table.Cell>
                <Badge variant={isAccepted ? 'outline' : 'destructive'}>{isAccepted ? 'Confirmed' : 'Pending'}</Badge>
              </Table.Cell>
              <Table.Cell class="table-cell">
                <a href="/user/{owner.id}" class="flex items-center gap-1">
                  <span>{owner.name}</span>
                  <ArrowUpRight class="h-4 w-4" />
                </a>
              </Table.Cell>
              <Table.Cell class="table-cell">
                <a href="/user/{assigned.id}" class="flex items-center gap-1">
                  <span>{assigned.name}</span>
                  <ArrowUpRight class="h-4 w-4" />
                </a>
              </Table.Cell>
              <Table.Cell class="table-cell">{formatDateTime(createdAt)}</Table.Cell>
              <Table.Cell class="table-cell">{formatDateTime(expiredAt)}</Table.Cell>
              <Table.Cell>
                <Button href="/booking/vm/{id}" size="sm" class="ml-auto gap-1">
                  View
                  <ArrowUpRight class="h-4 w-4" />
                </Button>
              </Table.Cell>
            </Table.Row>
          {/each}
        </Table.Body>
      </Table.Root>
    </Card.Content>
    <Card.Footer>
      <div class="text-muted-foreground text-xs">
        Showing <strong>1-{$vmListStore.length}</strong> of <strong>{$vmListStore.length}</strong> bookings
      </div>
    </Card.Footer>
  </Card.Root>
{/if}
