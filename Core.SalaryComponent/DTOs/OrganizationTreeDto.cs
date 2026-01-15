namespace Core.SalaryComponent.DTOs;

public class OrganizationTreeDto
{
    public string OrganizationId { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public List<OrganizationTreeDto>? Items { get; set; }
}
