<script>
	import { cn } from "$lib/utils/utils.js";
	export let el = undefined;
	export let className = undefined;
	export { className as class };
</script>

<span
	bind:this={el}
	role="link"
	aria-disabled="true"
	aria-current="page"
	class={cn("font-normal text-foreground", className)}
	{...$$restProps}
>
	<slot />
</span>
