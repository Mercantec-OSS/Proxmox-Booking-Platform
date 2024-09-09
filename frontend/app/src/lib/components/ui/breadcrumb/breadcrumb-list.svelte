<script>
	import { cn } from "$lib/utils/utils.js";
	export let el = undefined;
	let className = undefined;
	export { className as class };
</script>

<ol
	bind:this={el}
	class={cn(
		"flex flex-wrap items-center gap-1.5 break-words text-sm text-muted-foreground sm:gap-2.5",
		className
	)}
	{...$$restProps}
>
	<slot />
</ol>
