namespace Core.SalaryComponent.Entities;

public class SalaryCompositionOrganization
{
    public Guid Id { get; set; }
    public Guid SalaryCompositionId { get; set; }
    public Guid OrganizationId { get; set; }
    public DateTime CreatedDate { get; set; }
}
