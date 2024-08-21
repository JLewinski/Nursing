using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Nursing.Core.Models.DTO;
using Nursing.Models;

#nullable disable

namespace Nursing.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeftBreastTotal = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    RightBreastTotal = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TotalTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Started = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Finished = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastIsLeft = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedings", x => x.Id);
                });

            Services.CacheDatabase cacheDatabase = new();
            List<Models.OldFeeding> feedings;
            try
            {
                feedings = cacheDatabase.GetFeedings(DateTime.MinValue, DateTime.MaxValue).Result;
                var columns = new[] {
                    "Id",
                    "LeftBreastTotal",
                    "RightBreastTotal",
                    "TotalTime",
                    "Started",
                    "Finished",
                    "LastIsLeft",
                    "LastUpdated"
                };

                object[,] data = new object[feedings.Count, columns.Length];

                for (int i = 0; i < feedings.Count; i++)
                {
                    var maxLeft = feedings[i].LeftBreast.Count > 0 ? feedings[i].LeftBreast.Max(x => x.StartTime) : DateTime.MinValue;
                    var maxRight = feedings[i].RightBreast.Count > 0 ? feedings[i].RightBreast.Max(x => x.StartTime) : DateTime.MinValue;
                    data[i, 0] = feedings[i].Id;
                    data[i, 1] = feedings[i].LeftBreastTotal;
                    data[i, 2] = feedings[i].RightBreastTotal;
                    data[i, 3] = feedings[i].TotalTime;
                    data[i, 4] = feedings[i].Started;
                    data[i, 5] = feedings[i].Finished;
                    data[i, 6] = maxLeft > maxRight;
                    data[i, 7] = DateTime.UtcNow;
                }

                migrationBuilder.InsertData("Feedings", columns, data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedings");
        }
    }
}
