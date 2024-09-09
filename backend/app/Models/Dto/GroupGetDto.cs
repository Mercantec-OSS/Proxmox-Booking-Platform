namespace Dto;

public class GroupGetDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name {  get; set; } = string.Empty;
    public List<UserGetDto> Members { get; set; } = new();
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
