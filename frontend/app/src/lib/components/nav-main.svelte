<script>
  import * as Sidebar from '$lib/components/ui/sidebar/index.js';

  let { items } = $props();
</script>

<Sidebar.Group>
  <Sidebar.GroupLabel>Dashboard</Sidebar.GroupLabel>
  <Sidebar.Menu>
    {#each items as item (item.title)}
      <Sidebar.MenuItem>
        {#snippet child({ props })}
          <a href={item.url} {...props}>
            <Sidebar.MenuButton class={item.isActive ? 'active' : ''}>
              {#snippet tooltipContent()}
                {item.title}
              {/snippet}
              {#if item.icon}
                <item.icon />
              {/if}
              <span>{item.title}</span>
            </Sidebar.MenuButton>
          </a>
        {/snippet}
      </Sidebar.MenuItem>
    {/each}
  </Sidebar.Menu>
</Sidebar.Group>
