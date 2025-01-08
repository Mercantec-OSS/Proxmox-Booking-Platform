namespace Dto;

public class VmInfoGetDto
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("cpu")]
    public int Cpu { get; set; }
    [JsonPropertyName("ram")]
    public int Ram { get; set; }
}
