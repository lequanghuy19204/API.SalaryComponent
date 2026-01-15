using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IRepository;

public interface ISalaryCompositionRepository
{
    Task<Guid> CreateAsync(SalaryCompositionCreateDto dto);
    Task<SalaryCompositionDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<SalaryCompositionDto>> GetAllAsync();
    Task<bool> UpdateAsync(Guid id, SalaryCompositionCreateDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> IsCodeExistsAsync(string code, Guid? excludeId = null);
    Task<bool> UpdateStatusAsync(Guid id, int status);
    Task BulkUpdateStatusAsync(List<Guid> ids, int status);
}
