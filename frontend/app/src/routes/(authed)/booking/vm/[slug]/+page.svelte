<script>
  import VMActionsDropdown from '$lib/components/authed/bookings/vm/vm-actions-dropdown.svelte';
  import { selectedBookingStore } from '$lib/utils/store';
  import { toast } from 'svelte-sonner';
  import { goto } from '$app/navigation';
  import { afterNavigate } from '$app/navigation';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import { Badge } from '$lib/components/ui/badge';
  import { Label } from '$lib/components/ui/label';
  import { ChevronLeft } from 'lucide-svelte';

  export let data;
  $: selectedBookingStore.set(data.vmData);

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
  {#if data.vmData && !data.errorMessage}
    <div class="mx-auto grid max-w-[59rem] flex-1 auto-rows-max gap-4">
      <div class="flex items-center gap-4">
        <Button href="/" variant="outline" size="icon" class="h-7 w-7">
          <ChevronLeft class="h-4 w-4" />
          <span class="sr-only">Back</span>
        </Button>
        <h1 class="flex-1 shrink-0 whitespace-nowrap text-xl font-semibold tracking-tight sm:grow-0">Booking details</h1>
        <Badge variant="outline" class="ml-auto sm:ml-0 text-indigo-500">Virtual machine</Badge>
        <div class="hidden items-center gap-2 md:ml-auto md:flex">
          <VMActionsDropdown />
        </div>
      </div>
      <div class="grid gap-4 md:grid-cols-[1fr_250px] lg:grid-cols-3 lg:gap-8">
        <div class="grid auto-rows-max items-start gap-4 lg:col-span-2 lg:gap-8">
          <Card.Root>
            <Card.Header>
              <Card.Title>Details</Card.Title>
              <Card.Description>Basic information about the booking.</Card.Description>
            </Card.Header>
            <Card.Content>
              <div class="grid gap-6">
                <div class="grid gap-3">
                  <Label for="name">Name</Label>
                </div>
                <div class="grid gap-3">
                  <Label for="description">Note</Label>
                </div>
              </div>
            </Card.Content>
          </Card.Root>
          <Card.Root>
            <Card.Header>
              <Card.Title>Stock</Card.Title>
              <Card.Description>Lipsum dolor sit amet, consectetur adipiscing elit</Card.Description>
            </Card.Header>
            <Card.Content>
              <p>table</p>
            </Card.Content>
            <Card.Footer class="justify-center border-t p-4">
              <Button size="sm" variant="ghost" class="gap-1">Add Variant</Button>
            </Card.Footer>
          </Card.Root>
          <Card.Root>
            <Card.Header>
              <Card.Title>Product Category</Card.Title>
            </Card.Header>
            <Card.Content>
              <div class="grid gap-6 sm:grid-cols-3">
                <div class="grid gap-3">
                  <Label for="category">Category</Label>
                </div>
                <div class="grid gap-3">
                  <Label for="subcategory">Subcategory (optional)</Label>
                </div>
              </div>
            </Card.Content>
          </Card.Root>
        </div>
        <div class="grid auto-rows-max items-start gap-4 lg:gap-8">
          <Card.Root>
            <Card.Header>
              <Card.Title>Product Status</Card.Title>
            </Card.Header>
            <Card.Content>
              <div class="grid gap-6">
                <div class="grid gap-3">
                  <Label for="status">Status</Label>
                </div>
              </div>
            </Card.Content>
          </Card.Root>
          <Card.Root class="overflow-hidden">
            <Card.Header>
              <Card.Title>Product Images</Card.Title>
              <Card.Description>Lipsum dolor sit amet, consectetur adipiscing elit</Card.Description>
            </Card.Header>
            <Card.Content>
              <div class="grid gap-2">
                <div class="grid grid-cols-3 gap-2">
                  <button> </button>
                  <button> </button>
                  <button class="flex aspect-square w-full items-center justify-center rounded-md border border-dashed">
                    <span class="sr-only">Upload</span>
                  </button>
                </div>
              </div>
            </Card.Content>
          </Card.Root>
          <Card.Root>
            <Card.Header>
              <Card.Title>Archive Product</Card.Title>
              <Card.Description>Lipsum dolor sit amet, consectetur adipiscing elit.</Card.Description>
            </Card.Header>
            <Card.Content>
              <div></div>
              <Button size="sm" variant="secondary">Archive Product</Button>
            </Card.Content>
          </Card.Root>
        </div>
      </div>
      <div class="flex items-center justify-center gap-2 md:hidden">
        <Button variant="outline" size="sm">Discard</Button>
        <Button size="sm">Save Product</Button>
      </div>
    </div>
  {/if}
</main>
