using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using _1670_Source_Duy.Models;

namespace _1670_Source_Duy.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<_1670_Source_Duy.Models.ApplicationRole> ApplicationRole { get; set; } = default!;
        public DbSet<_1670_Source_Duy.Models.Category> Category { get; set; } = default!;
        public DbSet<_1670_Source_Duy.Models.Book> Book { get; set; } = default!;
        public DbSet<_1670_Source_Duy.Models.Comment> Comment { get; set; } = default!;
    }
}