using Microsoft.AspNetCore.Mvc;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IServices;

namespace API.SalaryComponent.Controllers;

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
        try
        {
            var exists = await _service.CheckCodeExistsAsync(id);
            var systemItem = await _service.GetByIdAsync(id);
            return Ok(new CheckExistsResultDto
            {
                Exists = exists,
                Code = systemItem?.SalaryCompositionSystemCode ?? ""
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Di chuyển thành phần lương từ system sang composition (tạo mới)
    /// </summary>
    [HttpPost("{id}/move")]
    public async Task<ActionResult<MoveResultDto>> Move(Guid id)
    {
        try
        {
            var newId = await _service.MoveToCompositionAsync(id);
            return Ok(new MoveResultDto
            {
                Success = true,
                Message = "Di chuyển thành phần lương thành công",
                NewSalaryCompositionId = newId
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Ghi đè thành phần lương đã tồn tại
    /// </summary>
    [HttpPost("{id}/overwrite")]
    public async Task<ActionResult<MoveResultDto>> Overwrite(Guid id)
    {
        try
        {
            var existingId = await _service.OverwriteToCompositionAsync(id);
            return Ok(new MoveResultDto
            {
                Success = true,
                Message = "Cập nhật thành phần lương thành công",
                NewSalaryCompositionId = existingId
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class CheckExistsResultDto
{
    public bool Exists { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class MoveResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid NewSalaryCompositionId { get; set; }
}
