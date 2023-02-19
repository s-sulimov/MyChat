using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sulimov.MyChat.Server.DAL.Migrations
{
    public partial class Edit_Roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46257ec7-2114-41cb-8108-bc8617e0bf82");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8197ed2c-3d52-4c72-b902-7cae19ee7b4e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "828ac412-f89c-4a5c-aa16-dcfbd9da15f7", "eacec96d-0b14-4b0a-aa37-e466c0f09056", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f41a8ee7-56bf-4242-9184-94e021a1832f", "5e82b679-d6cb-4c16-a275-27442a71dde7", "User", "User" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "828ac412-f89c-4a5c-aa16-dcfbd9da15f7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f41a8ee7-56bf-4242-9184-94e021a1832f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "46257ec7-2114-41cb-8108-bc8617e0bf82", "1bc12428-62b3-429e-a716-1f54973b8ede", "User", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8197ed2c-3d52-4c72-b902-7cae19ee7b4e", "2af041ce-a2fd-48e0-b374-5e3440c96b6d", "Admin", null });
        }
    }
}
