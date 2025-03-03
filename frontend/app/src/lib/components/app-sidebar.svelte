<script>
  import { BookOpen, House } from 'lucide-svelte';
  import { page } from '$app/state';
  import NavMain from '$lib/components/nav-main.svelte';
  import NavUser from '$lib/components/nav-user.svelte';
  import * as Sidebar from '$lib/components/ui/sidebar/index.js';
  import UserSearch from '$lib/components/authed/user-search.svelte';

  let { ref = $bindable(null), collapsible = 'icon', user, ...restProps } = $props();

  function isActive(href) {
    return page.url.pathname === href;
  }
  const data = {
    navMain: [
      {
        title: 'Home',
        url: '/',
        icon: House,
        isActive: isActive('/')
      },
      {
        title: 'Booking Guide',
        url: 'https://mars.merhot.dk/w/index.php/Booking-guide',
        icon: BookOpen
      }
    ]
  };
</script>

<Sidebar.Root bind:ref {collapsible} {...restProps}>
  <Sidebar.Header>
    <Sidebar.Menu>
      <Sidebar.MenuItem>
        <Sidebar.MenuButton size="lg">
          {#snippet child({ props })}
            <a href="/" {...props}>
              <div class=" text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
                <img
                  src="/images/mercantec-logo.svg"
                  alt="Mercantec"
                  class="h-6 w-6 transition-all [filter:brightness(0)_saturate(100%)_invert(39%)_sepia(93%)_saturate(1699%)_hue-rotate(2deg)_brightness(103%)_contrast(106%)]"
                />
              </div>
              <div class="grid flex-1 text-left text-sm leading-tight">
                <span class="truncate font-semibold">Mercantec</span>
                <span class="truncate text-xs">Booking System</span>
              </div>
            </a>
          {/snippet}
        </Sidebar.MenuButton>
      </Sidebar.MenuItem>
    </Sidebar.Menu>
    <UserSearch />
  </Sidebar.Header>
  <Sidebar.Content>
    <NavMain items={data.navMain} />
  </Sidebar.Content>
  <Sidebar.Footer>
    {#if user}
      <NavUser {user} />
    {/if}
  </Sidebar.Footer>
  <Sidebar.Rail />
</Sidebar.Root>
