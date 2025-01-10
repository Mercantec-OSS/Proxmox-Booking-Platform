<script>
  import * as AlertDialog from '$lib/components/ui/alert-dialog';
  import { createEventDispatcher } from 'svelte';

  let { alertTitle, alertDescription, open = false } = $props();
  const dispatch = createEventDispatcher();

  function handleCancel() {
    dispatch('notify', { confirmed: false });
  }

  function handleContinue() {
    dispatch('notify', { confirmed: true });
  }
</script>

<AlertDialog.Root bind:open>
  <AlertDialog.Content>
    <AlertDialog.Header>
      <AlertDialog.Title>{alertTitle}</AlertDialog.Title>
      <AlertDialog.Description>{alertDescription}</AlertDialog.Description>
    </AlertDialog.Header>
    <AlertDialog.Footer>
      <AlertDialog.Cancel onclick={handleCancel}>Cancel</AlertDialog.Cancel>
      <AlertDialog.Action onclick={handleContinue}>Continue</AlertDialog.Action>
    </AlertDialog.Footer>
  </AlertDialog.Content>
</AlertDialog.Root>
