public class ProxmoxVmInfoTimeFrame
{
    [JsonPropertyName("maxcpu")]
    public int MaxCpu { get; set; } = 0;

    [JsonPropertyName("time")]
    public int Time { get; set; }

    [JsonPropertyName("diskread")]
    public double DiskRead { get; set; }

    [JsonPropertyName("mem")]
    public double Mem { get; set; }

    [JsonPropertyName("netin")]
    public double NetIn { get; set; }

    [JsonPropertyName("disk")]
    public long Disk { get; set; }

    [JsonPropertyName("maxdisk")]
    public long MaxDisk { get; set; }

    [JsonPropertyName("netout")]
    public double NetOut { get; set; }

    [JsonPropertyName("maxmem")]
    public long MaxMem { get; set; }

    [JsonPropertyName("cpu")]
    public double Cpu { get; set; }

    [JsonPropertyName("diskwrite")]
    public double Diskwrite { get; set; }
}

