using Microsoft.AspNetCore.Mvc;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IServices;

namespace API.SalaryComponent.Controllers;

/// <summary>
/// Controller quản lý cấu hình lưới (Grid Config)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GridConfigsController : ControllerBase
{
    private readonly IGridConfigService _service;

    public GridConfigsController(IGridConfigService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy cấu hình lưới theo tên
    /// </summary>
    /// <param name="gridName">Tên của lưới cần lấy cấu hình</param>
    /// <returns>Danh sách cấu hình lưới</returns>
    [HttpGet("{gridName}")]
    public async Task<ActionResult<IEnumerable<GridConfigDto>>> GetByName(string gridName)
    {
        var result = await _service.GetByNameAsync(gridName);
        return Ok(result);
    }

    /// <summary>
    /// Lưu cấu hình lưới
    /// </summary>
    /// <param name="dto">Dữ liệu cấu hình lưới cần lưu</param>
    /// <returns>NoContent nếu lưu thành công</returns>
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

    /// <summary>
    /// Xóa cấu hình lưới theo tên
    /// </summary>
    /// <param name="gridName">Tên của lưới cần xóa cấu hình</param>
    /// <returns>NoContent nếu xóa thành công</returns>
    [HttpDelete("{gridName}")]
    public async Task<ActionResult> Delete(string gridName)
    {
        await _service.DeleteByNameAsync(gridName);
        return NoContent();
    }
}
