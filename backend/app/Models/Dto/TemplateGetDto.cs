namespace Dto;

public class TemplateGetDto
{
    [JsonPropertyName("internalName")]
    public string Name {  get; set; } = string.Empty;
    [JsonPropertyName("displayName")]
    public string DisplayName {  get; set; } = string.Empty;
    [JsonIgnore]
    public List<string> Keywords { get; set; } = new();

    static public TemplateGetDto MakeGetDTO(string template)
    {
        string internalName = template;
        string displayName = template.Split("--").First().Replace("_", " ");
        List<string> keywords = template.ToLower().Split("--").Last().Split("_").ToList();

        return new TemplateGetDto
        {
            Name = internalName,
            DisplayName = displayName,
            Keywords = keywords
        };
    }

    static public List<TemplateGetDto> MakeGetDtoFromList(List<string> templates)
    {
        return templates.ConvertAll(MakeGetDTO);
    }
}
