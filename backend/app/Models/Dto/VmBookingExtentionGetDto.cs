namespace Dto;

public class VmBookingExtentionGetDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("owner")]
    public UserGetDto? Owner { get; set; }

    [JsonPropertyName("assigned")]
    public UserGetDto? Assigned { get; set; }

    [JsonPropertyName("bookingId")]
    public int BookingId { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";

    [JsonPropertyName("isAccepted")]
    public bool IsAccepted { get; set; }

    [JsonPropertyName("newExpiringAt")]
    public DateTime NewExpiringAt { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("expiredAt")]
    public DateTime ExpiredAt { get; set; }

}
