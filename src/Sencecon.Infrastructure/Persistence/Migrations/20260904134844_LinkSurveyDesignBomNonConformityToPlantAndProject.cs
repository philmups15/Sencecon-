using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sencecon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LinkSurveyDesignBomNonConformityToPlantAndProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlantId",
                table: "Surveys",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlantId",
                table: "NonConformities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Designs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlantId",
                table: "BomItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_PlantId",
                table: "Surveys",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_NonConformities_PlantId",
                table: "NonConformities",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Designs_ProjectId",
                table: "Designs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_PlantId",
                table: "BomItems",
                column: "PlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_BomItems_Plants_PlantId",
                table: "BomItems",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Designs_Projects_ProjectId",
                table: "Designs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NonConformities_Plants_PlantId",
                table: "NonConformities",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_Plants_PlantId",
                table: "Surveys",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BomItems_Plants_PlantId",
                table: "BomItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Designs_Projects_ProjectId",
                table: "Designs");

            migrationBuilder.DropForeignKey(
                name: "FK_NonConformities_Plants_PlantId",
                table: "NonConformities");

            migrationBuilder.DropForeignKey(
                name: "FK_Surveys_Plants_PlantId",
                table: "Surveys");

            migrationBuilder.DropIndex(
                name: "IX_Surveys_PlantId",
                table: "Surveys");

            migrationBuilder.DropIndex(
                name: "IX_NonConformities_PlantId",
                table: "NonConformities");

            migrationBuilder.DropIndex(
                name: "IX_Designs_ProjectId",
                table: "Designs");

            migrationBuilder.DropIndex(
                name: "IX_BomItems_PlantId",
                table: "BomItems");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "NonConformities");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Designs");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "BomItems");
        }
    }
}
