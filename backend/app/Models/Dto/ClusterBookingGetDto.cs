namespace Dto;

public class ClusterBookingGetDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("owner")]
    public UserGetDto? Owner { get; set; }

    [JsonPropertyName("amountStudents")]
    public int AmountStudents { get; set; }

    [JsonPropertyName("vCenters")]
    public List<VCenterGetDto> VCenters { get; set; } = [];

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("expiredAt")]
    public DateTime ExpiredAt { get; set; }
}
