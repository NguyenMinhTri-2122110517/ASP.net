using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;
using NguyenMinhTri_2122110517.Services;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NguyenMinhTri_2122110517.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public BannerController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banner>>> GetAll()
        {
            return await _context.Banners.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BannerResponse>> GetById(int id)
        {
            var banner = await _context.Banners.FindAsync(id);

            if (banner == null)
            {
                return NotFound("Không tìm thấy banner với ID này");
            }

            return new BannerResponse
            {
                Id = banner.Id,
                Name = banner.Name,
                Link = banner.Link,
                ImageUrl = _fileService.GetFileUrl(banner.Image, "banners"),
                Description = banner.Description,
                Position = banner.Position,
                Sort_Order = banner.Sort_Order,
                Created_At = banner.Created_At,
                Status = banner.Status
            };
        }

        [HttpPost]
        public async Task<ActionResult<Banner>> Create([FromForm] BannerCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageName = await _fileService.SaveFileAsync(request.Image, "banners");

            var newBanner = new Banner
            {
                Name = request.Name,
                Link = request.Link,
                Image = imageName,
                Description = request.Description,
                Position = request.Position,
                Sort_Order = request.Sort_Order,
                Created_By = request.Created_By,
                Created_At = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Banners.Add(newBanner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newBanner.Id }, newBanner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] BannerUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingBanner = await _context.Banners.FindAsync(id);
            if (existingBanner == null)
            {
                return NotFound("Không tìm thấy banner để cập nhật");
            }

            if (request.Image != null)
            {
                _fileService.DeleteFile(existingBanner.Image, "banners");
                existingBanner.Image = await _fileService.SaveFileAsync(request.Image, "banners");
            }

            existingBanner.Name = request.Name;
            existingBanner.Link = request.Link;
            existingBanner.Description = request.Description;
            existingBanner.Position = request.Position;
            existingBanner.Sort_Order = request.Sort_Order;
            existingBanner.Updated_By = request.Updated_By ?? 0;
            existingBanner.Updated_At = DateTime.UtcNow;
            existingBanner.Status = request.Status;

            _context.Entry(existingBanner).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound("Không tìm thấy banner để xóa");
            }

            _fileService.DeleteFile(banner.Image, "banners");
            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class BannerCreateRequest
        {
            [Required]
            public string Name { get; set; }
            public string Link { get; set; }

            [Required]
            public IFormFile Image { get; set; }
            public string Description { get; set; }
            public string Position { get; set; }
            public int Sort_Order { get; set; }
            public int Created_By { get; set; }
            public int Status { get; set; }
        }

        public class BannerUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string Link { get; set; }
            public IFormFile? Image { get; set; }
            public string Description { get; set; }
            public string Position { get; set; }
            public int Sort_Order { get; set; }
            public int? Updated_By { get; set; }
            public int Status { get; set; }
        }

        public class BannerResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Link { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
            public string Position { get; set; }
            public int Sort_Order { get; set; }
            public DateTime Created_At { get; set; }
            public int Status { get; set; }
        }
    }
}
