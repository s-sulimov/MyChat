using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sulimov.MyChat.Server.DAL.Migrations
{
    public partial class Added_ChatRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbChatDbUser");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "828ac412-f89c-4a5c-aa16-dcfbd9da15f7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f41a8ee7-56bf-4242-9184-94e021a1832f");

            migrationBuilder.AddColumn<string>(
                name: "DbUserId",
                table: "Chats",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DbChatUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    DbChatId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbChatUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbChatUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DbChatUser_ChatRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ChatRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DbChatUser_Chats_DbChatId",
                        column: x => x.DbChatId,
                        principalTable: "Chats",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "37795a80-da6a-4dea-9217-d9d3d2da38a5", "87922471-6e17-445c-b246-2cec40c5f5c0", "User", "User" },
                    { "bf0c4574-b76f-45bd-9c3e-7891ef576147", "4b37a9cf-9e35-4ea9-af55-4a8dc689390d", "Admin", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "ChatRoles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Owner" },
                    { 2, "Admin" },
                    { 3, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_DbUserId",
                table: "Chats",
                column: "DbUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DbChatUser_DbChatId",
                table: "DbChatUser",
                column: "DbChatId");

            migrationBuilder.CreateIndex(
                name: "IX_DbChatUser_RoleId",
                table: "DbChatUser",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_DbChatUser_UserId",
                table: "DbChatUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_DbUserId",
                table: "Chats",
                column: "DbUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_DbUserId",
                table: "Chats");

            migrationBuilder.DropTable(
                name: "DbChatUser");

            migrationBuilder.DropTable(
                name: "ChatRoles");

            migrationBuilder.DropIndex(
                name: "IX_Chats_DbUserId",
                table: "Chats");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "37795a80-da6a-4dea-9217-d9d3d2da38a5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bf0c4574-b76f-45bd-9c3e-7891ef576147");

            migrationBuilder.DropColumn(
                name: "DbUserId",
                table: "Chats");

            migrationBuilder.CreateTable(
                name: "DbChatDbUser",
                columns: table => new
                {
                    ChatsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbChatDbUser", x => new { x.ChatsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_DbChatDbUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbChatDbUser_Chats_ChatsId",
                        column: x => x.ChatsId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "828ac412-f89c-4a5c-aa16-dcfbd9da15f7", "eacec96d-0b14-4b0a-aa37-e466c0f09056", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f41a8ee7-56bf-4242-9184-94e021a1832f", "5e82b679-d6cb-4c16-a275-27442a71dde7", "User", "User" });

            migrationBuilder.CreateIndex(
                name: "IX_DbChatDbUser_UsersId",
                table: "DbChatDbUser",
                column: "UsersId");
        }
    }
}
