namespace Dto;

public class VmBookingGetDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("owner")]
    public UserGetDto? Owner { get; set; }

    [JsonPropertyName("assigned")]
    public UserGetDto? Assigned { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("isAccepted")]
    public bool IsAccepted { get; set; } = false;

    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("extentions")]
    public List<VmBookingExtentionGetDto> Extentions { get; set; } = new List<VmBookingExtentionGetDto>();

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("expiredAt")]
    public DateTime ExpiredAt { get; set; }
}
