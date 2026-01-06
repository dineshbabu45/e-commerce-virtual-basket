using Microsoft.EntityFrameworkCore;
using VirtualBasket.Api.Models;

namespace VirtualBasket.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Discount> DiscountRules { get; set; }
    public DbSet<DiscountProduct> DiscountProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DiscountProduct relationships
        modelBuilder.Entity<DiscountProduct>()
            .HasOne(dp => dp.DiscountRule)
            .WithMany(dr => dr.DiscountProducts)
            .HasForeignKey(dp => dp.DiscountRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DiscountProduct>()
            .HasOne(dp => dp.Product)
            .WithMany()
            .HasForeignKey(dp => dp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed initial products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Apple", Price = 0.50m },
            new Product { Id = 2, Name = "Banana", Price = 0.30m },
            new Product { Id = 3, Name = "Orange", Price = 0.60m },
            new Product { Id = 4, Name = "Milk", Price = 2.50m },
            new Product { Id = 5, Name = "Bread", Price = 1.50m }
        );

        // Seed discount rules
        modelBuilder.Entity<Discount>().HasData(
            new Discount { Id = 1, Type = "Percentage", PercentageOff = 10 }, // 10% off Apples
            new Discount { Id = 2, Type = "Percentage", PercentageOff = 5 }, // 5% off Bananas
            new Discount { Id = 3, Type = "Percentage", PercentageOff = 20 }, // 20% off Oranges
            new Discount { Id = 4, Type = "BuyXGetYFree", BuyQuantity = 2, GetQuantity = 1 }, // Buy 2 Apples, Get 1 Free
            new Discount { Id = 5, Type = "BuyXGetYFree", BuyQuantity = 1, GetQuantity = 1 }  // Buy 1 Milk, Get 1 Free
        );

        // Seed discount-product mappings
        modelBuilder.Entity<DiscountProduct>().HasData(
            // 10% off Apples (Discount 1)
            new DiscountProduct { Id = 1, DiscountRuleId = 1, ProductId = 1 },
            // 5% off Bananas (Discount 2)
            new DiscountProduct { Id = 2, DiscountRuleId = 2, ProductId = 2 },
            // 20% off Oranges (Discount 3)
            new DiscountProduct { Id = 3, DiscountRuleId = 3, ProductId = 3 },
            // Buy 2 Apples Get 1 Free (Discount 4)
            new DiscountProduct { Id = 4, DiscountRuleId = 4, ProductId = 1 },
            // Buy 1 Milk Get 1 Free (Discount 5)
            new DiscountProduct { Id = 5, DiscountRuleId = 5, ProductId = 4 }
        );
    }
}
