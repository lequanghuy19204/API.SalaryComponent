using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IServices;

public interface ISalaryCompositionSystemService
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
    /// Kiểm tra mã đã tồn tại trong pa_salary_composition chưa
    /// </summary>
    Task<bool> CheckCodeExistsAsync(Guid id);

    /// <summary>
    /// Di chuyển thành phần lương từ system sang composition
    /// - Copy dữ liệu sang pa_salary_composition
    /// - Gán đơn vị gốc (cấp cao nhất)
    /// - Xóa khỏi pa_salary_composition_system
    /// </summary>
    Task<Guid> MoveToCompositionAsync(Guid id);

    /// <summary>
    /// Ghi đè thành phần lương đã tồn tại
    /// - Cập nhật dữ liệu từ system sang composition đã tồn tại
    /// - Xóa khỏi pa_salary_composition_system
    /// </summary>
    Task<Guid> OverwriteToCompositionAsync(Guid id);
}
