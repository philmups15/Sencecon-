using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sencecon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOpportunityStageDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NegotiationNotes",
                table: "Opportunities",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProposalNotes",
                table: "Opportunities",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SiteVisitDate",
                table: "Opportunities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteVisitNotes",
                table: "Opportunities",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "WonDate",
                table: "Opportunities",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NegotiationNotes",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "ProposalNotes",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "SiteVisitDate",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "SiteVisitNotes",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "WonDate",
                table: "Opportunities");
        }
    }
}
