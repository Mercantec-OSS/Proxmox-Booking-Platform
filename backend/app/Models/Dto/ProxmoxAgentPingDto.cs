namespace Dto;
public class ProxmoxAgentPingDto
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}