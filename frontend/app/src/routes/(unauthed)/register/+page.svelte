<script>
  import { goto } from '$app/navigation';
  import { authService } from '$lib/services/auth-service';
  import { Label } from '$lib/components/ui/label';
  import { Input } from '$lib/components/ui/input';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Card from '$lib/components/ui/card/index.js';
  import { toast } from 'svelte-sonner';
  import { LoaderCircle } from 'lucide-svelte';

  let credentials = $state({ name: '', surname: '', email: '', password: '' });
  let isLoading = $state(false);

  async function handleRegister(credentials) {
    try {
      isLoading = true;
      await authService.register(credentials);

      const loginResponse = await authService.login(credentials);
      if (loginResponse.token) {
        document.cookie = `token=${loginResponse.token}; path=/; expires=${new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toUTCString()};`;

        await goto('/');
      }
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }
</script>

<main class="flex flex-col flex-grow items-center justify-center">
  <form
    name="registration"
    onsubmit={(e) => {
      e.preventDefault();
      handleRegister(credentials);
    }}
  >
    <Card.Root class="max-w-sm">
      <Card.Header>
        <Card.Title class="text-xl">Sign Up</Card.Title>
        <Card.Description>Enter your information to create an account</Card.Description>
      </Card.Header>
      <Card.Content>
        <div class="grid gap-4">
          <div class="grid grid-cols-2 gap-4">
            <div class="grid gap-2">
              <Label for="first-name">First name</Label>
              <Input id="first-name" placeholder="Max" name="given-name" required bind:value={credentials.name} />
            </div>
            <div class="grid gap-2">
              <Label for="last-name">Last name</Label>
              <Input id="last-name" placeholder="Robinson" name="family-name" required bind:value={credentials.surname} />
            </div>
          </div>
          <div class="grid gap-2">
            <Label for="email">Email</Label>
            <Input id="email" type="email" placeholder="m@example.com" autocomplete="email" required bind:value={credentials.email} />
          </div>
          <div class="grid gap-2">
            <Label for="password">Password</Label>
            <Input id="password" type="password" autocomplete="new-password" bind:value={credentials.password} />
          </div>
          <Button type="submit" class="w-full" disabled={isLoading}
            >{#if isLoading}
              <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
            {/if}
            Create an account</Button
          >
        </div>
        <div class="mt-4 text-center text-sm">
          Already have an account?
          <a href="/" class="underline hover:text-primary transition-colors"> Sign in </a>
        </div>
      </Card.Content>
    </Card.Root>
  </form>
</main>
