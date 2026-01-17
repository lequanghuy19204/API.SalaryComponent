using Microsoft.AspNetCore.Mvc;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IServices;
using Core.SalaryComponent.Exceptions;

namespace API.SalaryComponent.Controllers;

/// <summary>
/// Controller quản lý thành phần lương đang sử dụng
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalaryCompositionsController : ControllerBase
{
    private readonly ISalaryCompositionService _service;

    public SalaryCompositionsController(ISalaryCompositionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy danh sách tất cả thành phần lương
    /// </summary>
    /// <returns>Danh sách thành phần lương</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalaryCompositionDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách thành phần lương có phân trang
    /// </summary>
    /// <param name="request">Thông tin phân trang và tìm kiếm</param>
    /// <returns>Kết quả phân trang chứa danh sách thành phần lương</returns>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResultDto<SalaryCompositionDto>>> GetPaged([FromQuery] PagingRequestDto request)
    {
        var result = await _service.GetPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thành phần lương theo ID
    /// </summary>
    /// <param name="id">ID của thành phần lương</param>
    /// <returns>Thành phần lương tương ứng</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<SalaryCompositionDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy thành phần lương" });
        }
        return Ok(result);
    }

    /// <summary>
    /// Tạo mới thành phần lương
    /// </summary>
    /// <param name="dto">Dữ liệu thành phần lương cần tạo</param>
    /// <returns>ID của thành phần lương vừa tạo</returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] SalaryCompositionCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (DuplicateException ex)
        {
            return Conflict(new { message = ex.Message, errorCode = ex.ErrorCode });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật thành phần lương
    /// </summary>
    /// <param name="id">ID của thành phần lương cần cập nhật</param>
    /// <param name="dto">Dữ liệu thành phần lương cần cập nhật</param>
    /// <returns>NoContent nếu cập nhật thành công</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] SalaryCompositionCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy thành phần lương" });
            }
            return NoContent();
        }
        catch (DuplicateException ex)
        {
            return Conflict(new { message = ex.Message, errorCode = ex.ErrorCode });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa thành phần lương
    /// </summary>
    /// <param name="id">ID của thành phần lương cần xóa</param>
    /// <returns>NoContent nếu xóa thành công</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy thành phần lương" });
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật trạng thái thành phần lương
    /// </summary>
    /// <param name="id">ID của thành phần lương cần cập nhật trạng thái</param>
    /// <param name="dto">Dữ liệu trạng thái mới</param>
    /// <returns>NoContent nếu cập nhật thành công</returns>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
    {
        try
        {
            var result = await _service.UpdateStatusAsync(id, dto.Status);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy thành phần lương" });
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật trạng thái hàng loạt
    /// </summary>
    /// <param name="dto">Dữ liệu chứa danh sách ID và trạng thái mới</param>
    /// <returns>NoContent nếu cập nhật thành công</returns>
    [HttpPatch("bulk-status")]
    public async Task<ActionResult> BulkUpdateStatus([FromBody] BulkUpdateStatusDto dto)
    {
        try
        {
            await _service.BulkUpdateStatusAsync(dto.Ids, dto.Status);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

/// <summary>
/// DTO cập nhật trạng thái
/// </summary>
public class UpdateStatusDto
{
    /// <summary>
    /// Trạng thái mới
    /// </summary>
    public int Status { get; set; }
}

/// <summary>
/// DTO cập nhật trạng thái hàng loạt
/// </summary>
public class BulkUpdateStatusDto
{
    /// <summary>
    /// Danh sách ID cần cập nhật
    /// </summary>
    public List<Guid> Ids { get; set; } = new();
    
    /// <summary>
    /// Trạng thái mới
    /// </summary>
    public int Status { get; set; }
}
