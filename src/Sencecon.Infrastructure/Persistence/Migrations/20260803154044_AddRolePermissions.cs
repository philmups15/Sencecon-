using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sencecon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CanRead = table.Column<bool>(type: "boolean", nullable: false),
                    CanWrite = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CanRead", "CanWrite", "Created", "LastModified", "Module", "Role" },
                values: new object[,]
                {
                    { new Guid("0cde12a8-44f3-e635-6efd-f5c28ff867a0"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "bomItems", "Sales" },
                    { new Guid("0e274dc6-dd6e-0d50-3cc2-f91dc7c9e80d"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "projects", "User" },
                    { new Guid("105692da-b06a-155f-4aae-7dea6dc3e4a7"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "surveys", "DesignEngineer" },
                    { new Guid("1066f7b1-87bb-70b8-8dde-76c27a72d3a3"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "bomItems", "User" },
                    { new Guid("195ec6f6-9fca-f89d-8b7b-1db976beaa3d"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "opportunities", "User" },
                    { new Guid("1ab05bc9-f312-2ce7-8936-9942c17b5274"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "opportunities", "DesignEngineer" },
                    { new Guid("23179d4c-4d68-6c03-55c5-03699d1b2991"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "workOrders", "Admin" },
                    { new Guid("295e6ecd-83c4-683c-ac72-ac952c26acae"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "bomItems", "DesignEngineer" },
                    { new Guid("29d4d5c7-7cba-5bf6-47dc-6600e4af0b9e"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "projects", "DesignEngineer" },
                    { new Guid("2a65be3c-3d7e-ab90-8696-2e365d37bb58"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "surveys", "Admin" },
                    { new Guid("2e11cd89-24d7-3282-e68d-3397484bc384"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "projects", "Sales" },
                    { new Guid("3bd316d2-cc5f-9565-3279-bf864828ad86"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "designs", "Sales" },
                    { new Guid("3edb2781-040e-bdda-7bb4-5cd5f981a44f"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "nonConformities", "User" },
                    { new Guid("4227697d-e0d2-3498-73b6-b3087f8a1d6d"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "nonConformities", "Admin" },
                    { new Guid("4e95d7d3-7171-4244-bd81-bdb1a2fdfc38"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "nonConformities", "ProjectManager" },
                    { new Guid("62409c0e-409c-3712-8e4b-1becc30b4edb"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "plants", "User" },
                    { new Guid("627c2f32-9c74-93a6-b752-63376f990504"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "designs", "ProjectManager" },
                    { new Guid("6285c2af-8ae1-c9b1-cac3-9bb2687c2f2f"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "workOrders", "User" },
                    { new Guid("62f67b56-93f6-236e-9904-37c216907bd9"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "bomItems", "Admin" },
                    { new Guid("70ef8b34-65bb-e82f-3483-2466ac9452dd"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "designs", "User" },
                    { new Guid("7d88b863-f5b2-3e0c-ce08-08019cf5b13d"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "designs", "DesignEngineer" },
                    { new Guid("7f2584a4-b091-8824-a29e-19a95ec58a85"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "bomItems", "ProjectManager" },
                    { new Guid("7fefdacd-b0f6-3bcb-9550-5deb0bd39f55"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "workOrders", "DesignEngineer" },
                    { new Guid("84279f98-b3f5-0ec6-1c7e-4cade8c832aa"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "workOrders", "ProjectManager" },
                    { new Guid("8b399951-9065-81e5-99e2-71537fc04c35"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "reports", "DesignEngineer" },
                    { new Guid("8ce88a52-3225-c177-b5ed-43b90265f25b"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "opportunities", "ProjectManager" },
                    { new Guid("8eb3340e-4a5c-285c-ead7-e5c147459057"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "nonConformities", "DesignEngineer" },
                    { new Guid("95994d98-c183-569a-5025-6a63d2cd6b62"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "reports", "Sales" },
                    { new Guid("963cb677-e916-3f45-ed52-035325d2162d"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "designs", "Admin" },
                    { new Guid("9f7de17b-3286-3eae-1450-8be092e74b8f"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "plants", "ProjectManager" },
                    { new Guid("b1178a6c-6f19-6420-4492-7f401324bb6b"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "plants", "Admin" },
                    { new Guid("b2a2ccf9-6918-ee29-977d-803da1ec1512"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "surveys", "User" },
                    { new Guid("b38275da-35f6-fbc5-8863-caaea78062ef"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "surveys", "ProjectManager" },
                    { new Guid("c79bbcc5-2d9d-0b52-5dc8-d684df1c1b78"), true, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "reports", "User" },
                    { new Guid("c9f8630d-8e1a-b584-abb8-8067554dff91"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "plants", "DesignEngineer" },
                    { new Guid("cf0e486d-e02e-1062-cc42-87a4e5602d22"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "plants", "Sales" },
                    { new Guid("d7dab99a-422a-532f-0a3a-bf0df6ed19e6"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "opportunities", "Sales" },
                    { new Guid("e7438882-1a86-c66e-f2ae-c3944e8c7bd9"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "opportunities", "Admin" },
                    { new Guid("e7978259-6e5e-096a-b209-a305a91c1670"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "surveys", "Sales" },
                    { new Guid("e798150b-5934-1c0b-aa9d-de3a262a68c7"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "nonConformities", "Sales" },
                    { new Guid("ea8ffabf-1152-b99c-a59a-118e9c00b697"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "reports", "Admin" },
                    { new Guid("ec8013a8-89b6-dd67-10c0-49d34ce87d9b"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "projects", "Admin" },
                    { new Guid("ee31358a-c0fd-a198-04a6-9d1a3c3c46c9"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "reports", "ProjectManager" },
                    { new Guid("f9890f4e-ccc5-60e2-4601-a6e9a76afa60"), false, false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "workOrders", "Sales" },
                    { new Guid("fec30011-97f3-f369-f70c-266b6a0af1f9"), true, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "projects", "ProjectManager" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_Role_Module",
                table: "RolePermissions",
                columns: new[] { "Role", "Module" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");
        }
    }
}
