namespace Core.SalaryComponent.DTOs;

public class OrganizationTreeDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<OrganizationTreeDto>? Items { get; set; }
}
