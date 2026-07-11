using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductModelCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "model_code",
                table: "products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE products
                SET model_code = LEFT(
                    CASE
                        WHEN product_code ~ '^[0-9]{3,6}[A-Za-z]' THEN substring(product_code from '^([0-9]{3,6})')
                        WHEN product_code ~* '^[A-Za-z]+[0-9]{2,6}' AND product_code !~* '^TYB' THEN substring(product_code from '^([A-Za-z]+[0-9]{2,6})')
                        WHEN product_code ~* '^[0-9]+(\.[0-9]+)?E[0-9]+$' THEN 'MDL-' || substring(md5(coalesce(product_name, '') || id::text) from 1 for 8)
                        WHEN product_code ~* '^TYB' THEN 'MDL-' || substring(md5(coalesce(product_name, '') || id::text) from 1 for 8)
                        ELSE NULLIF(trim(product_code), '')
                    END,
                    50);
                """);

            migrationBuilder.Sql("""
                UPDATE products target
                SET model_code = grouped.model_code
                FROM (
                    SELECT
                        product_name,
                        brand_id,
                        category_id,
                        season_id,
                        MIN(model_code) AS model_code
                    FROM products
                    WHERE model_code !~* '^MDL-'
                    GROUP BY product_name, brand_id, category_id, season_id
                ) grouped
                WHERE target.product_name = grouped.product_name
                    AND target.brand_id IS NOT DISTINCT FROM grouped.brand_id
                    AND target.category_id IS NOT DISTINCT FROM grouped.category_id
                    AND target.season_id IS NOT DISTINCT FROM grouped.season_id
                    AND target.model_code ~* '^MDL-';
                """);

            migrationBuilder.Sql("""
                UPDATE product_import_profiles
                SET mapping_json = replace(mapping_json::text, '"modelCode": "Barkod"', '"modelCode": null')::jsonb
                WHERE mapping_json::text LIKE '%"modelCode": "Barkod"%';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_products_model_code",
                table: "products",
                column: "model_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_products_model_code",
                table: "products");

            migrationBuilder.DropColumn(
                name: "model_code",
                table: "products");
        }
    }
}
