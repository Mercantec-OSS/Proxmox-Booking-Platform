namespace Dto;
public class ProxmoxNetworkIpDto
{
    [JsonPropertyName("ip-address-type")]
    public string IpAddressType { get; set; } = string.Empty;

    [JsonPropertyName("prefix")]
    public int Prefix { get; set; }

    [JsonPropertyName("ip-address")]
    public string IpAddress { get; set; } = string.Empty;

    [JsonIgnore]
    public bool IsIpv4 => IpAddressType == "ipv4";

    [JsonIgnore]
    public bool IsIpv6 => IpAddressType == "ipv6";
}