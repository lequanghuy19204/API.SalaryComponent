using Microsoft.AspNetCore.Mvc;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;

namespace API.SalaryComponent.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationRepository _repository;

    public OrganizationsController(IOrganizationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<OrganizationTreeDto>>> GetTree()
    {
        var result = await _repository.GetTreeAsync();
        return Ok(result);
    }
}
