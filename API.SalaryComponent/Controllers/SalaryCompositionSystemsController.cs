using Microsoft.AspNetCore.Mvc;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IServices;

namespace API.SalaryComponent.Controllers;

/// <summary>
/// Controller quản lý thành phần lương mặc định của hệ thống
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalaryCompositionSystemsController : ControllerBase
{
    private readonly ISalaryCompositionSystemService _service;

    public SalaryCompositionSystemsController(ISalaryCompositionSystemService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy danh sách thành phần lương hệ thống
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalaryCompositionSystemDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách thành phần lương hệ thống có phân trang
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResultDto<SalaryCompositionSystemDto>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 15,
        [FromQuery] string? searchText = null,
        [FromQuery] string? type = null)
    {
        var result = await _service.GetPagedAsync(pageNumber, pageSize, searchText, type);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thành phần lương hệ thống theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SalaryCompositionSystemDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy thành phần lương hệ thống" });
        }
        return Ok(result);
    }

    /// <summary>
    /// Kiểm tra mã đã tồn tại trong danh sách sử dụng chưa
    /// </summary>
    [HttpGet("{id}/check-exists")]
    public async Task<ActionResult<CheckExistsResultDto>> CheckExists(Guid id)
    {
        var exists = await _service.CheckCodeExistsAsync(id);
        var systemItem = await _service.GetByIdAsync(id);
        return Ok(new CheckExistsResultDto
        {
            Exists = exists,
            Code = systemItem?.SalaryCompositionSystemCode ?? ""
        });
    }

    /// <summary>
    /// Di chuyển thành phần lương từ system sang composition (tạo mới)
    /// </summary>
    [HttpPost("{id}/move")]
    public async Task<ActionResult<MoveResultDto>> Move(Guid id)
    {
        var newId = await _service.MoveToCompositionAsync(id);
        return Ok(new MoveResultDto
        {
            Success = true,
            Message = "Di chuyển thành phần lương thành công",
            NewSalaryCompositionId = newId
        });
    }

    /// <summary>
    /// Ghi đè thành phần lương đã tồn tại
    /// </summary>
    [HttpPost("{id}/overwrite")]
    public async Task<ActionResult<MoveResultDto>> Overwrite(Guid id)
    {
        var existingId = await _service.OverwriteToCompositionAsync(id);
        return Ok(new MoveResultDto
        {
            Success = true,
            Message = "Cập nhật thành phần lương thành công",
            NewSalaryCompositionId = existingId
        });
    }

    /// <summary>
    /// Di chuyển nhiều thành phần lương vào danh sách sử dụng
    /// </summary>
    [HttpPost("move-multiple")]
    public async Task<ActionResult<MoveMultipleResultDto>> MoveMultiple([FromBody] MoveMultipleRequestDto request)
    {
        var result = await _service.MoveMultipleToCompositionAsync(request.Ids);
        return Ok(result);
    }
}

/// <summary>
/// DTO kết quả kiểm tra tồn tại
/// </summary>
public class CheckExistsResultDto
{
    /// <summary>
    /// Kết quả kiểm tra tồn tại (true nếu đã tồn tại)
    /// </summary>
    public bool Exists { get; set; }
    
    /// <summary>
    /// Mã thành phần lương
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// DTO kết quả di chuyển
/// </summary>
public class MoveResultDto
{
    /// <summary>
    /// Kết quả di chuyển (true nếu thành công)
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Thông báo kết quả
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// ID thành phần lương mới được tạo
    /// </summary>
    public Guid NewSalaryCompositionId { get; set; }
}
