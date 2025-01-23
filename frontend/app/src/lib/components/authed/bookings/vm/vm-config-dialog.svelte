<script>
  import { vmService } from '$lib/services/vm-service';
  import * as Dialog from '$lib/components/ui/dialog';
  import { Label } from '$lib/components/ui/label';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Select from '$lib/components/ui/select/index.js';
  import { LoaderCircle } from 'lucide-svelte';
  import { toast } from 'svelte-sonner';
  import { Slider } from '$lib/components/ui/slider/index.js';

  let { vmConfigDialogOpen = $bindable() } = $props();

  let loadingStates = {
    saveConfig: false
  };

  let osOptions = $state([]);

  $effect(() => {
    vmService.getVMTemplates().then((templates) => {
      osOptions = templates.map((template) => ({
        value: template.internalName,
        label: template.displayName
      }));
    });
  });

  let systemConfig = $state({
    cpuCores: 1,
    ramGB: 2,
    storageGB: '100',
    operatingSystem: null
  });

  const storageOptions = ['100', '200', '500'];

  let constraints = $derived({
    cpu: { min: 1, max: 6 },
    ram: { min: 2, max: 16 },
    storage: { validSizes: storageOptions },
    os: {
      validOptions: osOptions.map((os) => os.value)
    }
  });

  function validateConfig(config) {
    let errors = [];

    if (!Number.isInteger(config.cpuCores) || config.cpuCores < constraints.cpu.min || config.cpuCores > constraints.cpu.max) {
      errors.push(`CPU cores must be between ${constraints.cpu.min} and ${constraints.cpu.max} cores`);
    }

    if (!Number.isInteger(config.ramGB) || config.ramGB < constraints.ram.min || config.ramGB > constraints.ram.max) {
      errors.push(`RAM must be between ${constraints.ram.min}GB and ${constraints.ram.max}GB`);
    }

    if (!constraints.storage.validSizes.includes(String(config.storageGB))) {
      errors.push(`Storage must be one of: ${constraints.storage.validSizes.map((s) => `${s}GB`).join(', ')}`);
    }

    if (!constraints.os.validOptions.includes(config.operatingSystem)) {
      errors.push('Invalid operating system selected');
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
      toast.success('VM configuration saved');
      vmConfigDialogOpen = false;
    } catch (error) {
      toast.error(error.message);
    } finally {
      loadingStates.saveConfig = false;
    }
  }
</script>

<Dialog.Root bind:open={vmConfigDialogOpen}>
  <Dialog.Content class="sm:max-w-[425px]">
    <Dialog.Header>
      <Dialog.Title>System Configuration</Dialog.Title>
      <Dialog.Description>Configure your system resources and operating system.</Dialog.Description>
    </Dialog.Header>
    <div class="grid gap-4 py-4">
      <div class="grid grid-cols-4 items-center gap-4">
        <Label class="text-right">CPU Cores</Label>
        <div class="col-span-3">
          <Slider type="single" bind:value={systemConfig.cpuCores} min={constraints.cpu.min} max={constraints.cpu.max} step={1} class="w-full" />
          <div class="mt-1 text-sm text-muted-foreground">
            Selected: {systemConfig.cpuCores}
            {systemConfig.cpuCores === 1 ? 'core' : 'cores'}
          </div>
        </div>
      </div>

      <div class="grid grid-cols-4 items-center gap-4">
        <Label class="text-right">RAM</Label>
        <div class="col-span-3">
          <Slider type="single" bind:value={systemConfig.ramGB} min={constraints.ram.min} max={constraints.ram.max} step={2} class="w-full" />
          <div class="mt-1 text-sm text-muted-foreground">
            Selected: {systemConfig.ramGB} GB
          </div>
        </div>
      </div>

      <div class="grid grid-cols-4 items-center gap-4">
        <Label class="text-right">Storage</Label>
        <div class="col-span-3">
          <Select.Root bind:value={systemConfig.storageGB} name="Select Storage" type="single">
            <Select.Trigger class="w-full">
              {systemConfig.storageGB ? `${systemConfig.storageGB} GB` : 'Select storage'}
            </Select.Trigger>
            <Select.Content>
              <Select.Group>
                <Select.GroupHeading>Storage Options</Select.GroupHeading>
                {#each constraints.storage.validSizes as size}
                  <Select.Item value={size}>
                    {size} GB
                  </Select.Item>
                {/each}
              </Select.Group>
            </Select.Content>
          </Select.Root>
        </div>
      </div>

      <div class="grid grid-cols-4 items-center gap-4">
        <Label class="text-right">OS</Label>
        <!-- <div class="col-span-3">
          <Select.Root bind:value={systemConfig.operatingSystem} name="Select OS">
            <Select.Trigger class="w-full">
              {systemConfig.operatingSystem ? osOptions.find((os) => os.value === systemConfig.operatingSystem)?.label : 'Select OS'}
            </Select.Trigger>
            <Select.Content>
              <Select.Group>
                <Select.GroupHeading>Operating Systems</Select.GroupHeading>
                {#each osOptions as os}
                  <Select.Item value={os.value}>
                    {os.label}
                  </Select.Item>
                {/each}
              </Select.Group>
            </Select.Content>
          </Select.Root>
        </div> -->
      </div>
    </div>
    <Dialog.Footer>
      <Button variant="outline" onmousedown={() => (vmConfigDialogOpen = false)}>Cancel</Button>
      <Button type="submit" disabled={loadingStates.saveConfig} onclick={handleUpdateConfig}>
        {#if loadingStates.saveConfig}
          <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
        {/if}
        Save Configuration
      </Button>
    </Dialog.Footer>
  </Dialog.Content>
</Dialog.Root>
