namespace Dto;
public class ProxmoxTaskDto
{
    [JsonPropertyName("node")]
    public string Node { get; set; } = string.Empty;

    [JsonPropertyName("upid")]
    public string Upid { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public string User { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("starttime")]
    public int Starttime { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string? Id { get; set; } = string.Empty;

    [JsonPropertyName("saved")]
    public JsonElement Saved { get; set; }

    [JsonPropertyName("endtime")]
    public int Endtime { get; set; }

    [JsonPropertyName("is-finished")]
    public bool IsFinished => Endtime != 0;

    [JsonPropertyName("successful")]
    public bool Successful => Status == "OK" && IsFinished && Saved.ToString() == "1";
}