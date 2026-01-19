using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantDrugProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drugs_ActiveIngredient",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_BrandName",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_PharmacologicalGroup",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "ActiveIngredient",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "AdverseEffects",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "BrandName",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "BreastfeedingPrecautions",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "Contraindications",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "DosageAdults",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "DosageChildren",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "DosageHepaticImpairment",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "DosageRenalImpairment",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "Indications",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "OtherPrecautions",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "PharmacologicalGroup",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "PregnancyPrecautions",
                table: "Drugs");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Drugs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Drugs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_Code",
                table: "Drugs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_Name",
                table: "Drugs",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drugs_Code",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_Name",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Drugs");

            migrationBuilder.AddColumn<string>(
                name: "ActiveIngredient",
                table: "Drugs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdverseEffects",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandName",
                table: "Drugs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BreastfeedingPrecautions",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contraindications",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DosageAdults",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DosageChildren",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DosageHepaticImpairment",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DosageRenalImpairment",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Indications",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherPrecautions",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacologicalGroup",
                table: "Drugs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PregnancyPrecautions",
                table: "Drugs",
                type: "text",
                nullable: true);

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
        }
    }
}
