using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Blockexplorer.Entities.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressEntities",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 34, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    LastModifiedBlockHeight = table.Column<int>(nullable: false),
                    TxIdBlob = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlockEntities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    BlockData = table.Column<string>(nullable: true),
                    BlockHash = table.Column<string>(maxLength: 64, nullable: true),
                    Height = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionEntities",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    BlockEntityId = table.Column<int>(nullable: true),
                    TransactionData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionEntities_BlockEntities_BlockEntityId",
                        column: x => x.BlockEntityId,
                        principalTable: "BlockEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockEntities_BlockHash",
                table: "BlockEntities",
                column: "BlockHash",
                unique: true,
                filter: "[BlockHash] IS NOT NULL")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionEntities_BlockEntityId",
                table: "TransactionEntities",
                column: "BlockEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressEntities");

            migrationBuilder.DropTable(
                name: "TransactionEntities");

            migrationBuilder.DropTable(
                name: "BlockEntities");
        }
    }
}
