using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;
using NguyenMinhTri_2122110517.Services;
using NguyenMinhTri_2122110517.Attributes;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Cors;

namespace NguyenMinhTri_2122110517.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public CategoryController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Không tìm thấy category với ID này");
            }

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Parent_Id = category.Parent_Id,
                Sort_Order = category.Sort_Order,
                ImageUrl = _fileService.GetFileUrl(category.Image, "categories"),
                Description = category.Description,
                Created_At = category.Created_At,
                Status = category.Status
            };
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create([FromForm] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageName = await _fileService.SaveFileAsync(request.Image, "categories");

            var newCategory = new Category
            {
                Name = request.Name,
                Slug = request.Slug,
                Parent_Id = request.Parent_Id,
                Sort_Order = request.Sort_Order,
                Image = imageName,
                Description = request.Description,
                Created_At = DateTime.UtcNow,
                Updated_At = DateTime.UtcNow,
                Created_By = request.Created_By, // Thay "System" bằng Created_By từ request
                Updated_By = request.Created_By, // Ban đầu, Updated_By giống Created_By
                Status = request.Status ? 1 : 0 // Chuyển đổi bool thành int (true -> 1, false -> 0)
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newCategory.Id }, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CategoryUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound("Không tìm thấy category để cập nhật");
            }

            if (request.Image != null)
            {
                _fileService.DeleteFile(existingCategory.Image, "categories");
                existingCategory.Image = await _fileService.SaveFileAsync(request.Image, "categories");
            }

            existingCategory.Name = request.Name;
            existingCategory.Slug = request.Slug;
            existingCategory.Parent_Id = request.Parent_Id;
            existingCategory.Sort_Order = request.Sort_Order;
            existingCategory.Description = request.Description;
            existingCategory.Updated_At = DateTime.UtcNow;
            existingCategory.Updated_By = request.Updated_By ?? 0; // Gán giá trị mặc định là 0 nếu Updated_By là null
            existingCategory.Status = request.Status ? 1 : 0; // Chuyển đổi bool thành int

            _context.Entry(existingCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Không tìm thấy category để xóa");
            }

            _fileService.DeleteFile(category.Image, "categories");
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        public class CategoryCreateRequest
        {
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }
            public int Parent_Id { get; set; }
            public int Sort_Order { get; set; }

            [Required(ErrorMessage = "Image is required")]
            [DataType(DataType.Upload)]
            [MaxFileSize(5 * 1024 * 1024)] // 5MB
            [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
            public IFormFile Image { get; set; }
            public string Description { get; set; }
            public int Created_By { get; set; } // Thêm Created_By
            public bool Status { get; set; } // Sử dụng bool trong request, sẽ chuyển đổi thành int
        }

        public class CategoryUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }
            public int Parent_Id { get; set; }
            public int Sort_Order { get; set; }
            public IFormFile? Image { get; set; }
            public string Description { get; set; }
            public int? Updated_By { get; set; } // Thêm Updated_By
            public bool Status { get; set; } // Sử dụng bool trong request, sẽ chuyển đổi thành int
        }

        public class CategoryResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
            public int Parent_Id { get; set; }
            public int Sort_Order { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
            public DateTime Created_At { get; set; }
            public int Status { get; set; } // Sử dụng int để khớp với model
        }
    }
}