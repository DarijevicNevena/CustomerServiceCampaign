using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerService.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Purchases_AgentId",
                table: "Purchases",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_CampaignId",
                table: "Purchases",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Agents_AgentId",
                table: "Purchases",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Campaigns_CampaignId",
                table: "Purchases",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Agents_AgentId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Campaigns_CampaignId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_AgentId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_CampaignId",
                table: "Purchases");
        }
    }
}
