<script>
  import { goto, afterNavigate } from '$app/navigation';
  import { authService } from '$lib/services/auth-service';
  import { toast } from 'svelte-sonner';
  import { LoaderCircle, Fingerprint, MailOpen, RectangleEllipsis, BadgeCheck, ArrowLeft } from 'lucide-svelte';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Card from '$lib/components/ui/card/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { Label } from '$lib/components/ui/label/index.js';
  import SendEmail from '$lib/components/unauthed/forgot-password/sendEmail.svelte';
  import ValidateOtp from '$lib/components/unauthed/forgot-password/validate-otp.svelte';
  import NewPassword from '$lib/components/unauthed/forgot-password/new-password.svelte';
  import ResetDone from '$lib/components/unauthed/forgot-password/reset-done.svelte';

  let emailInput = $state('');
  let otpInput = $state('');
  let passwordInput = $state('');
  let confirmPasswordInput = $state('');
  let step = $state(1);

  /* Show spinner on submit button when awaiting create booking */
  let isLoading = $state(false);

  async function sendEmail() {
    try {
      isLoading = true;
      await authService.sendPasswordResetEmail(emailInput);
      toast.success('Email sent! Check your inbox for OTP code.');
      step = 2;
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }

  async function validateOtp() {
    try {
      isLoading = true;
      await authService.validateToken(otpInput, emailInput);
      step = 3;
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }

  async function resetPassword() {
    if (passwordInput.length < 6) {
      toast.error('Password must be at least 6 characters long.');
      return;
    }

    if (passwordInput !== confirmPasswordInput) {
      toast.error('Passwords do not match.');
      return;
    }

    try {
      isLoading = true;
      await authService.resetPassword(otpInput, emailInput, passwordInput);
      step = 4;
    } catch (error) {
      toast.error(error.message);
    } finally {
      isLoading = false;
    }
  }
</script>

<main class="flex flex-col flex-grow items-center justify-center gap-y-4">
  <!-- Visual to show progress in resetting password -->
  <div class="flex items-center w-full max-w-sm justify-between">
    {#each [1, 2, 3, 4], index}
      <div class="w-20 h-2 rounded-lg {step === index + 1 ? 'bg-primary' : 'bg-muted'}"></div>
    {/each}
  </div>

  {#if step === 1}
    <SendEmail bind:emailInput bind:isLoading {sendEmail} />
  {:else if step === 2}
    <ValidateOtp bind:otpInput bind:isLoading {emailInput} {validateOtp} {sendEmail} />
  {:else if step === 3}
    <NewPassword bind:passwordInput bind:confirmPasswordInput bind:isLoading {resetPassword} />
  {:else if step === 4}
    <ResetDone />
  {/if}
</main>
