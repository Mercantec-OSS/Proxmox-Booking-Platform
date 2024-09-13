namespace Dto
{
    public class VCenterInfoDTO
    {
        [JsonPropertyName("totalHosts")]
        public int TotalHosts { get; set; } = 0;

        [JsonPropertyName("activeHosts")]
        public int ActiveHosts { get; set; } = 0;

        [JsonPropertyName("cpuTotal")]
        public string CpuTotal { get; set; } = "";

        [JsonPropertyName("cpuUsage")]
        public string CpuUsage { get; set; } = "";

        [JsonPropertyName("ramTotal")]
        public string RamTotal { get; set; } = "";

        [JsonPropertyName("ramUsage")]
        public string RamUsage { get; set; } = "";

        [JsonPropertyName("storageTotal")]
        public string StorageTotal { get; set; } = "";

        [JsonPropertyName("storageUsage")]
        public string StorageUsage { get; set; } = "";

        [JsonPropertyName("amountVMs")]
        public int AmountVMs { get; set; } = 0;

        [JsonPropertyName("amountTemplates")]
        public int AmountTemplates { get; set; } = 0;
    }
}
