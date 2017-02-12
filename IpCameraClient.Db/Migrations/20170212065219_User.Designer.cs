using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using IpCameraClient.Db;
using IpCameraClient.Model;

namespace IpCameraClient.Db.Migrations
{
    [DbContext(typeof(SQLiteContext))]
    [Migration("20170212065219_User")]
    partial class User
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("ContentLocation");

                    b.Property<int>("ContentType");

                    b.Property<DateTime>("DateTime");

                    b.HasKey("RecordId");

                    b.HasIndex("CameraId");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("IpCameraClient.Model.Record", b =>
                {
                    b.HasOne("IpCameraClient.Model.Camera", "Camera")
                        .WithMany("Records")
                        .HasForeignKey("CameraId");
                });
        }
    }
}
