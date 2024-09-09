<script>
  import { Sun, Moon, LogOut } from 'lucide-svelte';
  import { authService } from '$lib/services/auth-service';
  import { getCookie, deleteCookie } from '$lib/utils/cookie';
  import { goto } from '$app/navigation';
  import { Button } from '$lib/components/ui/button/index.js';
  import { resetMode, setMode } from 'mode-watcher';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
  import UserSearch from '$lib/components/authed/user-search.svelte';
  import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
  import { page } from '$app/stores';

  async function handleLogout() {
    try {
      await authService.logout(getCookie('token'));
      deleteCookie('token');
      goto('/login');
    } catch (error) {
      deleteCookie('token');
    }
  }

  // Get the first path segment and capitalize it
  function getFirstPathSegment(path) {
    const segments = path.split('/').filter(Boolean);
    if (segments.length > 0) {
      const firstSegment = segments[0];
      return firstSegment.charAt(0).toUpperCase() + firstSegment.slice(1);
    }
    return '';
  }

  // Reactive declaration to format the path whenever the URL changes
  $: firstSegment = getFirstPathSegment($page.url.pathname);
</script>

<div class="flex flex-wrap w-full justify-between pt-2 px-4">
  <div class="flex items-center">
    <Breadcrumb.Root>
      <Breadcrumb.List>
        <Breadcrumb.Item>
          <Breadcrumb.Link href="/">Home</Breadcrumb.Link>
        </Breadcrumb.Item>
        {#if firstSegment}
          <Breadcrumb.Separator />
          <Breadcrumb.Item>
            <Breadcrumb.Page>{firstSegment}</Breadcrumb.Page>
          </Breadcrumb.Item>
        {/if}
      </Breadcrumb.List>
    </Breadcrumb.Root>
  </div>

  <div class="flex flex-wrap gap-x-3 items-center">
    <div>
      <UserSearch />
    </div>
    <Button on:click={handleLogout} size="icon"><LogOut class="size-[1.2rem]" /></Button>
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
