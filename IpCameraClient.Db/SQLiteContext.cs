using IpCameraClient.Model;
using Microsoft.EntityFrameworkCore;
using System;
using IpCameraClient.Model.Telegram;

namespace IpCameraClient.Db
{
    public class SQLiteContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<TelegramUser> TelegramUser { get; set; }

        public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options)
        {
            Records.Include(x => x.Camera);
            Cameras.Include(x => x.Records);
        }

        public void Seed(Action<SQLiteContext> seedAction)
        {
            if (!Database.EnsureCreated()) return;
            seedAction(this);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite($"Filename=CameraClient.db");
        
    }
}
