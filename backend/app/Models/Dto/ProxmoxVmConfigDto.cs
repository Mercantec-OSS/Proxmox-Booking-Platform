public class ProxmoxVmConfigDto     {
        [JsonPropertyName("ide2")]
        public string Ide2 { get; set; } = string.Empty;

        [JsonPropertyName("boot")]
        public string Boot { get; set; } = string.Empty;

        [JsonPropertyName("ostype")]
        public string Ostype { get; set; } = string.Empty;

        [JsonPropertyName("net0")]
        public string Net0 { get; set; } = string.Empty;

        [JsonPropertyName("scsi1")]
        public string Scsi1 { get; set; } = string.Empty;

        [JsonPropertyName("digest")]
        public string Digest { get; set; } = string.Empty;

        [JsonPropertyName("sockets")]
        public int Sockets { get; set; }

        [JsonPropertyName("meta")]
        public string Meta { get; set; } = string.Empty;

        [JsonPropertyName("smbios1")]
        public string Smbios1 { get; set; } = string.Empty;

        [JsonPropertyName("vga")]
        public string Vga { get; set; } = string.Empty;

        [JsonPropertyName("cores")]
        public int Cores { get; set; }

        [JsonPropertyName("numa")]
        public int Numa { get; set; }

        [JsonPropertyName("scsihw")]
        public string ScsiHw { get; set; } = string.Empty;

        [JsonPropertyName("vmgenid")]
        public string VmGenId { get; set; } = string.Empty;

        [JsonPropertyName("machine")]
        public string Machine { get; set; } = string.Empty;

        [JsonPropertyName("ide0")]
        public string Ide0 { get; set; } = string.Empty;

        [JsonPropertyName("memory")]
        public string Memory { get; set; } = string.Empty;

        [JsonPropertyName("cpu")]
        public string Cpu { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        public bool ExtraStorageExists => !string.IsNullOrEmpty(Scsi1); 
    }