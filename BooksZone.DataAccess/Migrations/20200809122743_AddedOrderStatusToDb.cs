using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksZone.DataAccess.Migrations
{
    public partial class AddedOrderStatusToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "OrderHeaders",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "OrderHeaders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "OrderHeaders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentStatus",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
