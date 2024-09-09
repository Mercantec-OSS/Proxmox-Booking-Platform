namespace Dto;

public class EsxiHostCreateDto
{
    public int VCenterId { get; set; }
    public string Ip { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}