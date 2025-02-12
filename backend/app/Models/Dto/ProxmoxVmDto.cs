namespace Dto;
public class ProxmoxVmDto
{
    [JsonPropertyName("uptime")]
    public long UpTime { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("netin")]
    public long NetIn { get; set; }
    
    [JsonPropertyName("maxdisk")]
    public long MaxDisk { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("node")]
    public string Node { get; set; } = string.Empty;
    
    [JsonPropertyName("maxcpu")]
    public long MaxCPU { get; set; }
    
    [JsonPropertyName("mem")]
    public long Mem { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public string TagsRaw { get; set; } = string.Empty;

    [JsonIgnore]
    public List<string> Tags => TagsRaw.Split(';').ToList();
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("diskread")]
    public long DiskRead { get; set; }
    
    [JsonPropertyName("disk")]
    public long Disk { get; set; }
    
    [JsonPropertyName("maxmem")]
    public long MaxMem { get; set; }
    
    [JsonPropertyName("diskwrite")]
    public long DiskWrite { get; set; }
    
    [JsonPropertyName("cpu")]
    public float CPU { get; set; }
    
    [JsonPropertyName("template")]
    public int Template { get; set; }
    
    [JsonPropertyName("netout")]
    public long NetOut { get; set; }
    
    [JsonPropertyName("vmid")]
    public int VmId { get; set; }

    [JsonPropertyName("mem-usage")]
    public float MemUsage => (float) Mem / MaxMem;

    [JsonPropertyName("uptime-text")]
    public string UpTimeText => TimeSpan.FromSeconds(UpTime).ToString();
}