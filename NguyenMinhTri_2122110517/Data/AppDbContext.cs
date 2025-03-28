using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Model;

namespace NguyenMinhTri_2122110517.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}
