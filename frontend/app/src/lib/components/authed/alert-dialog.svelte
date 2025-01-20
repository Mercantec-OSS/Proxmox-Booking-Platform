<script>
  import * as AlertDialog from '$lib/components/ui/alert-dialog';

  let { alertTitle, alertDescription, open, onNotify } = $props();

  function handleCancel() {
    onNotify?.({ confirmed: false });
    open = false;
  }

  function handleContinue() {
    onNotify?.({ confirmed: true });
    open = false;
  }
</script>

<AlertDialog.Root bind:open>
  <AlertDialog.Content>
    <AlertDialog.Header>
      <AlertDialog.Title>{alertTitle}</AlertDialog.Title>
      <AlertDialog.Description>{alertDescription}</AlertDialog.Description>
    </AlertDialog.Header>
    <AlertDialog.Footer>
      <AlertDialog.Cancel onmousedown={handleCancel}>Cancel</AlertDialog.Cancel>
      <AlertDialog.Action onclick={handleContinue}>Continue</AlertDialog.Action>
    </AlertDialog.Footer>
  </AlertDialog.Content>
</AlertDialog.Root>
