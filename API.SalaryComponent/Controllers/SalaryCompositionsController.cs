using Microsoft.AspNetCore.Mvc;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IServices;
using Core.SalaryComponent.Exceptions;

namespace API.SalaryComponent.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalaryCompositionsController : ControllerBase
{
    private readonly ISalaryCompositionService _service;

    public SalaryCompositionsController(ISalaryCompositionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalaryCompositionDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

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

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Không tìm thấy thành phần lương" });
        }
        return NoContent();
    }

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

public class UpdateStatusDto
{
    public int Status { get; set; }
}

public class BulkUpdateStatusDto
{
    public List<Guid> Ids { get; set; } = new();
    public int Status { get; set; }
}
