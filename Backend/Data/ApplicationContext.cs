using System;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
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