// Data/AgriEnergyDbContext.cs
using AgriEnergyConnect.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.API.Data
{
    public class AgriEnergyDbContext : IdentityDbContext<ApplicationUser>
    {
        public AgriEnergyDbContext(DbContextOptions<AgriEnergyDbContext> options)
            : base(options)
        {
        }

        public DbSet<FarmerModel> Farmers { get; set; }
        public DbSet<ProductModel> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<FarmerModel>(entity =>
            {
                entity.ToTable("Farmers");
                entity.HasKey(f => f.Id);
            });

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Farmer)
                .WithMany(f => f.Products)
                .HasForeignKey(p => p.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductModel>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);


        }


    }
}