using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GLMS.Migrations
{
    /// <inheritdoc />
    public partial class FixedName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stu",
                table: "ServiceRequests",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ServiceRequests",
                newName: "Stu");
        }
    }
}
