<script>
  import '../../app.css';
  import Navbar from '$lib/components/unauthed/navbar.svelte';
  import { Toaster } from '$lib/components/ui/sonner';
  import { ModeWatcher } from 'mode-watcher';
</script>

<Toaster />
<ModeWatcher />

<div class="flex flex-col h-screen bg-background text-foreground">
  <Navbar />
  <slot />
</div>
