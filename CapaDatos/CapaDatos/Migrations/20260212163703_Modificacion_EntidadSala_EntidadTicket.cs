using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapaDatos.Migrations
{
    /// <inheritdoc />
    public partial class Modificacion_EntidadSala_EntidadTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadTickets",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "MontoTotal",
                table: "Tickets",
                newName: "Precio");

            migrationBuilder.AddColumn<string>(
                name: "Asiento",
                table: "Tickets",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Columnas",
                table: "Salas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Filas",
                table: "Salas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asiento",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Columnas",
                table: "Salas");

            migrationBuilder.DropColumn(
                name: "Filas",
                table: "Salas");

            migrationBuilder.RenameColumn(
                name: "Precio",
                table: "Tickets",
                newName: "MontoTotal");

            migrationBuilder.AddColumn<int>(
                name: "CantidadTickets",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
