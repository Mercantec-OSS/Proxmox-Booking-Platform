namespace Dto;

public class ResponseVCenterVmDto
{
    [JsonPropertyName("memory_size_MiB")]
    public int RamMemory { get; set; } = 0;
    [JsonPropertyName("vm")]
    public string InternName { get; set; } = "";
    [JsonPropertyName("name")]
    public string ExternName { get; set; } = "";
    [JsonPropertyName("power_state")]
    public string PowerState { get; set; } = "";
    [JsonPropertyName("cpu_count")]
    public int CpuCount { get; set; } = 0;
}