using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IServices;

/// <summary>
/// Interface Service xử lý nghiệp vụ thành phần lương đang sử dụng
/// Cung cấp các chức năng CRUD và quản lý trạng thái thành phần lương
/// </summary>
public interface ISalaryCompositionService
{
    /// <summary>
    /// Tạo mới một thành phần lương
    /// - Validate dữ liệu đầu vào
    /// - Kiểm tra trùng mã
    /// - Lưu vào database và liên kết với các đơn vị được chọn
    /// </summary>
    /// <param name="dto">Thông tin thành phần lương cần tạo</param>
    /// <returns>ID của thành phần lương vừa tạo</returns>
    Task<Guid> CreateAsync(SalaryCompositionCreateDto dto);

    /// <summary>
    /// Lấy thông tin chi tiết thành phần lương theo ID
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
    /// - Validate dữ liệu đầu vào
    /// - Kiểm tra trùng mã (loại trừ chính nó)
    /// - Cập nhật thông tin và liên kết đơn vị
    /// </summary>
    /// <param name="id">ID của thành phần lương cần cập nhật</param>
    /// <param name="dto">Thông tin mới của thành phần lương</param>
    /// <returns>True nếu cập nhật thành công</returns>
    Task<bool> UpdateAsync(Guid id, SalaryCompositionCreateDto dto);

    /// <summary>
    /// Xóa thành phần lương theo ID
    /// - Xóa các liên kết với đơn vị
    /// - Xóa bản ghi thành phần lương
    /// </summary>
    /// <param name="id">ID của thành phần lương cần xóa</param>
    /// <returns>True nếu xóa thành công</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Cập nhật trạng thái sử dụng của thành phần lương
    /// </summary>
    /// <param name="id">ID của thành phần lương</param>
    /// <param name="status">Trạng thái mới: 1 = Đang sử dụng, 0 = Ngừng sử dụng</param>
    /// <returns>True nếu cập nhật thành công</returns>
    Task<bool> UpdateStatusAsync(Guid id, int status);

    /// <summary>
    /// Cập nhật trạng thái hàng loạt cho nhiều thành phần lương
    /// Dùng cho tính năng chọn nhiều và thay đổi trạng thái cùng lúc
    /// </summary>
    /// <param name="ids">Danh sách ID các thành phần lương cần cập nhật</param>
    /// <param name="status">Trạng thái mới: 1 = Đang sử dụng, 0 = Ngừng sử dụng</param>
    Task BulkUpdateStatusAsync(List<Guid> ids, int status);

    /// <summary>
    /// Lấy danh sách thành phần lương có phân trang
    /// </summary>
    /// <param name="request">Thông tin phân trang và bộ lọc</param>
    /// <returns>Kết quả phân trang</returns>
    Task<PagedResultDto<SalaryCompositionDto>> GetPagedAsync(PagingRequestDto request);
}
