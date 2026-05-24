using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolicyGuard.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthentication2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "UG9saWN5R3VhcmRBZG1pbg==.O2psDnM1aFO70lk3DjLHlPHjGODbJgm1mBbNnhbuuAI=");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "UG9saWN5R3VhcmRSZXZpZQ==.7it8fGTFjEGTtQUBlKXihfEYQDiArcf0z0WvwivmcZI=");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "UG9saWN5R3VhcmRBdWRpdA==.amhEDPqJtIkpaHIi5g3vdxcwMxWeV3Gqh5f7nRcrO08=");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "R8Tn5hBRBKWIFvVTKoAmsA==.tn2Z+MmAeMF4528zyhNpatj1IoPHH30Fti1fvcFZX6Y=");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "QkYUw48eVCIT05FVCeXvsw==.njVqEGnNwA33TB742ZaK1Fxo3Vrxho5N9NIxg8tXkl0=");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "fchwFbXfZxZn52I5NbcXpQ==.gDnwfyRBsCXwRPnGJ5yD/prSlQULW/B7cKREqZgEOgA=");
        }
    }
}
