using Microsoft.EntityFrameworkCore.Migrations;

namespace IpCameraClient.Db.Migrations
{
    public partial class User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgLocation",
                table: "Records",
                newName: "ContentLocation");

            migrationBuilder.AddColumn<int>(
                name: "ContentType",
                table: "Records",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Records");

            migrationBuilder.RenameColumn(
                name: "ContentLocation",
                table: "Records",
                newName: "ImgLocation");
        }
    }
}
