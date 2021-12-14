using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.State).IsRequired();
                entity.Property(e => e.City).IsRequired();
                entity.Property(e => e.Street).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Cpf).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Cpf).IsRequired();
                entity.Property(e => e.Login).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });
        }
    }
}