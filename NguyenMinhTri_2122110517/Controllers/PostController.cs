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
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public PostController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAll()
        {
            return await _context.Posts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetById(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound("Không tìm thấy bài viết với ID này");
            return post;
        }

        [HttpPost]
        public async Task<ActionResult<Post>> Create([FromForm] PostCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var imageName = await _fileService.SaveFileAsync(request.Image, "posts");

            var newPost = new Post
            {
                Title = request.Title,
                Topic_Id = request.TopicId ?? 0,
                Content = request.Content,
                Description = request.Description,
                Image = imageName,
                Type = request.Type,
                Created_By = request.CreatedBy,
                Created_At = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newPost.Id }, newPost);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] PostUpdateRequest request)
        {
            if (id != request.Id) return BadRequest();

            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost == null) return NotFound("Không tìm thấy bài viết để cập nhật");

            if (request.Image != null)
            {
                _fileService.DeleteFile(existingPost.Image, "posts");
                existingPost.Image = await _fileService.SaveFileAsync(request.Image, "posts");
            }

            existingPost.Title = request.Title;
            existingPost.Topic_Id = request.TopicId ?? 0;
            existingPost.Content = request.Content;
            existingPost.Description = request.Description;
            existingPost.Type = request.Type;
            existingPost.Updated_By = request.UpdatedBy ?? 0;
            existingPost.Updated_At = DateTime.UtcNow;
            existingPost.Status = request.Status;

            _context.Entry(existingPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound("Không tìm thấy bài viết để xóa");

            _fileService.DeleteFile(post.Image, "posts");
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class PostCreateRequest
        {
            [Required] public string Title { get; set; }
            public int? TopicId { get; set; }
            public string Content { get; set; }
            public string Description { get; set; }
            [Required] public IFormFile Image { get; set; }
            public string Type { get; set; }
            public int CreatedBy { get; set; }
            public int Status { get; set; }
        }

        public class PostUpdateRequest
        {
            public int Id { get; set; }
            [Required] public string Title { get; set; }
            public int? TopicId { get; set; }
            public string Content { get; set; }
            public string Description { get; set; }
            public IFormFile? Image { get; set; }
            public string Type { get; set; }
            public int? UpdatedBy { get; set; }
            public int Status { get; set; }
        }
    }
}
