using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IServices;

/// <summary>
/// Interface Service xử lý nghiệp vụ thành phần lương mặc định của hệ thống
/// Cung cấp các chức năng xem danh mục và di chuyển thành phần lương từ hệ thống sang danh sách sử dụng
/// </summary>
public interface ISalaryCompositionSystemService
{
    /// <summary>
    /// Lấy tất cả thành phần lương mặc định của hệ thống
    /// Đây là danh sách các thành phần lương được MISA định nghĩa sẵn
    /// </summary>
    /// <returns>Danh sách thành phần lương hệ thống</returns>
    Task<IEnumerable<SalaryCompositionSystemDto>> GetAllAsync();

    /// <summary>
    /// Lấy thông tin chi tiết thành phần lương hệ thống theo ID
    /// </summary>
    /// <param name="id">ID của thành phần lương hệ thống</param>
    /// <returns>Thông tin thành phần lương hoặc null nếu không tìm thấy</returns>
    Task<SalaryCompositionSystemDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Kiểm tra mã thành phần lương đã tồn tại trong danh sách sử dụng chưa
    /// Dùng để cảnh báo người dùng trước khi di chuyển
    /// </summary>
    /// <param name="id">ID của thành phần lương hệ thống cần kiểm tra</param>
    /// <returns>True nếu mã đã tồn tại trong pa_salary_composition</returns>
    Task<bool> CheckCodeExistsAsync(Guid id);

    /// <summary>
    /// Di chuyển thành phần lương từ danh mục hệ thống sang danh sách sử dụng
    /// Quy trình:
    /// 1. Copy dữ liệu từ pa_salary_composition_system sang pa_salary_composition
    /// 2. Gán đơn vị gốc (cấp cao nhất) cho thành phần lương mới
    /// 3. Xóa bản ghi khỏi pa_salary_composition_system
    /// </summary>
    /// <param name="id">ID của thành phần lương hệ thống cần di chuyển</param>
    /// <returns>ID của thành phần lương mới trong pa_salary_composition</returns>
    Task<Guid> MoveToCompositionAsync(Guid id);

    /// <summary>
    /// Ghi đè thành phần lương đã tồn tại trong danh sách sử dụng
    /// Dùng khi mã thành phần lương đã tồn tại và người dùng chọn ghi đè
    /// Quy trình:
    /// 1. Cập nhật dữ liệu từ pa_salary_composition_system sang bản ghi đã tồn tại
    /// 2. Xóa bản ghi khỏi pa_salary_composition_system
    /// </summary>
    /// <param name="id">ID của thành phần lương hệ thống cần ghi đè</param>
    /// <returns>ID của thành phần lương đã được cập nhật</returns>
    Task<Guid> OverwriteToCompositionAsync(Guid id);

    /// <summary>
    /// Di chuyển nhiều thành phần lương từ hệ thống sang danh sách sử dụng cùng lúc
    /// - Bỏ qua các thành phần có mã đã tồn tại (không ghi đè tự động)
    /// - Trả về kết quả chi tiết: số thành công, số thất bại, danh sách mã bị bỏ qua
    /// </summary>
    /// <param name="ids">Danh sách ID các thành phần lương hệ thống cần di chuyển</param>
    /// <returns>Kết quả di chuyển bao gồm số lượng thành công/thất bại và danh sách mã bị bỏ qua</returns>
    Task<MoveMultipleResultDto> MoveMultipleToCompositionAsync(List<Guid> ids);

    /// <summary>
    /// Lấy danh sách thành phần lương hệ thống có phân trang với bộ lọc nâng cao
    /// </summary>
    /// <param name="request">DTO chứa thông tin phân trang và bộ lọc</param>
    /// <returns>Kết quả phân trang</returns>
    Task<PagedResultDto<SalaryCompositionSystemDto>> GetPagedAsync(SystemPagingRequestDto request);
}
