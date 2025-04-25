using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;
using System.ComponentModel.DataAnnotations;

namespace NguyenMinhTri_2122110517.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menu>>> GetAll()
        {
            return await _context.Menus.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Menu>> GetById(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound("Không tìm thấy menu với ID này");
            return menu;
        }

        [HttpPost]
        public async Task<ActionResult<Menu>> Create([FromBody] MenuCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newMenu = new Menu
            {
                Name = request.Name,
                Link = request.Link,
                Type = request.Type,
                Table_Id = request.TableId,
                Sort_Order = request.SortOrder,
                Parent_Id = request.ParentId,
                Position = request.Position,
                Created_By = request.CreatedBy,
                Created_At = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Menus.Add(newMenu);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newMenu.Id }, newMenu);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MenuUpdateRequest request)
        {
            if (id != request.Id) return BadRequest();

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound("Không tìm thấy menu để cập nhật");

            menu.Name = request.Name;
            menu.Link = request.Link;
            menu.Type = request.Type;
            menu.Table_Id = request.TableId;
            menu.Sort_Order = request.SortOrder;
            menu.Parent_Id = request.ParentId;
            menu.Position = request.Position;
            menu.Updated_By = request.UpdatedBy ?? 0;
            menu.Created_At = DateTime.UtcNow;
            menu.Status = request.Status;

            _context.Entry(menu).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound("Không tìm thấy menu để xóa");

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class MenuCreateRequest
        {
            [Required] public string Name { get; set; }
            public string Link { get; set; }
            public string Type { get; set; }
            public int TableId { get; set; }
            public int SortOrder { get; set; }
            public int ParentId { get; set; }
            public string Position { get; set; }
            public int CreatedBy { get; set; }
            public int Status { get; set; }
        }

        public class MenuUpdateRequest
        {
            public int Id { get; set; }
            [Required] public string Name { get; set; }
            public string Link { get; set; }
            public string Type { get; set; }
            public int TableId { get; set; }
            public int SortOrder { get; set; }
            public int ParentId { get; set; }
            public string Position { get; set; }
            public int? UpdatedBy { get; set; }
            public int Status { get; set; }
        }
    }
}