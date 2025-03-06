<script>
  import { scaleTime } from 'd3-scale';
  import { Area, Axis, Chart, Highlight, LinearGradient, RectClipPath, Svg, Tooltip } from 'layerchart';
  import { format, PeriodType } from '@layerstack/utils';
  import appleStock from './apple-stock.json';

  const data = appleStock.map((d) => {
    return { date: new Date(d.date), value: d.value };
  });
</script>

<div class="h-44 w-44">
  <Chart {data} x="date" xScale={scaleTime()} y="value" yDomain={[0, null]} yNice padding={{ top: 48, bottom: 24 }} tooltip={{ mode: 'bisect-x' }} let:width let:height let:padding let:tooltip>
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
      {format(data.value, 'currency')}
    </Tooltip.Root>

    <Tooltip.Root
      x="data"
      y={height + padding.top + 2}
      anchor="top"
      variant="none"
      class="text-sm font-semibold bg-primary text-primary-content leading-3 px-2 py-1 rounded whitespace-nowrap"
      let:data
    >
      {format(data.date, PeriodType.Day)}
    </Tooltip.Root>
  </Chart>
</div>
