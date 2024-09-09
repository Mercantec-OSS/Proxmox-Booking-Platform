namespace Dto;

public class VCenterGetDto
{

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("ip")]
    public string Ip { get; set; } = "";

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = "";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "";

    [JsonPropertyName("bookingId")]
    public int? BookingId { get; set; } = null;
    [JsonPropertyName("esxiHosts")]
    public List<EsxiHostGetDto> EsxiHosts { get; set; } = new List<EsxiHostGetDto>();

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}