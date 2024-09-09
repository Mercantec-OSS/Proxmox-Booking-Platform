<script>
  import { Sun, Moon } from 'lucide-svelte';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
  import { resetMode, setMode } from 'mode-watcher';
</script>

<div class="flex w-full justify-end">
  <div class="flex gap-x-3 items-center mt-2 mr-4">
    <DropdownMenu.Root>
      <DropdownMenu.Trigger asChild let:builder>
        <Button builders={[builder]} size="icon">
          <Sun class="size-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
          <Moon class="absolute size-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
          <span class="sr-only">Toggle theme</span>
        </Button>
      </DropdownMenu.Trigger>
      <DropdownMenu.Content align="end">
        <DropdownMenu.Item on:click={() => setMode('light')}>Light</DropdownMenu.Item>
        <DropdownMenu.Item on:click={() => setMode('dark')}>Dark</DropdownMenu.Item>
        <DropdownMenu.Item on:click={() => resetMode()}>System</DropdownMenu.Item>
      </DropdownMenu.Content>
    </DropdownMenu.Root>
  </div>
</div>
