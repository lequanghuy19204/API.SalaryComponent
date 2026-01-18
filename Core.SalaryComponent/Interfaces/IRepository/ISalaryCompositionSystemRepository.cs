using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IRepository;

/// <summary>
/// Interface Repository quản lý thành phần lương mặc định của hệ thống (pa_salary_composition_system)
/// Đây là danh mục thành phần lương được MISA định nghĩa sẵn, người dùng có thể chọn để đưa vào sử dụng
/// </summary>
public interface ISalaryCompositionSystemRepository
{
    /// <summary>
    /// Lấy tất cả thành phần lương mặc định của hệ thống
    /// </summary>
    /// <returns>Danh sách thành phần lương hệ thống</returns>
    Task<IEnumerable<SalaryCompositionSystemDto>> GetAllAsync();

    /// <summary>
    /// Lấy thành phần lương hệ thống theo ID
    /// </summary>
    /// <param name="id">ID của thành phần lương hệ thống</param>
    /// <returns>Thông tin thành phần lương hoặc null nếu không tìm thấy</returns>
    Task<SalaryCompositionSystemDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Xóa thành phần lương hệ thống theo ID
    /// Thường được gọi sau khi đã di chuyển sang danh sách sử dụng
    /// </summary>
    /// <param name="id">ID của thành phần lương hệ thống cần xóa</param>
    /// <returns>True nếu xóa thành công</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Lấy ID đơn vị gốc (cấp cao nhất trong cây tổ chức - parent_id = NULL)
    /// Dùng để gán đơn vị mặc định khi di chuyển thành phần lương từ system sang composition
    /// </summary>
    /// <returns>ID của đơn vị gốc hoặc null nếu không tìm thấy</returns>
    Task<Guid?> GetRootOrganizationIdAsync();

    /// <summary>
    /// Kiểm tra mã thành phần lương đã tồn tại trong bảng pa_salary_composition chưa
    /// Dùng để cảnh báo trùng mã trước khi di chuyển
    /// </summary>
    /// <param name="code">Mã thành phần lương cần kiểm tra</param>
    /// <returns>True nếu mã đã tồn tại trong danh sách sử dụng</returns>
    Task<bool> IsCodeExistsInCompositionAsync(string code);

    /// <summary>
    /// Lấy ID của thành phần lương trong pa_salary_composition theo mã
    /// Dùng khi cần ghi đè (overwrite) thành phần lương đã tồn tại
    /// </summary>
    /// <param name="code">Mã thành phần lương cần tìm</param>
    /// <returns>ID của thành phần lương hoặc null nếu không tìm thấy</returns>
    Task<Guid?> GetCompositionIdByCodeAsync(string code);

    /// <summary>
    /// Lấy danh sách thành phần lương hệ thống có phân trang với bộ lọc nâng cao
    /// </summary>
    /// <param name="request">DTO chứa thông tin phân trang và bộ lọc</param>
    /// <returns>Kết quả phân trang thành phần lương hệ thống</returns>
    Task<PagedResultDto<SalaryCompositionSystemDto>> GetPagedAsync(SystemPagingRequestDto request);
}
