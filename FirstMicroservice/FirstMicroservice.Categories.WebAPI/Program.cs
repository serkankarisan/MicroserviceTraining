using FirstMicroservice.Categories.WebAPI.Dtos;
using FirstMicroservice.Categories.WebAPI.Models;
using FirstMicroservice.Todos.WebAPI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            //options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            options.UseSqlServer(builder.Configuration.GetConnectionString("DockerSqlServer"));
        });
        var app = builder.Build();

        app.MapGet("/categories/getall", async ([FromServices] ApplicationDbContext context, CancellationToken cancellationToken) =>
        {
            var categories = await context.Categories.ToListAsync(cancellationToken);
            return categories;

        });

        app.MapPost("/categories/create", async ([FromBody] CreateCategoryDto request, [FromServices] ApplicationDbContext context, CancellationToken cancellationToken) =>
        {
            bool isNameExists = await context.Categories.AnyAsync(q => q.Name == request.Name, cancellationToken);
            if (isNameExists)
            {
                return Results.BadRequest(new { Message = "Category already exists" });
            }

            var category = new Category
            {
                Name = request.Name,
            };

            await context.Categories.AddAsync(category, cancellationToken);
            await context.SaveChangesAsync();

            return Results.Ok(new { Message = "Category created successfully" });
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