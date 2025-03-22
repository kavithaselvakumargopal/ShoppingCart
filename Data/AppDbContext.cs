using Microsoft.EntityFrameworkCore;
using ShoppingCart.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace ShoppingCart.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerOrder> Orders { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Data Source=LAPTOP-FD6VE97S;Initial Catalog=Payment_Cart;Integrated Security=True; TrustServerCertificate=True;");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer");
        }

        // public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    }


}
