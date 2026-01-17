using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IRepository;

/// <summary>
/// Interface Repository quản lý cấu hình lưới (Grid Config).
/// Cung cấp các phương thức truy cập dữ liệu cho cấu hình lưới.
/// </summary>
public interface IGridConfigRepository
{
    /// <summary>
    /// Lấy danh sách cấu hình lưới theo tên.
    /// </summary>
    /// <param name="gridName">Tên của lưới cần lấy cấu hình.</param>
    /// <returns>Danh sách các cấu hình lưới.</returns>
    Task<IEnumerable<GridConfigDto>> GetByNameAsync(string gridName);

    /// <summary>
    /// Lưu cấu hình lưới (xóa cũ và thêm mới).
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
