using BookApi.Models;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class OrderController: ControllerBase
{
    private readonly OrderService _service;
    public OrderController(OrderService orderService)
    {
        _service = orderService;
    }
    // get all order
    [HttpGet("GetAllOrders")]
    public async Task<IActionResult> GetAllOrder()
    {
        var orders = await _service.GetAllAsync();
        return Ok(orders);
    }

    // add new order
    [HttpPost("AddOrder")]
    public async Task<IActionResult> AddNewOrder([FromBody] Order order)
    {
        var orderId = await _service.AddAsync(order);
        return new JsonResult(orderId.ToString());
    }
}