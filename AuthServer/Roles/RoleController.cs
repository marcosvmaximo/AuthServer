using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Roles;

[ApiController]
[Route("roles")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _service;

    public RoleController(IRoleService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleResponse>> Insert([FromBody] CreateRoleRequest roleRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var role = await _service.Insert(roleRequest);
        var response = new RoleResponse(role);

        return CreatedAtAction(nameof(Insert), new { id = role.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<RoleResponse>> List()
    {
        var roles = _service.FindAll().Result
            .Select(role => new RoleResponse(role))
            .ToList();

        return Ok(roles);
    }
}