using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchasingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    supplier_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tax_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    payment_term = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    row_version = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "purchase_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expected_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    row_version = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_purchase_orders_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "goods_receipts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    receipt_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    receipt_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    warehouse = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    row_version = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goods_receipts", x => x.id);
                    table.ForeignKey(
                        name: "FK_goods_receipts_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_goods_receipts_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchase_invoices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    invoice_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invoice_amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    row_version = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_invoices", x => x.id);
                    table.ForeignKey(
                        name: "FK_purchase_invoices_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_invoices_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchase_order_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fabric_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    color_id = table.Column<Guid>(type: "uuid", nullable: true),
                    item_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    received_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    row_version = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_order_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_purchase_order_items_colors_color_id",
                        column: x => x.color_id,
                        principalTable: "colors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_order_items_fabric_types_fabric_type_id",
                        column: x => x.fabric_type_id,
                        principalTable: "fabric_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_order_items_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "goods_receipt_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    goods_receipt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    item_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    received_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    acceptance = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    difference_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    row_version = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goods_receipt_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_goods_receipt_items_goods_receipts_goods_receipt_id",
                        column: x => x.goods_receipt_id,
                        principalTable: "goods_receipts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_goods_receipt_items_purchase_order_items_purchase_order_ite~",
                        column: x => x.purchase_order_item_id,
                        principalTable: "purchase_order_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchase_invoice_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    item_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    row_version = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_invoice_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_purchase_invoice_items_purchase_invoices_purchase_invoice_id",
                        column: x => x.purchase_invoice_id,
                        principalTable: "purchase_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchase_invoice_items_purchase_order_items_purchase_order_~",
                        column: x => x.purchase_order_item_id,
                        principalTable: "purchase_order_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipt_items_goods_receipt_id",
                table: "goods_receipt_items",
                column: "goods_receipt_id");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipt_items_purchase_order_item_id",
                table: "goods_receipt_items",
                column: "purchase_order_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_purchase_order_id",
                table: "goods_receipts",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_receipt_number",
                table: "goods_receipts",
                column: "receipt_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_status",
                table: "goods_receipts",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_supplier_id",
                table: "goods_receipts",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_invoice_items_purchase_invoice_id",
                table: "purchase_invoice_items",
                column: "purchase_invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_invoice_items_purchase_order_item_id",
                table: "purchase_invoice_items",
                column: "purchase_order_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_invoices_invoice_number",
                table: "purchase_invoices",
                column: "invoice_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_invoices_purchase_order_id",
                table: "purchase_invoices",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_invoices_status",
                table: "purchase_invoices",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_invoices_supplier_id",
                table: "purchase_invoices",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_items_color_id",
                table: "purchase_order_items",
                column: "color_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_items_fabric_type_id",
                table: "purchase_order_items",
                column: "fabric_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_items_purchase_order_id",
                table: "purchase_order_items",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_items_status",
                table: "purchase_order_items",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_orders_purchase_number",
                table: "purchase_orders",
                column: "purchase_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_orders_status",
                table: "purchase_orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_orders_supplier_id",
                table: "purchase_orders",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_supplier_code",
                table: "suppliers",
                column: "supplier_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_supplier_name",
                table: "suppliers",
                column: "supplier_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "goods_receipt_items");

            migrationBuilder.DropTable(
                name: "purchase_invoice_items");

            migrationBuilder.DropTable(
                name: "goods_receipts");

            migrationBuilder.DropTable(
                name: "purchase_invoices");

            migrationBuilder.DropTable(
                name: "purchase_order_items");

            migrationBuilder.DropTable(
                name: "purchase_orders");

            migrationBuilder.DropTable(
                name: "suppliers");
        }
    }
}
