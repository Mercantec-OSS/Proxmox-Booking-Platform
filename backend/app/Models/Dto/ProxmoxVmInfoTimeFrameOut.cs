public class ProxmoxVmInfoTimeFrameOut
{
    [JsonPropertyName("maxcpu")]
    public int MaxCpu { get; set; } = 0;

    [JsonPropertyName("time")]
    public int Time { get; set; }

    [JsonPropertyName("diskread")]
    public long DiskRead { get; set; }

    [JsonPropertyName("mem")]
    public long Mem { get; set; }

    [JsonPropertyName("netin")]
    public long NetIn { get; set; }

    [JsonPropertyName("disk")]
    public long Disk { get; set; }

    [JsonPropertyName("maxdisk")]
    public long MaxDisk { get; set; }

    [JsonPropertyName("netout")]
    public long NetOut { get; set; }

    [JsonPropertyName("maxmem")]
    public long MaxMem { get; set; }

    [JsonPropertyName("cpu")]
    public float Cpu { get; set; }

    [JsonPropertyName("diskwrite")]
    public long Diskwrite { get; set; }

    public static ProxmoxVmInfoTimeFrameOut GetFromIn(ProxmoxVmInfoTimeFrameIn timeFrameIn)
    {
        return new ProxmoxVmInfoTimeFrameOut
        {
            MaxCpu = (int) Math.Round(timeFrameIn.MaxCpu),
            Time = (int) Math.Round(timeFrameIn.Time),
            DiskRead = (long) Math.Round(timeFrameIn.DiskRead),
            Mem = (long) Math.Round(timeFrameIn.Mem),
            NetIn = (long) Math.Round(timeFrameIn.NetIn),
            Disk = (long) Math.Round(timeFrameIn.Disk),
            MaxDisk = (long) Math.Round(timeFrameIn.MaxDisk),
            NetOut = (long) Math.Round(timeFrameIn.NetOut),
            MaxMem = (long) Math.Round(timeFrameIn.MaxMem),
            Cpu = (float) Math.Round(timeFrameIn.Cpu, 5),
            Diskwrite = (long) Math.Round(timeFrameIn.Diskwrite)
        };
    }
}

