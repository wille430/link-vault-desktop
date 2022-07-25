using Microsoft.EntityFrameworkCore;
using LinkVault.Models;
using System;

namespace LinkVault.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Link> Links { get; set; }
        public DbSet<LinkCollection> Collections { get; set; }

        public string DbPath { get; }

        public AppDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "linkvault.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

    }
}