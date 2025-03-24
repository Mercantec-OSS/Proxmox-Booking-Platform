<script>
  import { LoaderCircle, BadgeCheck, ArrowLeft } from 'lucide-svelte';
  import { Button } from '$lib/components/ui/button/index.js';
  import * as Card from '$lib/components/ui/card/index.js';
  import { Input } from '$lib/components/ui/input/index.js';
  import { Label } from '$lib/components/ui/label/index.js';

  let { emailInput = $bindable(''), isLoading = $bindable(false), sendEmail = $bindable(() => {}) } = $props();
</script>

<Card.Root class="w-full max-w-sm">
  <Card.Header class="justify-center items-center">
    <BadgeCheck class="my-4" size={32} />
    <Card.Title class="text-2xl">All done!</Card.Title>
    <Card.Description class="text-center">Your password has been reset.</Card.Description>
  </Card.Header>
  <Card.Content>
    <Button href="/login" class="w-full">Login</Button>
  </Card.Content>
</Card.Root>
