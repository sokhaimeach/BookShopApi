using BookApi.Models;
using BookApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController:ControllerBase
{
    private readonly UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    // get all users
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    // add user
    [HttpPost("AddUser")]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        var userId = await _userService.AddAsync(user);
        return Ok(userId);
    }
    // delete user
    [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var isDeleted = await _userService.DeleteAsync(id);
        return Ok(isDeleted);
    }
    // update user
    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        var isUpdated = await _userService.UpdateAsync(user);
        return Ok(isUpdated);
    }
}