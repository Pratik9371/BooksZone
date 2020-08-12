using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksZone.DataAccess.Migrations
{
    public partial class PriceChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ShoppingCarts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "ShoppingCarts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
