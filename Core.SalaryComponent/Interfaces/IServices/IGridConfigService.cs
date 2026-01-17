using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IServices;

/// <summary>
/// Interface Service xử lý nghiệp vụ cấu hình lưới.
/// Cung cấp các phương thức xử lý logic nghiệp vụ cho cấu hình lưới.
/// </summary>
public interface IGridConfigService
{
    /// <summary>
    /// Lấy danh sách cấu hình lưới theo tên.
    /// </summary>
    /// <param name="gridName">Tên của lưới cần lấy cấu hình.</param>
    /// <returns>Danh sách các cấu hình lưới.</returns>
    Task<IEnumerable<GridConfigDto>> GetByNameAsync(string gridName);

    /// <summary>
    /// Lưu cấu hình lưới.
    /// </summary>
    /// <param name="dto">Đối tượng chứa thông tin cấu hình lưới cần lưu.</param>
    /// <returns>Task hoàn thành khi lưu xong.</returns>
    Task SaveAsync(GridConfigSaveDto dto);

    /// <summary>
    /// Xóa cấu hình lưới theo tên.
    /// </summary>
    /// <param name="gridName">Tên của lưới cần xóa cấu hình.</param>
    /// <returns>Task hoàn thành khi xóa xong.</returns>
    Task DeleteByNameAsync(string gridName);
}
