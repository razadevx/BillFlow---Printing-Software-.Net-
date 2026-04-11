using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillFlow.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BusinessName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, defaultValue: "My Printing Business"),
                    OwnerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    RatePerSqFt = table.Column<decimal>(type: "REAL", nullable: false, defaultValue: 50m),
                    DefaultCreditLimit = table.Column<decimal>(type: "REAL", nullable: false, defaultValue: 10000m),
                    InvoicePrefix = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "INV"),
                    LastInvoiceNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerCode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "REAL", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastPaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderCode = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WorkStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalArea = table.Column<decimal>(type: "REAL", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "REAL", nullable: false),
                    PastBill = table.Column<decimal>(type: "REAL", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "REAL", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "REAL", nullable: false),
                    PendingAmount = table.Column<decimal>(type: "REAL", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyScheduleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WorkOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyScheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyScheduleItems_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KhataEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "REAL", nullable: false),
                    Balance = table.Column<decimal>(type: "REAL", nullable: false),
                    WorkOrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhataEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KhataEntries_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KhataEntries_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LineItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Width = table.Column<decimal>(type: "REAL", nullable: false),
                    Height = table.Column<decimal>(type: "REAL", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Area = table.Column<decimal>(type: "REAL", nullable: false),
                    Rate = table.Column<decimal>(type: "REAL", nullable: false),
                    Total = table.Column<decimal>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineItems_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "BusinessName", "DefaultCreditLimit", "InvoicePrefix", "LastInvoiceNumber", "OwnerName", "Phone", "RatePerSqFt", "UpdatedAt" },
                values: new object[] { 1, "My Printing Business", 10000m, "INV", 0, "", "", 50m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerCode",
                table: "Customers",
                column: "CustomerCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Name",
                table: "Customers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Phone",
                table: "Customers",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_DailyScheduleItems_ScheduleDate",
                table: "DailyScheduleItems",
                column: "ScheduleDate");

            migrationBuilder.CreateIndex(
                name: "IX_DailyScheduleItems_WorkOrderId",
                table: "DailyScheduleItems",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_KhataEntries_CustomerId",
                table: "KhataEntries",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_KhataEntries_TransactionDate",
                table: "KhataEntries",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_KhataEntries_WorkOrderId",
                table: "KhataEntries",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_WorkOrderId",
                table: "LineItems",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_CustomerId",
                table: "WorkOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_OrderCode",
                table: "WorkOrders",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_PaymentStatus",
                table: "WorkOrders",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ScheduledDate",
                table: "WorkOrders",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_WorkStatus",
                table: "WorkOrders",
                column: "WorkStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyScheduleItems");

            migrationBuilder.DropTable(
                name: "KhataEntries");

            migrationBuilder.DropTable(
                name: "LineItems");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
