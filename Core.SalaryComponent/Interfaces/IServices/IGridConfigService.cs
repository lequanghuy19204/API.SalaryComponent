using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IServices;

public interface IGridConfigService
{
    Task<IEnumerable<GridConfigDto>> GetByNameAsync(string gridName);
    Task SaveAsync(GridConfigSaveDto dto);
    Task DeleteByNameAsync(string gridName);
}
