<script>
  import { goto } from '$app/navigation';
  import { authService } from '$lib/services/auth-service';
  import { Label } from '$lib/components/ui/label';
  import { Input } from '$lib/components/ui/input';
  import { Button } from '$lib/components/ui/button/index.js';
  import { toast } from 'svelte-sonner';
  import { LoaderCircle } from 'lucide-svelte';

  let credentials = { name: '', surname: '', email: '', password: '' };
  /* Show spinner on submit button when awaiting create booking */
  let isLoading = false;

  async function handleRegister(credentials) {
    try {
      isLoading = true;
      await authService.register(credentials);

      const loginResponse = await authService.login(credentials);
      if (loginResponse.token) {
        document.cookie = `token=${loginResponse.token}; path=/; expires=${new Date(Date.now() + 1 * 24 * 60 * 60 * 1000).toUTCString()};`;

        await goto('/');
      }
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }
</script>

<main class="flex flex-col h-screen">
  <div class="flex flex-col flex-grow justify-center items-center w-full">
    <h1 class="text-4xl text-center font-bold text-highlight">Register</h1>
    <div class="p-4 max-w-sm w-full">
      <form on:submit|preventDefault={handleRegister(credentials)} class="grid items-start gap-4">
        <div class="grid grid-cols-2 gap-2">
          <div class="grid gap-2">
            <Label for="firstName" class="px-2">First Name</Label>
            <Input type="text" id="firstName" bind:value={credentials.name} required />
          </div>
          <div class="grid gap-2">
            <Label for="lastName" class="px-2">Last Name</Label>
            <Input type="text" id="lastName" bind:value={credentials.surname} required />
          </div>
        </div>
        <div class="grid gap-2">
          <Label for="email">Email</Label>
          <Input type="email" autocapitalize="none" autocomplete="email" autocorrect="off" id="email" bind:value={credentials.email} required />
        </div>
        <div class="grid gap-2">
          <Label for="password">Password</Label>
          <Input type="password" autocapitalize="none" autocomplete="password" autocorrect="off" id="password" bind:value={credentials.password} required />
        </div>
        <Button type="submit" disabled={isLoading}
          >{#if isLoading}
            <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
          {/if}
          Create account</Button
        >
      </form>
      <p class="text-center mt-4 text-sm">Already have an account? <span><a href="/login" class="font-semibold underline hover:text-highlight transition ease-in-out">Login</a></span></p>
    </div>
  </div>
</main>
