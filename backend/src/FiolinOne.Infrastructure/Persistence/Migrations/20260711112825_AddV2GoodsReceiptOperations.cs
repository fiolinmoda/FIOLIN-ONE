using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddV2GoodsReceiptOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "box",
                table: "product_variants",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "last_supplier_id",
                table: "product_variants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shelf",
                table: "product_variants",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "operation_goods_receipts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    movement_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    purchase_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    shelf = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    box = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    stock_before = table.Column<int>(type: "integer", nullable: false),
                    stock_after = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operation_goods_receipts", x => x.id);
                    table.ForeignKey(
                        name: "FK_operation_goods_receipts_product_variants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_operation_goods_receipts_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_last_supplier_id",
                table: "product_variants",
                column: "last_supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_operation_goods_receipts_movement_type",
                table: "operation_goods_receipts",
                column: "movement_type");

            migrationBuilder.CreateIndex(
                name: "IX_operation_goods_receipts_product_variant_id",
                table: "operation_goods_receipts",
                column: "product_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_operation_goods_receipts_supplier_id",
                table: "operation_goods_receipts",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_operation_goods_receipts_transaction_date",
                table: "operation_goods_receipts",
                column: "transaction_date");

            migrationBuilder.AddForeignKey(
                name: "FK_product_variants_suppliers_last_supplier_id",
                table: "product_variants",
                column: "last_supplier_id",
                principalTable: "suppliers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_variants_suppliers_last_supplier_id",
                table: "product_variants");

            migrationBuilder.DropTable(
                name: "operation_goods_receipts");

            migrationBuilder.DropIndex(
                name: "IX_product_variants_last_supplier_id",
                table: "product_variants");

            migrationBuilder.DropColumn(
                name: "box",
                table: "product_variants");

            migrationBuilder.DropColumn(
                name: "last_supplier_id",
                table: "product_variants");

            migrationBuilder.DropColumn(
                name: "shelf",
                table: "product_variants");
        }
    }
}
