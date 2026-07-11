using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestoreImportedVariantBarcodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE product_variants AS variant
                SET barcode = product.product_code
                FROM products AS product
                WHERE variant.product_id = product.id
                  AND variant.barcode <> product.product_code
                  AND lower(variant.barcode) LIKE lower(product.product_code) || '%'
                  AND length(variant.barcode) > length(product.product_code);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data repair is intentionally not reversible because the generated barcode suffix
            // cannot be reconstructed safely after restoring the original imported barcode.
        }
    }
}
