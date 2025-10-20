using BookApi.Models;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BookController:ControllerBase
{
    private readonly BookService _bookService;
    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }
    // get all book
    [HttpGet("GetAllBook")]
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await _bookService.GetAllAsync();
        return Ok(books);
    }
    // add new book
    [HttpPost("AddBook")]
    public async Task<IActionResult> AddBook([FromBody] Book book)
    {
        var result = await _bookService.AddAsync(book);
        return Ok(result);
    }
    // delete book
    [HttpDelete("DeleteBook/{id}")]
    public async Task<IActionResult> DeleteBook(string id)
    {
        var isDeleted = await _bookService.DeleteAsync(id);
        return Ok(isDeleted);
    }
    // update book
    [HttpPut("UpdateBook")]
    public async Task<IActionResult> UpdateBook([FromBody] Book book)
    {
        var result = await _bookService.UpdateAsync(book);
        return Ok(result);
    }
    // get one book by id
    [HttpGet("GetOneBook/{id}")]
    public async Task<IActionResult> GetOneBook(string id)
    {
        var book = await _bookService.GetOneAsync(id);
        return Ok(book);
    }
}