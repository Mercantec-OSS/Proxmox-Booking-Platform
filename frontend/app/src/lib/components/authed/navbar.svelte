<script>
  import { Sun, Moon, LogOut, Bell, ChevronDown, SunMoon, User } from 'lucide-svelte';
  import { authService } from '$lib/services/auth-service';
  import { getCookie, deleteCookie } from '$lib/utils/cookie';
  import { goto } from '$app/navigation';
  import { Button } from '$lib/components/ui/button/index.js';
  import { resetMode, setMode } from 'mode-watcher';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
  import UserSearch from '$lib/components/authed/user-search.svelte';
  import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
  import { page } from '$app/stores';
  import * as Avatar from '$lib/components/ui/avatar/index.js';
  import { userStore } from '$lib/utils/store';
  import * as HoverCard from '$lib/components/ui/hover-card/index.js';

  let dropdownOpen = false;
  $: firstCharFirstName = $userStore?.name?.[0] || '.';
  $: firstCharLastName = $userStore?.surname?.[0] || '.';
  $: initials = `${firstCharFirstName}${firstCharLastName}`.toUpperCase();

  async function handleLogout() {
    try {
      await authService.logout(getCookie('token'));
      deleteCookie('token');
      goto('/login');
    } catch (error) {
      deleteCookie('token');
    }
  }

  function isActive(href) {
    return $page.url.pathname === href;
  }
</script>

<div class="grid grid-cols-3 items-center w-full px-8 py-3 bg-background">
  <!-- Logo and company name -->
  <div>
    <img src="/images/mercantec-logo.svg" alt="Mercantec" class="h-12 w-auto brightness-0 saturate-0 dark:brightness-100 dark:saturate-100" />
  </div>
  <!-- Navigation links -->
  <nav class="flex justify-center my-auto">
    <ul class="flex gap-x-5 py-5 px-5 bg-muted rounded-3xl">
      <li>
        <a href="/" class="p-4 rounded-3xl transition-colors duration-200 {isActive('/') ? 'bg-foreground text-background' : 'hover:bg-foreground hover:text-background'}"> Dashboard </a>
      </li>
      <li>
        <a href="/monitor" class="p-4 rounded-3xl transition-colors duration-200 {isActive('/monitor') ? 'bg-foreground text-background' : 'hover:bg-foreground hover:text-background'}">
          Monitoring
        </a>
      </li>
      <li>
        <a href="/support" class="p-4 rounded-3xl transition-colors duration-200 {isActive('/support') ? 'bg-foreground text-background' : 'hover:bg-foreground hover:text-background'}"> Support </a>
      </li>
    </ul>
  </nav>
  <!-- Search, logout, change theme etc -->
  <div class="flex justify-end">
    <div class="flex gap-x-4">
      <div>
        <UserSearch />
      </div>
      <HoverCard.Root openDelay={100} closeDelay="150">
        <HoverCard.Trigger asChild let:builder>
          <Button builders={[builder]} variant="ghost" size="icon"><Bell /></Button>
        </HoverCard.Trigger>
        <HoverCard.Content class="w-80">
          <div class="flex justify-between space-x-4">
            <p class="text-sm">Notifications not yet implemented</p>
          </div>
        </HoverCard.Content>
      </HoverCard.Root>
    </div>
    <DropdownMenu.Root bind:open={dropdownOpen}>
      <DropdownMenu.Trigger asChild let:builder>
        <Button builders={[builder]} variant="ghost"
          ><Avatar.Root>
            <Avatar.Fallback>{initials}</Avatar.Fallback>
          </Avatar.Root>
          <ChevronDown class="size-4 ml-2" /></Button
        >
      </DropdownMenu.Trigger>
      <DropdownMenu.Content>
        <DropdownMenu.Group>
          <DropdownMenu.Label>{$userStore.role}</DropdownMenu.Label>
          <DropdownMenu.Separator />
          <DropdownMenu.Item on:click={() => goto(`/user/${$userStore.id}`)}
            ><User class="mr-2 h-4 w-4" />
            <span>Profile</span></DropdownMenu.Item
          >
          <DropdownMenu.Sub>
            <DropdownMenu.SubTrigger
              ><SunMoon class="mr-2 h-4 w-4" />
              <span>Theme</span></DropdownMenu.SubTrigger
            >
            <DropdownMenu.SubContent>
              <DropdownMenu.Item on:click={() => setMode('light')}>Light</DropdownMenu.Item>
              <DropdownMenu.Item on:click={() => setMode('dark')}>Dark</DropdownMenu.Item>
              <DropdownMenu.Item on:click={() => resetMode()}>System</DropdownMenu.Item>
            </DropdownMenu.SubContent>
          </DropdownMenu.Sub>
          <DropdownMenu.Separator />
          <DropdownMenu.Item on:click={handleLogout}
            ><LogOut class="mr-2 h-4 w-4" />
            <span>Log out</span></DropdownMenu.Item
          >
        </DropdownMenu.Group>
      </DropdownMenu.Content>
    </DropdownMenu.Root>
  </div>
</div>

<!-- <Button on:click={handleLogout} size="icon"><LogOut class="size-[1.2rem]" /></Button> -->
<!-- <DropdownMenu.Root>
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
</DropdownMenu.Root> -->
