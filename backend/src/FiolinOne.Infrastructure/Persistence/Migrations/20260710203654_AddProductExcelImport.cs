using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiolinOne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductExcelImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product_import_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    file_name = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    total_records = table.Column<int>(type: "integer", nullable: false),
                    inserted_records = table.Column<int>(type: "integer", nullable: false),
                    existing_records = table.Column<int>(type: "integer", nullable: false),
                    skipped_records = table.Column<int>(type: "integer", nullable: false),
                    error_records = table.Column<int>(type: "integer", nullable: false),
                    duration_milliseconds = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_import_histories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_import_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    file_signature = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    mapping_json = table.Column<string>(type: "jsonb", nullable: false),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_import_profiles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_import_profiles_file_signature",
                table: "product_import_profiles",
                column: "file_signature",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_import_histories");

            migrationBuilder.DropTable(
                name: "product_import_profiles");
        }
    }
}
