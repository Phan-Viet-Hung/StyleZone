using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL_Empty.Migrations
{
    /// <inheritdoc />
    public partial class fixdb4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Brand__66603565",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Categ__656C112C",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Color__628FA481",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Mater__6477ECF3",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Origi__6754599E",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__SizeI__6383C8BA",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Suppl__68487DD7",
                table: "ProductDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "SupplierId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "SizeId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "OriginId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ColorId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Brand__66603565",
                table: "ProductDetail",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Categ__656C112C",
                table: "ProductDetail",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Color__628FA481",
                table: "ProductDetail",
                column: "ColorId",
                principalTable: "Color",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Mater__6477ECF3",
                table: "ProductDetail",
                column: "MaterialId",
                principalTable: "Material",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Origi__6754599E",
                table: "ProductDetail",
                column: "OriginId",
                principalTable: "Origin",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__SizeI__6383C8BA",
                table: "ProductDetail",
                column: "SizeId",
                principalTable: "Size",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Suppl__68487DD7",
                table: "ProductDetail",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Brand__66603565",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Categ__656C112C",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Color__628FA481",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Mater__6477ECF3",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Origi__6754599E",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__SizeI__6383C8BA",
                table: "ProductDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductDe__Suppl__68487DD7",
                table: "ProductDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "SupplierId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SizeId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OriginId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ColorId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BrandId",
                table: "ProductDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Brand__66603565",
                table: "ProductDetail",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Categ__656C112C",
                table: "ProductDetail",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Color__628FA481",
                table: "ProductDetail",
                column: "ColorId",
                principalTable: "Color",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Mater__6477ECF3",
                table: "ProductDetail",
                column: "MaterialId",
                principalTable: "Material",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Origi__6754599E",
                table: "ProductDetail",
                column: "OriginId",
                principalTable: "Origin",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__SizeI__6383C8BA",
                table: "ProductDetail",
                column: "SizeId",
                principalTable: "Size",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ProductDe__Suppl__68487DD7",
                table: "ProductDetail",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
