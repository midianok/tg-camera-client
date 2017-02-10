using IpCameraClient.Model;
using Microsoft.EntityFrameworkCore;

namespace IpCameraClient.Db
{
    public class SQLiteContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
        public DbSet<Camera> Cameras { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./CameraRecords.db");
        }
    }
}
