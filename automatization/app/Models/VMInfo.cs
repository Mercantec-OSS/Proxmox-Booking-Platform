namespace VMwareBookingSystem.Models;

public class VMInfo {
    [JsonPropertyName("ip")]
    public string Ip { get; set; }
    [JsonPropertyName("username")]
    public string UserName { get; set; }
    [JsonPropertyName("password")]
    public string Password { get; set; }
}