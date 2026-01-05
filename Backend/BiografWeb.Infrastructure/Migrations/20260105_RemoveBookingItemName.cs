using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiografWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBookingItemName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "booking_items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "booking_items",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}

