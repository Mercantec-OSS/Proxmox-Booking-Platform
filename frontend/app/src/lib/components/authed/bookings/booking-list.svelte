<script>
  import { vmListStore, userStore, selectedBookingStore } from '$lib/utils/store';
  import { Badge } from '$lib/components/ui/badge';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import * as Table from '$lib/components/ui/table';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
  import { ArrowUpRight, CirclePlus, ListRestart, ShieldEllipsis, ChevronDown, ArrowUpDown } from 'lucide-svelte';
  import { vmService } from '$lib/services/vm-service';
  import VMExtensionRequestDialog from '$lib/components/authed/bookings/dialogs/vm-extension-request-dialog.svelte';
  import { getCoreRowModel, getFilteredRowModel, getSortedRowModel } from '@tanstack/table-core';
  import { Input } from '$lib/components/ui/input';
  import { FlexRender, createSvelteTable } from '$lib/components/ui/data-table';

  let userAuthed = $derived($userStore.role === 'Admin' || $userStore.role === 'Teacher');
  let vmExtensionRequestDialogOpen = $state(false);
  let data = $derived($vmListStore);

  const dateTimeFormatter = new Intl.DateTimeFormat(undefined, {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true
  });

  const formatDateTime = (date) => dateTimeFormatter.format(new Date(date)).replace(',', '');

  const columns = [
    {
      accessorKey: 'type',
      header: 'Template',
      cell: ({ row }) => row.getValue('type'),
      enableSorting: true,
      sortingFn: 'alphanumeric'
    },
    {
      accessorKey: 'message',
      header: 'Note',
      cell: ({ row }) => row.getValue('message'),
      enableSorting: true,
      sortingFn: 'alphanumeric'
    },
    {
      accessorKey: 'uuid',
      header: 'UUID',
      cell: ({ row }) => row.getValue('uuid').split('-')[0],
      enableSorting: true,
      sortingFn: 'alphanumeric'
    },
    {
      accessorKey: 'isAccepted',
      header: 'Status',
      cell: ({ row }) => row.getValue('isAccepted'),
      enableSorting: true,
      sortingFn: (rowA, rowB, columnId) => {
        const a = rowA.getValue(columnId);
        const b = rowB.getValue(columnId);
        return a === b ? 0 : a ? -1 : 1;
      }
    },
    {
      accessorKey: 'owner',
      header: 'Owner',
      cell: ({ row }) => {
        const owner = row.getValue('owner');
        return `${owner.name} ${owner.surname}`;
      },
      filterFn: (row, id, value) => {
        const owner = row.getValue(id);
        return `${owner.name} ${owner.surname}`.toLowerCase().includes(value.toLowerCase());
      },
      enableSorting: true,
      sortingFn: (rowA, rowB, columnId) => {
        const a = rowA.getValue(columnId);
        const b = rowB.getValue(columnId);
        return `${a.name} ${a.surname}`.localeCompare(`${b.name} ${b.surname}`);
      }
    },
    {
      accessorKey: 'assigned',
      header: 'Assigned to',
      cell: ({ row }) => {
        const assigned = row.getValue('assigned');
        return `${assigned.name} ${assigned.surname}`;
      },
      enableSorting: true,
      sortingFn: (rowA, rowB, columnId) => {
        const a = rowA.getValue(columnId);
        const b = rowB.getValue(columnId);
        return `${a.name} ${a.surname}`.localeCompare(`${b.name} ${b.surname}`);
      }
    },
    {
      accessorKey: 'createdAt',
      header: 'Created at',
      cell: ({ row }) => formatDateTime(row.getValue('createdAt')),
      enableSorting: true,
      sortingFn: (rowA, rowB, columnId) => {
        const a = new Date(rowA.getValue(columnId));
        const b = new Date(rowB.getValue(columnId));
        return a.getTime() - b.getTime();
      }
    },
    {
      accessorKey: 'expiredAt',
      header: 'Expire at',
      cell: ({ row }) => formatDateTime(row.getValue('expiredAt')),
      enableSorting: true,
      sortingFn: (rowA, rowB, columnId) => {
        const a = new Date(rowA.getValue(columnId));
        const b = new Date(rowB.getValue(columnId));
        return a.getTime() - b.getTime();
      }
    },
    {
      accessorKey: 'id',
      id: 'action',
      cell: ({ row }) => row.getValue('id'),
      enableSorting: false
    }
  ];

  let sorting = $state([]);
  let columnFilters = $state([]);
  let columnVisibility = $state({});

  const handleStateUpdate = (updater, state) => {
    if (typeof updater === 'function') {
      return updater(state);
    }
    return updater;
  };

  const table = createSvelteTable({
    get data() {
      return data;
    },
    columns,
    state: {
      get sorting() {
        return sorting;
      },
      get columnVisibility() {
        return columnVisibility;
      },
      get columnFilters() {
        return columnFilters;
      }
    },
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    onSortingChange: (updater) => {
      sorting = handleStateUpdate(updater, sorting);
    },
    onColumnFiltersChange: (updater) => {
      columnFilters = handleStateUpdate(updater, columnFilters);
    },
    onColumnVisibilityChange: (updater) => {
      columnVisibility = handleStateUpdate(updater, columnVisibility);
    }
  });
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
      <!-- Table with vm bookings -->
      <div class="w-full">
        <div class="flex flex-wrap justify-between items-center py-4">
          <!-- Filter by owner -->
          <Input
            placeholder="Filter by owner..."
            value={table.getColumn('owner')?.getFilterValue() ?? ''}
            oninput={(e) => table.getColumn('owner')?.setFilterValue(e.currentTarget.value)}
            onchange={(e) => table.getColumn('owner')?.setFilterValue(e.currentTarget.value)}
            class="max-w-sm"
          />

          <div class="flex flex-wrap">
            <!-- Create booking -->
            <Button class="w-39 mr-4" href="/create"><CirclePlus class="h-4 w-4 mr-1" /> Create Booking</Button>

            <!-- Filter columns -->
            <DropdownMenu.Root>
              <DropdownMenu.Trigger>
                {#snippet child({ props })}
                  <Button {...props} variant="outline" class="text-primary hover:text-primary/90 border border-primary hover:border-primary/90">
                    Columns <ChevronDown class="ml-2 size-4" />
                  </Button>
                {/snippet}
              </DropdownMenu.Trigger>
              <DropdownMenu.Content align="end">
                {#each table.getAllColumns().filter((col) => col.getCanHide()) as column}
                  <DropdownMenu.CheckboxItem class="capitalize" bind:checked={() => column.getIsVisible(), (v) => column.toggleVisibility(!!v)}>
                    {column.id}
                  </DropdownMenu.CheckboxItem>
                {/each}
              </DropdownMenu.Content>
            </DropdownMenu.Root>
          </div>
        </div>
        <div class="rounded-md border">
          <Table.Root>
            <Table.Header>
              {#each table.getHeaderGroups() as headerGroup (headerGroup.id)}
                <Table.Row>
                  <Table.Head></Table.Head>
                  {#each headerGroup.headers as header (header.id)}
                    <Table.Head>
                      {#if !header.isPlaceholder && header.id !== 'action'}
                        <button class="flex items-center gap-1" onclick={() => header.column.toggleSorting(header.column.getIsSorted() === 'asc')}>
                          <FlexRender content={header.column.columnDef.header} context={header.getContext()} />
                          {#if header.column.getCanSort()}
                            {#if header.column.getIsSorted() === 'asc'}
                              <ArrowUpDown class="h-4 w-4 ml-2 rotate-0" />
                            {:else if header.column.getIsSorted() === 'desc'}
                              <ArrowUpDown class="h-4 w-4 ml-2 rotate-180" />
                            {:else}
                              <ArrowUpDown class="h-4 w-4 ml-2" />
                            {/if}
                          {/if}
                        </button>
                      {/if}
                    </Table.Head>
                  {/each}
                </Table.Row>
              {/each}
            </Table.Header>
            <Table.Body>
              {#each table.getRowModel().rows as row (row.id)}
                <Table.Row>
                  <Table.Cell>
                    <div class="flex gap-x-3 items-center">
                      <div class="h-9 w-1 rounded-full bg-primary"></div>
                    </div>
                  </Table.Cell>
                  {#each row.getVisibleCells() as cell (cell.id)}
                    <Table.Cell>
                      {#if cell.column.id === 'type'}
                        <Badge variant="outline">{cell.getValue()}</Badge>
                      {:else if cell.column.id === 'message'}
                        <span class="block truncate max-w-sm">
                          "{cell.getValue()}"
                        </span>
                      {:else if cell.column.id === 'isAccepted'}
                        <Badge class="text-primary border-primary" variant="outline">
                          {row.original.extentions?.some((ext) => !ext.isAccepted) ? 'Pending Extension' : cell.getValue() ? 'Confirmed' : 'Pending'}
                        </Badge>
                      {:else if cell.column.id === 'assigned'}
                        <a href={`/user/${cell.getValue().id}`} class="flex items-center gap-1">
                          <span>{cell.getValue().name}</span>
                          <span>{cell.getValue().surname}</span>
                          <ArrowUpRight class="h-4 w-4" />
                        </a>
                      {:else if cell.column.id === 'owner'}
                        <a href={`/user/${cell.getValue().id}`} class="flex items-center gap-1">
                          <span>{cell.getValue().name}</span>
                          <span>{cell.getValue().surname}</span>
                          <ArrowUpRight class="h-4 w-4" />
                        </a>
                      {:else if cell.column.id === 'action'}
                        {#if row.original.extentions?.length && !row.original.extentions[row.original.extentions.length - 1].isAccepted && userAuthed}
                          <Button
                            onmousedown={() => {
                              selectedBookingStore.set(row.original);
                              vmExtensionRequestDialogOpen = true;
                            }}
                            size="sm"
                            class="ml-auto gap-1"
                          >
                            View
                            <ArrowUpRight class="h-4 w-4" />
                          </Button>
                        {:else}
                          <Button href={`/booking/vm/${cell.getValue()}`} size="sm" class="ml-auto gap-1">
                            View
                            <ArrowUpRight class="h-4 w-4" />
                          </Button>
                        {/if}
                      {:else}
                        <FlexRender content={cell.column.columnDef.cell} context={cell.getContext()} />
                      {/if}
                    </Table.Cell>
                  {/each}
                </Table.Row>
              {:else}
                <Table.Row>
                  <Table.Cell colspan={columns.length} class="h-24 text-center">No results.</Table.Cell>
                </Table.Row>
              {/each}
            </Table.Body>
          </Table.Root>
        </div>
      </div>
    </Card.Content>
    <Card.Footer>
      <div class="text-muted-foreground text-xs">
        Showing <strong>1-{$vmListStore.length}</strong> of <strong>{$vmListStore.length}</strong> bookings
      </div>
    </Card.Footer>
  </Card.Root>
{/if}
