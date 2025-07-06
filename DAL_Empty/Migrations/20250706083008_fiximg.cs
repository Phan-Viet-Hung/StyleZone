using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL_Empty.Migrations
{
    /// <inheritdoc />
    public partial class fiximg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Image__ProductDe__6FE99F9F",
                table: "Image");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductDetailId",
                table: "Image",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "Image",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Image",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Image",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsMainImage",
                table: "Image",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Image",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__Image__ProductDe__6FE99F9F",
                table: "Image",
                column: "ProductDetailId",
                principalTable: "ProductDetail",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Image__ProductDe__6FE99F9F",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "AltText",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "IsMainImage",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Image");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductDetailId",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__Image__ProductDe__6FE99F9F",
                table: "Image",
                column: "ProductDetailId",
                principalTable: "ProductDetail",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
