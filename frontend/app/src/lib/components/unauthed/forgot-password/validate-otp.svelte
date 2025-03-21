<script>
  import { LoaderCircle, MailOpen, ArrowLeft } from 'lucide-svelte';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Card from '$lib/components/ui/card/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { Label } from '$lib/components/ui/label/index.js';
  import * as InputOTP from '$lib/components/ui/input-otp/index.js';
  import { REGEXP_ONLY_DIGITS } from 'bits-ui';

  let { otpInput = $bindable(''), isLoading = $bindable(false), emailInput, validateOtp = $bindable(() => {}), sendEmail = $bindable(() => {}) } = $props();
</script>

<Card.Root class="w-full max-w-sm">
  <Card.Header class="justify-center items-center">
    <MailOpen class="my-4" size={32} />
    <Card.Title class="text-2xl">Password reset</Card.Title>
    <Card.Description class="text-center">We sent a code to<span class="font-bold">{emailInput}</span></Card.Description>
  </Card.Header>
  <Card.Content>
    <form
      name="validate otp"
      onsubmit={(e) => {
        e.preventDefault();
        validateOtp();
      }}
    >
      <div class="grid gap-4">
        <div class="flex justify-center">
          <InputOTP.Root maxlength={5} pattern={REGEXP_ONLY_DIGITS} bind:value={otpInput}>
            {#snippet children({ cells })}
              <InputOTP.Group>
                {#each cells as cell (cell)}
                  <InputOTP.Slot {cell} />
                {/each}
              </InputOTP.Group>
            {/snippet}
          </InputOTP.Root>
        </div>
        <Button type="submit" class="w-full" disabled={isLoading}
          >{#if isLoading}
            <LoaderCircle class="mr-2 h-4 w-4 animate-spin" />
          {/if}
          Continue</Button
        >
      </div>
    </form>

    <p class="text-center text-xs text-muted-foreground mt-4">
      Didn't receive the email? <Button onclick={sendEmail} variant="link" size="xs" class="text-xs">Click to resend</Button>
    </p>
  </Card.Content>
  <Card.Footer class="justify-center items-center">
    <a href="/login" class="text-center text-sm hover:text-primary transition-colors flex items-center"><ArrowLeft class="mr-2 h-4 w-4" /> Back to login </a>
  </Card.Footer>
</Card.Root>
