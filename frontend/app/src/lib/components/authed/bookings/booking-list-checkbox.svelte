<script>
  import { Checkbox } from '$lib/components/ui/checkbox/index.js';

  let { checked = false, onCheckedChange = (v) => (checked = v), ...restProps } = $props();
</script>

<Checkbox bind:checked={() => checked, onCheckedChange} {...restProps} />
