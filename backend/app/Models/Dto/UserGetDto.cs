namespace Dto;

public class UserGetDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = "";

    [JsonPropertyName("groupId")]
    public int? GroupId { get; set; }

    [JsonPropertyName("creationAt")]
    public DateTime CreatedAt { get; set; }
}
