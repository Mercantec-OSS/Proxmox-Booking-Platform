namespace Dto;

public class VCenterUpdateDto
{
    public int Id { get; set; }
    public string Ip { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string DatacenterName { get; set; } = "";
    public string ClusterName { get; set; } = "";
    public string JsonConfig { get; set; } = "";
}