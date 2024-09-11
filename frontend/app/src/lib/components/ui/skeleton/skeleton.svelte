<script>
	import { cn } from "$lib/utils/utils.js";
	let className = undefined;
	export { className as class };
</script>

<div class={cn("bg-muted animate-pulse rounded-md", className)} {...$$restProps}></div>
