using Microsoft.EntityFrameworkCore;
using MiniETicaret.Products.WebAPI.Context;
using MiniETicaret.Products.WebAPI.Dtos;
using MiniETicaret.Products.WebAPI.Models;

namespace MiniETicaret.Products.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            });
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.MapGet("/getall", async (ApplicationDbContext context, CancellationToken cancellationToken) =>
            {
                var products = await context.Products.OrderBy(q => q.Name).ToListAsync(cancellationToken);
                return products;
            });
            app.MapPost("/create", async (CreateProductDto request, ApplicationDbContext context, CancellationToken cancellationToken) =>
            {
                bool isNameExists = await context.Products.AnyAsync(q => q.Name == request.Name, cancellationToken);
                if (isNameExists)
                {
                    return "Ürün adı daha önce oluşturulmuş";
                }

                Product product = new Product
                {
                    Name = request.Name,
                    Price = request.Price,
                    Stock = request.Stock,
                };
                await context.Products.AddAsync(product, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                return "Ürün kaydı başarıyla oluşturuldu.";
            });

            using (var scoped = app.Services.CreateScope())
            {
                var srv = scoped.ServiceProvider;
                var context = srv.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }
            app.Run();
        }
    }
}
