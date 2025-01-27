<script>
  import { vmService } from '$lib/services/vm-service';
  import * as Dialog from '$lib/components/ui/dialog';
  import { Label } from '$lib/components/ui/label';
  import { Button } from '$lib/components/ui/button/index.js';
  import { LoaderCircle } from 'lucide-svelte';
  import { toast } from 'svelte-sonner';
  import { Slider } from '$lib/components/ui/slider/index.js';
  import { selectedBookingStore } from '$lib/utils/store';

  let { configureSpecsDialogOpen = $bindable() } = $props();

  $effect(() => {
    if ($selectedBookingStore) {
      systemConfig = {
        uuid: $selectedBookingStore.uuid,
        cpu: $selectedBookingStore.cpu,
        ram: $selectedBookingStore.ram
      };
    }
  });

  let loadingStates = {
    saveConfig: false
  };

  let systemConfig = $state({
    uuid: null,
    cpu: null,
    ram: null
  });

  let constraints = $derived({
    cpu: { min: 1, max: 6 },
    ram: { min: 2, max: 16 }
  });

  function validateConfig(config) {
    let errors = [];

    if (!Number.isInteger(config.cpu) || config.cpu < constraints.cpu.min || config.cpu > constraints.cpu.max) {
      errors.push(`CPU cores must be between ${constraints.cpu.min} and ${constraints.cpu.max} cores`);
    }

    if (!Number.isInteger(config.ram) || config.ram < constraints.ram.min || config.ram > constraints.ram.max) {
      errors.push(`RAM must be between ${constraints.ram.min}GB and ${constraints.ram.max}GB`);
    }
    return errors;
  }

  async function handleUpdateConfig() {
    const validationErrors = validateConfig(systemConfig);

    if (validationErrors.length > 0) {
      validationErrors.forEach((error) => toast.error(error));
      return;
    }

    loadingStates.saveConfig = true;

    try {
      await vmService.updateResources(systemConfig);
      toast.success('VM configuration saved. Effects will take place in a minute');
      configureSpecsDialogOpen = false;
    } catch (error) {
      toast.error(error.message);
    } finally {
      loadingStates.saveConfig = false;
    }
  }
</script>

<Dialog.Root bind:open={configureSpecsDialogOpen}>
  <Dialog.Content class="sm:max-w-[425px]">
    <Dialog.Header>
      <Dialog.Title>System Configuration</Dialog.Title>
      <Dialog.Description>Configure your system resources and operating system.</Dialog.Description>
    </Dialog.Header>
    <div class="grid gap-4 py-4">
      <div class="grid grid-cols-6 items-center gap-4">
        <Label class="text-right">CPU Cores</Label>
        <div class="col-span-5 space-y-2">
          <Slider type="single" bind:value={systemConfig.cpu} min={constraints.cpu.min} max={constraints.cpu.max} step={1} class="w-full" />
          <div class="mt-1 text-sm text-muted-foreground">
            Selected: {systemConfig.cpu}
            {systemConfig.cpu === 1 ? 'core' : 'cores'}
          </div>
        </div>
      </div>

      <div class="grid grid-cols-6 items-center gap-4">
        <Label class="text-right">RAM</Label>
        <div class="col-span-5 space-y-2">
          <Slider type="single" bind:value={systemConfig.ram} min={constraints.ram.min} max={constraints.ram.max} step={2} class="w-full" />
          <div class="mt-1 text-sm text-muted-foreground">
            Selected: {systemConfig.ram} GB
          </div>
        </div>
      </div>
    </div>
    <Dialog.Footer>
      <Button
        variant="outline"
        onmousedown={() => {
          configureSpecsDialogOpen = false;
        }}>Cancel</Button
      >
      <Button type="submit" disabled={loadingStates.saveConfig} onclick={handleUpdateConfig}>
        {#if loadingStates.saveConfig}
          <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
        {/if}
        Save Configuration
      </Button>
    </Dialog.Footer>
  </Dialog.Content>
</Dialog.Root>
