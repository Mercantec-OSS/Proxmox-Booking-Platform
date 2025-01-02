<script>
  import * as Card from '$lib/components/ui/card';
  import * as Table from '$lib/components/ui/table';
  import { vmService } from '$lib/services/vm-service';
  import { onMount, onDestroy } from 'svelte';


  const data = {};
  let updateIntervalId;

  async function fetchVcenterInfo() {
    data.info = await vmService.getVcenterInfo();
  }

  onMount(async () => {
    fetchVcenterInfo();
    updateIntervalId = setInterval(fetchVcenterInfo, 60000);
  });

  onDestroy(() => {
    clearInterval(updateIntervalId);
  });
</script>

<Card.Root>
  <Card.Header>
    <h2 class="text-lg font-semibold md:text-xl">vCenter cluster info</h2>
  </Card.Header>
  <Card.Content>
    <Table.Root>
      <Table.Header>
        <Table.Row>
          <Table.Head class="table-cell">Total CPU</Table.Head>
          <Table.Head class="table-cell">Used CPU</Table.Head>
          <Table.Head class="table-cell">Total Ram</Table.Head>
          <Table.Head class="table-cell">Used  Ram</Table.Head>
          <Table.Head class="table-cell">Total storage</Table.Head>
          <Table.Head class="table-cell">Used storage</Table.Head>
        </Table.Row>
      </Table.Header>
      <Table.Body>
        <Table.Row>
          <Table.Cell>{data.info?.cpuTotal ?? 'Loading...'}</Table.Cell>
          <Table.Cell>{data.info?.cpuUsage ?? 'Loading...'}</Table.Cell>
          <Table.Cell>{data.info?.ramTotal ?? 'Loading...'}</Table.Cell>
          <Table.Cell>{data.info?.ramUsage ?? 'Loading...'}</Table.Cell>
          <Table.Cell>{data.info?.storageTotal ?? 'Loading...'}</Table.Cell>
          <Table.Cell>{data.info?.storageUsage ?? 'Loading...'}</Table.Cell>
        </Table.Row>
      </Table.Body>
    </Table.Root>
  </Card.Content>
</Card.Root>
