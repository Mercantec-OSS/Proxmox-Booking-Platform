namespace Dto;
public class ProxmoxVncDto
{
    [JsonPropertyName("user")]
    public string User { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("cert")]
    public string Cert { get; set; } = string.Empty;

    [JsonPropertyName("upid")]
    public string Upid { get; set; } = string.Empty;

    [JsonPropertyName("port")]
    public string Port { get; set; } = string.Empty;

    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = string.Empty;
}