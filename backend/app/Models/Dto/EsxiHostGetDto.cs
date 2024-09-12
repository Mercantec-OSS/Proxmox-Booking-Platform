namespace Dto;

public class EsxiHostGetDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("vCenterId")]
    public int? VCenterId { get; set; }

    [JsonPropertyName("bookingId")]
    public int? BookingId { get; set; } = null;

    [JsonPropertyName("ip")]
    public string Ip { get; set; } = "";

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = "";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "";

    [JsonPropertyName("datastore_name")]
    public string DatastoreName { get; set; } = "";

    [JsonPropertyName("network_name")]
    public string NetworkName { get; set; } = "";

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}