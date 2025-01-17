<script>
  import { vmListStore, userStore } from '$lib/utils/store';
  import { Badge } from '$lib/components/ui/badge';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import * as Table from '$lib/components/ui/table';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
  import { ArrowUpRight, CirclePlus, ListRestart, ShieldEllipsis } from 'lucide-svelte';
  import { vmService } from '$lib/services/vm-service';
  import VMExtensionRequestDialog from '$lib/components/authed/bookings/vm/vm-extension-request-dialog.svelte';
  import { selectedBookingStore } from '$lib/utils/store';
  let userAuthed = $derived($userStore.role === 'Admin' || $userStore.role === 'Teacher');

  let vmExtensionRequestDialogOpen = $state(false);

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

<VMExtensionRequestDialog bind:vmExtensionRequestDialogOpen />

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
      <div class="flex flex-wrap justify-end pb-6">
        <Button class="w-39 mr-3" href="/create" variant="outline"><CirclePlus class="h-4 w-4 mr-1" /> Create Booking</Button>
        {#if userAuthed}
          <DropdownMenu.Root>
            <DropdownMenu.Trigger>
              <Button variant="outline" class="w-39 text-primary hover:text-primary/90 border border-primary hover:border-primary/90"><ShieldEllipsis class="h-4 w-4 mr-1" />Teacher actions</Button>
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
          {#each $vmListStore as booking (booking.id)}
            <Table.Row>
              <Table.Cell class="table-cell">
                <div class="flex gap-x-3 items-center">
                  <div class="h-9 w-1 rounded-full bg-primary"></div>
                </div>
              </Table.Cell>
              <Table.Cell>
                <Badge variant="outline">{booking.type}</Badge>
              </Table.Cell>
              <Table.Cell class="max-w-sm lg:max-w-md">
                <span class="block truncate">
                  "{booking.message}"
                </span>
              </Table.Cell>
              <Table.Cell>
                <span title={booking.uuid}>{booking.uuid?.slice(0, 7)}...</span>
              </Table.Cell>
              <Table.Cell>
                <Badge class="text-primary border-primary" variant={booking.isAccepted ? 'outline' : 'destructive'}>
                  {booking.extentions?.some((ext) => !ext.isAccepted) ? 'Pending Extension' : booking.isAccepted ? 'Confirmed' : 'Pending'}
                </Badge>
              </Table.Cell>
              <Table.Cell class="table-cell">
                <a href="/user/{booking.owner.id}" class="flex items-center gap-1">
                  <span>{booking.owner.name}</span>
                  <ArrowUpRight class="h-4 w-4" />
                </a>
              </Table.Cell>
              <Table.Cell class="table-cell">
                <a href="/user/{booking.assigned.id}" class="flex items-center gap-1">
                  <span>{booking.assigned.name}</span>
                  <ArrowUpRight class="h-4 w-4" />
                </a>
              </Table.Cell>
              <Table.Cell class="table-cell">{formatDateTime(booking.createdAt)}</Table.Cell>
              <Table.Cell class="table-cell">{formatDateTime(booking.expiredAt)}</Table.Cell>
              <Table.Cell>
                {#if booking.extentions?.length && !booking.extentions[booking.extentions.length - 1].isAccepted && userAuthed}
                  <Button
                    onmousedown={() => {
                      selectedBookingStore.set(booking);
                      vmExtensionRequestDialogOpen = true;
                    }}
                    size="sm"
                    class="ml-auto gap-1"
                  >
                    View
                    <ArrowUpRight class="h-4 w-4" />
                  </Button>
                {:else}
                  <Button href="/booking/vm/{booking.id}" size="sm" class="ml-auto gap-1">
                    View
                    <ArrowUpRight class="h-4 w-4" />
                  </Button>
                {/if}
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
