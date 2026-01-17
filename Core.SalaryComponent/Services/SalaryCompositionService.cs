using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Exceptions;
using Core.SalaryComponent.Interfaces.IServices;
using Core.SalaryComponent.Interfaces.IRepository;

namespace Core.SalaryComponent.Services;

/// <summary>
/// Service xử lý nghiệp vụ thành phần lương đang sử dụng
/// </summary>
public class SalaryCompositionService : ISalaryCompositionService
{
    private readonly ISalaryCompositionRepository _repository;

    /// <summary>
    /// Khởi tạo service với repository
    /// </summary>
    /// <param name="repository">Repository thành phần lương</param>
    public SalaryCompositionService(ISalaryCompositionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Tạo mới thành phần lương, kiểm tra trùng mã
    /// </summary>
    /// <param name="dto">Dữ liệu thành phần lương cần tạo</param>
    /// <returns>ID của thành phần lương mới</returns>
    /// <exception cref="DuplicateException">Ném ra khi mã thành phần đã tồn tại</exception>
    public async Task<Guid> CreateAsync(SalaryCompositionCreateDto dto)
    {
        var isCodeExists = await _repository.IsCodeExistsAsync(dto.SalaryCompositionCode);
        if (isCodeExists)
        {
            throw new DuplicateException("Mã thành phần đã tồn tại");
        }

        return await _repository.CreateAsync(dto);
    }

    /// <summary>
    /// Lấy thành phần lương theo ID
    /// </summary>
    /// <param name="id">ID thành phần lương</param>
    /// <returns>Thông tin thành phần lương</returns>
    /// <exception cref="NotFoundException">Ném ra khi không tìm thấy thành phần lương</exception>
    public async Task<SalaryCompositionDto?> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
        {
            throw NotFoundException.WithEntity("Thành phần lương", id);
        }
        return result;
    }

    /// <summary>
    /// Lấy tất cả thành phần lương
    /// </summary>
    /// <returns>Danh sách tất cả thành phần lương</returns>
    public async Task<IEnumerable<SalaryCompositionDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Cập nhật thành phần lương, kiểm tra trùng mã
    /// </summary>
    /// <param name="id">ID thành phần lương cần cập nhật</param>
    /// <param name="dto">Dữ liệu cập nhật</param>
    /// <returns>Kết quả cập nhật</returns>
    /// <exception cref="NotFoundException">Ném ra khi không tìm thấy thành phần lương</exception>
    /// <exception cref="DuplicateException">Ném ra khi mã thành phần đã tồn tại</exception>
    public async Task<bool> UpdateAsync(Guid id, SalaryCompositionCreateDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw NotFoundException.WithEntity("Thành phần lương", id);
        }

        var isCodeExists = await _repository.IsCodeExistsAsync(dto.SalaryCompositionCode, id);
        if (isCodeExists)
        {
            throw new DuplicateException("Mã thành phần đã tồn tại");
        }

        return await _repository.UpdateAsync(id, dto);
    }

    /// <summary>
    /// Xóa thành phần lương (không cho xóa loại system)
    /// </summary>
    /// <param name="id">ID thành phần lương cần xóa</param>
    /// <returns>Kết quả xóa</returns>
    /// <exception cref="NotFoundException">Ném ra khi không tìm thấy thành phần lương</exception>
    /// <exception cref="InvalidOperationException">Ném ra khi cố xóa thành phần lương hệ thống</exception>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw NotFoundException.WithEntity("Thành phần lương", id);
        }

        // Không cho phép xóa thành phần lương mặc định của hệ thống
        if (existing.SalaryCompositionSource == "system")
        {
            throw new InvalidOperationException("Đây là thành phần lương mặc định của hệ thống nên không thể xóa. Vui lòng kiểm tra lại.");
        }

        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Cập nhật trạng thái thành phần lương
    /// </summary>
    /// <param name="id">ID thành phần lương</param>
    /// <param name="status">Trạng thái mới</param>
    /// <returns>Kết quả cập nhật</returns>
    /// <exception cref="NotFoundException">Ném ra khi không tìm thấy thành phần lương</exception>
    public async Task<bool> UpdateStatusAsync(Guid id, int status)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw NotFoundException.WithEntity("Thành phần lương", id);
        }

        return await _repository.UpdateStatusAsync(id, status);
    }

    /// <summary>
    /// Cập nhật trạng thái hàng loạt
    /// </summary>
    /// <param name="ids">Danh sách ID thành phần lương</param>
    /// <param name="status">Trạng thái mới</param>
    public async Task BulkUpdateStatusAsync(List<Guid> ids, int status)
    {
        await _repository.BulkUpdateStatusAsync(ids, status);
    }

    /// <summary>
    /// Lấy danh sách có phân trang
    /// </summary>
    /// <param name="request">Thông tin phân trang và tìm kiếm</param>
    /// <returns>Kết quả phân trang</returns>
    public async Task<PagedResultDto<SalaryCompositionDto>> GetPagedAsync(PagingRequestDto request)
    {
        return await _repository.GetPagedAsync(request);
    }
}
