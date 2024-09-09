namespace Dto;

public class EsxiHostUpdateDto
{
    public int Id { get; set; }
    public int VCenterId { get; set; }
    public string Ip { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}