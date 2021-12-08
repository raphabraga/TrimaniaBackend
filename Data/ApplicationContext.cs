using System;
using Microsoft.EntityFrameworkCore;
using TrimaniaBackend.Models;

namespace TrimaniaBackend.Data
{
    public class ApplicationContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(Environment.GetEnvironmentVariable("DB_CString"));
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}