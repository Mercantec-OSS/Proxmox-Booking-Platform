<script>
  import '../../app.css';
  import Topbar from '$lib/components/authed/topbar.svelte';
  import Sidebar from '$lib/components/authed/sidebar.svelte';
  import { Toaster } from '$lib/components/ui/sonner';
  import { ModeWatcher } from 'mode-watcher';

  let { children } = $props();
</script>

<Toaster />
<ModeWatcher />

<div class="flex flex-col bg-background text-foreground">
  <Sidebar />
  <div class="flex flex-col min-h-screen w-full sm:py-4 sm:pl-14">
    <Topbar />
    {@render children()}
  </div>
</div>
