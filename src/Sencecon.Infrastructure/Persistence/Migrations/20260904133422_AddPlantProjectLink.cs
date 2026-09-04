using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sencecon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantProjectLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Plants",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plants_ProjectId",
                table: "Plants",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Projects_ProjectId",
                table: "Plants",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Projects_ProjectId",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_ProjectId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Plants");
        }
    }
}
