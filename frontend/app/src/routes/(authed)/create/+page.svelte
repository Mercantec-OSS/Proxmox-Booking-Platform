<script>
  import { clusterService } from '$lib/services/cluster-service';
  import { vmService } from '$lib/services/vm-service';
  import { userStore } from '$lib/utils/store';
  import { goto } from '$app/navigation';
  import { CalendarDate, today, getLocalTimeZone } from '@internationalized/date';
  import { ChevronLeft, ChevronDown, LoaderCircle, CalendarIcon } from 'lucide-svelte';
  import { toast } from 'svelte-sonner';
  import { cn } from '$lib/utils/utils.js';
  import { Badge } from '$lib/components/ui/badge';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import * as Tabs from '$lib/components/ui/tabs';
  import { Calendar } from '$lib/components/ui/calendar';
  import * as Popover from '$lib/components/ui/popover';
  import * as Select from '$lib/components/ui/select/index.js';
  import { Textarea } from '$lib/components/ui/textarea/index.js';
  import { Label } from '$lib/components/ui/label';
  import { Input } from '$lib/components/ui/input';
  import * as Command from '$lib/components/ui/command/index.js';

  export let data;

  /* Update stores (global vars) to the data returned from the fetch requests in SSR */
  userStore.set(data.userInfo);

  let userAuthed = data.userInfo.role !== 'Student';
  let isLoading = false;
  let vmBookingInput = { type: null, ownerId: null, assignedId: null, message: null, expiringAt: null };
  let clusterBookingInput = { amountStudents: null, amountDays: null };
  $: clusterBookingInfo = {
    numberOfVcenters: Math.floor(clusterBookingInput.amountStudents / 3),
    numberOfHosts: Math.floor(clusterBookingInput.amountStudents / 3) * 3
  };

  /* Calendar date picker to select booking expire date */
  let calendarDatePicked;
  let vmCalendarDatePicked;
  $: vmCalendarDateFormated = new Date(vmCalendarDatePicked).toLocaleDateString(undefined, { dateStyle: 'long' });
  $: calendarDateFormated = new Date(calendarDatePicked).toLocaleDateString(undefined, { dateStyle: 'long' });

  /* For select search components */
  let selectedStudent = null;
  let selectedTeacher = null;
  let selectStudentOpen = false;
  let selectTeacherOpen = false;

  $: console.log(vmBookingInput);

  // Assign student as booking owner on select
  function handleStudentSelect(value) {
    const [name, surname, id] = value.split(' ');
    const parsedId = parseInt(id, 10);

    if (parsedId === vmBookingInput.ownerId) {
      selectedStudent = null;
      vmBookingInput.ownerId = null;
    } else {
      vmBookingInput.ownerId = parsedId;
      selectedStudent = `${name} ${surname}`;
    }

    selectStudentOpen = false;
  }

  // Assign teacher as the assigned teacher for the booking
  function handleTeacherSelect(value) {
    const [name, surname, id] = value.split(' ');
    const parsedId = parseInt(id, 10);

    if (parsedId === vmBookingInput.assignedId) {
      selectedTeacher = null;
      vmBookingInput.assignedId = null;
    } else {
      vmBookingInput.assignedId = parsedId;
      selectedTeacher = `${name} ${surname}`;
    }

    selectTeacherOpen = false;
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

      toast.success(`VM booking created`);
      goto('/');
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }

  /* Create cluster booking */
  async function handleCreateClusterBoooking() {
    clusterBookingInput.amountDays = Math.ceil((new Date(calendarDatePicked) - new Date()) / (1000 * 60 * 60 * 24));

    if (clusterBookingInput.amountDays < 1) {
      toast.error(`Please select an expire date`);
      return;
    }

    try {
      isLoading = true;

      await clusterService.createClusterBooking(clusterBookingInput);

      toast.success(`Cluster booking created`);
      goto('/');
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }
</script>

<main class="grid flex-1 items-start gap-4 p-4 sm:px-6 md:gap-8">
  <div class="mx-auto grid flex-1 auto-rows-max gap-4">
    <div class="flex flex-wrap items-center gap-4">
      <Button href="/" variant="outline" size="icon" class="size-7">
        <ChevronLeft class="size-4" />
        <span class="sr-only">Back</span>
      </Button>
      <h1 class="flex-1 shrink-0 whitespace-nowrap text-xl font-semibold tracking-tight sm:grow-0">Create booking</h1>
    </div>
    <div class="grid gap-4 lg:gap-8">
      <div class="grid auto-rows-max items-start gap-4 lg:gap-8">
        <Card.Root>
          <Card.Header>
            <Card.Title>Book a virtual machine or cluster</Card.Title>
            <Card.Description>Cluster bookings are currently limited to teachers.</Card.Description>
          </Card.Header>
          <Card.Content>
            <Tabs.Root>
              <div class="flex justify-between items-center">
                <Tabs.List class="grid w-full md:w-96 grid-cols-2">
                  <Tabs.Trigger value="vm">Virtual machine</Tabs.Trigger>
                  <Tabs.Trigger disabled={!userAuthed} value="cluster">Cluster</Tabs.Trigger>
                </Tabs.List>
              </div>
              <!-- Virtual machine booking -->
              <Tabs.Content value="vm">
                <form on:submit|preventDefault={handleCreateVMBoooking} class="grid items-center py-4 gap-4">
                  <div class="grid gap-2">
                    <div class="flex justify-between items-center">
                      <Label for="studentCount">Virtual machine templates</Label>
                      <div>
                        <Badge class="w-auto">? VMs available</Badge>
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
                          {#each data.vmTemplates as vmTemplate}
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
                      <Popover.Root bind:open={selectStudentOpen}>
                        <Popover.Trigger asChild let:builder>
                          <Button builders={[builder]} variant="outline" class="justify-between px-3 {selectedStudent ?? 'text-muted-foreground'}">
                            {selectedStudent ?? 'Select a student'}
                            <ChevronDown class="h-4 w-4" />
                          </Button>
                        </Popover.Trigger>
                        <Popover.Content class="p-0" align="end">
                          <Command.Root>
                            <Command.Input placeholder="Select a student..." />
                            <Command.List>
                              <Command.Empty>No students found.</Command.Empty>
                              <Command.Group>
                                {#each data.listOfUsers as user (user.id)}
                                  {#if user.role === 'Student'}
                                    <Command.Item value={`${user.name} ${user.surname} ${user.id}`} onSelect={handleStudentSelect} class="items-start px-4 py-2">
                                      <p>{user.name} {user.surname}</p>
                                    </Command.Item>
                                  {/if}
                                {/each}
                              </Command.Group>
                            </Command.List>
                          </Command.Root>
                        </Popover.Content>
                      </Popover.Root>
                    </div>
                  {:else}
                    <!-- Select teacher component -->
                    <div class="grid gap-2">
                      <div class="flex justify-between items-center">
                        <Label for="teacherCount">Assign to teacher</Label>
                      </div>
                      <Popover.Root bind:open={selectTeacherOpen}>
                        <Popover.Trigger asChild let:builder>
                          <Button builders={[builder]} variant="outline" class="justify-between px-3 {selectedTeacher ?? 'text-muted-foreground'}">
                            {selectedTeacher ?? 'Select a teacher'}
                            <ChevronDown class="h-4 w-4" />
                          </Button>
                        </Popover.Trigger>
                        <Popover.Content class="p-0" align="end">
                          <Command.Root>
                            <Command.Input placeholder="Select a teacher..." />
                            <Command.List>
                              <Command.Empty>No teachers found.</Command.Empty>
                              <Command.Group>
                                {#each data.listOfUsers as user (user.id)}
                                  {#if user.role === 'Teacher'}
                                    <Command.Item value={`${user.name} ${user.surname} ${user.id}`} onSelect={handleTeacherSelect} class="items-start px-4 py-2">
                                      <p>{user.name} {user.surname}</p>
                                    </Command.Item>
                                  {/if}
                                {/each}
                              </Command.Group>
                            </Command.List>
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
              </Tabs.Content>
              <!-- Cluster booking -->
              <Tabs.Content value="cluster">
                <div class="flex flex-col md:flex-row py-4 gap-x-6">
                  <form on:submit|preventDefault={handleCreateClusterBoooking} class="grid items-start gap-4">
                    <div class="grid gap-2">
                      <div class="flex justify-between items-center gap-x-4">
                        <Label for="studentCount">Amount of students</Label>
                        <div>
                          <Badge class="w-auto">{data.clustersAvailable === null ? '?' : data.clustersAvailable} clusters available</Badge>
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
                  <div class="flex flex-col min-w-80 bg-muted shadow-sm rounded-xl items-center justify-center sm:py-4">
                    <p class="font-medium">vCenters: {clusterBookingInfo.numberOfVcenters}</p>
                    <p class="font-medium">ESXi hosts: {clusterBookingInfo.numberOfHosts}</p>
                  </div>
                </div>
              </Tabs.Content>
            </Tabs.Root>
          </Card.Content>
        </Card.Root>
      </div>
    </div>
  </div>
</main>
