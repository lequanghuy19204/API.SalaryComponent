using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;
using Core.SalaryComponent.Interfaces.IServices;

namespace Core.SalaryComponent.Services;

public class SalaryCompositionSystemService : ISalaryCompositionSystemService
{
    private readonly ISalaryCompositionSystemRepository _systemRepository;
    private readonly ISalaryCompositionRepository _compositionRepository;

    public SalaryCompositionSystemService(
        ISalaryCompositionSystemRepository systemRepository,
        ISalaryCompositionRepository compositionRepository)
    {
        _systemRepository = systemRepository;
        _compositionRepository = compositionRepository;
    }

    public async Task<IEnumerable<SalaryCompositionSystemDto>> GetAllAsync()
    {
        return await _systemRepository.GetAllAsync();
    }

    public async Task<SalaryCompositionSystemDto?> GetByIdAsync(Guid id)
    {
        return await _systemRepository.GetByIdAsync(id);
    }

    public async Task<bool> CheckCodeExistsAsync(Guid id)
    {
        var systemItem = await _systemRepository.GetByIdAsync(id);
        if (systemItem == null)
        {
            throw new InvalidOperationException("Không tìm thấy thành phần lương hệ thống");
        }

        return await _systemRepository.IsCodeExistsInCompositionAsync(systemItem.SalaryCompositionSystemCode);
    }

    public async Task<Guid> MoveToCompositionAsync(Guid id)
    {
        var systemItem = await _systemRepository.GetByIdAsync(id);
        if (systemItem == null)
        {
            throw new InvalidOperationException("Không tìm thấy thành phần lương hệ thống");
        }

        var rootOrgId = await _systemRepository.GetRootOrganizationIdAsync();
        if (rootOrgId == null)
        {
            throw new InvalidOperationException("Không tìm thấy đơn vị gốc trong hệ thống");
        }

        var createDto = BuildCreateDto(systemItem, rootOrgId.Value);
        var newId = await _compositionRepository.CreateAsync(createDto);
        await _systemRepository.DeleteAsync(id);

        return newId;
    }

    public async Task<Guid> OverwriteToCompositionAsync(Guid id)
    {
        var systemItem = await _systemRepository.GetByIdAsync(id);
        if (systemItem == null)
        {
            throw new InvalidOperationException("Không tìm thấy thành phần lương hệ thống");
        }

        var existingId = await _systemRepository.GetCompositionIdByCodeAsync(systemItem.SalaryCompositionSystemCode);
        if (existingId == null)
        {
            throw new InvalidOperationException("Không tìm thấy thành phần lương cần ghi đè");
        }

        var rootOrgId = await _systemRepository.GetRootOrganizationIdAsync();
        if (rootOrgId == null)
        {
            throw new InvalidOperationException("Không tìm thấy đơn vị gốc trong hệ thống");
        }

        var updateDto = BuildCreateDto(systemItem, rootOrgId.Value);
        await _compositionRepository.UpdateAsync(existingId.Value, updateDto);
        await _systemRepository.DeleteAsync(id);

        return existingId.Value;
    }

    private static SalaryCompositionCreateDto BuildCreateDto(SalaryCompositionSystemDto systemItem, Guid rootOrgId)
    {
        return new SalaryCompositionCreateDto
        {
            SalaryCompositionCode = systemItem.SalaryCompositionSystemCode,
            SalaryCompositionName = systemItem.SalaryCompositionSystemName,
            SalaryCompositionType = systemItem.SalaryCompositionSystemType,
            SalaryCompositionNature = systemItem.SalaryCompositionSystemNature,
            SalaryCompositionTaxOption = systemItem.SalaryCompositionSystemTaxOption ?? "taxable",
            SalaryCompositionTaxDeduction = systemItem.SalaryCompositionSystemTaxDeduction,
            SalaryCompositionQuota = systemItem.SalaryCompositionSystemQuota,
            SalaryCompositionAllowExceedQuota = systemItem.SalaryCompositionSystemAllowExceedQuota,
            SalaryCompositionValueType = systemItem.SalaryCompositionSystemValueType,
            SalaryCompositionValueCalculation = systemItem.SalaryCompositionSystemValueCalculation,
            SalaryCompositionSumScope = systemItem.SalaryCompositionSystemSumScope,
            SalaryCompositionOrgLevel = systemItem.SalaryCompositionSystemOrgLevel,
            SalaryCompositionComponentToSum = systemItem.SalaryCompositionSystemComponentToSum,
            SalaryCompositionValueFormula = systemItem.SalaryCompositionSystemValueFormula,
            SalaryCompositionDescription = systemItem.SalaryCompositionSystemDescription,
            SalaryCompositionShowOnPayslip = systemItem.SalaryCompositionSystemShowOnPayslip,
            SalaryCompositionSource = "system",
            SalaryCompositionStatus = 1,
            OrganizationIds = new List<Guid> { rootOrgId }
        };
    }
}
