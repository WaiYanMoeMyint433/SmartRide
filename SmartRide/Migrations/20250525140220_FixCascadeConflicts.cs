using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRide.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeConflicts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalRides = table.Column<int>(type: "int", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    GenerateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: true),
                    VehicleId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "RideRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: true),
                    PickupLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DropoffLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedFare = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerUserId = table.Column<int>(type: "int", nullable: true),
                    DriverUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RideRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_RideRequests_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_RideRequests_Users_CustomerUserId",
                        column: x => x.CustomerUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_RideRequests_Users_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_RideRequests_Users_DriverUserId",
                        column: x => x.DriverUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Rides",
                columns: table => new
                {
                    RideId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    PickupLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DropoffLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Fare = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CustomerUserId = table.Column<int>(type: "int", nullable: true),
                    DriverUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rides", x => x.RideId);
                    table.ForeignKey(
                        name: "FK_Rides_RideRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RideRequests",
                        principalColumn: "RequestId");
                    table.ForeignKey(
                        name: "FK_Rides_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Rides_Users_CustomerUserId",
                        column: x => x.CustomerUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Rides_Users_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Rides_Users_DriverUserId",
                        column: x => x.DriverUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RideId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Rides_RideId",
                        column: x => x.RideId,
                        principalTable: "Rides",
                        principalColumn: "RideId");
                    table.ForeignKey(
                        name: "FK_Payments_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RideId",
                table: "Payments",
                column: "RideId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RideRequests_CustomerId",
                table: "RideRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RideRequests_CustomerUserId",
                table: "RideRequests",
                column: "CustomerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RideRequests_DriverId",
                table: "RideRequests",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RideRequests_DriverUserId",
                table: "RideRequests",
                column: "DriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rides_CustomerId",
                table: "Rides",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rides_CustomerUserId",
                table: "Rides",
                column: "CustomerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rides_DriverId",
                table: "Rides",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Rides_DriverUserId",
                table: "Rides",
                column: "DriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rides_RequestId",
                table: "Rides",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Rides");

            migrationBuilder.DropTable(
                name: "RideRequests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
