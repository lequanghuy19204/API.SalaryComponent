using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Exceptions;
using Core.SalaryComponent.Interfaces.IServices;
using Core.SalaryComponent.Interfaces.IRepository;

namespace Core.SalaryComponent.Services;

public class SalaryCompositionService : ISalaryCompositionService
{
    private readonly ISalaryCompositionRepository _repository;

    public SalaryCompositionService(ISalaryCompositionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateAsync(SalaryCompositionCreateDto dto)
    {
        var isCodeExists = await _repository.IsCodeExistsAsync(dto.Code);
        if (isCodeExists)
        {
            throw DuplicateException.WithField("Mã thành phần", dto.Code);
        }

        return await _repository.CreateAsync(dto);
    }

    public async Task<SalaryCompositionDto?> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
        {
            throw NotFoundException.WithEntity("Thành phần lương", id);
        }
        return result;
    }

    public async Task<IEnumerable<SalaryCompositionDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<bool> UpdateAsync(Guid id, SalaryCompositionCreateDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw NotFoundException.WithEntity("Thành phần lương", id);
        }

        var isCodeExists = await _repository.IsCodeExistsAsync(dto.Code, id);
        if (isCodeExists)
        {
            throw DuplicateException.WithField("Mã thành phần", dto.Code);
        }

        return await _repository.UpdateAsync(id, dto);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw NotFoundException.WithEntity("Thành phần lương", id);
        }

        return await _repository.DeleteAsync(id);
    }
}
