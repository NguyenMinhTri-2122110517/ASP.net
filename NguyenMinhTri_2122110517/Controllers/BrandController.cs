using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;
using NguyenMinhTri_2122110517.Services;
using System.ComponentModel.DataAnnotations;

namespace NguyenMinhTri_2122110517.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public BrandController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetAll()
        {
            return await _context.Brands.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandResponse>> GetById(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound("Không tìm thấy thương hiệu với ID này");
            }

            return new BrandResponse
            {
                Id = brand.Id,
                Name = brand.Name,
                Slug = brand.Slug,
                ImageUrl = _fileService.GetFileUrl(brand.Image, "brands"),
                Description = brand.Description,
                Sort_Order = brand.Sort_Order,
                Created_At = brand.Created_At,
                Status = brand.Status
            };
        }

        [HttpPost]
        public async Task<ActionResult<Brand>> Create([FromForm] BrandCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageName = await _fileService.SaveFileAsync(request.Image, "brands");

            var newBrand = new Brand
            {
                Name = request.Name,
                Slug = request.Slug,
                Image = imageName,
                Description = request.Description,
                Sort_Order = request.Sort_Order,
                Created_By = request.Created_By,
                Created_At = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Brands.Add(newBrand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newBrand.Id }, newBrand);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] BrandUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingBrand = await _context.Brands.FindAsync(id);
            if (existingBrand == null)
            {
                return NotFound("Không tìm thấy thương hiệu để cập nhật");
            }

            if (request.Image != null)
            {
                _fileService.DeleteFile(existingBrand.Image, "brands");
                existingBrand.Image = await _fileService.SaveFileAsync(request.Image, "brands");
            }

            existingBrand.Name = request.Name;
            existingBrand.Slug = request.Slug;
            existingBrand.Description = request.Description;
            existingBrand.Sort_Order = request.Sort_Order;
            existingBrand.Updated_By = request.Updated_By ?? 0; // Gán giá trị mặc định là 0 nếu Updated_By là null
            existingBrand.Updated_At = DateTime.UtcNow;
            existingBrand.Status = request.Status;

            _context.Entry(existingBrand).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound("Không tìm thấy thương hiệu để xóa");
            }

            _fileService.DeleteFile(brand.Image, "brands");
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class BrandCreateRequest
        {
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }

            [Required]
            public IFormFile Image { get; set; }
            public string Description { get; set; }
            public int Sort_Order { get; set; }
            public int Created_By { get; set; }
            public int Status { get; set; }
        }

        public class BrandUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }
            public IFormFile? Image { get; set; }
            public string Description { get; set; }
            public int Sort_Order { get; set; }
            public int? Updated_By { get; set; }
            public int Status { get; set; }
        }

        public class BrandResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
            public int Sort_Order { get; set; }
            public DateTime Created_At { get; set; }
            public int Status { get; set; }
        }
    }
}