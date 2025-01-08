namespace Dto;
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

    static public VCenterInfoDTO FromCommandOutput(string output)
    {
        string[] lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        VCenterInfoDTO dto = new VCenterInfoDTO();
        foreach (var line in lines)
        {
            if (line.StartsWith("total_hosts:"))
            {
                dto.TotalHosts = int.Parse(line.Split(':')[1].Trim());
            }
            else if (line.StartsWith("active_hosts:"))
            {
                dto.ActiveHosts = int.Parse(line.Split(':')[1].Trim());
            }
            else if (line.StartsWith("cpu_total:"))
            {
                dto.CpuTotal = line.Split(':')[1].Trim();
            }
            else if (line.StartsWith("cpu_usage:"))
            {
                dto.CpuUsage = line.Split(':')[1].Trim();
            }
            else if (line.StartsWith("ram_total:"))
            {
                dto.RamTotal = line.Split(':')[1].Trim();
            }
            else if (line.StartsWith("ram_usage:"))
            {
                dto.RamUsage = line.Split(':')[1].Trim();
            }
            else if (line.StartsWith("storage_total:"))
            {
                dto.StorageTotal = line.Split(':')[1].Trim();
            }
            else if (line.StartsWith("storage_usage:"))
            {
                dto.StorageUsage = line.Split(':')[1].Trim();
            }
            else if (line.StartsWith("total_vms:"))
            {
                dto.AmountVMs = int.Parse(line.Split(':')[1].Trim());
            }
            else if (line.StartsWith("total_templates:"))
            {
                dto.AmountTemplates = int.Parse(line.Split(':')[1].Trim());
            }
        }

        return dto;
    }
}
