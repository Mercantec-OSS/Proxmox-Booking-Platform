<script>
  import { Sun, Moon, LogOut, Bell, ChevronDown, SunMoon, User, PanelLeft, Package2, Home, ShoppingCart, Package, UsersRound, ChartLine, Search } from 'lucide-svelte';
  import { authService } from '$lib/services/auth-service';
  import { getCookie, deleteCookie } from '$lib/utils/cookie';
  import { goto } from '$app/navigation';
  import { resetMode, setMode } from 'mode-watcher';
  import UserSearch from '$lib/components/authed/user-search.svelte';
  import { page } from '$app/stores';
  import { userStore } from '$lib/utils/store';
  import { Badge } from '$lib/components/ui/badge/index.js';
  import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Card from '$lib/components/ui/card/index.js';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { Separator } from '$lib/components/ui/separator/index.js';
  import * as Sheet from '$lib/components/ui/sheet/index.js';
  import * as Table from '$lib/components/ui/table/index.js';
  import * as Tabs from '$lib/components/ui/tabs/index.js';
  import * as Tooltip from '$lib/components/ui/tooltip/index.js';

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

<header class="bg-background sticky top-0 z-30 flex h-14 items-center gap-4 border-b px-4 sm:static sm:h-auto sm:border-0 sm:bg-transparent sm:px-6">
  <Sheet.Root>
    <Sheet.Trigger asChild let:builder>
      <Button builders={[builder]} size="icon" variant="outline" class="sm:hidden">
        <PanelLeft class="h-5 w-5" />
        <span class="sr-only">Toggle Menu</span>
      </Button>
    </Sheet.Trigger>
    <Sheet.Content side="left" class="sm:max-w-xs">
      <nav class="grid gap-6 text-lg font-medium">
        <a href="##" class="bg-primary text-primary-foreground group flex h-10 w-10 shrink-0 items-center justify-center gap-2 rounded-full text-lg font-semibold md:text-base">
          <Package2 class="h-5 w-5 transition-all group-hover:scale-110" />
          <span class="sr-only">Mercantec</span>
        </a>
        <a href="##" class="text-muted-foreground hover:text-foreground flex items-center gap-4 px-2.5">
          <Home class="h-5 w-5" />
          Dashboard
        </a>
        <a href="##" class="text-foreground flex items-center gap-4 px-2.5">
          <ShoppingCart class="h-5 w-5" />
          Orders
        </a>
        <a href="##" class="text-muted-foreground hover:text-foreground flex items-center gap-4 px-2.5">
          <Package class="h-5 w-5" />
          Products
        </a>
        <a href="##" class="text-muted-foreground hover:text-foreground flex items-center gap-4 px-2.5">
          <UsersRound class="h-5 w-5" />
          Customers
        </a>
        <a href="##" class="text-muted-foreground hover:text-foreground flex items-center gap-4 px-2.5">
          <ChartLine class="h-5 w-5" />
          Settings
        </a>
      </nav>
    </Sheet.Content>
  </Sheet.Root>
  <Breadcrumb.Root class="hidden md:flex">
    <Breadcrumb.List>
      <Breadcrumb.Item>
        <Breadcrumb.Page href="/">Dashboard</Breadcrumb.Page>
      </Breadcrumb.Item>
      <Breadcrumb.Separator />
    </Breadcrumb.List>
  </Breadcrumb.Root>
  <div class="relative ml-auto flex-1 md:grow-0">
    <Search class="text-muted-foreground absolute left-2.5 top-2.5 h-4 w-4" />
    <Input type="search" placeholder="Search..." class="bg-background w-full rounded-lg pl-8 md:w-[200px] lg:w-[336px]" />
  </div>
  <DropdownMenu.Root>
    <DropdownMenu.Trigger asChild let:builder>
      <Button variant="outline" size="icon" class="overflow-hidden rounded-full" builders={[builder]}>
        <img src="/images/placeholder-user.jpg" width={36} height={36} alt="Avatar" class="overflow-hidden rounded-full" />
      </Button>
    </DropdownMenu.Trigger>
    <DropdownMenu.Content align="end">
      <DropdownMenu.Label>My Account</DropdownMenu.Label>
      <DropdownMenu.Separator />
      <DropdownMenu.Item>Settings</DropdownMenu.Item>
      <DropdownMenu.Item>Support</DropdownMenu.Item>
      <DropdownMenu.Separator />
      <DropdownMenu.Item>Logout</DropdownMenu.Item>
    </DropdownMenu.Content>
  </DropdownMenu.Root>
</header>
