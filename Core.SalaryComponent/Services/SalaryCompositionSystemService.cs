using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;
using Core.SalaryComponent.Interfaces.IServices;

namespace Core.SalaryComponent.Services;

/// <summary>
/// Service xử lý nghiệp vụ thành phần lương mặc định của hệ thống
/// </summary>
public class SalaryCompositionSystemService : ISalaryCompositionSystemService
{
    private readonly ISalaryCompositionSystemRepository _systemRepository;
    private readonly ISalaryCompositionRepository _compositionRepository;

    /// <summary>
    /// Khởi tạo service với các repository
    /// </summary>
    /// <param name="systemRepository">Repository thành phần lương hệ thống</param>
    /// <param name="compositionRepository">Repository thành phần lương đang sử dụng</param>
    public SalaryCompositionSystemService(
        ISalaryCompositionSystemRepository systemRepository,
        ISalaryCompositionRepository compositionRepository)
    {
        _systemRepository = systemRepository;
        _compositionRepository = compositionRepository;
    }

    /// <summary>
    /// Lấy tất cả thành phần lương hệ thống
    /// </summary>
    /// <returns>Danh sách thành phần lương hệ thống</returns>
    public async Task<IEnumerable<SalaryCompositionSystemDto>> GetAllAsync()
    {
        return await _systemRepository.GetAllAsync();
    }

    /// <summary>
    /// Lấy thành phần lương hệ thống theo ID
    /// </summary>
    /// <param name="id">ID thành phần lương hệ thống</param>
    /// <returns>Thông tin thành phần lương hệ thống</returns>
    public async Task<SalaryCompositionSystemDto?> GetByIdAsync(Guid id)
    {
        return await _systemRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Kiểm tra mã đã tồn tại trong danh sách sử dụng
    /// </summary>
    /// <param name="id">ID thành phần lương hệ thống</param>
    /// <returns>True nếu mã đã tồn tại, False nếu chưa</returns>
    /// <exception cref="InvalidOperationException">Ném ra khi không tìm thấy thành phần lương hệ thống</exception>
    public async Task<bool> CheckCodeExistsAsync(Guid id)
    {
        var systemItem = await _systemRepository.GetByIdAsync(id);
        if (systemItem == null)
        {
            throw new InvalidOperationException("Không tìm thấy thành phần lương hệ thống");
        }

        return await _systemRepository.IsCodeExistsInCompositionAsync(systemItem.SalaryCompositionSystemCode);
    }

    /// <summary>
    /// Di chuyển từ hệ thống sang danh sách sử dụng
    /// </summary>
    /// <param name="id">ID thành phần lương hệ thống</param>
    /// <returns>ID của thành phần lương mới trong danh sách sử dụng</returns>
    /// <exception cref="InvalidOperationException">Ném ra khi không tìm thấy thành phần lương hoặc đơn vị gốc</exception>
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

    /// <summary>
    /// Ghi đè thành phần lương đã tồn tại
    /// </summary>
    /// <param name="id">ID thành phần lương hệ thống</param>
    /// <returns>ID của thành phần lương đã được ghi đè</returns>
    /// <exception cref="InvalidOperationException">Ném ra khi không tìm thấy thành phần lương cần ghi đè hoặc đơn vị gốc</exception>
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

    /// <summary>
    /// Xây dựng DTO tạo mới từ dữ liệu hệ thống
    /// </summary>
    /// <param name="systemItem">Thành phần lương hệ thống</param>
    /// <param name="rootOrgId">ID đơn vị gốc</param>
    /// <returns>DTO tạo mới thành phần lương</returns>
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

    /// <summary>
    /// Di chuyển nhiều thành phần lương
    /// </summary>
    /// <param name="ids">Danh sách ID thành phần lương hệ thống</param>
    /// <returns>Kết quả di chuyển gồm số lượng thành công, thất bại và các mã bị bỏ qua</returns>
    /// <exception cref="InvalidOperationException">Ném ra khi không tìm thấy đơn vị gốc</exception>
    public async Task<MoveMultipleResultDto> MoveMultipleToCompositionAsync(List<Guid> ids)
    {
        var result = new MoveMultipleResultDto
        {
            SuccessCount = 0,
            FailedCount = 0,
            SkippedCodes = new List<string>()
        };

        var rootOrgId = await _systemRepository.GetRootOrganizationIdAsync();
        if (rootOrgId == null)
        {
            throw new InvalidOperationException("Không tìm thấy đơn vị gốc trong hệ thống");
        }

        foreach (var id in ids)
        {
            try
            {
                var systemItem = await _systemRepository.GetByIdAsync(id);
                if (systemItem == null) continue;

                var codeExists = await _systemRepository.IsCodeExistsInCompositionAsync(systemItem.SalaryCompositionSystemCode);
                if (codeExists)
                {
                    result.SkippedCodes.Add(systemItem.SalaryCompositionSystemCode);
                    result.FailedCount++;
                    continue;
                }

                var createDto = BuildCreateDto(systemItem, rootOrgId.Value);
                await _compositionRepository.CreateAsync(createDto);
                await _systemRepository.DeleteAsync(id);
                result.SuccessCount++;
            }
            catch
            {
                result.FailedCount++;
            }
        }

        return result;
    }

    /// <summary>
    /// Lấy danh sách có phân trang với bộ lọc nâng cao
    /// </summary>
    /// <param name="request">DTO chứa thông tin phân trang và bộ lọc</param>
    /// <returns>Kết quả phân trang</returns>
    public async Task<PagedResultDto<SalaryCompositionSystemDto>> GetPagedAsync(SystemPagingRequestDto request)
    {
        return await _systemRepository.GetPagedAsync(request);
    }
}
