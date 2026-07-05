using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "production_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    planned_quantity = table.Column<int>(type: "integer", nullable: false),
                    production_reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_production_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_production_orders_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cutting_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fabric_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consumed_weight_kg = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    waste_weight_kg = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    cutting_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    operator_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_cutting_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_cutting_records_fabrics_fabric_id",
                        column: x => x.fabric_id,
                        principalTable: "fabrics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cutting_records_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "production_order_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    planned_quantity = table.Column<int>(type: "integer", nullable: false),
                    barcode_generated = table.Column<bool>(type: "boolean", nullable: false),
                    barcode_printed = table.Column<bool>(type: "boolean", nullable: false),
                    barcode_value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_production_order_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_production_order_items_product_variants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_production_order_items_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "production_timeline_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    event_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production_timeline_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_production_timeline_entries_production_orders_production_or~",
                        column: x => x.production_order_id,
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "production_warehouse_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actual_quantity = table.Column<int>(type: "integer", nullable: false),
                    warehouse_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_production_warehouse_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_production_warehouse_entries_production_orders_production_o~",
                        column: x => x.production_order_id,
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workshop_shipments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workshop = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    shipment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expected_return_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sent_quantity = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_workshop_shipments", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_shipments_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workshop_returns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workshop_shipment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    returned_quantity = table.Column<int>(type: "integer", nullable: false),
                    extra_quantity = table.Column<int>(type: "integer", nullable: false),
                    missing_quantity = table.Column<int>(type: "integer", nullable: false),
                    return_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_workshop_returns", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_returns_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_workshop_returns_workshop_shipments_workshop_shipment_id",
                        column: x => x.workshop_shipment_id,
                        principalTable: "workshop_shipments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cutting_records_fabric_id",
                table: "cutting_records",
                column: "fabric_id");

            migrationBuilder.CreateIndex(
                name: "IX_cutting_records_production_order_id",
                table: "cutting_records",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_order_items_product_variant_id",
                table: "production_order_items",
                column: "product_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_order_items_production_order_id",
                table: "production_order_items",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_orders_product_id",
                table: "production_orders",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_orders_production_number",
                table: "production_orders",
                column: "production_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_production_orders_status",
                table: "production_orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_production_timeline_entries_production_order_id",
                table: "production_timeline_entries",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_warehouse_entries_production_order_id",
                table: "production_warehouse_entries",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_returns_production_order_id",
                table: "workshop_returns",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_returns_workshop_shipment_id",
                table: "workshop_returns",
                column: "workshop_shipment_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_shipments_production_order_id",
                table: "workshop_shipments",
                column: "production_order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cutting_records");

            migrationBuilder.DropTable(
                name: "production_order_items");

            migrationBuilder.DropTable(
                name: "production_timeline_entries");

            migrationBuilder.DropTable(
                name: "production_warehouse_entries");

            migrationBuilder.DropTable(
                name: "workshop_returns");

            migrationBuilder.DropTable(
                name: "workshop_shipments");

            migrationBuilder.DropTable(
                name: "production_orders");
        }
    }
}
