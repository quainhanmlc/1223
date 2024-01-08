using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ASM.Models;

namespace ASM.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ASM.Models.Category> Category { get; set; } = default!;
        public DbSet<ASM.Models.OrderItem> OrderItems { get; set; } = default!;

        public DbSet<ASM.Models.Book> Book { get; set; } = default!;
        public DbSet<ASM.Models.Order> Order { get; set; } = default!;
    }
}
