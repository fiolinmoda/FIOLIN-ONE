using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_variants_product_colors_color_id",
                table: "product_variants");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variants_product_sizes_size_id",
                table: "product_variants");

            migrationBuilder.DropTable(
                name: "product_colors");

            migrationBuilder.DropTable(
                name: "product_sizes");

            migrationBuilder.DropColumn(
                name: "brand",
                table: "products");

            migrationBuilder.DropColumn(
                name: "category",
                table: "products");

            migrationBuilder.DropColumn(
                name: "season",
                table: "products");

            migrationBuilder.AddColumn<Guid>(
                name: "brand_id",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "season_id",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "colors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_colors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fabric_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fabric_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "seasons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seasons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sizes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sizes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_products_brand_id",
                table: "products",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_season_id",
                table: "products",
                column: "season_id");

            migrationBuilder.CreateIndex(
                name: "IX_brands_code",
                table: "brands",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_code",
                table: "categories",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_colors_code",
                table: "colors",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fabric_types_code",
                table: "fabric_types",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_seasons_code",
                table: "seasons",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sizes_code",
                table: "sizes",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variants_colors_color_id",
                table: "product_variants",
                column: "color_id",
                principalTable: "colors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variants_sizes_size_id",
                table: "product_variants",
                column: "size_id",
                principalTable: "sizes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_brands_brand_id",
                table: "products",
                column: "brand_id",
                principalTable: "brands",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_category_id",
                table: "products",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_seasons_season_id",
                table: "products",
                column: "season_id",
                principalTable: "seasons",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_variants_colors_color_id",
                table: "product_variants");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variants_sizes_size_id",
                table: "product_variants");

            migrationBuilder.DropForeignKey(
                name: "FK_products_brands_brand_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_category_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_seasons_season_id",
                table: "products");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "colors");

            migrationBuilder.DropTable(
                name: "fabric_types");

            migrationBuilder.DropTable(
                name: "seasons");

            migrationBuilder.DropTable(
                name: "sizes");

            migrationBuilder.DropIndex(
                name: "IX_products_brand_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_category_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_season_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "brand_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "season_id",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "brand",
                table: "products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "season",
                table: "products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "product_colors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_colors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_sizes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_sizes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_colors_name",
                table: "product_colors",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_sizes_name",
                table: "product_sizes",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variants_product_colors_color_id",
                table: "product_variants",
                column: "color_id",
                principalTable: "product_colors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variants_product_sizes_size_id",
                table: "product_variants",
                column: "size_id",
                principalTable: "product_sizes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
