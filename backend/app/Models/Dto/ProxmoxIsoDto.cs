namespace Dto;

public class ProxmoxIsoDto
{
    [JsonPropertyName("volid")]
    public string VolId { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("ctime")]
    public int Ctime { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name => VolId.Split(":iso/").Last();
}