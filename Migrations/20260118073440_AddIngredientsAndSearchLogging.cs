using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MedManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientsAndSearchLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DosageFormId",
                table: "Drugs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "Drugs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Drugs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Mechanism",
                table: "DrugInteractions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "DosageForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DosageForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MechanismInformations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MechanismInformations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteInformations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteInformations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    SearchQuery = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    ResultCount = table.Column<int>(type: "integer", nullable: false),
                    FoundResults = table.Column<bool>(type: "boolean", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    SearchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DrugIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DrugId = table.Column<int>(type: "integer", nullable: false),
                    IngredientId = table.Column<int>(type: "integer", nullable: false),
                    Strength = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrugIngredients_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DrugIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredientMechanisms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IngredientId = table.Column<int>(type: "integer", nullable: false),
                    MechanismId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientMechanisms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientMechanisms_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientMechanisms_MechanismInformations_MechanismId",
                        column: x => x.MechanismId,
                        principalTable: "MechanismInformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InteractionMechanisms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InteractionId = table.Column<int>(type: "integer", nullable: false),
                    MechanismId = table.Column<int>(type: "integer", nullable: false),
                    MechanismType = table.Column<string>(type: "text", nullable: true),
                    InteractionText = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionMechanisms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InteractionMechanisms_DrugInteractions_InteractionId",
                        column: x => x.InteractionId,
                        principalTable: "DrugInteractions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InteractionMechanisms_MechanismInformations_MechanismId",
                        column: x => x.MechanismId,
                        principalTable: "MechanismInformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_DosageFormId",
                table: "Drugs",
                column: "DosageFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_RouteId",
                table: "Drugs",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_Status",
                table: "Drugs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DosageForms_Code",
                table: "DosageForms",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DosageForms_Name",
                table: "DosageForms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DrugIngredients_DrugId_IngredientId",
                table: "DrugIngredients",
                columns: new[] { "DrugId", "IngredientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DrugIngredients_IngredientId",
                table: "DrugIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientMechanisms_IngredientId_MechanismId",
                table: "IngredientMechanisms",
                columns: new[] { "IngredientId", "MechanismId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientMechanisms_MechanismId",
                table: "IngredientMechanisms",
                column: "MechanismId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Code",
                table: "Ingredients",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Name",
                table: "Ingredients",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionMechanisms_InteractionId_MechanismId",
                table: "InteractionMechanisms",
                columns: new[] { "InteractionId", "MechanismId" });

            migrationBuilder.CreateIndex(
                name: "IX_InteractionMechanisms_MechanismId",
                table: "InteractionMechanisms",
                column: "MechanismId");

            migrationBuilder.CreateIndex(
                name: "IX_MechanismInformations_Code",
                table: "MechanismInformations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MechanismInformations_Name",
                table: "MechanismInformations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RouteInformations_Code",
                table: "RouteInformations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteInformations_Name",
                table: "RouteInformations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SearchLogs_EntityType",
                table: "SearchLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_SearchLogs_SearchedAt",
                table: "SearchLogs",
                column: "SearchedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SearchLogs_SearchQuery",
                table: "SearchLogs",
                column: "SearchQuery");

            migrationBuilder.CreateIndex(
                name: "IX_SearchLogs_UserId",
                table: "SearchLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_DosageForms_DosageFormId",
                table: "Drugs",
                column: "DosageFormId",
                principalTable: "DosageForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_RouteInformations_RouteId",
                table: "Drugs",
                column: "RouteId",
                principalTable: "RouteInformations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_DosageForms_DosageFormId",
                table: "Drugs");

            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_RouteInformations_RouteId",
                table: "Drugs");

            migrationBuilder.DropTable(
                name: "DosageForms");

            migrationBuilder.DropTable(
                name: "DrugIngredients");

            migrationBuilder.DropTable(
                name: "IngredientMechanisms");

            migrationBuilder.DropTable(
                name: "InteractionMechanisms");

            migrationBuilder.DropTable(
                name: "RouteInformations");

            migrationBuilder.DropTable(
                name: "SearchLogs");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "MechanismInformations");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_DosageFormId",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_RouteId",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_Status",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "DosageFormId",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Drugs");

            migrationBuilder.AlterColumn<string>(
                name: "Mechanism",
                table: "DrugInteractions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
