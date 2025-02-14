namespace Dto;
public class ProxmoxNetworkStatsDto
{
    [JsonPropertyName("rx-bytes")]
    public long RxBytes { get; set; }

    [JsonPropertyName("rx-packets")]
    public long RxPackets { get; set; }

    [JsonPropertyName("tx-dropped")]
    public long TxDropped { get; set; }

    [JsonPropertyName("tx-errs")]
    public long TxErrs { get; set; }

    [JsonPropertyName("rx-errs")]
    public long RxErrs { get; set; }

    [JsonPropertyName("rx-dropped")]
    public long RxDropped { get; set; }

    [JsonPropertyName("tx-packets")]
    public long TxPackets { get; set; }

    [JsonPropertyName("tx-bytes")]
    public long TxBytes { get; set; }
}