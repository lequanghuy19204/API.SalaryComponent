using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IServices;

public interface ISalaryCompositionService
{
    Task<Guid> CreateAsync(SalaryCompositionCreateDto dto);
    Task<SalaryCompositionDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<SalaryCompositionDto>> GetAllAsync();
    Task<bool> UpdateAsync(Guid id, SalaryCompositionCreateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
