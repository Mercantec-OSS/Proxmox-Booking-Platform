<script>
  import { env } from '$env/dynamic/public';
</script>

<main class="flex h-screen justify-center items-center">
  <div class="text-center">
    <p class="text-5xl font-bold text-highlight">{env.PUBLIC_VERSION}</p>

    <div class="mt-6 flex items-center justify-center gap-x-6">
      <a href="/" class="flex gap-x-2 items-center font-semibold text-xl text-gray-900 dark:text-white hover:text-highlight transition ease-in-out"
        ><span aria-hidden="true">&larr;</span>Back to dashboard</a
      >
    </div>
  </div>
</main>
