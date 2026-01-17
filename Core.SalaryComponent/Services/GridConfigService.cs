using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;
using Core.SalaryComponent.Interfaces.IServices;

namespace Core.SalaryComponent.Services;

/// <summary>
/// Service xử lý nghiệp vụ cấu hình lưới (Grid Config)
/// </summary>
public class GridConfigService : IGridConfigService
{
    private readonly IGridConfigRepository _repository;

    /// <summary>
    /// Khởi tạo service với repository
    /// </summary>
    /// <param name="repository">Repository cấu hình lưới</param>
    public GridConfigService(IGridConfigRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lấy danh sách cấu hình lưới theo tên
    /// </summary>
    /// <param name="gridName">Tên lưới cần tìm</param>
    /// <returns>Danh sách cấu hình lưới</returns>
    public async Task<IEnumerable<GridConfigDto>> GetByNameAsync(string gridName)
    {
        return await _repository.GetByNameAsync(gridName);
    }

    /// <summary>
    /// Lưu cấu hình lưới
    /// </summary>
    /// <param name="dto">Dữ liệu cấu hình cần lưu</param>
    public async Task SaveAsync(GridConfigSaveDto dto)
    {
        await _repository.SaveAsync(dto);
    }

    /// <summary>
    /// Xóa cấu hình lưới theo tên
    /// </summary>
    /// <param name="gridName">Tên lưới cần xóa</param>
    public async Task DeleteByNameAsync(string gridName)
    {
        await _repository.DeleteByNameAsync(gridName);
    }
}
