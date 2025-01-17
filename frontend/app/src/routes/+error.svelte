<script>
  import { page } from '$app/stores';
  import { ModeWatcher } from 'mode-watcher';
</script>

<ModeWatcher />

<main class="flex min-h-screen justify-center items-center bg-background text-foreground">
  <div class="text-center">
    <p class="text-5xl font-bold text-primary">{$page?.status}</p>
    <h1 class="mt-4 text-4xl font-bold tracking-tight sm:text-4xl">
      {$page?.error?.message}
    </h1>
    {#if $page?.status === 404}
      <p class="mt-6 text-lg lg:text-2xl leading-7">Sorry, the page doesn't exist</p>
    {:else}
      <p class="mt-6 text-lg lg:text-2xl leading-7">Sorry, an error occurred</p>
    {/if}

    <div class="mt-6 flex items-center justify-center gap-x-6">
      <a href="/" class="flex gap-x-2 items-center font-semibold text-xl hover:text-primary transition ease-in-out"><span aria-hidden="true">&larr;</span>Back to dashboard</a>
    </div>
  </div>
</main>
