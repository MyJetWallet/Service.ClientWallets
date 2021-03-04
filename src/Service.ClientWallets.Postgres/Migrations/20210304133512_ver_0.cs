using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.ClientWallets.Postgres.Migrations
{
    public partial class ver_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "clientwallets");

            migrationBuilder.CreateTable(
                name: "wallets",
                schema: "clientwallets",
                columns: table => new
                {
                    WalletId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    BrokerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BrandId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Name = table.Column<string>(type: "character varying(3072)", maxLength: 3072, nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallets", x => x.WalletId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wallets_BrokerId_ClientId",
                schema: "clientwallets",
                table: "wallets",
                columns: new[] { "BrokerId", "ClientId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wallets",
                schema: "clientwallets");
        }
    }
}
