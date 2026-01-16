using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;
using Core.SalaryComponent.Interfaces.IServices;

namespace Core.SalaryComponent.Services;

public class GridConfigService : IGridConfigService
{
    private readonly IGridConfigRepository _repository;

    public GridConfigService(IGridConfigRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GridConfigDto>> GetByNameAsync(string gridName)
    {
        return await _repository.GetByNameAsync(gridName);
    }

    public async Task SaveAsync(GridConfigSaveDto dto)
    {
        await _repository.SaveAsync(dto);
    }

    public async Task DeleteByNameAsync(string gridName)
    {
        await _repository.DeleteByNameAsync(gridName);
    }
}
