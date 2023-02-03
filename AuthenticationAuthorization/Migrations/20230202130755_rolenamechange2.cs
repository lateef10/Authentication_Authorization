using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationAuthorization.Migrations
{
    public partial class rolenamechange2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userProps_roles_RoleId",
                table: "userProps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "rolesProps");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rolesProps",
                table: "rolesProps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_userProps_rolesProps_RoleId",
                table: "userProps",
                column: "RoleId",
                principalTable: "rolesProps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userProps_rolesProps_RoleId",
                table: "userProps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rolesProps",
                table: "rolesProps");

            migrationBuilder.RenameTable(
                name: "rolesProps",
                newName: "roles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_userProps_roles_RoleId",
                table: "userProps",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
