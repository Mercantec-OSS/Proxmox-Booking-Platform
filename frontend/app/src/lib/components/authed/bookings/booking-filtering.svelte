<script>
  import * as Select from '$lib/components/ui/select';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
  import { Button } from '$lib/components/ui/button';
  import * as Tabs from '$lib/components/ui/tabs/index.js';
  import * as Command from '$lib/components/ui/command';
  import * as Popover from '$lib/components/ui/popover';
  import { Separator } from '$lib/components/ui/separator';
  import { Badge } from '$lib/components/ui/badge';
  import { GripHorizontal, AlignJustify, PlusCircle, ChevronDown, Check } from 'lucide-svelte';
  import { clusterListStore, vmListStore, selectedBookingTypes, selectedBookingStatus, selectedBookingPreview } from '$lib/utils/store';

  let typesOpen = false;
  let selectedOrder = { value: 'latest', label: 'Latest' };
  let options = [
    { title: 'Clusters', selected: true, count: $clusterListStore.length },
    { title: 'Virtual Machines', selected: true, count: $vmListStore.length }
  ];
  let statusOptions = {
    confirmed: true,
    other: true
  };

  // hack: rerender the list of bookings when the status type changes
  $: selectedBookingTypes.set(options);
  $: statusOptions, vmListStore.set($vmListStore);

  // Handle which booking types are selected. Can be 'Clusters' or 'Virtual Machines'
  function handleSelect(value) {
    options = options.map((option) => {
      if (option.title === value) {
        return { ...option, selected: !option.selected };
      }
      return option;
    });

    // hack: rerender the list of bookings when the type changes
    vmListStore.set($vmListStore);
  }

  // Sort a list by latest or oldest
  function sortList(list, order) {
    return list.sort((a, b) => {
      const aDate = new Date(a.createdAt);
      const bDate = new Date(b.createdAt);

      if (order === 'latest') {
        // Sort by newest date first
        return bDate - aDate;
      } else {
        // Sort by oldest date first
        return aDate - bDate;
      }
    });
  }

  // Update the order of the list. Can be 'latest' or 'oldest'
  function updateOrder(list) {
    const order = selectedOrder.value;

    // Sort the lists
    if (list === 'vm') {
      vmListStore.set(sortList([...$vmListStore], order));
    } else if (list === 'cluster') {
      clusterListStore.set(sortList([...$clusterListStore], order));
    } else {
      // Sort and update both stores
      vmListStore.set(sortList([...$vmListStore], order));
      clusterListStore.set(sortList([...$clusterListStore], order));
    }
  }

  // Update the preview type. Can be 'Card' or 'Table'
  function updatePreviewType(type) {
    selectedBookingPreview.set(type);
  }

  $: selectedBookingStatus.set(statusOptions);

  $: $vmListStore, updateOrder('vm');
  $: $clusterListStore, updateOrder('cluster');

  // Default to table view if there are more than 5 bookings
  $: if ($clusterListStore.length + $vmListStore.length > 5) {
    selectedBookingPreview.set('table');
  }
</script>

<div class="flex flex-wrap justify-between gap-x-2 w-full">
  <div>
    <!-- Filter by VM, Cluster or both -->
    <Popover.Root bind:open={typesOpen}>
      <Popover.Trigger asChild let:builder>
        <Button builders={[builder]} variant="outline" class="border-dashed">
          <PlusCircle class="mr-2 h-4 w-4" />
          Booking Types

          {#if options.filter((option) => option.selected).length > 0}
            <Separator orientation="vertical" class="mx-2 h-4" />
            <Badge variant="secondary" class="rounded-sm px-1 font-normal lg:hidden">
              {options.filter((option) => option.selected).length}
            </Badge>
            <div class="hidden space-x-1 lg:flex">
              {#if options.filter((option) => option.selected).length > 2}
                <Badge variant="secondary" class="rounded-sm px-1 font-normal">
                  {options.filter((option) => option.selected).length} Selected
                </Badge>
              {:else}
                {#each options as option}
                  {#if option.selected}
                    <Badge variant="secondary" class="rounded-sm px-1 font-normal">
                      {option.title}
                    </Badge>
                  {/if}
                {/each}
              {/if}
            </div>
          {/if}
        </Button>
      </Popover.Trigger>
      <Popover.Content class="w-[200px] p-0" align="start" side="bottom">
        <Command.Root>
          <Command.Input placeholder="Booking Types" />
          <Command.List>
            <Command.Empty>No results found.</Command.Empty>
            <Command.Group>
              {#each options as option}
                <Command.Item
                  value={option.title}
                  onSelect={(currentValue) => {
                    handleSelect(currentValue);
                  }}
                >
                  {#if option.selected}
                    <Check class="mr-2 h-4 w-4" />
                  {:else}
                    <div class="mr-2 h-4 w-4"></div>
                  {/if}
                  <span>
                    {option.title}
                  </span>
                  <span class="ml-auto flex h-4 w-4 items-center justify-center font-mono text-xs">
                    {option.count}
                  </span>
                </Command.Item>
              {/each}
            </Command.Group>
          </Command.List>
        </Command.Root>
      </Popover.Content>
    </Popover.Root>
  </div>

  <div class="flex flex-wrap gap-x-2">
    <!-- Sort by new or old -->
    <Select.Root bind:selected={selectedOrder}>
      <Select.Trigger class="w-[6.4rem]">
        <Select.Value placeholder="Sorting" />
      </Select.Trigger>
      <Select.Content>
        <Select.Item
          on:click={() => {
            updateOrder();
          }}
          value="latest">Latest</Select.Item
        >
        <Select.Item
          on:click={() => {
            updateOrder();
          }}
          value="oldest">Oldest</Select.Item
        >
      </Select.Content>
    </Select.Root>

    <!-- Filter by booking status, accepted or pending -->
    <DropdownMenu.Root>
      <DropdownMenu.Trigger asChild let:builder>
        <Button variant="outline" builders={[builder]}>Status <ChevronDown class="h-4 w-4 opacity-50" /></Button>
      </DropdownMenu.Trigger>
      <DropdownMenu.Content>
        <DropdownMenu.CheckboxItem bind:checked={statusOptions.confirmed}>Confirmed</DropdownMenu.CheckboxItem>
        <DropdownMenu.CheckboxItem bind:checked={statusOptions.other}>Other</DropdownMenu.CheckboxItem>
      </DropdownMenu.Content>
    </DropdownMenu.Root>

    <!-- Toggle between card booking previews and a table -->
    <Tabs.Root bind:value={$selectedBookingPreview} class="shrink-0">
      <Tabs.List class="grid w-full grid-cols-2">
        <Tabs.Trigger on:click={() => updatePreviewType('card')} value="card"><GripHorizontal class="size-5 shrink-0" /></Tabs.Trigger>
        <Tabs.Trigger on:click={() => updatePreviewType('table')} value="table"><AlignJustify class="size-5 shrink-0" /></Tabs.Trigger>
      </Tabs.List>
    </Tabs.Root>
  </div>
</div>
