<script>
  import { LogOut, Bell, User, PanelLeft, Home, CircleHelp } from 'lucide-svelte';
  import { authService } from '$lib/services/auth-service';
  import { getCookie, deleteCookie } from '$lib/utils/cookie';
  import { goto } from '$app/navigation';
  import UserSearch from '$lib/components/authed/user-search.svelte';
  import { page } from '$app/stores';
  import { userStore } from '$lib/utils/store';
  import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
  import * as Sheet from '$lib/components/ui/sheet/index.js';
  import * as HoverCard from '$lib/components/ui/hover-card';

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

<header class="sticky top-0 z-30 bg-background flex h-14 items-center gap-4 border-b px-4 sm:static sm:h-auto sm:border-0 sm:bg-transparent sm:px-6">
  <Sheet.Root>
    <Sheet.Trigger class="sm:hidden">
      <PanelLeft class="h-5 w-5" />
      <span class="sr-only">Toggle Menu</span>
    </Sheet.Trigger>
    <Sheet.Content side="left" class="sm:max-w-xs">
      <nav class="grid gap-6 text-lg font-medium">
        <a href="/" class="text-primary-foreground group flex h-auto w-40 px-2.5 shrink-0 items-center justify-center gap-2 rounded-full text-lg font-semibold md:text-base">
          <img
            src="/images/mercantec-logo-full.svg"
            alt="Mercantec"
            class="size-full transition-all group-hover:scale-110 [filter:brightness(0)_saturate(100%)_invert(27%)_sepia(94%)_saturate(1394%)_hue-rotate(218deg)_brightness(95%)_contrast(101%)]"
          />
          <span class="sr-only">Mercantec</span>
        </a>
        <a href="/" class="{isActive('/') ? 'text-foreground' : 'text-muted-foreground'} hover:text-foreground flex items-center gap-4 px-2.5">
          <Home class="h-5 w-5" />
          Dashboard
        </a>
        <!-- <a href="/analytics" class="{isActive('/analytics') ? 'text-foreground' : 'text-muted-foreground'} flex items-center gap-4 px-2.5">
          <ChartLine class="h-5 w-5" />
          Analytics
        </a> -->
        <a href="/help" class="{isActive('/help') ? 'text-foreground' : 'text-muted-foreground'} hover:text-foreground flex items-center gap-4 px-2.5">
          <CircleHelp class="h-5 w-5" />
          Help
        </a>
        <Button variant="outline" onmousedown={handleLogout} class="text-muted-foreground hover:text-foreground flex items-center gap-2 px-2.5">
          <LogOut class="h-4 w-4" />
          Logout
        </Button>
      </nav>
    </Sheet.Content>
  </Sheet.Root>
  <Breadcrumb.Root class="hidden md:flex">
    <Breadcrumb.List>
      <Breadcrumb.Item>
        <Breadcrumb.Link href="/">Dashboard</Breadcrumb.Link>
      </Breadcrumb.Item>
      <Breadcrumb.Separator />
      {#if $page.url.pathname === '/'}
        <Breadcrumb.Item>
          <Breadcrumb.Page href="/">Overview</Breadcrumb.Page>
        </Breadcrumb.Item>
      {:else if $page.url.pathname.includes('user')}
        <Breadcrumb.Item>
          <Breadcrumb.Page>User Profile</Breadcrumb.Page>
        </Breadcrumb.Item>
      {:else if $page.url.pathname.includes('booking')}
        <Breadcrumb.Item>
          <Breadcrumb.Page>Booking Details</Breadcrumb.Page>
        </Breadcrumb.Item>
      {:else if $page.url.pathname.includes('create')}
        <Breadcrumb.Item>
          <Breadcrumb.Page>Create Booking</Breadcrumb.Page>
        </Breadcrumb.Item>
      {/if}
    </Breadcrumb.List>
  </Breadcrumb.Root>
  <div class="relative ml-auto flex-1 md:grow-0 text-center md:text-right">
    <UserSearch />
  </div>
  <HoverCard.Root openDelay={200} closeDelay={150}>
    <HoverCard.Trigger>
      <Button variant="outline" size="icon" class="hidden md:flex"><Bell /></Button>
    </HoverCard.Trigger>
    <HoverCard.Content>
      <div class="flex justify-between space-x-4">
        <p class="text-sm">Notifications not yet implemented</p>
      </div>
    </HoverCard.Content>
  </HoverCard.Root>

  <DropdownMenu.Root>
    <DropdownMenu.Trigger>
      <Button variant="outline" size="icon" class="overflow-hidden rounded-full">
        <User />
      </Button>
    </DropdownMenu.Trigger>
    <DropdownMenu.Content align="end">
      <DropdownMenu.Label>My Account</DropdownMenu.Label>
      <DropdownMenu.Separator />
      <DropdownMenu.Item
        onmousedown={() => {
          goto(`/user/${$userStore.id}`);
        }}><User class="size-4 mr-2" />Profile</DropdownMenu.Item
      >
      <DropdownMenu.Item
        onmousedown={() => {
          goto('/help');
        }}><CircleHelp class="size-4 mr-2" />Help</DropdownMenu.Item
      >
      <DropdownMenu.Separator />
      <DropdownMenu.Item onmousedown={handleLogout}><LogOut class="size-4 mr-2" />Logout</DropdownMenu.Item>
    </DropdownMenu.Content>
  </DropdownMenu.Root>
</header>
