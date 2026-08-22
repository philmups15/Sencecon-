using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sencecon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyProjectLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Surveys",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_ProjectId",
                table: "Surveys",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_Projects_ProjectId",
                table: "Surveys",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Surveys_Projects_ProjectId",
                table: "Surveys");

            migrationBuilder.DropIndex(
                name: "IX_Surveys_ProjectId",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Surveys");
        }
    }
}
