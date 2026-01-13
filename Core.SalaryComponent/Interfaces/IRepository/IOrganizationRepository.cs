using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IRepository;

public interface IOrganizationRepository
{
    Task<IEnumerable<OrganizationTreeDto>> GetTreeAsync();
}
