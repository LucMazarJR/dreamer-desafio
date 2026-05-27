using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dedg_back.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_user",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    time_zone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    manager_id = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_user", x => x.id_user);
                    table.ForeignKey(
                        name: "FK_tb_user_tb_user_manager_id",
                        column: x => x.manager_id,
                        principalTable: "tb_user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_monthly_period",
                columns: table => new
                {
                    id_monthly_period = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    year = table.Column<int>(type: "int", nullable: false),
                    month = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    closed_by_id = table.Column<int>(type: "int", nullable: true),
                    closed_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_monthly_period", x => x.id_monthly_period);
                    table.ForeignKey(
                        name: "FK_tb_monthly_period_tb_user_closed_by_id",
                        column: x => x.closed_by_id,
                        principalTable: "tb_user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_time_event",
                columns: table => new
                {
                    id_time_event = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    event_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    timezone_at_recording = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_travel = table.Column<bool>(type: "bit", nullable: false),
                    observation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_time_event", x => x.id_time_event);
                    table.ForeignKey(
                        name: "FK_tb_time_event_tb_user_user_id",
                        column: x => x.user_id,
                        principalTable: "tb_user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_monthly_period_closed_by_id",
                table: "tb_monthly_period",
                column: "closed_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_tb_time_event_user_id",
                table: "tb_time_event",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tb_user_manager_id",
                table: "tb_user",
                column: "manager_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_monthly_period");

            migrationBuilder.DropTable(
                name: "tb_time_event");

            migrationBuilder.DropTable(
                name: "tb_user");
        }
    }
}
