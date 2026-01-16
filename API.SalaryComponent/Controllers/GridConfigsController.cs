using Microsoft.AspNetCore.Mvc;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IServices;

namespace API.SalaryComponent.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GridConfigsController : ControllerBase
{
    private readonly IGridConfigService _service;

    public GridConfigsController(IGridConfigService service)
    {
        _service = service;
    }

    [HttpGet("{gridName}")]
    public async Task<ActionResult<IEnumerable<GridConfigDto>>> GetByName(string gridName)
    {
        var result = await _service.GetByNameAsync(gridName);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Save([FromBody] GridConfigSaveDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.GridConfigName))
        {
            return BadRequest(new { message = "Grid name is required" });
        }

        await _service.SaveAsync(dto);
        return NoContent();
    }

    [HttpDelete("{gridName}")]
    public async Task<ActionResult> Delete(string gridName)
    {
        await _service.DeleteByNameAsync(gridName);
        return NoContent();
    }
}
