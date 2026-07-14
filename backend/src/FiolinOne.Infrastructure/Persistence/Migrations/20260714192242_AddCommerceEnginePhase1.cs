using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCommerceEnginePhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cms_banners",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    subtitle = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    image_url = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    link_url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    placement = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cms_banners", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cms_menus",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    location = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cms_menus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cms_pages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cms_pages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cms_seo_pages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    route = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    meta_title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    meta_description = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    canonical = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    open_graph_title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    open_graph_description = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    twitter_card = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    schema_json = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cms_seo_pages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cms_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    group = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cms_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cms_sliders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    subtitle = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    image_url = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    link_url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cms_sliders", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cms_banners_placement_sort_order",
                table: "cms_banners",
                columns: new[] { "placement", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_cms_menus_location_sort_order",
                table: "cms_menus",
                columns: new[] { "location", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_cms_pages_slug",
                table: "cms_pages",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cms_seo_pages_route",
                table: "cms_seo_pages",
                column: "route",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cms_settings_key",
                table: "cms_settings",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cms_sliders_sort_order",
                table: "cms_sliders",
                column: "sort_order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cms_banners");

            migrationBuilder.DropTable(
                name: "cms_menus");

            migrationBuilder.DropTable(
                name: "cms_pages");

            migrationBuilder.DropTable(
                name: "cms_seo_pages");

            migrationBuilder.DropTable(
                name: "cms_settings");

            migrationBuilder.DropTable(
                name: "cms_sliders");
        }
    }
}
