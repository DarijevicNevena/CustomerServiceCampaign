using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerService.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckConstraintsForPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$b1cV4i2BoDOps/yLtkynKu67b0eyNqBEwwgootrNGV4aXXpHJYdEa");

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$aYrwhwSP/93iMaWJnIDGgOj1UikIn.niN884ccG8Cv.3CVPE.EtqW");

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$PzFmzVj8BhmiQqr04r2bF.TKpUSLfMjT.DGwFKM2jyyh/B9TrcZpm");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Purchase_Discount",
                table: "Purchases",
                sql: "[Discount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Purchase_Price",
                table: "Purchases",
                sql: "[Price] > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Purchase_Discount",
                table: "Purchases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Purchase_Price",
                table: "Purchases");

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$pZp085OLFQwfFxmQUBtlUuCSnKj0nm8eg3Cm/gBy.3krenHJlwCR2");

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$9XTTbTcUTXvRqSSTChOx.OR6WDGbccsCsnTMqUwQ.c4fAikE0w2fy");

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$TvztysNH6ygwRqlQKfCopOl.Ei4Uyy0lA9OtwePdTR2pbRYW1mjFO");
        }
    }
}
