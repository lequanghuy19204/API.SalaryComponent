using Microsoft.AspNetCore.Mvc;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;

namespace API.SalaryComponent.Controllers;

/// <summary>
/// Controller quản lý đơn vị/tổ chức
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationRepository _repository;

    public OrganizationsController(IOrganizationRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lấy cây đơn vị tổ chức
    /// </summary>
    /// <returns>Danh sách đơn vị tổ chức dạng cây</returns>
    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<OrganizationTreeDto>>> GetTree()
    {
        var result = await _repository.GetTreeAsync();
        return Ok(result);
    }
}
