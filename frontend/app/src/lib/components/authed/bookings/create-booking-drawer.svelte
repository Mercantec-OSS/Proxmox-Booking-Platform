<script>
  import { tick } from 'svelte';
  import { CalendarDate, today, getLocalTimeZone } from '@internationalized/date';
  import { clusterListStore, vmListStore, userStore } from '$lib/utils/store';
  import { toast } from 'svelte-sonner';
  import { LoaderCircle, Ellipsis, CalendarIcon, Check, ChevronsUpDown } from 'lucide-svelte';
  import { cn } from '$lib/utils/utils.js';
  import { clusterService } from '$lib/services/cluster-service';
  import { vmService } from '$lib/services/vm-service';
  import { userService } from '$lib/services/user-service';
  import { ScrollArea } from '$lib/components/ui/scroll-area/index.js';
  import { Calendar } from '$lib/components/ui/calendar';
  import * as Popover from '$lib/components/ui/popover';
  import { Badge } from '$lib/components/ui/badge';
  import * as Tabs from '$lib/components/ui/tabs/index.js';
  import * as Select from '$lib/components/ui/select/index.js';
  import { Textarea } from '$lib/components/ui/textarea/index.js';
  import * as Drawer from '$lib/components/ui/drawer';
  import { Label } from '$lib/components/ui/label';
  import { Input } from '$lib/components/ui/input';
  import * as Command from '$lib/components/ui/command/index.js';
  import { Button } from '$lib/components/ui/button/index.js';

  $: userAuthed = $userStore.role === 'Admin' || $userStore.role === 'Teacher';

  let open = false;
  let isLoading = false;
  let clustersAvailable = '? clusters available';
  let vmsAvailable = '? VMs available';
  let vmTemplates = [];
  let vmBookingInput = { type: null, ownerId: null, assignedId: null, message: null, expiringAt: null, machineType: 0 };
  let clusterBookingInput = { amountStudents: null, amountDays: null };
  let listOfUsers = [];
  $: clusterBookingInfo = {
    numberOfVcenters: Math.floor(clusterBookingInput.amountStudents / 3),
    numberOfHosts: Math.floor(clusterBookingInput.amountStudents / 3) * 3
  };

  /* Calendar date picker to select booking expire date */
  let calendarDatePicked;
  let vmCalendarDatePicked;
  $: vmCalendarDateFormated = new Date(vmCalendarDatePicked).toLocaleDateString(undefined, { dateStyle: 'long' });
  $: calendarDateFormated = new Date(calendarDatePicked).toLocaleDateString(undefined, { dateStyle: 'long' });
  $: clusterBookingInput.amountDays = calendarDatePicked ? Math.ceil((new Date(calendarDatePicked) - new Date()) / (1000 * 60 * 60 * 24)) : null;

  // Vars used for search select component
  let searchSelectOpen = false;
  let searchSelectvalue = null;
  $: selectedSearchSelectvalue = (() => {
    const selectedUser = listOfUsers.find((user) => user.id === searchSelectvalue);
    return selectedUser ? `${selectedUser.name} ${selectedUser.surname}`.trim() : `Select a ${userAuthed ? 'student' : 'teacher'}`;
  })();

  // Handle focus on the search select component
  function closeAndFocusTrigger(triggerId) {
    searchSelectOpen = false;
    tick().then(() => {
      document.getElementById(triggerId)?.focus();
    });
  }

  /* Create cluster booking */
  async function handleCreateClusterBoooking() {
    if (clusterBookingInput.amountDays < 1) {
      toast.error(`Please select an expire date`);
      return;
    }

    try {
      isLoading = true;
      await clusterService.createClusterBooking(clusterBookingInput);
      clusterListStore.set(await clusterService.getClusterBookingsFrontend());
      clusterBookingInput = { amountStudents: null, amountDays: null };
      calendarDatePicked = null;
      open = false;

      toast.success(`Cluster booking created`);
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }

  /* Create VM booking */
  async function handleCreateVMBoooking() {
    // Set booking owner to the creator if not set
    if (!vmBookingInput.ownerId) {
      vmBookingInput.ownerId = $userStore.id;
    }

    // Assign booking to the teacher who made it if not set
    if (userAuthed && !vmBookingInput.assignedId) {
      vmBookingInput.assignedId = $userStore.id;
    }

    // Validate submitted booking
    if (!vmBookingInput.type) {
      toast.error(`Please select a VM template`);
      return;
    } else if (!userAuthed && !vmBookingInput.assignedId) {
      toast.error(`Please assign the booking to your teacher`);
      return;
    } else if (!vmCalendarDatePicked) {
      toast.error(`Please select an expire date`);
      return;
    } else if (!vmBookingInput.message) {
      toast.error(`Please add a comment describing the purpose of the booking`);
      return;
    }

    try {
      isLoading = true;

      vmBookingInput.expiringAt = new Date(vmCalendarDatePicked).toISOString();
      await vmService.createVMBooking(vmBookingInput);
      vmListStore.set(await vmService.getVMBookingsFrontend());
      vmBookingInput = { type: null, ownerId: null, assignedTo: null, expiringAt: null };
      vmCalendarDatePicked = null;
      open = false;

      toast.success(`VM booking created`);
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }

  // Fetch info about the bookings such as clusters and VMs available, VM templates etc
  async function fetchBookingInfo() {
    try {
      // Fetch amount of clusters available (if user is authed)
      if (userAuthed) {
        clustersAvailable = 'loading';
        const clusterCount = await clusterService.getClustersAvailableCount();
        clustersAvailable = clusterCount.length > 0 ? `${clusterCount.length} cluster available` : 'no clusters available';
      }

      // Fetch amount of VMs available for booking
      if (userAuthed) {
        vmsAvailable = 'loading';
        const vmCount = await vmService.getVMsAvailableCount();
        vmsAvailable = vmCount > 0 ? `${vmCount.length} VMs available` : 'no VMs available';
      }

      // Fetch VM templates
      vmTemplates = await vmService.getVMTemplates();

      // Fetch list of users
      listOfUsers = await userService.getAllUsers();
    } catch (error) {
      toast.error(error.message);
    }
  }

  // Reactive statement that gets amount of available clusters and VMs when the drawer opens
  $: if (open) fetchBookingInfo();
</script>

<Drawer.Root bind:open>
  <Drawer.Trigger asChild let:builder>
    <Button builders={[builder]} variant="outline" class="font-bold">Create booking</Button>
  </Drawer.Trigger>
  <Drawer.Content class="flex w-full max-h-[80vh]">
    <div class="w-full h-full overflow-y-auto custom-scrollbar">
      <Tabs.Root value={userAuthed ? 'Cluster' : 'singleVM'} class="flex flex-col items-center w-full my-7">
        <Tabs.List class="grid w-10/12 md:w-1/3 grid-cols-2">
          <Tabs.Trigger value="Cluster" disabled={!userAuthed}>Cluster</Tabs.Trigger>
          <Tabs.Trigger value="singleVM">Virtual machine</Tabs.Trigger>
        </Tabs.List>
        <!-- Cluster booking tab -->
        {#if userAuthed}
          <Tabs.Content value="Cluster">
            <div class="flex flex-col md:flex-row mx-auto py-2">
              <div class="max-w-md">
                <Drawer.Header>
                  <Drawer.Title class="text-center">Create cluster booking</Drawer.Title>
                  <Drawer.Description class="text-center">Create a cluster booking here. Click create booking when done.</Drawer.Description>
                </Drawer.Header>
                <div class="px-4 pb-4">
                  <form on:submit|preventDefault={handleCreateClusterBoooking} class="grid items-start gap-4">
                    <div class="grid gap-2">
                      <div class="flex justify-between items-center">
                        <Label for="studentCount">Amount of students</Label>
                        <div>
                          {#if clustersAvailable === 'loading'}
                            <Badge>Loading<Ellipsis class="h-4 w-4 ml-2 animate-pulse" /></Badge>
                          {:else}
                            <Badge class="w-auto">{clustersAvailable}</Badge>
                          {/if}
                        </div>
                      </div>
                      <Input type="number" id="studentCount" placeholder="0" min="3" max="100" required bind:value={clusterBookingInput.amountStudents} />
                    </div>
                    <div class="grid gap-2">
                      <Label for="dayCount">Booking expire date</Label>
                      <!-- Date picker -->
                      <Popover.Root>
                        <Popover.Trigger asChild let:builder>
                          <Button variant="outline" class={cn('justify-start text-left font-normal', !calendarDatePicked && 'text-muted-foreground')} builders={[builder]}>
                            <CalendarIcon class="mr-2 h-4 w-4" />
                            {calendarDatePicked ? calendarDateFormated : 'Pick a date'}
                          </Button>
                        </Popover.Trigger>
                        <Popover.Content class="w-auto p-0">
                          <Calendar
                            bind:value={calendarDatePicked}
                            initialFocus
                            calendarLabel="Booking expire date"
                            minValue={new CalendarDate(today(getLocalTimeZone()).year, today(getLocalTimeZone()).month, today(getLocalTimeZone()).day + 1)}
                            maxValue={new CalendarDate(
                              today(getLocalTimeZone()).year + (today(getLocalTimeZone()).month + 6 > 12 ? 1 : 0),
                              (today(getLocalTimeZone()).month + 6) % 12 || 12,
                              today(getLocalTimeZone()).day
                            )}
                          />
                        </Popover.Content>
                      </Popover.Root>
                    </div>
                    <Button type="submit" disabled={isLoading}
                      >{#if isLoading}
                        <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
                      {/if}
                      Create booking</Button
                    >
                  </form>
                </div>
              </div>

              <!-- Booking information -->
              <div class="flex flex-col min-w-80 bg-muted shadow-sm rounded-xl items-center justify-center mx-4 py-4 mb-4 gap-y-1">
                <p class="font-medium">vCenters: {clusterBookingInfo.numberOfVcenters}</p>
                <p class="font-medium">ESXi hosts: {clusterBookingInfo.numberOfHosts}</p>
              </div>
            </div>
          </Tabs.Content>
        {/if}
        <!-- Virtual machine booking tab -->
        <Tabs.Content value="singleVM">
          <div class="flex flex-col md:flex-row mx-auto py-2">
            <div class="max-w-md">
              <Drawer.Header>
                <Drawer.Title class="text-center">Create virtual machine</Drawer.Title>
                <Drawer.Description class="text-center">Create a virtual machine booking here. Click create booking when done.</Drawer.Description>
              </Drawer.Header>
              <div class="px-4 pb-4">
                <form on:submit|preventDefault={handleCreateVMBoooking} class="grid items-start gap-4">
                  <div class="grid gap-2">
                    <div class="flex justify-between items-center">
                      <Label for="studentCount">Virtual machine templates</Label>
                      <div>
                        {#if vmsAvailable === 'loading'}
                          <Badge>Loading<Ellipsis class="h-4 w-4 ml-2 animate-pulse" /></Badge>
                        {:else}
                          <Badge class="w-auto">{vmsAvailable}</Badge>
                        {/if}
                      </div>
                    </div>

                    <!-- Select VM template component -->
                    <Select.Root
                      onSelectedChange={(value) => {
                        vmBookingInput.type = value.value;
                      }}
                    >
                      <Select.Trigger>
                        <Select.Value placeholder="Select a template" />
                      </Select.Trigger>
                      <Select.Content>
                        <Select.Group>
                          {#each vmTemplates as vmTemplate}
                            <Select.Item value={vmTemplate} label={vmTemplate}>{vmTemplate}</Select.Item>
                          {/each}
                        </Select.Group>
                      </Select.Content>
                      <Select.Input name="template" />
                    </Select.Root>
                  </div>

                  <!-- Select student component -->
                  {#if userAuthed}
                    <div class="grid gap-2">
                      <div class="flex justify-between items-center">
                        <Label for="studentCount">Assign to student</Label>
                      </div>

                      <Popover.Root bind:open={searchSelectOpen} let:ids>
                        <Popover.Trigger asChild let:builder>
                          <Button builders={[builder]} variant="outline" role="combobox" aria-expanded={open} class="justify-between">
                            {selectedSearchSelectvalue}
                            <ChevronsUpDown class="ml-2 h-4 w-4 shrink-0 opacity-50" />
                          </Button>
                        </Popover.Trigger>
                        <Popover.Content class="p-0">
                          <Command.Root>
                            <Command.Input placeholder="Search {userAuthed ? 'students' : 'teachers'}" />
                            <Command.Empty>No users found.</Command.Empty>
                            <Command.Group>
                              {#each listOfUsers as user}
                                {#if user.role === 'Student'}
                                  <Command.Item
                                    value={`${user.name} ${user.surname} ${user.id}`}
                                    onSelect={(value) => {
                                      const selectedId = parseInt(value.split(' ').pop(), 10);

                                      // Add logic to deselect the selected user
                                      if (selectedId === searchSelectvalue) {
                                        searchSelectvalue = null;
                                        vmBookingInput.ownerId = null;
                                        closeAndFocusTrigger(ids.trigger);

                                        return;
                                      }

                                      vmBookingInput.ownerId = selectedId;
                                      searchSelectvalue = selectedId;
                                      closeAndFocusTrigger(ids.trigger);
                                    }}
                                  >
                                    <Check class={cn('mr-2 h-4 w-4', searchSelectvalue !== `${user.name} ${user.surname}` && 'text-transparent')} />
                                    {user.name}
                                    {user.surname}
                                  </Command.Item>
                                {/if}
                              {/each}
                            </Command.Group>
                          </Command.Root>
                        </Popover.Content>
                      </Popover.Root>
                    </div>
                  {:else}
                    <!-- Select teacher component -->
                    <div class="grid gap-2">
                      <div class="flex justify-between items-center">
                        <Label for="studentCount">Assign to teacher</Label>
                      </div>

                      <Popover.Root bind:open={searchSelectOpen} let:ids>
                        <Popover.Trigger asChild let:builder>
                          <Button builders={[builder]} variant="outline" role="combobox" aria-expanded={open} class="justify-between">
                            {selectedSearchSelectvalue}
                            <ChevronsUpDown class="ml-2 h-4 w-4 shrink-0 opacity-50" />
                          </Button>
                        </Popover.Trigger>
                        <Popover.Content class="p-0">
                          <Command.Root>
                            <Command.Input placeholder="Search {userAuthed ? 'students' : 'teachers'}" />
                            <Command.Empty>No users found.</Command.Empty>
                            <Command.Group>
                              {#each listOfUsers as user}
                                {#if user.role === 'Teacher'}
                                  <Command.Item
                                    value={`${user.name} ${user.surname} ${user.id}`}
                                    onSelect={(value) => {
                                      const selectedId = parseInt(value.split(' ').pop(), 10);

                                      // Add logic to deselect the selected user
                                      if (selectedId === searchSelectvalue) {
                                        searchSelectvalue = null;
                                        vmBookingInput.assignedId = null;
                                        closeAndFocusTrigger(ids.trigger);

                                        return;
                                      }

                                      vmBookingInput.assignedId = selectedId;
                                      searchSelectvalue = selectedId;
                                      closeAndFocusTrigger(ids.trigger);
                                    }}
                                  >
                                    <Check class={cn('mr-2 h-4 w-4', searchSelectvalue !== `${user.name} ${user.surname}` && 'text-transparent')} />
                                    {user.name}
                                    {user.surname}
                                  </Command.Item>
                                {/if}
                              {/each}
                            </Command.Group>
                          </Command.Root>
                        </Popover.Content>
                      </Popover.Root>
                    </div>
                  {/if}

                  <div class="grid gap-2">
                    <Label for="dayCount">Booking expire date</Label>
                    <!-- Date picker -->
                    <Popover.Root>
                      <Popover.Trigger asChild let:builder>
                        <Button variant="outline" class={cn('justify-start text-left font-normal', !vmCalendarDatePicked && 'text-muted-foreground')} builders={[builder]}>
                          <CalendarIcon class="mr-2 h-4 w-4" />
                          {vmCalendarDatePicked ? vmCalendarDateFormated : 'Pick a date'}
                        </Button>
                      </Popover.Trigger>
                      <Popover.Content class="w-auto p-0">
                        <Calendar
                          bind:value={vmCalendarDatePicked}
                          initialFocus
                          calendarLabel="Booking expire date"
                          minValue={new CalendarDate(today(getLocalTimeZone()).year, today(getLocalTimeZone()).month, today(getLocalTimeZone()).day + 1)}
                          maxValue={new CalendarDate(
                            today(getLocalTimeZone()).year + (today(getLocalTimeZone()).month + 6 > 12 ? 1 : 0),
                            (today(getLocalTimeZone()).month + 6) % 12 || 12,
                            today(getLocalTimeZone()).day
                          )}
                        />
                      </Popover.Content>
                    </Popover.Root>
                  </div>

                  <div class="grid w-full gap-1.5">
                    <Label for="comment">Comment about booking</Label>
                    <Textarea class="max-h-20" placeholder="Describe the purpose of this VM booking" id="comment" bind:value={vmBookingInput.message} />
                  </div>

                  <Button type="submit" disabled={isLoading}
                    >{#if isLoading}
                      <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
                    {/if}
                    Create booking</Button
                  >
                </form>
              </div>
            </div>
          </div>
        </Tabs.Content>
      </Tabs.Root>
    </div>
  </Drawer.Content>
</Drawer.Root>
