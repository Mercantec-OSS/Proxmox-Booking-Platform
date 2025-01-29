namespace Dto;
public class ProxmoxNodeDto
{
    [JsonPropertyName("maxdisk")]
    public long MaxDisk { get; set; }

    [JsonPropertyName("uptime")]
    public int UpTime { get; set; }

    [JsonPropertyName("level")]
    public string Level { get; set; } = String.Empty;

    [JsonPropertyName("disk")]
    public long Disk { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = String.Empty;

    [JsonPropertyName("node")]
    public string Node { get; set; } = String.Empty;

    [JsonPropertyName("maxcpu")]
    public int MaxCpu { get; set; }

    [JsonPropertyName("cgroup-mode")]
    public int CGroupMode { get; set; }

    [JsonPropertyName("hastate")]
    public string HaState { get; set; } = String.Empty;

    [JsonPropertyName("mem")]
    public long Mem { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = String.Empty;

    [JsonPropertyName("maxmem")]
    public long MaxMem { get; set; }

    [JsonPropertyName("cpu")]
    public float Cpu { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = String.Empty;

    [JsonPropertyName("mem-usage")]
    public float MemUsage => (float) Mem / MaxMem;

    [JsonPropertyName("disk-usage")]
    public float DiskUsage => (float) Disk / MaxDisk;

    [JsonPropertyName("uptime-text")]
    public string UpTimeText => TimeSpan.FromSeconds(UpTime).ToString();

    [JsonPropertyName("load-index")]
    public float LoadIndex => (float)(0.5 * Cpu + 0.5 * MemUsage);

    [JsonPropertyName("is-online")]
    public bool IsOnline => Status == "online";

    [JsonPropertyName("is-maintainance")]
    public bool IsMaintainance => HaState == "maintainance";

    [JsonPropertyName("is-offline")]
    public bool IsOffline => Status == "offline";

    [JsonPropertyName("allowed-bookings")]
    public bool ReadyForBookings => IsOnline && HaState == "online";
}