using Microsoft.EntityFrameworkCore;
using WebAppApiArq.Entities;
using WebAppApiArq.Entities;
using WebAppApiArq.Entities;
using WebAppApiArq.Models;

namespace WebAppApiArq.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> 
            options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ProductBalance> ProductBalances { get; set; }
        public DbSet<ProductKardex> ProductKardexes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Configurar el campo Name en la tabla Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("tbProducts");

                entity.HasKey(p => p.Id); // porque agregar un campo con el nombre product para no confundir la convencion

                entity.Property(p => p.Name)
                      .IsRequired()          // Campo requerido
                      .HasMaxLength(128);    // Longitud máxima de 128 caracteres

                // Configurar la precisión y escala del campo Price
                entity.Property(p => p.Price)
                      .HasPrecision(18, 2);  // Hasta 18 dígitos con 2 decimales

                // Configuración de la relación con ProductBalance
                entity.HasMany(p => p.ProductBalances)
                      .WithOne(pb => pb.Product)
                      .HasForeignKey(pb => pb.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Configuración de la relación con ProductKardex
                entity.HasMany(p => p.ProductKardexs)
                      .WithOne(pk => pk.Product)
                      .HasForeignKey(pk => pk.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);


            });

            // Mapea Category a la tabla "tbCategories" y define la longitud del campo "Name"
            modelBuilder.Entity<Category>()
                .ToTable("tbCategories")
                .Property(c => c.Name)
                .HasMaxLength(128)
                .IsRequired(); // Campo obligatorio


            // Mapea Supplier a la tabla "tbSuppliers" y define la longitud del campo "Name"
            modelBuilder.Entity<Supplier>()
                .ToTable("tbSuppliers")
                .Property(s => s.Name)
                .HasMaxLength(128)
                .IsRequired(); // Campo obligatorio


            // Relación Product -> Category (muchos a uno)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);  // Configura como opcional

            // Relación Product -> Supplier (muchos a uno)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);  // Configura como opcional



            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Token); // Define la clave primaria
                entity.Property(rt => rt.Expires).IsRequired(); // Asegúrate de que Expires es requerido
                entity.Property(rt => rt.UserId).IsRequired(); // Asegúrate de que UserId es requerido
                entity.Property(rt => rt.Created).IsRequired(); // Asegúrate de que Created es requerido

                // Si necesitas un índice único para el token, puedes agregarlo
                entity.HasIndex(rt => rt.Token).IsUnique();
            });


            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("tbUser"); // Nombre de la tabla en la base de datos

                entity.HasKey(u => u.Id); // Define la clave primaria

                entity.Property(u => u.UserName)
                      .IsRequired()
                      .HasMaxLength(128); // Longitud de UserName

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(128); // Longitud de Name

                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(128); // Longitud de Password

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(100); // Establece restricciones en Email

                entity.Property(u => u.EmailConfirmed)
                      .IsRequired(); // Asegúrate de que EmailConfirmed es requerido

                entity.Property(u => u.Created)
                      .IsRequired(); // Asegúrate de que Created es requerido

                entity.Property(u => u.Roles)
                      .IsRequired()
                      .HasMaxLength(512); // Longitud de Roles
            });


            // Configuración de ProductBalance
            modelBuilder.Entity<ProductBalance>(entity =>
            {
                entity.ToTable("tbBalances");

                entity.HasKey(pb => pb.Id);

                entity.Property(pb => pb.Created)
                      .IsRequired();

                entity.Property(pb => pb.Amount)
                      .HasPrecision(18, 2);

                // Configuración de la relación con Product
                entity.HasOne(pb => pb.Product)
                      .WithMany(p => p.ProductBalances)
                      .HasForeignKey(pb => pb.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Configuración de la relación con User
                entity.HasOne(pb => pb.User)
                      .WithMany()
                      .HasForeignKey(pb => pb.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Configuración de ProductKardex
            modelBuilder.Entity<ProductKardex>(entity =>
            {
                entity.ToTable("tbKardexs");

                entity.HasKey(pk => pk.Id);

                entity.Property(pk => pk.Created)
                      .IsRequired();

                entity.Property(pk => pk.Amount)
                      .HasPrecision(18, 2);

                // Configuración de la relación con Product
                entity.HasOne(pk => pk.Product)
                      .WithMany(p => p.ProductKardexs)
                      .HasForeignKey(pk => pk.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Configuración de la relación con User
                entity.HasOne(pk => pk.User)
                      .WithMany()
                      .HasForeignKey(pk => pk.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });


            base.OnModelCreating(modelBuilder);
        }



  }


}
