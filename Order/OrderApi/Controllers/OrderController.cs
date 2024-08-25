using Microsoft.AspNetCore.Mvc;

using OrderDtos;
using OrderServices.Services.Contracts;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("add/{userId}")]
        public async Task<IActionResult> AddOrderAsync(int userId, [FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDto = await _orderService.AddOrderAsync(userId, createOrderDto);

            return Ok(orderDto);
        }
    }
}