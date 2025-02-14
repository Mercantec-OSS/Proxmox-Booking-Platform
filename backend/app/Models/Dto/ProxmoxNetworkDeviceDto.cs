namespace Dto;
public class ProxmoxNetworkDeviceDto
{
    [JsonPropertyName("statistics")]
    public ProxmoxNetworkStatsDto Statistics { get; set; } = new();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("hardware-address")]
    public string HardwareAddress { get; set; } = string.Empty;

    [JsonPropertyName("ip-addresses")]
    public List<ProxmoxNetworkIpDto> IpAddresses { get; set; } = new();
}