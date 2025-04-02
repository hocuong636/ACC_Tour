using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ACC_Tour.Migrations
{
    /// <inheritdoc />
    public partial class FixFeedbackTourIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Tours_TourId",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<int>(
                name: "TourId",
                table: "Feedbacks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Tours_TourId",
                table: "Feedbacks",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Tours_TourId",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<int>(
                name: "TourId",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Tours_TourId",
                table: "Feedbacks",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
