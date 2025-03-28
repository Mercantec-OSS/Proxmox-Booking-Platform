<script>
  import { LogOut, Bell, User, PanelLeft, Home, CircleHelp, CirclePlus } from 'lucide-svelte';
  import { authService } from '$lib/services/auth-service';
  import { getCookie, deleteCookie } from '$lib/utils/cookie';
  import { goto } from '$app/navigation';
  import UserSearch from '$lib/components/authed/user-search.svelte';
  import { page } from '$app/state';
  import { userStore } from '$lib/utils/store';
  import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
  import * as Sheet from '$lib/components/ui/sheet/index.js';
  import * as HoverCard from '$lib/components/ui/hover-card';
  import InviteUserDialog from '$lib/components/authed/user/invite-user-dialog.svelte';

  let userAuthed = $derived($userStore?.role === 'Admin' || $userStore?.role === 'Teacher' || false);
  let inviteUserDialogOpen = $state(false);

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
    return page.url.pathname === href;
  }
</script>

<InviteUserDialog bind:inviteUserDialogOpen />

<header class="sticky top-0 z-30 bg-background flex h-14 items-center border-b px-4 sm:static sm:h-auto sm:border-0 sm:bg-transparent sm:px-6">
  <!-- Mobile menu -->
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
            class="size-full transition-all group-hover:scale-110 [filter:brightness(0)_saturate(100%)_invert(39%)_sepia(93%)_saturate(1699%)_hue-rotate(2deg)_brightness(103%)_contrast(106%)]"
          />
          <span class="sr-only">Mercantec</span>
        </a>
        <a href="/" class="{isActive('/') ? 'text-foreground' : 'text-muted-foreground'} hover:text-foreground flex items-center gap-4 px-2.5">
          <Home class="h-5 w-5" />
          Dashboard
        </a>
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

  <!-- Breadcrumb -->
  <Breadcrumb.Root class="hidden md:flex">
    <Breadcrumb.List>
      <Breadcrumb.Item>
        <Breadcrumb.Link href="/">Dashboard</Breadcrumb.Link>
      </Breadcrumb.Item>
      <Breadcrumb.Separator />
      {#if page.url.pathname === '/'}
        <Breadcrumb.Item>
          <Breadcrumb.Page href="/">Overview</Breadcrumb.Page>
        </Breadcrumb.Item>
      {:else if page.url.pathname.includes('user')}
        <Breadcrumb.Item>
          <Breadcrumb.Page>User Profile</Breadcrumb.Page>
        </Breadcrumb.Item>
      {:else if page.url.pathname.includes('booking')}
        <Breadcrumb.Item>
          <Breadcrumb.Page>Booking Details</Breadcrumb.Page>
        </Breadcrumb.Item>
      {:else if page.url.pathname.includes('create')}
        <Breadcrumb.Item>
          <Breadcrumb.Page>Create Booking</Breadcrumb.Page>
        </Breadcrumb.Item>
      {/if}
    </Breadcrumb.List>
  </Breadcrumb.Root>

  <!-- User search -->
  <div class="relative ml-auto flex-1 md:grow-0 text-center md:text-right px-2">
    <UserSearch />
  </div>

  <!-- User menu -->
  <div class="pl-2">
    <DropdownMenu.Root>
      <DropdownMenu.Trigger>
        <Button variant="outline" size="icon" class="overflow-hidden rounded-lg">
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
            window.open('https://mars.merhot.dk/w/index.php/Booking-guide', '_blank');
          }}><CircleHelp class="size-4 mr-2" />Help</DropdownMenu.Item
        >
        <DropdownMenu.Separator />
        {#if userAuthed}
          <DropdownMenu.Item onmousedown={() => (inviteUserDialogOpen = true)}><CirclePlus class="size-4 mr-2" />Invite User</DropdownMenu.Item>
        {/if}
        <DropdownMenu.Separator />
        <DropdownMenu.Item onmousedown={handleLogout}><LogOut class="size-4 mr-2" />Logout</DropdownMenu.Item>
      </DropdownMenu.Content>
    </DropdownMenu.Root>
  </div>
</header>
