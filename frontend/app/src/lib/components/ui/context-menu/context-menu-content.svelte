<script>
	import { ContextMenu as ContextMenuPrimitive } from "bits-ui";
	import { cn, flyAndScale } from "$lib/utils/utils.js";
	let className = undefined;
	export let transition = flyAndScale;
	export let transitionConfig = undefined;
	export { className as class };
</script>

<ContextMenuPrimitive.Content
	{transition}
	{transitionConfig}
	class={cn(
		"bg-popover text-popover-foreground z-50 min-w-[8rem] rounded-md border p-1 shadow-md focus:outline-none",
		className
	)}
	{...$$restProps}
	on:keydown
>
	<slot />
</ContextMenuPrimitive.Content>
