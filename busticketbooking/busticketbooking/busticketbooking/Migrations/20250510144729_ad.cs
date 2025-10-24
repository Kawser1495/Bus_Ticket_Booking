using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace busticketbooking.Migrations
{
    /// <inheritdoc />
    public partial class ad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdminID", "Email", "Name", "Password" },
                values: new object[] { 1, "Admin@gmail.com", "Super Admin", "Admin@123" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdminID",
                keyValue: 1);
        }
    }
}
