<script>
  import { scaleTime } from 'd3-scale';
  import { Area, Chart, Highlight, LinearGradient, RectClipPath, Svg, Tooltip } from 'layerchart';
  import { format, PeriodType } from '@layerstack/utils';

  let { usageInfo } = $props();

  const data = usageInfo.map((d) => {
    return {
      date: new Date(d.time * 1000),
      value: parseFloat(((d.mem / d.maxmem) * 100).toFixed(2))
    };
  });

  function formatDate(date) {
    const options = {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    };
    return date.toLocaleDateString('en-US', options);
  }
</script>

<div class="h-32 w-full">
  <Chart {data} x="date" xScale={scaleTime()} y="value" yNice padding={{ top: 48, bottom: 24 }} tooltip={{ mode: 'bisect-x' }} let:width let:height let:padding let:tooltip>
    <Svg>
      <LinearGradient class="from-primary/50 to-primary/0" vertical let:gradient>
        <Area line={{ class: 'stroke-2 stroke-primary opacity-20' }} fill={gradient} />
        <RectClipPath x={0} y={0} width={tooltip.data ? tooltip.x : width} height={Math.max(height, 1)} spring>
          <Area line={{ class: 'stroke-2 stroke-primary' }} fill={gradient} />
        </RectClipPath>
      </LinearGradient>
      <Highlight points lines={{ class: 'stroke-primary [stroke-dasharray:unset]' }} />
    </Svg>

    <Tooltip.Root y={48} xOffset={4} variant="none" class="text-sm font-semibold text-primary leading-3" let:data>
      {data.value}% RAM
    </Tooltip.Root>

    <Tooltip.Root
      x="data"
      y={height + padding.top + 2}
      anchor="top"
      variant="none"
      class="text-sm font-semibold bg-primary text-primary-content leading-3 px-2 py-1 rounded whitespace-nowrap"
      let:data
    >
      {formatDate(data.date)}
    </Tooltip.Root>
  </Chart>
</div>
