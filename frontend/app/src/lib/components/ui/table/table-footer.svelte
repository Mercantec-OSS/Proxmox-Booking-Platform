<script>
	import { cn } from "$lib/utils/utils.js";
	let className = undefined;
	export { className as class };
</script>

<tfoot class={cn("bg-primary font-medium text-primary-foreground", className)} {...$$restProps}>
	<slot />
</tfoot>
