using System;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ChartItem> Items { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("DefaultConnection"),
            new MySqlServerVersion(new Version(8, 0, 27)
            ));
            // optionsBuilder.UseMySql("server=localhost;port=3306;database=trimaniadb;user=trilogo;password=1234",
            // new MySqlServerVersion(new Version(8, 0, 27)
            // ));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.State).IsRequired();
                entity.Property(e => e.City).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Login).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Login).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.Items);
                entity.HasOne(e => e.Client);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.StockQuantity).IsRequired();
            });

            modelBuilder.Entity<ChartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
            });
        }
    }
}