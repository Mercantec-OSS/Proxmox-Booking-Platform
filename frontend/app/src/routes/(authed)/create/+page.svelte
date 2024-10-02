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
  import { ArrowUpRight, CirclePlus } from 'lucide-svelte';

  export let data;
  let userAuthed = data.userInfo.role !== 'Student';

  /* Update stores (global vars) to the data returned from the fetch requests in SSR */
  userStore.set(data.userInfo);
</script>

<main class="flex flex-1 flex-col gap-4 p-4 lg:gap-6 lg:p-6">
  <h1 class="text-lg font-semibold md:text-2xl">Create booking</h1>

  <Card.Root>
    <Card.Header>
      <Card.Title>Book a virtual machine or cluster</Card.Title>
      <Card.Description>Cluster bookings are currently limited to teachers.</Card.Description>
    </Card.Header>
    <Card.Content>
      <Tabs.Root>
        <div class="flex justify-between items-center">
          <Tabs.List class="grid w-full md:w-96 grid-cols-2">
            <Tabs.Trigger value="vms">Virtual machine</Tabs.Trigger>
            <Tabs.Trigger disabled={!userAuthed} value="clusters">Cluster</Tabs.Trigger>
          </Tabs.List>
        </div>
        <!-- Virtual machine booking -->
        <Tabs.Content value="vm"></Tabs.Content>
        <!-- Cluster booking -->
        <Tabs.Content value="cluster"></Tabs.Content>
      </Tabs.Root>
    </Card.Content>
  </Card.Root>
</main>
