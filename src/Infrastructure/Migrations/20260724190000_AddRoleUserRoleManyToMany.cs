using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRN.ProductAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleUserRoleManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            // Seed roles used for data migration and runtime
            var adminRoleId = new Guid("11111111-1111-1111-1111-111111111111");
            var memberRoleId = new Guid("22222222-2222-2222-2222-222222222222");

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Name", "Description" },
                values: new object[,]
                {
                    { adminRoleId, "Admin", "Full access to mutating product operations" },
                    { memberRoleId, "Member", "Standard registered user" }
                });

            // Map existing User.Role column values into UserRole before dropping the column
            migrationBuilder.Sql($@"
INSERT INTO [UserRole] ([UserId], [RoleId])
SELECT u.[Id],
       CASE
           WHEN u.[Role] = N'Admin' THEN '{adminRoleId}'
           ELSE '{memberRoleId}'
       END
FROM [User] u
WHERE NOT EXISTS (
    SELECT 1 FROM [UserRole] ur WHERE ur.[UserId] = u.[Id]
);");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Member");

            migrationBuilder.Sql(@"
UPDATE u
SET u.[Role] = r.[Name]
FROM [User] u
INNER JOIN [UserRole] ur ON ur.[UserId] = u.[Id]
INNER JOIN [Role] r ON r.[Id] = ur.[RoleId]
WHERE r.[Name] IN (N'Admin', N'Member');");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
