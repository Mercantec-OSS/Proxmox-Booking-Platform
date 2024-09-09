<script>
  import { goto } from '$app/navigation';
  import { authService } from '$lib/services/auth-service';
  import { Label } from '$lib/components/ui/label';
  import { Input } from '$lib/components/ui/input';
  import { Button } from '$lib/components/ui/button/index.js';
  import { toast } from 'svelte-sonner';
  import { LoaderCircle } from 'lucide-svelte';

  let credentials = { email: '', password: '' };
  /* Show spinner on submit button when awaiting create booking */
  let isLoading = false;

  async function handleLogin(credentials) {
    try {
      isLoading = true;
      const response = await authService.login(credentials);
      if (response.hash) document.cookie = `token=${response.hash}; path=/; expires=${new Date(Date.now() + 1 * 24 * 60 * 60 * 1000).toUTCString()};`;
      await goto('/');
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }
</script>

<main class="flex flex-col h-screen">
  <div class="flex flex-col flex-grow justify-center items-center w-full">
    <h1 class="text-4xl text-center font-bold text-highlight">Login</h1>
    <div class="p-4 max-w-sm w-full">
      <form on:submit|preventDefault={handleLogin(credentials)} class="grid items-start gap-4">
        <div class="grid gap-2">
          <Label for="email">Email</Label>
          <Input type="email" autocapitalize="none" autocomplete="email" autocorrect="off" id="email" required bind:value={credentials.email} />
        </div>
        <div class="grid gap-2">
          <Label for="password">Password</Label>
          <Input type="password" autocapitalize="none" autocomplete="password" autocorrect="off" id="password" required bind:value={credentials.password} />
        </div>
        <Button type="submit" disabled={isLoading}
          >{#if isLoading}
            <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
          {/if}
          Log in</Button
        >
      </form>

      <p class="text-center mt-4 text-sm">Don't have an account? <span><a href="/register" class="font-semibold underline hover:text-highlight transition ease-in-out">Create one</a></span></p>
    </div>
  </div>
</main>
