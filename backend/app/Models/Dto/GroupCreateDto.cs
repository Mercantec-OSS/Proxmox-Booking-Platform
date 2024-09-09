namespace Dto;

public class GroupCreateDto
{
    [JsonPropertyName("name")]
    public string Name {  get; set; } = string.Empty;
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
