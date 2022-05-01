using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.MsSql.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Expansion");

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "Expansion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    basketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    orderNumber = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    externalOrderId = table.Column<int>(type: "int", nullable: true),
                    chequePrintDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    clientFullName = table.Column<string>(type: "nvarchar(98)", nullable: false),
                    clientMobilePhone = table.Column<string>(type: "nvarchar(13)", nullable: false),
                    clientMobilePhoneAlt1 = table.Column<string>(type: "nvarchar(13)", nullable: false),
                    containerBarcodes = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    contragentFullName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    contragentOKPO = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    deliveryAddress = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    deliveryDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    deliveryId = table.Column<int>(type: "int", nullable: false),
                    deliveryTimeFrom = table.Column<TimeSpan>(type: "time(0)", nullable: false),
                    deliveryTimeTo = table.Column<TimeSpan>(type: "time(0)", nullable: false),
                    driverId = table.Column<int>(type: "int", nullable: true),
                    driverName = table.Column<string>(type: "nvarchar(98)", nullable: false),
                    filialId = table.Column<int>(type: "int", nullable: false),
                    globalUserId = table.Column<int>(type: "int", nullable: true),
                    pickerName = table.Column<string>(type: "nvarchar(98)", nullable: false),
                    userInn = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    lastContainerBarcode = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    logisticsType = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    megaContainerBarcodes = table.Column<string>(type: "nvarchar(1000)", nullable: false),
                    orderBarcode = table.Column<string>(type: "nvarchar(40)", nullable: false),
                    orderCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    orderFrom = table.Column<byte>(type: "tinyint", nullable: false),
                    orderStatus = table.Column<int>(type: "int", nullable: false),
                    paymentId = table.Column<int>(type: "int", nullable: false),
                    placesCount = table.Column<int>(type: "int", nullable: true),
                    priority = table.Column<int>(type: "int", nullable: false),
                    remark = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    rroNumber = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    sumPaymentFromInternet = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    sumPaymentFromKassa = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    collectingState = table.Column<int>(type: "int", nullable: false),
                    collectStartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    collectEndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    merchantId = table.Column<int>(type: "int", nullable: false),
                    rowversion = table.Column<byte[]>(type: "timestamp", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "Expansion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    orderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    basketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    lagerId = table.Column<int>(type: "int", nullable: false),
                    orderNumber = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    externalOrderId = table.Column<int>(type: "int", nullable: false),
                    customParams = table.Column<string>(type: "nvarchar(2000)", nullable: false),
                    isActivityEnable = table.Column<bool>(type: "bit", nullable: true),
                    containerBarcode = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    dateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    freezeStatus = table.Column<byte>(type: "tinyint", nullable: true),
                    globalUserId = table.Column<int>(type: "int", nullable: true),
                    pickerName = table.Column<string>(type: "nvarchar(98)", nullable: false),
                    userInn = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    lagerName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    lagerUnit = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    orderQuantity = table.Column<decimal>(type: "decimal(12,3)", nullable: false),
                    pickerQuantity = table.Column<decimal>(type: "decimal(12,3)", nullable: true),
                    priceOut = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    replacementOnLagerId = table.Column<int>(type: "int", nullable: true),
                    replacementLagers = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    isWeighted = table.Column<bool>(type: "bit", nullable: true),
                    сollectable = table.Column<bool>(type: "bit", nullable: false),
                    isFilled = table.Column<bool>(type: "bit", nullable: true),
                    rowNum = table.Column<int>(type: "int", nullable: false),
                    sortingCategory = table.Column<string>(type: "nvarchar(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_orderId",
                        column: x => x.orderId,
                        principalSchema: "Expansion",
                        principalTable: "Orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FozzyOrderItems_BasketId",
                schema: "Expansion",
                table: "OrderItems",
                column: "basketId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_orderId",
                schema: "Expansion",
                table: "OrderItems",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "UX_FozzyOrder_BasketId",
                schema: "Expansion",
                table: "Orders",
                column: "basketId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "Expansion");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "Expansion");
        }
    }
}
