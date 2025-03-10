public class ProxmoxVmInfoTimeFrameIn
{
    [JsonPropertyName("maxcpu")]
    public double MaxCpu { get; set; } = 0;

    [JsonPropertyName("time")]
    public double Time { get; set; }

    [JsonPropertyName("diskread")]
    public double DiskRead { get; set; }

    [JsonPropertyName("mem")]
    public double Mem { get; set; }

    [JsonPropertyName("netin")]
    public double NetIn { get; set; }

    [JsonPropertyName("disk")]
    public double Disk { get; set; }

    [JsonPropertyName("maxdisk")]
    public double MaxDisk { get; set; }

    [JsonPropertyName("netout")]
    public double NetOut { get; set; }

    [JsonPropertyName("maxmem")]
    public double MaxMem { get; set; }

    [JsonPropertyName("cpu")]
    public double Cpu { get; set; }

    [JsonPropertyName("diskwrite")]
    public double Diskwrite { get; set; }
}

