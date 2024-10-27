using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class many_devices_for_research : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResearchHistory_Devices_DeviceId",
                table: "ResearchHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ResearchHistory_Researches_ResearchId",
                table: "ResearchHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHistory_Devices_DeviceId",
                table: "ServiceHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHistory_Users_ResponsibleId",
                table: "ServiceHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceHistory",
                table: "ServiceHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResearchHistory",
                table: "ResearchHistory");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "Researches");

            migrationBuilder.RenameTable(
                name: "ServiceHistory",
                newName: "ServiceHistories");

            migrationBuilder.RenameTable(
                name: "ResearchHistory",
                newName: "ResearchHistories");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHistory_ResponsibleId",
                table: "ServiceHistories",
                newName: "IX_ServiceHistories_ResponsibleId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHistory_DeviceId",
                table: "ServiceHistories",
                newName: "IX_ServiceHistories_DeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_ResearchHistory_ResearchId",
                table: "ResearchHistories",
                newName: "IX_ResearchHistories_ResearchId");

            migrationBuilder.RenameIndex(
                name: "IX_ResearchHistory_DeviceId",
                table: "ResearchHistories",
                newName: "IX_ResearchHistories_DeviceId");

            migrationBuilder.AddColumn<int[]>(
                name: "DeviceTypes",
                table: "Researches",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceHistories",
                table: "ServiceHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResearchHistories",
                table: "ResearchHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResearchHistories_Devices_DeviceId",
                table: "ResearchHistories",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResearchHistories_Researches_ResearchId",
                table: "ResearchHistories",
                column: "ResearchId",
                principalTable: "Researches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHistories_Devices_DeviceId",
                table: "ServiceHistories",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHistories_Users_ResponsibleId",
                table: "ServiceHistories",
                column: "ResponsibleId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResearchHistories_Devices_DeviceId",
                table: "ResearchHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ResearchHistories_Researches_ResearchId",
                table: "ResearchHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHistories_Devices_DeviceId",
                table: "ServiceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHistories_Users_ResponsibleId",
                table: "ServiceHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceHistories",
                table: "ServiceHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResearchHistories",
                table: "ResearchHistories");

            migrationBuilder.DropColumn(
                name: "DeviceTypes",
                table: "Researches");

            migrationBuilder.RenameTable(
                name: "ServiceHistories",
                newName: "ServiceHistory");

            migrationBuilder.RenameTable(
                name: "ResearchHistories",
                newName: "ResearchHistory");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHistories_ResponsibleId",
                table: "ServiceHistory",
                newName: "IX_ServiceHistory_ResponsibleId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHistories_DeviceId",
                table: "ServiceHistory",
                newName: "IX_ServiceHistory_DeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_ResearchHistories_ResearchId",
                table: "ResearchHistory",
                newName: "IX_ResearchHistory_ResearchId");

            migrationBuilder.RenameIndex(
                name: "IX_ResearchHistories_DeviceId",
                table: "ResearchHistory",
                newName: "IX_ResearchHistory_DeviceId");

            migrationBuilder.AddColumn<int>(
                name: "DeviceType",
                table: "Researches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceHistory",
                table: "ServiceHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResearchHistory",
                table: "ResearchHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResearchHistory_Devices_DeviceId",
                table: "ResearchHistory",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResearchHistory_Researches_ResearchId",
                table: "ResearchHistory",
                column: "ResearchId",
                principalTable: "Researches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHistory_Devices_DeviceId",
                table: "ServiceHistory",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHistory_Users_ResponsibleId",
                table: "ServiceHistory",
                column: "ResponsibleId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
