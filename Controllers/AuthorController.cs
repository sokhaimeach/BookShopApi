using BookApi.Models;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthorController:ControllerBase
{
    private readonly AuthorService _authorService;
    public AuthorController(AuthorService authorService)
    {
        _authorService = authorService;
    }
    // get all authors
    [HttpGet("GetAllAuthors")]
    public async Task<IActionResult> GetAllAuthors()
    {
        var authors = await _authorService.GetAllAsync();
        return Ok(authors);
    }
    // add author
    [HttpPost("AddAuthor")]
    public async Task<IActionResult> AddAuthor([FromBody] Author author)
    {
        var authorId = await _authorService.AddAsync(author);
        return new JsonResult(authorId.ToString());
    }
    // delete author
    [HttpDelete("DeleteAuthor/{id}")]
    public async Task<IActionResult> DeleteAuthor(string id)
    {
        var isDeleted = await _authorService.DeleteAsync(id);
        return Ok(isDeleted);
    }
    // update author
    [HttpPut("UpdateAuthor")]
    public async Task<IActionResult> UpdateAuthor([FromBody] Author author)
    {
        var isUpdated = await _authorService.UpdateAsync(author);
        return Ok(isUpdated);
    }
}