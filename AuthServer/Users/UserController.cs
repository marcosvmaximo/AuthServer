using AuthServer.Errors;
using AuthServer.Security;
using AuthServer.Users.Requests;
using AuthServer.Users.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthServer.Users
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService service, ILogger<UserController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Post([FromBody] CreateUserRequest request)
        {
            var user = await _service.Insert(request);
            var response = new UserResponse(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll([FromQuery] SortDir? sortDir, [FromQuery] string? role)
        {
            var users = await _service.List(sortDir, role);
            var response = users.ConvertAll(user => new UserResponse(user));
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponse>> Get([FromRoute] int id)
        {
            var user = await _service.FindByIdOrNull(id);
            if (user == null) return NotFound();
            return Ok(new UserResponse(user));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var user = await _service.Delete(id);
            if (user == null) return NotFound();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> Update([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            var updatedUser = await _service.Update(id, request.Name);
            if (updatedUser == null) return NoContent();
            return Ok(new UserResponse(updatedUser));
        }

        [HttpPut("{id:int}/roles/{role}")]
        // [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> GrantRole([FromRoute] int id, [FromRoute] string role)
        {
            var success = await _service.AddRole(id, role.ToUpper());
            return success ? Ok() : NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var loginResponse = await _service.Login(request.Email, request.Password);
            if (loginResponse == null) return Unauthorized();
            return Ok(loginResponse);
        }
    }
}