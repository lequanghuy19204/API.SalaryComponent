using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IRepository;

public interface ISalaryCompositionSystemRepository
{
    /// <summary>
    /// Lấy tất cả thành phần lương hệ thống
    /// </summary>
    Task<IEnumerable<SalaryCompositionSystemDto>> GetAllAsync();

    /// <summary>
    /// Lấy thành phần lương hệ thống theo ID
    /// </summary>
    Task<SalaryCompositionSystemDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Xóa thành phần lương hệ thống theo ID
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Lấy ID đơn vị gốc (cấp cao nhất - parent_id = NULL)
    /// </summary>
    Task<Guid?> GetRootOrganizationIdAsync();

    /// <summary>
    /// Kiểm tra mã đã tồn tại trong pa_salary_composition chưa
    /// </summary>
    Task<bool> IsCodeExistsInCompositionAsync(string code);

    /// <summary>
    /// Lấy ID của salary_composition theo code
    /// </summary>
    Task<Guid?> GetCompositionIdByCodeAsync(string code);
}
