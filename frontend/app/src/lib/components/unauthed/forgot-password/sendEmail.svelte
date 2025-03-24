<script>
  import { LoaderCircle, Fingerprint, ArrowLeft } from 'lucide-svelte';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Card from '$lib/components/ui/card/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { Label } from '$lib/components/ui/label/index.js';

  let { emailInput = $bindable(''), isLoading = $bindable(false), sendEmail = $bindable(() => {}) } = $props();
</script>

<Card.Root class="w-full max-w-sm">
  <Card.Header class="justify-center items-center">
    <Fingerprint class="my-4" size={32} />
    <Card.Title class="text-2xl">Forgot password?</Card.Title>
    <Card.Description class="text-center">No worries, we'll send you reset instructions.</Card.Description>
  </Card.Header>
  <Card.Content>
    <form
      name="send email"
      onsubmit={(e) => {
        e.preventDefault();
        sendEmail();
      }}
    >
      <div class="grid gap-4">
        <div class="grid gap-2">
          <Label for="email">Email</Label>
          <Input id="email" type="email" placeholder="m@example.com" autocomplete="email" required bind:value={emailInput} />
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
