<script>
  import '../../app.css';
  import Navbar from '$lib/components/unauthed/navbar.svelte';
  import { Toaster } from '$lib/components/ui/sonner';
  import { ModeWatcher } from 'mode-watcher';

  let { children } = $props();
</script>

<Toaster />
<ModeWatcher />

<div class="flex flex-col min-h-screen bg-background text-foreground">
  <Navbar />
  {@render children()}
</div>
