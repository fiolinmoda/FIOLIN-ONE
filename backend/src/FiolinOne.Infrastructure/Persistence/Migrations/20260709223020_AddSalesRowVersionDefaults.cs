using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesRowVersionDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "row_version",
                table: "sales_orders",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "'0'::xid",
                oldClrType: typeof(uint),
                oldType: "xid",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<uint>(
                name: "row_version",
                table: "sales_order_items",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "'0'::xid",
                oldClrType: typeof(uint),
                oldType: "xid",
                oldRowVersion: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "row_version",
                table: "sales_orders",
                type: "xid",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "xid",
                oldRowVersion: true,
                oldDefaultValueSql: "'0'::xid");

            migrationBuilder.AlterColumn<uint>(
                name: "row_version",
                table: "sales_order_items",
                type: "xid",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "xid",
                oldRowVersion: true,
                oldDefaultValueSql: "'0'::xid");
        }
    }
}
