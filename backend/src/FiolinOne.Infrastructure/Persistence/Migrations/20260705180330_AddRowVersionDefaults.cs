using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    [Migration("20260705180330_AddRowVersionDefaults")]
    [DbContext(typeof(ApplicationDbContext))]
    public partial class AddRowVersionDefaults : Migration
    {
        private static readonly string[] Tables =
        [
            "suppliers",
            "purchase_orders",
            "purchase_order_items",
            "goods_receipts",
            "goods_receipt_items",
            "purchase_invoices",
            "purchase_invoice_items",
            "fabrics",
            "fabric_movements",
            "fabric_reservations",
            "production_orders",
            "production_order_items",
            "cutting_records",
            "workshop_shipments",
            "workshop_returns",
            "production_warehouse_entries"
        ];

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.Sql($"""ALTER TABLE "{table}" ALTER COLUMN "row_version" SET DEFAULT '0'::xid;""");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.Sql($"""ALTER TABLE "{table}" ALTER COLUMN "row_version" DROP DEFAULT;""");
            }
        }
    }
}
