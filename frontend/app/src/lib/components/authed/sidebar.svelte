<script>
  import { Sun, Moon, LogOut, Home, CircleHelp } from 'lucide-svelte';
  import { authService } from '$lib/services/auth-service';
  import { getCookie, deleteCookie } from '$lib/utils/cookie';
  import { goto } from '$app/navigation';
  import { toggleMode } from 'mode-watcher';
  import { page } from '$app/stores';
  import * as Tooltip from '$lib/components/ui/tooltip/index.js';
  import { Separator } from '$lib/components/ui/separator/index.js';
  import { Button } from '$lib/components/ui/button';

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

<aside class="fixed inset-y-0 left-0 z-10 hidden w-14 flex-col border-r sm:flex">
  <!-- Top of side navigation bar -->
  <nav class="flex flex-col items-center gap-4 px-2 sm:py-5">
    <!-- Mercantec logo -->
    <a href="/" class="text-primary-foreground group flex h-9 w-9 shrink-0 items-center justify-center gap-2 rounded-full text-lg font-semibold md:h-8 md:w-8 md:text-base">
      <img
        src="/images/mercantec-logo.svg"
        alt="Mercantec"
        class="h-7 w-7 transition-all group-hover:scale-110 [filter:brightness(0)_saturate(100%)_invert(39%)_sepia(93%)_saturate(1699%)_hue-rotate(2deg)_brightness(103%)_contrast(106%)]"
      />
      <span class="sr-only">Mercantec</span>
    </a>
    <Separator />
    <!-- Dashboard nav link -->
    <Tooltip.Provider>
      <Tooltip.Root openDelay={400}>
        <Tooltip.Trigger>
          <a
            href="/"
            class="{isActive('/') || $page.url.pathname.includes('/booking/')
              ? 'text-foreground'
              : 'text-muted-foreground'} hover:text-foreground flex h-9 w-9 items-center justify-center rounded-lg transition-colors md:h-8 md:w-8"
          >
            <Home class="h-5 w-5" />
            <span class="sr-only">Dashboard</span>
          </a>
        </Tooltip.Trigger>
        <Tooltip.Content side="right">Dashboard</Tooltip.Content>
      </Tooltip.Root>
    </Tooltip.Provider>
    <!-- Analytics nav link -->
    <!-- <Tooltip.Root openDelay={400}>
      <Tooltip.Trigger>
        <a
          href="/analytics"
          class="{isActive('/analytics') ? 'text-foreground' : 'text-muted-foreground'} hover:text-foreground flex h-9 w-9 items-center justify-center rounded-lg transition-colors md:h-8 md:w-8"
          use:builder.action
         
        >
          <ChartLine class="h-5 w-5" />
          <span class="sr-only">Analytics</span>
        </a>
      </Tooltip.Trigger>
      <Tooltip.Content side="right">Analytics</Tooltip.Content>
    </Tooltip.Root> -->
    <!-- Help nav link -->
    <Tooltip.Provider>
      <Tooltip.Root openDelay={400}>
        <Tooltip.Trigger>
          <a
            href="https://mars.merhot.dk/w/index.php/Booking-guide"
            target="_blank"
            class="{isActive('/help') ? 'text-foreground' : 'text-muted-foreground'} hover:text-foreground flex h-9 w-9 items-center justify-center rounded-lg transition-colors md:h-8 md:w-8"
          >
            <CircleHelp class="h-5 w-5" />
            <span class="sr-only">Help</span>
          </a>
        </Tooltip.Trigger>
        <Tooltip.Content side="right">Help</Tooltip.Content>
      </Tooltip.Root>
    </Tooltip.Provider>
  </nav>

  <!-- Bottom of side navigation bar -->
  <nav class="mt-auto flex flex-col items-center px-2 sm:py-5">
    <!-- Change theme button -->
    <Tooltip.Provider>
      <Tooltip.Root openDelay={400}>
        <Tooltip.Trigger>
          <Button onmousedown={toggleMode} variant="ghost" size="icon" class="text-muted-foreground hover:text-foreground my-4">
            <Sun class="rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
            <Moon class="absolute rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
            <span class="sr-only">Toggle theme</span>
          </Button>
        </Tooltip.Trigger>
        <Tooltip.Content side="right">Toggle theme</Tooltip.Content>
      </Tooltip.Root>
    </Tooltip.Provider>

    <!-- Logout button -->
    <Tooltip.Provider>
      <Tooltip.Root openDelay={400}>
        <Tooltip.Trigger>
          <Button onmousedown={handleLogout} variant="ghost" size="icon" class="text-muted-foreground hover:text-foreground">
            <LogOut class="size-5" />
            <span class="sr-only">Logout</span>
          </Button>
        </Tooltip.Trigger>
        <Tooltip.Content side="right">Logout</Tooltip.Content>
      </Tooltip.Root>
    </Tooltip.Provider>
  </nav>
</aside>
