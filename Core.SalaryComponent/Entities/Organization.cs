namespace Core.SalaryComponent.Entities;

public class Organization
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string OrganizationCode { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public List<Organization>? Items { get; set; }
}
