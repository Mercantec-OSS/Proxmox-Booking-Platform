namespace Dto;

public class ResponseVCenterTicketDto
{
    [JsonPropertyName("ticket")]
    public string Uri { get; set; } = "";
    [JsonPropertyName("vcenterIp")]
    public string VcenterIp { get; set; } = "";
}