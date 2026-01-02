using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MedManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diseases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IcdCode = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diseases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drugs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActiveIngredient = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BrandName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PharmacologicalGroup = table.Column<string>(type: "text", nullable: true),
                    Indications = table.Column<string>(type: "text", nullable: true),
                    Contraindications = table.Column<string>(type: "text", nullable: true),
                    DosageAdults = table.Column<string>(type: "text", nullable: true),
                    DosageChildren = table.Column<string>(type: "text", nullable: true),
                    DosageHepaticImpairment = table.Column<string>(type: "text", nullable: true),
                    DosageRenalImpairment = table.Column<string>(type: "text", nullable: true),
                    AdverseEffects = table.Column<string>(type: "text", nullable: true),
                    PregnancyPrecautions = table.Column<string>(type: "text", nullable: true),
                    BreastfeedingPrecautions = table.Column<string>(type: "text", nullable: true),
                    OtherPrecautions = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drugs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CounselingChecklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DrugId = table.Column<int>(type: "integer", nullable: false),
                    CheckpointCategory = table.Column<string>(type: "text", nullable: false),
                    CheckpointText = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounselingChecklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CounselingChecklists_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiseaseProtocols",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiseaseId = table.Column<int>(type: "integer", nullable: false),
                    DrugId = table.Column<int>(type: "integer", nullable: false),
                    IsPreferred = table.Column<bool>(type: "boolean", nullable: false),
                    PreferenceOrder = table.Column<int>(type: "integer", nullable: false),
                    DosageRecommendation = table.Column<string>(type: "text", nullable: true),
                    SpecialConditions = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseProtocols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiseaseProtocols_Diseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Diseases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiseaseProtocols_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoseCalculations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DrugId = table.Column<int>(type: "integer", nullable: false),
                    CalculationType = table.Column<string>(type: "text", nullable: false),
                    Formula = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    MinDose = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxDose = table.Column<decimal>(type: "numeric", nullable: true),
                    Instructions = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoseCalculations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoseCalculations_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrugInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Drug1Id = table.Column<int>(type: "integer", nullable: false),
                    Drug2Id = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    Mechanism = table.Column<string>(type: "text", nullable: false),
                    ClinicalEffects = table.Column<string>(type: "text", nullable: false),
                    ManagementRecommendations = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrugInteractions_Drugs_Drug1Id",
                        column: x => x.Drug1Id,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DrugInteractions_Drugs_Drug2Id",
                        column: x => x.Drug2Id,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DrugReferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DrugId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Authors = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Doi = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrugReferences_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InteractionReferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InteractionId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Authors = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Doi = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InteractionReferences_DrugInteractions_InteractionId",
                        column: x => x.InteractionId,
                        principalTable: "DrugInteractions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CounselingChecklists_DrugId",
                table: "CounselingChecklists",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseProtocols_DiseaseId_DrugId",
                table: "DiseaseProtocols",
                columns: new[] { "DiseaseId", "DrugId" });

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseProtocols_DrugId",
                table: "DiseaseProtocols",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "IX_Diseases_IcdCode",
                table: "Diseases",
                column: "IcdCode");

            migrationBuilder.CreateIndex(
                name: "IX_Diseases_Name",
                table: "Diseases",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DoseCalculations_DrugId",
                table: "DoseCalculations",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "IX_DrugInteractions_Drug1Id_Drug2Id",
                table: "DrugInteractions",
                columns: new[] { "Drug1Id", "Drug2Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DrugInteractions_Drug2Id",
                table: "DrugInteractions",
                column: "Drug2Id");

            migrationBuilder.CreateIndex(
                name: "IX_DrugReferences_DrugId",
                table: "DrugReferences",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_ActiveIngredient",
                table: "Drugs",
                column: "ActiveIngredient");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_BrandName",
                table: "Drugs",
                column: "BrandName");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_PharmacologicalGroup",
                table: "Drugs",
                column: "PharmacologicalGroup");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionReferences_InteractionId",
                table: "InteractionReferences",
                column: "InteractionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CounselingChecklists");

            migrationBuilder.DropTable(
                name: "DiseaseProtocols");

            migrationBuilder.DropTable(
                name: "DoseCalculations");

            migrationBuilder.DropTable(
                name: "DrugReferences");

            migrationBuilder.DropTable(
                name: "InteractionReferences");

            migrationBuilder.DropTable(
                name: "Diseases");

            migrationBuilder.DropTable(
                name: "DrugInteractions");

            migrationBuilder.DropTable(
                name: "Drugs");
        }
    }
}
