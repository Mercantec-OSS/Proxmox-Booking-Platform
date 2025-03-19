namespace Dto;
public class ProxmoxStorageDto
{
        [JsonPropertyName("node")]
        public string Node { get; set; } = string.Empty;

        [JsonPropertyName("shared")]
        public int Shared { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("disk")]
        public long Disk { get; set; }

        [JsonPropertyName("maxdisk")]
        public long Maxdisk { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("storage")]
        public string Storage { get; set; } = string.Empty;

        [JsonPropertyName("plugintype")]
        public string Plugintype { get; set; } = string.Empty;

        [JsonPropertyName("usage-gb")]
        public int UsageGb => (int)(Disk / 1024 / 1024 / 1024);

        [JsonPropertyName("total-gb")]
        public int TotalGb => (int)(Maxdisk / 1024 / 1024 / 1024);

        [JsonPropertyName("usage")]
        public float Usage => (float)Disk / Maxdisk;
}