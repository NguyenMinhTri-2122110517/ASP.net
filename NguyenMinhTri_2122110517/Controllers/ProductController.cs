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
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public ProductController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm với ID này");
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var imageName = await _fileService.SaveFileAsync(request.Image, "products");

            var newProduct = new Product
            {
                Category_Id = request.CategoryId,
                Brand_Id = request.BrandId,
                Name = request.Name,
                Slug = request.Slug,
                Description = request.Description,
                Price = request.Price,
                Content = request.Content,
                Image = imageName,
                Created_At = DateTime.UtcNow,
                Created_By = request.CreatedBy,
                Status = request.Status
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateRequest request)
        {
            if (id != request.Id) return BadRequest();

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound("Không tìm thấy sản phẩm để cập nhật");

            if (request.Image != null)
            {
                _fileService.DeleteFile(existingProduct.Image, "products");
                existingProduct.Image = await _fileService.SaveFileAsync(request.Image, "products");
            }

            existingProduct.Category_Id = request.CategoryId;
            existingProduct.Brand_Id = request.BrandId;
            existingProduct.Name = request.Name;
            existingProduct.Slug = request.Slug;
            existingProduct.Description = request.Description;
            existingProduct.Price = request.Price;
            existingProduct.Content = request.Content;
            existingProduct.Updated_At = DateTime.UtcNow;
            existingProduct.Updated_By = request.UpdatedBy;
            existingProduct.Status = request.Status;

            _context.Entry(existingProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm để xóa");

            _fileService.DeleteFile(product.Image, "products");
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("bycategory/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetByCategoryId(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.Category_Id == categoryId)
                .ToListAsync();

            if (!products.Any()) return NotFound("Không tìm thấy sản phẩm nào cho danh mục này");
            return products;
        }

        public class ProductCreateRequest
        {
            [Required] public int CategoryId { get; set; }
            [Required] public int BrandId { get; set; }
            [Required] public string Name { get; set; }
            [Required] public string Slug { get; set; }
            public string Description { get; set; }
            public string Content { get; set; }
            [Required] public IFormFile Image { get; set; }
            [Required] public double Price { get; set; }
            public int CreatedBy { get; set; }
            public int Status { get; set; }
        }

        public class ProductUpdateRequest
        {
            public int Id { get; set; }
            [Required] public int CategoryId { get; set; }
            [Required] public int BrandId { get; set; }
            [Required] public string Name { get; set; }
            [Required] public string Slug { get; set; }
            public string Description { get; set; }
            public string Content { get; set; }
            public IFormFile? Image { get; set; }
            [Required] public double Price { get; set; }
            public int? UpdatedBy { get; set; }
            public int Status { get; set; }
        }
    }
}
