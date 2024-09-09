<script>
	import { cn } from "$lib/utils/utils.js";
	let className = undefined;
	export { className as class };
</script>

<span class={cn("ml-auto text-xs tracking-widest opacity-60", className)} {...$$restProps}>
	<slot />
</span>
