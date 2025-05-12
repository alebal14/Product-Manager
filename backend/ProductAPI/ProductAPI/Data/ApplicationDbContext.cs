using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;

namespace ProductAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table mappings
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<ProductType>().ToTable("ProductType");
            modelBuilder.Entity<Color>().ToTable("Color");
            modelBuilder.Entity<ProductColor>().ToTable("ProductColor");

            // Configure the many-to-many relationship between Product and Color
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Colors)
                .WithMany(c => c.Products)
                .UsingEntity<ProductColor>(
                    j => j
                        .HasOne(pc => pc.Color)
                        .WithMany()
                        .HasForeignKey(pc => pc.ColorId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne(pc => pc.Product)
                        .WithMany()
                        .HasForeignKey(pc => pc.ProductId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey(t => new { t.ProductId, t.ColorId });
                        j.ToTable("ProductColor");

                        // Column mappings for ProductColor
                        j.Property(p => p.ProductId).HasColumnName("product_id");
                        j.Property(p => p.ColorId).HasColumnName("color_id");
                    });

            // Configure Product-ProductType relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductType)
                .WithMany()
                .HasForeignKey(p => p.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.CreatedAt).HasColumnName("created_at");
                entity.Property(p => p.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(p => p.Img).HasColumnName("img").HasMaxLength(500);
                entity.Property(p => p.Description).HasColumnName("description").HasMaxLength(700); ;
                entity.Property(p => p.ProductTypeId).HasColumnName("product_type_id");
            });

            // ProductType configuration
            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.ToTable("ProductType");
                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.CreatedAt).HasColumnName("created_at");
                entity.Property(p => p.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Color configuration
            modelBuilder.Entity<Color>(entity =>
            {
                entity.ToTable("Color");
                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.CreatedAt).HasColumnName("created_at");
                entity.Property(c => c.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(c => c.Hex)
                    .HasColumnName("hex")
                    .IsRequired()
                    .HasMaxLength(7);
            });
        }
    }
}
