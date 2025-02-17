namespace Dto;

public class IsoDto
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";

    public static IsoDto GetFromProxmoxIso(ProxmoxIsoDto proxmoxIso)
    {
        return new IsoDto()
        {
            Name = proxmoxIso.Name,
            Path = ""
        };
    }
}