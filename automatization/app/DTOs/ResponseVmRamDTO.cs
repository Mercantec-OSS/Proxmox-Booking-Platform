public class ResponseVmRamDTO
{
    [JsonPropertyName("size_MiB")]
    public int SizeMiB { get; set; } = 0;
    public int SizeGB => SizeMiB / 1024;
}