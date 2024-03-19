using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextCommandFramework.Migrations
{
    /// <inheritdoc />
    public partial class addedprimarykeytoProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Profile",
                table: "Profile");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileId",
                table: "Profile",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Profile",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profile",
                table: "Profile",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Profile",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Profile");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileId",
                table: "Profile",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profile",
                table: "Profile",
                column: "ProfileId");
        }
    }
}
