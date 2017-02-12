using IpCameraClient.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace IpCameraClient.Db
{
    public class SQLiteContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
        public DbSet<Camera> Cameras { get; set; }

        public SQLiteContext() : base()
        {
            if (Database.EnsureCreated())
            {
                Seed();
            }
                
        }

        private void Seed()
        {
  
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite($"Filename=CameraClient.db");
        
    }
}
