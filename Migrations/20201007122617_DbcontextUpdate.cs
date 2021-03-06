﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace BataCMS.Migrations
{
    public partial class DbcontextUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IDNumber_PhoneNumber",
                table: "AspNetUsers",
                columns: new[] { "IDNumber", "PhoneNumber" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IDNumber_PhoneNumber",
                table: "AspNetUsers");
        }
    }
}
