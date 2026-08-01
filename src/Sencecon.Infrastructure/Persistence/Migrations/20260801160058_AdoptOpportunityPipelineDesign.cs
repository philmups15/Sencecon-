using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sencecon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdoptOpportunityPipelineDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpportunityNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OpportunityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunityNotes_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpportunityActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OpportunityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunityActivities_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityNotes_OpportunityId",
                table: "OpportunityNotes",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityActivities_OpportunityId",
                table: "OpportunityActivities",
                column: "OpportunityId");

            // Carry over any existing free-text Notes value as the first entry in the new notes thread.
            migrationBuilder.Sql(
                """
                INSERT INTO "OpportunityNotes" ("Id", "OpportunityId", "Text", "CreatedBy", "Created")
                SELECT md5(random()::text || clock_timestamp()::text || "Id"::text)::uuid, "Id", "Notes", "CreatedBy", "Created"
                FROM "Opportunities"
                WHERE "Notes" IS NOT NULL AND "Notes" <> '';
                """);

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Opportunities");

            migrationBuilder.AddColumn<bool>(
                name: "Converted",
                table: "Opportunities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StageData",
                table: "Opportunities",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "OpportunityAttachments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "OpportunityAttachments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Backfill existing attachments: each stands alone as v1 of a document titled after its file name.
            migrationBuilder.Sql(
                """
                UPDATE "OpportunityAttachments"
                SET "Title" = "FileName", "Version" = 1
                WHERE "Title" = '';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpportunityActivities");

            migrationBuilder.DropTable(
                name: "OpportunityNotes");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "OpportunityAttachments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "OpportunityAttachments");

            migrationBuilder.DropColumn(
                name: "Converted",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "StageData",
                table: "Opportunities");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Opportunities",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
