public class VCenterInfoDTO
{
    public int TotalHosts { get; set; } = 0;
    public int ActiveHosts { get; set; } = 0;
    public string CpuTotal { get; set; } = "";
    public string CpuUsage { get; set; } = "";
    public string RamTotal { get; set; } = "";
    public string RamUsage { get; set; } = "";
    public string StorageTotal { get; set; } = "";
    public string StorageUsage { get; set; } = "";
    public int AmountVMs { get; set; } = 0;
    public int AmountTemplates { get; set; } = 0;

    static public VCenterInfoDTO FromCommandOutput(string output) {
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