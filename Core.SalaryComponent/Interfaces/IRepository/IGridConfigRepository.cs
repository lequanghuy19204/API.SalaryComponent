using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IRepository;

public interface IGridConfigRepository
{
    Task<IEnumerable<GridConfigDto>> GetByNameAsync(string gridName);
    Task SaveAsync(GridConfigSaveDto dto);
    Task DeleteByNameAsync(string gridName);
}
