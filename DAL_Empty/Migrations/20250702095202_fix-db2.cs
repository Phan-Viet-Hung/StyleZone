using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL_Empty.Migrations
{
    /// <inheritdoc />
    public partial class fixdb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateBy",
                table: "Product",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "Product",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateBy",
                table: "Product",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "Product",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ProductDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProductDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "ProductDetail");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductDetail");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Product",
                newName: "UpdateBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Product",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Product",
                newName: "CreateBy");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Product",
                newName: "CreateAt");
        }
    }
}
