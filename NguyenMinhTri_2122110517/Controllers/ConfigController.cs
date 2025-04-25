using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NguyenMinhTri_2122110517.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConfigController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Config
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Config>>> GetConfigs()
        {
            return await _context.Configs.ToListAsync();
        }

        // GET: api/Config/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Config>> GetConfig(int id)
        {
            var config = await _context.Configs.FindAsync(id);

            if (config == null)
            {
                return NotFound();
            }

            return config;
        }

        // POST: api/Config
        [HttpPost]
        public async Task<ActionResult<Config>> PostConfig(Config config)
        {
            _context.Configs.Add(config);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConfig), new { id = config.Id }, config);
        }

        // PUT: api/Config/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConfig(int id, Config config)
        {
            if (id != config.Id)
            {
                return BadRequest();
            }

            _context.Entry(config).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Config/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfig(int id)
        {
            var config = await _context.Configs.FindAsync(id);
            if (config == null)
            {
                return NotFound();
            }

            _context.Configs.Remove(config);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConfigExists(int id)
        {
            return _context.Configs.Any(e => e.Id == id);
        }
    }
}
