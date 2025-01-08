namespace Dto;

public class ResponseVmIpDto
{
    [JsonPropertyName("mac_address")]
    public string MacAddress { get; set; } = "";

    [JsonPropertyName("ip")]
    public IPInfo IP { get; set; } = new IPInfo();

    public class IPInfo
    {
        [JsonPropertyName("ip_addresses")]
        public List<IPAddressInfo> IPAddresses { get; set; } = new List<IPAddressInfo>();
        public class IPAddressInfo
        {
            [JsonPropertyName("ip_address")]
            public string IPAddress { get; set; } = "";

            [JsonPropertyName("prefix_length")]
            public int PrefixLength { get; set; } = 0;

        }
    }
}