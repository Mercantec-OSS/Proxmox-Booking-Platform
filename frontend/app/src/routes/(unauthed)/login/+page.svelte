<script>
  import { goto } from '$app/navigation';
  import { authService } from '$lib/services/auth-service';
  import { toast } from 'svelte-sonner';
  import { LoaderCircle } from 'lucide-svelte';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Card from '$lib/components/ui/card/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { Label } from '$lib/components/ui/label/index.js';

  let credentials = $state({ email: '', password: '' });
  /* Show spinner on submit button when awaiting create booking */
  let isLoading = $state(false);

  async function handleLogin(credentials) {
    try {
      isLoading = true;
      const response = await authService.login(credentials);
      if (response.token) {
        const expirationDate = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toUTCString();
        document.cookie = `token=${response.token}; expires=${expirationDate}; path=/;`;
      }
      await goto('/');
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }
</script>

<main class="flex flex-col flex-grow items-center justify-center">
  <form
    name="login"
    onsubmit={(e) => {
      e.preventDefault();
      handleLogin(credentials);
    }}
  >
    <Card.Root class="max-w-sm">
      <Card.Header>
        <Card.Title class="text-2xl">Login</Card.Title>
        <Card.Description>Enter your email below to login to your account</Card.Description>
      </Card.Header>
      <Card.Content>
        <div class="grid gap-4">
          <div class="grid gap-2">
            <Label for="email">Email</Label>
            <Input id="email" type="email" placeholder="m@example.com" autocomplete="username" required bind:value={credentials.email} />
          </div>
          <div class="grid gap-2">
            <div class="flex items-center">
              <Label for="password">Password</Label>
            </div>
            <Input id="password" type="password" autocomplete="current-password" required bind:value={credentials.password} />
          </div>
          <Button type="submit" class="w-full" disabled={isLoading}
            >{#if isLoading}
              <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
            {/if}
            Login</Button
          >
        </div>
        <div class="mt-4 text-center text-sm">
          Don't have an account?
          <a href="/register" class="underline hover:text-primary transition-colors">Sign up</a>
        </div>
      </Card.Content>
    </Card.Root>
  </form>
</main>
