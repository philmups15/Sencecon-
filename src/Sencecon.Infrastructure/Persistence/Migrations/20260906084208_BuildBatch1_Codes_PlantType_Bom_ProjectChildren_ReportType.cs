using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sencecon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BuildBatch1_Codes_PlantType_Bom_ProjectChildren_ReportType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Atomic per-prefix code sequences (replaces read-max-then-increment).
            // Start each from the current max number in the matching table so
            // existing codes stay unique.
            foreach (var (seq, table) in new[]
            {
                ("opp_code_seq", "Opportunities"), ("prj_code_seq", "Projects"), ("plt_code_seq", "Plants"),
                ("wo_code_seq", "WorkOrders"), ("sur_code_seq", "Surveys"), ("dsn_code_seq", "Designs"),
                ("nc_code_seq", "NonConformities"),
            })
            {
                migrationBuilder.Sql($@"
                    CREATE SEQUENCE IF NOT EXISTS {seq};
                    DO $$
                    DECLARE m bigint;
                    BEGIN
                        SELECT COALESCE(MAX(NULLIF(regexp_replace(""Code"", '\D', '', 'g'), '')::bigint), 0)
                        INTO m FROM ""{table}"";
                        IF m = 0 THEN
                            PERFORM setval('{seq}', 1, false);
                        ELSE
                            PERFORM setval('{seq}', m, true);
                        END IF;
                    END $$;
                ");
            }

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Plants",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Plants",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Plants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "BomItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "BomItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectBudgetLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BudgetAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBudgetLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectBudgetLines_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMilestones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMilestones_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRisks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Mitigation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRisks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRisks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Owner = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subcontractors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Scope = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcontractors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subcontractors_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_ProjectId",
                table: "BomItems",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBudgetLines_ProjectId",
                table: "ProjectBudgetLines",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestones_ProjectId",
                table: "ProjectMilestones",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRisks_ProjectId",
                table: "ProjectRisks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_ProjectId",
                table: "ProjectTasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcontractors_ProjectId",
                table: "Subcontractors",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_BomItems_Projects_ProjectId",
                table: "BomItems",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var seq in new[]
            {
                "opp_code_seq", "prj_code_seq", "plt_code_seq", "wo_code_seq",
                "sur_code_seq", "dsn_code_seq", "nc_code_seq",
            })
            {
                migrationBuilder.Sql($"DROP SEQUENCE IF EXISTS {seq};");
            }

            migrationBuilder.DropForeignKey(
                name: "FK_BomItems_Projects_ProjectId",
                table: "BomItems");

            migrationBuilder.DropTable(
                name: "ProjectBudgetLines");

            migrationBuilder.DropTable(
                name: "ProjectMilestones");

            migrationBuilder.DropTable(
                name: "ProjectRisks");

            migrationBuilder.DropTable(
                name: "ProjectTasks");

            migrationBuilder.DropTable(
                name: "Subcontractors");

            migrationBuilder.DropIndex(
                name: "IX_BomItems_ProjectId",
                table: "BomItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "BomItems");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "BomItems");
        }
    }
}
