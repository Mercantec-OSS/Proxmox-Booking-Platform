namespace Dto;

public class VmUpdateResourcesDto
{
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = "";
    [JsonPropertyName("cpu")]
    public int Cpu { get; set; }
    [JsonPropertyName("ram")]
    public int Ram { get; set; }
}
