using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ACC_Tour.Migrations
{
    /// <inheritdoc />
    public partial class editDecriptionImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "DescriptionImages",
                newName: "ImageFile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageFile",
                table: "DescriptionImages",
                newName: "Url");
        }
    }
}
