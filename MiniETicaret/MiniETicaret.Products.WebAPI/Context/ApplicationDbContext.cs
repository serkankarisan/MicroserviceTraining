using Microsoft.EntityFrameworkCore;
using MiniETicaret.Products.WebAPI.Models;

namespace MiniETicaret.Products.WebAPI.Context
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Product>().Property(q => q.Price).HasColumnType("money");
            modelBuilder.Entity<Product>(builder =>
            {
                builder.Property(q => q.Price).HasColumnType("money");
            });
        }
    }
}
