﻿using FirstMicroservice.Categories.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstMicroservice.Todos.WebAPI.Context
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
    }
}