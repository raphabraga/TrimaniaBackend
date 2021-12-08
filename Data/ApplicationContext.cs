using Microsoft.EntityFrameworkCore;
using TrimaniaBackend.Models;

namespace TrimaniaBackend.Data
{
    public class ApplicationContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost; database=trimaniadb;user=trilogo;password=1234");
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}