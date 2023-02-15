using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Data;

public class HogwartsContext : DbContext
{
    public HogwartsContext(DbContextOptions<HogwartsContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Potion> Potions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .ToTable("Student");

        modelBuilder.Entity<Room>()
            .ToTable("Room");

        modelBuilder.Entity<Recipe>()
            .ToTable("Recipe");

        modelBuilder.Entity<Ingredient>()
            .ToTable("Ingredient");

        modelBuilder.Entity<Potion>()
            .ToTable("Potion");
    }
}