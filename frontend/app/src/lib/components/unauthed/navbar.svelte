<script>
  import { Sun, Moon } from 'lucide-svelte';
  import { Button } from '$lib/components/ui/button/index.js';
  import { buttonVariants } from '$lib/components/ui/button/index.js';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
  import { resetMode, setMode } from 'mode-watcher';
</script>

<div class="flex w-full justify-end">
  <div class="flex gap-x-3 items-center mt-2 mr-4">
    <DropdownMenu.Root>
      <DropdownMenu.Trigger class={buttonVariants({ variant: 'outline', size: 'icon' })}>
        <Sun class="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
        <Moon class="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
        <span class="sr-only">Toggle theme</span>
      </DropdownMenu.Trigger>
      <DropdownMenu.Content align="end">
        <DropdownMenu.Item onclick={() => setMode('light')}>Light</DropdownMenu.Item>
        <DropdownMenu.Item onclick={() => setMode('dark')}>Dark</DropdownMenu.Item>
        <DropdownMenu.Item onclick={() => resetMode()}>System</DropdownMenu.Item>
      </DropdownMenu.Content>
    </DropdownMenu.Root>
  </div>
</div>
