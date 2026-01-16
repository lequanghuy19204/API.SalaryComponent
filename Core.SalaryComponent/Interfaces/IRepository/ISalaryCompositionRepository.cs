using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IRepository;

/// <summary>
/// Interface Repository quản lý thành phần lương đang sử dụng (pa_salary_composition)
/// Cung cấp các phương thức CRUD và nghiệp vụ liên quan đến thành phần lương
/// </summary>
public interface ISalaryCompositionRepository
{
    /// <summary>
    /// Tạo mới một thành phần lương
    /// </summary>
    /// <param name="dto">Thông tin thành phần lương cần tạo</param>
    /// <returns>ID của thành phần lương vừa tạo</returns>
    Task<Guid> CreateAsync(SalaryCompositionCreateDto dto);

    /// <summary>
    /// Lấy thông tin thành phần lương theo ID
    /// </summary>
    /// <param name="id">ID của thành phần lương</param>
    /// <returns>Thông tin thành phần lương hoặc null nếu không tìm thấy</returns>
    Task<SalaryCompositionDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Lấy danh sách tất cả thành phần lương đang sử dụng
    /// </summary>
    /// <returns>Danh sách thành phần lương</returns>
    Task<IEnumerable<SalaryCompositionDto>> GetAllAsync();

    /// <summary>
    /// Cập nhật thông tin thành phần lương
    /// </summary>
    /// <param name="id">ID của thành phần lương cần cập nhật</param>
    /// <param name="dto">Thông tin mới của thành phần lương</param>
    /// <returns>True nếu cập nhật thành công, False nếu không tìm thấy</returns>
    Task<bool> UpdateAsync(Guid id, SalaryCompositionCreateDto dto);

    /// <summary>
    /// Xóa thành phần lương theo ID
    /// </summary>
    /// <param name="id">ID của thành phần lương cần xóa</param>
    /// <returns>True nếu xóa thành công, False nếu không tìm thấy</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Kiểm tra mã thành phần lương đã tồn tại trong hệ thống chưa
    /// </summary>
    /// <param name="code">Mã thành phần lương cần kiểm tra</param>
    /// <param name="excludeId">ID cần loại trừ (dùng khi cập nhật để không kiểm tra chính nó)</param>
    /// <returns>True nếu mã đã tồn tại, False nếu chưa</returns>
    Task<bool> IsCodeExistsAsync(string code, Guid? excludeId = null);

    /// <summary>
    /// Cập nhật trạng thái (status) của một thành phần lương
    /// Status: 1 = Đang sử dụng, 0 = Ngừng sử dụng
    /// </summary>
    /// <param name="id">ID của thành phần lương</param>
    /// <param name="status">Trạng thái mới (0 hoặc 1)</param>
    /// <returns>True nếu cập nhật thành công</returns>
    Task<bool> UpdateStatusAsync(Guid id, int status);

    /// <summary>
    /// Cập nhật trạng thái hàng loạt cho nhiều thành phần lương
    /// </summary>
    /// <param name="ids">Danh sách ID các thành phần lương cần cập nhật</param>
    /// <param name="status">Trạng thái mới (0 hoặc 1)</param>
    Task BulkUpdateStatusAsync(List<Guid> ids, int status);

    /// <summary>
    /// Lấy danh sách thành phần lương có phân trang
    /// </summary>
    /// <param name="request">Thông tin phân trang và bộ lọc</param>
    /// <returns>Kết quả phân trang</returns>
    Task<PagedResultDto<SalaryCompositionDto>> GetPagedAsync(PagingRequestDto request);
}
