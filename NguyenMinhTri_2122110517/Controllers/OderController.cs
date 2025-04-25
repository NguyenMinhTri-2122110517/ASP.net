using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;

namespace NguyenMinhTri_2122110517.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            return await _context.Orders.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return order;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Post([FromBody] Order order)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            order.Created_At = DateTime.UtcNow;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order updatedOrder)
        {
            if (id != updatedOrder.Id) return BadRequest();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            order.User_Id = updatedOrder.User_Id;
            order.Name = updatedOrder.Name;
            order.Email = updatedOrder.Email;
            order.Phone = updatedOrder.Phone;
            order.Address = updatedOrder.Address;
            order.Note = updatedOrder.Note;
            order.Updated_At = DateTime.UtcNow;
            order.Updated_By = updatedOrder.Updated_By;
            order.Status = updatedOrder.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("byuser/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByUserId(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.User_Id == userId)
                .ToListAsync();

            if (!orders.Any()) return NotFound("Không tìm thấy đơn hàng nào cho người dùng này");
            return orders;
        }
    }
}
