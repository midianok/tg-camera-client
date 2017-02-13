using IpCameraClient.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace IpCameraClient.Db
{
    public class SQLiteContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
        public DbSet<Camera> Cameras { get; set; }

        public SQLiteContext() : base()
        {
            Records.Include(x => x.Camera);
            Cameras.Include(x => x.Records);
        }

        public void Seed(Action<SQLiteContext> seedAction)
        {
            if (Database.EnsureCreated())
            {
                seedAction(this);
                SaveChanges();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite($"Filename=CameraClient.db");
        
    }
}
