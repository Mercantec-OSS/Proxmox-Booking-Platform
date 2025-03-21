<script>
  import { LoaderCircle, RectangleEllipsis, ArrowLeft } from 'lucide-svelte';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Card from '$lib/components/ui/card/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { Label } from '$lib/components/ui/label/index.js';

  let { passwordInput = $bindable(''), confirmPasswordInput = $bindable(''), isLoading = $bindable(false), resetPassword = $bindable(() => {}) } = $props();
</script>

<Card.Root class="w-full max-w-sm">
  <Card.Header class="justify-center items-center">
    <RectangleEllipsis class="my-4" size={32} />
    <Card.Title class="text-2xl">Set new password</Card.Title>
    <Card.Description class="text-center">Must be at least 6 characters.</Card.Description>
  </Card.Header>
  <Card.Content>
    <form
      name="new password"
      onsubmit={(e) => {
        e.preventDefault();
        resetPassword();
      }}
    >
      <div class="grid gap-4">
        <div class="grid gap-2">
          <Label for="password">Password</Label>
          <Input id="password" type="password" placeholder="••••••••" autocomplete="password" required bind:value={passwordInput} />
        </div>

        <div class="grid gap-2">
          <Label for="confirmPassword">Confirm password</Label>
          <Input id="confirmPassword" type="password" placeholder="••••••••" autocomplete="confirm-password" required bind:value={confirmPasswordInput} />
        </div>
        <Button type="submit" class="w-full" disabled={isLoading}
          >{#if isLoading}
            <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
          {/if}
          Reset password</Button
        >
      </div>
    </form>
  </Card.Content>
  <Card.Footer class="justify-center items-center">
    <a href="/login" class="text-center text-sm hover:text-primary transition-colors flex items-center"><ArrowLeft class="mr-2 h-4 w-4" /> Back to login </a>
  </Card.Footer>
</Card.Root>
