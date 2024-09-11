namespace DTOs;

public class CreateVmDTO {
    public string Name { get; set; } = "";
    public string Template { get; set; } = "";
    public string RootPassword { get; set; } = "";
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
}