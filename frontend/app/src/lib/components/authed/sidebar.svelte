<script>
  import { Sun, Moon, LogOut, Home, ChartLine, LifeBuoy } from 'lucide-svelte';
  import { authService } from '$lib/services/auth-service';
  import { getCookie, deleteCookie } from '$lib/utils/cookie';
  import { goto } from '$app/navigation';
  import { resetMode, setMode } from 'mode-watcher';
  import { page } from '$app/stores';
  import { userStore } from '$lib/utils/store';

  import { Badge } from '$lib/components/ui/badge/index.js';
  import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Card from '$lib/components/ui/card/index.js';
  import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { Label } from '$lib/components/ui/label/index.js';
  import * as Select from '$lib/components/ui/select/index.js';
  import * as Sheet from '$lib/components/ui/sheet/index.js';
  import * as Table from '$lib/components/ui/table/index.js';
  import { Textarea } from '$lib/components/ui/textarea/index.js';
  import * as ToggleGroup from '$lib/components/ui/toggle-group/index.js';
  import * as Tooltip from '$lib/components/ui/tooltip/index.js';
  import { Separator } from '$lib/components/ui/separator/index.js';

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
</script>

<aside class="bg-background fixed inset-y-0 left-0 z-10 hidden w-14 flex-col border-r sm:flex">
  <nav class="flex flex-col items-center gap-4 px-2 sm:py-5">
    <a href="/" class="text-primary-foreground group flex h-9 w-9 shrink-0 items-center justify-center gap-2 rounded-full text-lg font-semibold md:h-8 md:w-8 md:text-base">
      <img src="/images/mercantec-logo.svg" alt="Mercantec" class="h-7 w-7 transition-all group-hover:scale-110 brightness-0 saturate-0 dark:brightness-100 dark:saturate-100" />
      <span class="sr-only">Mercantec</span>
    </a>
    <Separator />
    <Tooltip.Root>
      <Tooltip.Trigger asChild let:builder>
        <a href="##" class="text-muted-foreground hover:text-foreground flex h-9 w-9 items-center justify-center rounded-lg transition-colors md:h-8 md:w-8" use:builder.action {...builder}>
          <Home class="h-5 w-5" />
          <span class="sr-only">Dashboard</span>
        </a>
      </Tooltip.Trigger>
      <Tooltip.Content side="right">Dashboard</Tooltip.Content>
    </Tooltip.Root>
    <Tooltip.Root>
      <Tooltip.Trigger asChild let:builder>
        <a href="/analytics" class="text-muted-foreground hover:text-foreground flex h-9 w-9 items-center justify-center rounded-lg transition-colors md:h-8 md:w-8" use:builder.action {...builder}>
          <ChartLine class="h-5 w-5" />
          <span class="sr-only">Analytics</span>
        </a>
      </Tooltip.Trigger>
      <Tooltip.Content side="right">Analytics</Tooltip.Content>
    </Tooltip.Root>
    <Tooltip.Root>
      <Tooltip.Trigger asChild let:builder>
        <a href="/help" class="text-muted-foreground hover:text-foreground flex h-9 w-9 items-center justify-center rounded-lg transition-colors md:h-8 md:w-8" use:builder.action {...builder}>
          <LifeBuoy class="h-5 w-5" />
          <span class="sr-only">Help</span>
        </a>
      </Tooltip.Trigger>
      <Tooltip.Content side="right">Help</Tooltip.Content>
    </Tooltip.Root>
  </nav>
  <nav class="mt-auto flex flex-col items-center gap-4 px-2 sm:py-5">
    <Tooltip.Root>
      <Tooltip.Trigger asChild let:builder>
        <button
          on:click={handleLogout}
          class="text-muted-foreground hover:text-foreground flex h-9 w-9 items-center justify-center rounded-lg transition-colors md:h-8 md:w-8"
          use:builder.action
          {...builder}
        >
          <LogOut class="h-5 w-5" />
          <span class="sr-only">Logout</span>
        </button>
      </Tooltip.Trigger>
      <Tooltip.Content side="right">Logout</Tooltip.Content>
    </Tooltip.Root>
  </nav>
</aside>
