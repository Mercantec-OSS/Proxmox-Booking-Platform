<script>
  import '../../app.css';
  import Navbar from '$lib/components/authed/navbar.svelte';
  import { Toaster } from '$lib/components/ui/sonner';
  import { ModeWatcher } from 'mode-watcher';
</script>

<Toaster />
<ModeWatcher />

<div class="flex flex-col min-h-screen">
  <Navbar />
  <slot />
</div>
