<script>
  import { userStore } from '$lib/utils/store';
  import { clusterService } from '$lib/services/cluster-service';
  import { vmService } from '$lib/services/vm-service';
  import { Badge } from '$lib/components/ui/badge';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import * as Table from '$lib/components/ui/table';
  import * as Tabs from '$lib/components/ui/tabs';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
  import { ArrowUpRight, CirclePlus, ChevronLeft } from 'lucide-svelte';
  import { tick } from 'svelte';
  import { CalendarDate, today, getLocalTimeZone } from '@internationalized/date';
  import { toast } from 'svelte-sonner';
  import { LoaderCircle, Ellipsis, CalendarIcon, Check, ChevronsUpDown } from 'lucide-svelte';
  import { cn } from '$lib/utils/utils.js';
  import { userService } from '$lib/services/user-service';
  import { ScrollArea } from '$lib/components/ui/scroll-area/index.js';
  import { Calendar } from '$lib/components/ui/calendar';
  import * as Popover from '$lib/components/ui/popover';
  import * as Select from '$lib/components/ui/select/index.js';
  import { Textarea } from '$lib/components/ui/textarea/index.js';
  import * as Drawer from '$lib/components/ui/drawer';
  import { Label } from '$lib/components/ui/label';
  import { Input } from '$lib/components/ui/input';
  import * as Command from '$lib/components/ui/command/index.js';
  import { goto } from '$app/navigation';

  export let data;
  /* Update stores (global vars) to the data returned from the fetch requests in SSR */
  userStore.set(data.userInfo);

  let userAuthed = data.userInfo.role !== 'Student';
  let isLoading = false;
  let clustersAvailable = '? clusters available';
  let vmsAvailable = '? VMs available';
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
  $: clusterBookingInput.amountDays = calendarDatePicked ? Math.ceil((new Date(calendarDatePicked) - new Date()) / (1000 * 60 * 60 * 24)) : null;

  /* Create cluster booking */
  async function handleCreateClusterBoooking() {
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
                <div></div>
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
