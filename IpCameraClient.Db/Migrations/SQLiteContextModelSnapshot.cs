using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using IpCameraClient.Db;

namespace IpCameraClient.Db.Migrations
{
    [DbContext(typeof(SQLiteContext))]
    partial class SQLiteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("IpCameraClient.Model.Camera", b =>
                {
                    b.Property<int>("CameraId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Model");

                    b.HasKey("CameraId");

                    b.ToTable("Cameras");
                });

            modelBuilder.Entity("IpCameraClient.Model.Record", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CameraId");

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("ImgLocation");

                    b.HasKey("RecordId");

                    b.HasIndex("CameraId");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("IpCameraClient.Model.Record", b =>
                {
                    b.HasOne("IpCameraClient.Model.Camera")
                        .WithMany("Records")
                        .HasForeignKey("CameraId");
                });
        }
    }
}
