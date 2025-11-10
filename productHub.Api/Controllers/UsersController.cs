using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using productHub.Application.DTOs.Users;
using productHub.Application.Interfaces;

namespace productHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // todas requieren JWT
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    // GET /api/users  (solo Admin)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _users.GetAllAsync();
        return Ok(list);
    }

    // GET /api/users/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user is null) return NotFound();
        return Ok(user);
    }

    // POST /api/users  (solo Admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] UserDto dto)
    {
        var created = await _users.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/users/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserDto dto)
    {
        var ok = await _users.UpdateAsync(id, dto);
        if (!ok) return NotFound();
        return NoContent();
    }

    // DELETE /api/users/{id}  (solo Admin)
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _users.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}