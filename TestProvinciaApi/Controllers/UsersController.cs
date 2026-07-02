using Microsoft.AspNetCore.Mvc;
using TestProvincia.Application.DTOs;
using TestProvincia.Application.Interfaces;

namespace TestProvinciaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    /// <summary>
    /// Endpoint para obtener todos los usuarios
    /// </summary>
    /// <returns>Todos los usuarios.</returns>

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    /// <summary>
    /// Endpoint para obtener un usuario en especifico por la Id
    /// </summary>
    /// <param name="id">Id de usuario.</param>
    /// <returns>Usuario asociado a ese Id.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
            return NotFound();
        return Ok(user);
    }
    /// <summary>
    /// Endpoint para Crear un usuario
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
    /// <summary>
    /// Endpoint para modificar un usuario existente
    /// </summary>
    /// <param name="id">Id de usuario.</param>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);
        if (user is null)
            return NotFound();
        return Ok(user);
    }
    /// <summary>
    /// Endpoint para borrar un usuario existente
    /// </summary>
    /// <param name="id">Id de usuario.</param>

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
